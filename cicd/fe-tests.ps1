. ./cicd/common.ps1

Write-Output "**** running frontend tests ****"
$slnDir=$PWD.ToString()
Set-Location WebApplication/ClientApp
npm test -- --coverage
CheckLastExitCode
Set-Location $slnDir