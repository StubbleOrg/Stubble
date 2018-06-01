#tool "nuget:?package=ReportGenerator"

#tool nuget:?package=Codecov
#addin nuget:?package=Cake.Codecov

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testFramework = Argument("testFramework", "");
var framework = Argument("framework", "");
var runCoverage = Argument<bool>("runCoverage", true);

var BuildDirs = new List<ConvertableDirectoryPath> {
    Directory("./src/Stubble.Core/bin/Any CPU/"),
    Directory("./src/Stubble.Core.Tests/bin/Any CPU/"),
    Directory("./src/Stubble.Core.Compilation/bin/Any CPU/")
};

var artifactsDir = Directory("./artifacts/");
var coverageDir = Directory("./coverage-results");

var cakeEnvironment = Context.Environment;

Task("Clean")
    .Does(() =>
{
    foreach (var dir in BuildDirs) 
    {
        CleanDirectory(dir + Directory(configuration));
    }
    CleanDirectory(artifactsDir);
    CleanDirectory(coverageDir);
    CleanDirectory("./coverage-report");
    CleanDirectory("./test/Stubble.Core.Tests/TestResults");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    if (AppVeyor.IsRunningOnAppVeyor) {
        DotNetCoreRestore("./src/Stubble.Core/Stubble.Core.csproj");
        DotNetCoreRestore("./test/Stubble.Core.Tests/Stubble.Core.Tests.csproj");

        DotNetCoreRestore("./src/Stubble.Compilation/Stubble.Compilation.csproj");
        DotNetCoreRestore("./test/Stubble.Compilation.Tests/Stubble.Compilation.Tests.csproj");
    } else {
        DotNetCoreRestore("./Stubble.Core.sln");
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
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

    DotNetCoreBuild("./src/Stubble.Core/", setting);
    DotNetCoreBuild("./test/Stubble.Core.Tests/", testSetting);
    DotNetCoreBuild("./src/Stubble.Compilation/", setting);
    DotNetCoreBuild("./test/Stubble.Compilation.Tests/", testSetting);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    RunCoverageForTestProjectCoverlet("./test/Stubble.Core.Tests/Stubble.Core.Tests.csproj", runCoverage);
    RunCoverageForTestProjectCoverlet("./test/Stubble.Compilation.Tests/Stubble.Compilation.Tests.csproj", runCoverage);

    if (runCoverage && AppVeyor.IsRunningOnAppVeyor)
    {
        foreach(var file in GetFiles("./test/Stubble.Core.Tests/TestResults/*"))
        {
            AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.MSTest);
            AppVeyor.UploadArtifact(file);
        }
    }
});

private void RunCoverageForTestProjectCoverlet(string projectPath, bool runCoverage) {
    var coverletSettings = new CoverletSettings {
        CollectCoverage = runCoverage,
        CoverletOutputFormat = CoverletOutputFormat.opencover,
        CoverletOutputDirectory = coverageDir,
        CoverletOutputName = $"results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}"
    }
    .WithFilter("[Stubble.Core.Tests]*")
    .WithFilter("[Stubble.Compilation.Tests]*")
    .WithFilter("[Stubble.Core]*.Imported.*")
    .WithFilter("[Stubble.Compilation]*.Import.*")
    .WithFilter("[Stubble.Compilation]Stubble.Compilation.Contexts.RegistryResult")
    .WithFilter("[Stubble.Core]Stubble.Core.Helpers.*")
    .WithFilter("[Stubble.Compilation]Stubble.Compilation.Helpers.*");

    var testSettings = new DotNetCoreTestSettings {
        Configuration = configuration,
        NoBuild = true,
        Verbosity = DotNetCoreVerbosity.Quiet,
        Framework = testFramework,
        ArgumentCustomization = args =>
            args.Append("--logger:trx")
    };
    testSettings = ApplyCoverletSettings(testSettings, coverletSettings, cakeEnvironment);

    DotNetCoreTest(projectPath, testSettings);
}

Task("Pack")
    .WithCriteria(!BuildSystem.IsRunningOnTravisCI)
    .IsDependentOn("Test")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        OutputDirectory = artifactsDir,
        NoBuild = true,
        Configuration = configuration,
    };

    DotNetCorePack("./src/Stubble.Core/Stubble.Core.csproj", settings);
    DotNetCorePack("./src/Stubble.Compilation/Stubble.Compilation.csproj", settings);
});

Task("CodeCov")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var coverageFiles = GetFiles("./coverage-results/*.xml")
        .Select(f => f.FullPath)
        .ToArray();

    var settings = new CodecovSettings();

    if (AppVeyor.IsRunningOnAppVeyor) {
        var token = EnvironmentVariable("CODECOV_REPO_TOKEN");
        settings.Token = token;
    }

    foreach(var file in coverageFiles)
    {
        settings.Files = new [] { file };

        // Upload coverage reports.
        Codecov(settings);
    }
});

Task("CoverageReport")
    .IsDependentOn("Test")
    .Does(() =>
{
    ReportGenerator("./coverage-results/*.xml", "./coverage-report/");
});

Task("AppVeyor")
    .IsDependentOn("CodeCov");

Task("Travis")
    .IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);

static DotNetCoreTestSettings ApplyCoverletSettings(DotNetCoreTestSettings settings, CoverletSettings coverletSettings, ICakeEnvironment env) {
    var argsCustomisation = settings.ArgumentCustomization;
    settings.ArgumentCustomization = args => coverletSettings.ApplySettings(argsCustomisation(args), env);
    return settings;
}

class CoverletSettings {
    public bool CollectCoverage { get; set; } = true;
    public CoverletOutputFormat CoverletOutputFormat { get; set; }
    public DirectoryPath CoverletOutputDirectory { get; set; }
    public string CoverletOutputName { get; set; }
    public List<string> ExcludeByFile { get; set; } = new List<string>();
    public List<string> Exclude { get; set; } = new List<string>();

    public CoverletSettings WithFilter(string filter) {
        Exclude.Add(filter);
        return this;
    }

    public ProcessArgumentBuilder ApplySettings(ProcessArgumentBuilder builder, ICakeEnvironment env) {
        var msbuildSettings = new DotNetCoreMSBuildSettings()
            .WithProperty(nameof(CollectCoverage), CollectCoverage.ToString())
            .WithProperty(nameof(CoverletOutputFormat), CoverletOutputFormat.ToString());

        if (!string.IsNullOrEmpty(CoverletOutputName))
        {
            msbuildSettings = msbuildSettings
                .WithProperty(nameof(CoverletOutputName), CoverletOutputName.ToString());
        }

        if (CoverletOutputDirectory != null)
        {
            msbuildSettings = msbuildSettings
                .WithProperty(nameof(CoverletOutputDirectory), CoverletOutputDirectory.MakeAbsolute(env).FullPath);
        }

        builder.AppendMSBuildSettings(msbuildSettings, env);

        if (ExcludeByFile.Count > 0)
        {
            builder.Append($"/property:{nameof(ExcludeByFile)}=\\\"{string.Join(",", ExcludeByFile)}\\\"");
        }

        if (Exclude.Count > 0)
        {
            builder.Append($"/property:{nameof(Exclude)}=\\\"{string.Join(",", Exclude)}\\\"");
        }

        return builder;
    }
}

enum CoverletOutputFormat {
    json,
    lcov,
    opencover,
    cobertura
}