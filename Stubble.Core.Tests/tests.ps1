Param(
  [switch]$runReports,
  [switch]$uploadToCoveralls
)

$Package_Locations = "..\..\..\packages";
$OpenCover_Location = Get-ChildItem ($Package_Locations + "\OpenCover*") | Select-Object -First 1 -ExpandProperty FullName;
$Xunit_Location = Get-ChildItem ($Package_Locations + "\xunit.runner.console*\tools") | Select-Object -First 1 -ExpandProperty FullName;
$ReportGenerator_Location = Get-ChildItem ($Package_Locations + "\ReportGenerator*\tools") | Select-Object -First 1 -ExpandProperty FullName;
$CoverallsNet_Location = Get-ChildItem ($Package_Locations + "\coveralls.io*\tools") | Select-Object -First 1 -ExpandProperty FullName;

iex ($OpenCover_Location + '\OpenCover.Console.exe -register:user -target:"' + $Xunit_Location + "\xunit.console.exe /noshadow /appveyor" + '" -targetargs:"Stubble.Core.Tests.dll" -output:coverage.xml -skipautoprops -returntargetcode -filter:"+[Stubble.*]*"');

if($runReports)
{
    iex ($ReportGenerator_Location + '\ReportGenerator.exe -reports:"coverage.xml" -targetdir:"./reports/CodeCoverage"'); 
}

if($uploadToCoveralls)
{
    iex ($CoverallsNet_Location + '\coveralls.net.exe --opencover -i coverage.xml --repoToken $env:COVERALLS_REPO_TOKEN --commitId $env:APPVEYOR_REPO_COMMIT --commitBranch $env:APPVEYOR_REPO_BRANCH --commitAuthor $env:APPVEYOR_REPO_COMMIT_AUTHOR --commitEmail $env:APPVEYOR_REPO_COMMIT_AUTHOR_EMAIL --commitMessage $env:APPVEYOR_REPO_COMMIT_MESSAGE --jobId $env:APPVEYOR_JOB_ID'); 
}
