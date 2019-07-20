#tool "nuget:?package=ReportGenerator&version=4.2.10"

#tool nuget:?package=Codecov&version=1.5.0
#addin nuget:?package=Cake.Codecov&version=0.6.0
#addin nuget:?package=Cake.Coverlet&version=2.3.4

public class MyBuildData
{
	public string Configuration { get; }
    public string TestFramework { get; }
    public string Framework { get; }
	public bool RunCoverage { get; }

    public ConvertableDirectoryPath ArtifactsDirectory { get; }
    public ConvertableDirectoryPath CoverageDirectory { get; }
    public ConvertableDirectoryPath CoverageReportDirectory { get; }
    public ConvertableDirectoryPath TestResultsDirectory { get; }

    public DotNetCoreBuildSettings BuildSettings { get; }
    public DotNetCoreBuildSettings TestBuildSettings { get; }
    public DotNetCoreTestSettings TestSettings { get; }
    public DotNetCorePackSettings PackSettings { get; }
    public CoverletSettings CoverletSettings { get; }

    public IReadOnlyList<ConvertableDirectoryPath> BuildDirs { get; }

	public MyBuildData(
		string configuration,
		string testFramework,
        string framework,
        bool runCoverage,
        ConvertableDirectoryPath artifactsDirectory,
        ConvertableDirectoryPath coverageDirectory,
        ConvertableDirectoryPath coverageReportDirectory,
        ConvertableDirectoryPath testResultsDirectory,
        IReadOnlyList<ConvertableDirectoryPath> buildDirectories)
	{
		Configuration = configuration;
		TestFramework = testFramework;
        Framework = framework;
        RunCoverage = runCoverage;
        ArtifactsDirectory = artifactsDirectory;
        CoverageDirectory = coverageDirectory;
        CoverageReportDirectory = coverageReportDirectory;
        TestResultsDirectory = testResultsDirectory;
        BuildDirs = buildDirectories;

        var setting = new DotNetCoreBuildSettings {
            Configuration = configuration,
            NoRestore = true,
            ArgumentCustomization = args => args.Append("/property:WarningLevel=0") // Until Warnings are fixed in StyleCop
        };

        if(!string.IsNullOrEmpty(framework))
        {
            setting.Framework = framework;
        }

        var testSetting = new DotNetCoreBuildSettings {
            NoRestore = true,
            Configuration = configuration
        };

        if(!string.IsNullOrEmpty(testFramework))
        {
            testSetting.Framework = testFramework;
        }

        BuildSettings = setting;
        TestBuildSettings = testSetting;

        CoverletSettings = new CoverletSettings {
            CollectCoverage = runCoverage,
            CoverletOutputFormat = CoverletOutputFormat.opencover | CoverletOutputFormat.cobertura,
            CoverletOutputDirectory = CoverageDirectory,
            CoverletOutputName = $"results",
        }
        .WithFilter("[Stubble.Core.Tests]*")
        .WithFilter("[Stubble.Core]*.Imported.*")
        .WithFilter("[Stubble.Compilation]Stubble.Compilation.Helpers.*")
        .WithFilter("[Stubble.Compilation.Tests]*")
        .WithFilter("[Stubble.Compilation]*.Import.*")
        .WithFilter("[Stubble.Compilation]Stubble.Compilation.Contexts.RegistryResult")
        .WithFilter("[Stubble.Core]Stubble.Core.Helpers.*")
        .WithDateTimeTransformer();

        var testSettings = new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true,
            Verbosity = DotNetCoreVerbosity.Quiet,
            Framework = testFramework,
            ArgumentCustomization = args =>
                args.Append("--logger:trx")
        };

        TestSettings = testSettings;

        PackSettings = new DotNetCorePackSettings
        {
            OutputDirectory = ArtifactsDirectory,
            NoBuild = true,
            Configuration = Configuration,
        };
	}
}

var target = Argument("target", "Default");

Setup<MyBuildData>(setupContext =>
{
	return new MyBuildData(
		configuration: Argument("configuration", "Release"),
        testFramework: Argument("testFramework", ""),
        framework: Argument("framework", ""),
		runCoverage: Argument<bool>("runCoverage", true),
        artifactsDirectory: Directory("./artifacts/"),
        coverageDirectory: Directory("./coverage-results"),
        coverageReportDirectory: Directory("./coverage-report"),
        testResultsDirectory: Directory("./test/Stubble.Core.Tests/TestResults"),
        buildDirectories: new List<ConvertableDirectoryPath> {
            Directory("./src/Stubble.Core/bin/Any CPU/"),
            Directory("./src/Stubble.Core.Tests/bin/Any CPU/"),
            Directory("./src/Stubble.Core.Compilation/bin/Any CPU/")
        });
});

Task("Clean")
    .Does<MyBuildData>((data) =>
{
    foreach (var dir in data.BuildDirs)
    {
        CleanDirectory(dir + Directory(data.Configuration));
    }
    CleanDirectory(data.ArtifactsDirectory);
    CleanDirectory(data.CoverageDirectory);
    CleanDirectory(data.CoverageReportDirectory);
    CleanDirectory(data.TestResultsDirectory);
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore("./Stubble.Core.sln");
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does<MyBuildData>((data) =>
{
    DotNetCoreBuild("./src/Stubble.Core/", data.BuildSettings);
    DotNetCoreBuild("./test/Stubble.Core.Tests/", data.TestBuildSettings);
    DotNetCoreBuild("./src/Stubble.Compilation/", data.BuildSettings);
    DotNetCoreBuild("./test/Stubble.Compilation.Tests/", data.TestBuildSettings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does<MyBuildData>((data) =>
{
    DotNetCoreTest("./test/Stubble.Core.Tests/Stubble.Core.Tests.csproj", data.TestSettings, data.CoverletSettings);
    DotNetCoreTest("./test/Stubble.Compilation.Tests/Stubble.Compilation.Tests.csproj", data.TestSettings, data.CoverletSettings);
});

Task("Pack")
    .IsDependentOn("Test")
    .Does<MyBuildData>((data) =>
{
    DotNetCorePack("./src/Stubble.Core/Stubble.Core.csproj", data.PackSettings);
    DotNetCorePack("./src/Stubble.Compilation/Stubble.Compilation.csproj", data.PackSettings);
});

Task("CodeCov")
    .IsDependentOn("Pack")
    .WithCriteria(BuildSystem.IsRunningOnAzurePipelinesHosted && IsRunningOnWindows())
    .Does<MyBuildData>((data) =>
{
    var coverageFiles = GetFiles((string)data.CoverageDirectory + "/*opencover.xml")
        .Select(f => f.FullPath)
        .ToArray();

    var settings = new CodecovSettings();
    var token = EnvironmentVariable("CODECOV_REPO_TOKEN");
    settings.Token = token;

    foreach(var file in coverageFiles)
    {
        settings.Files = new [] { file };

        // Upload coverage reports.
        Codecov(settings);
    }
});

Task("CoverageReport")
    .Does<MyBuildData>((data) =>
{
    ReportGenerator((string)data.CoverageDirectory + "/*opencover.xml", data.CoverageReportDirectory);
});

Task("Default")
    .IsDependentOn("CodeCov");

RunTarget(target);