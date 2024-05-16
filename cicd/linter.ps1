. ./cicd/common.ps1

Write-Output "**** running linter ****"
$slnDir=$PWD.ToString()
Set-Location WebApplication/ClientApp
npm run lint
CheckLastExitCode
Set-Location $slnDir