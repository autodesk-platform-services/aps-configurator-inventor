. ./cicd/common.ps1

Write-Output "**** running backend tests ****"
dotnet test
CheckLastExitCode
