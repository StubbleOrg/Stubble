#tool "nuget:?package=coveralls.net"
#tool "nuget:?package=ReportGenerator"

#addin "nuget:?package=Cake.Coveralls"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var testFramework = Argument("testFramework", "");
var framework = Argument("framework", "");
var runCoverage = Argument<bool>("runCoverage", true);

var buildDir = Directory("./src/Stubble.Core/bin/Any CPU/") + Directory(configuration);
var testBuildDir = Directory("./test/Stubble.Core.Tests/bin/Any CPU/") + Directory(configuration);

var artifactsDir = Directory("./artifacts/");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(testBuildDir);
    CleanDirectory("./artifacts");
    CleanDirectory("./coverage-results");
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

private Action<ICakeContext> testAction(string projectPath) {
    return (tool) => {
        var testSettings = new DotNetCoreTestSettings {
            Configuration = configuration,
            NoBuild = true,
            Verbosity = DotNetCoreVerbosity.Quiet,
            Framework = testFramework,
            ArgumentCustomization = args =>
                args.Append("--logger:trx")
        };

        tool.DotNetCoreTest(projectPath, testSettings);
    };
}

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    if (runCoverage || AppVeyor.IsRunningOnAppVeyor)
    {
        RunCoverageForTestProject("./test/Stubble.Core.Tests/Stubble.Core.Tests.csproj");
        RunCoverageForTestProject("./test/Stubble.Compilation.Tests/Stubble.Compilation.Tests.csproj");

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            foreach(var file in GetFiles("./test/Stubble.Core.Tests/TestResults/*"))
            {
                AppVeyor.UploadTestResults(file, AppVeyorTestResultsType.MSTest);
                AppVeyor.UploadArtifact(file);
            }
        }
    } else {
        testAction("./test/Stubble.Core.Tests/Stubble.Core.Tests.csproj")(Context);
        testAction("./test/Stubble.Compilation.Tests/Stubble.Compilation.Tests.csproj")(Context);
    }
});

private void RunCoverageForTestProject(string projectPath) {
    var path = new FilePath("./OpenCover-Experimental/OpenCover.Console.exe").MakeAbsolute(Context.Environment);

    Information(path.ToString());

    CreateDirectory("./coverage-results/");
    OpenCover(
        testAction(projectPath),
        new FilePath(string.Format($"./coverage-results/results-{DateTime.UtcNow:dd-MM-yyyy-HH-mm-ss-FFF}.xml")),
        new OpenCoverSettings {
            Register = "user",
            SkipAutoProps = true,
            OldStyle = true,
            ToolPath = path,
            ReturnTargetCodeOffset = 0
        }
        .WithFilter("-[Stubble.Core.Tests]*")
        .WithFilter("-[Stubble.Compilation.Tests]*")
        .WithFilter("+[Stubble.*]*")
        .WithFilter("-[Stubble.Core]*.Imported.*")
        .WithFilter("-[Stubble.Compilation]*.Import.*")
        .WithFilter("-[Stubble.Compilation]Stubble.Compilation.Contexts.RegistryResult")
        .WithFilter("-[Stubble.Core]Stubble.Core.Helpers.*")
        .WithFilter("-[Stubble.Compilation]Stubble.Compilation.Helpers.*")
    );
}

Task("Pack")
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

Task("Coveralls")
    .IsDependentOn("Pack")
    .Does(() =>
{
    if (!AppVeyor.IsRunningOnAppVeyor) return;

    var token = EnvironmentVariable("COVERALLS_REPO_TOKEN");

    CoverallsNet("./coverage-results/results.xml", CoverallsNetReportType.OpenCover, new CoverallsNetSettings()
    {
        RepoToken = token,
        CommitId = EnvironmentVariable("APPVEYOR_REPO_COMMIT"),
        CommitBranch = EnvironmentVariable("APPVEYOR_REPO_BRANCH"),
        CommitAuthor = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR"),
        CommitEmail = EnvironmentVariable("APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL"),
        CommitMessage = EnvironmentVariable("APPVEYOR_REPO_COMMIT_MESSAGE")
    });
});

Task("CoverageReport")
    .IsDependentOn("Test")
    .Does(() =>
{
    ReportGenerator("./coverage-results/*.xml", "./coverage-report/");
});

Task("AppVeyor")
    .IsDependentOn("Coveralls");

Task("Travis")
    .IsDependentOn("Test");

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);