. ./cicd/common.ps1

xcopy /Y /E WebApplication\bin\Release\$doteNetRelease\publish\* output\uitest\
xcopy /Y /E WebApplication\AppBundles\* output\uitest\AppBundles\

$slnDir=$PWD.ToString()

Write-Output "**** Starting the server ****"
Set-Location output\uitest\
$serverProcess = Start-Process -NoNewWindow dotnet -ArgumentList "WebApplication.dll", "clear=true", "initialize=true", "allowCleanSelf=true" -PassThru
Set-Location $slnDir\WebApplication
Write-Output "Waiting for server to initialize"
..\cicd\waitForServer.ps1
Write-Output "**** running the UI tests ****"
Set-Location ClientApp
mkdir output
try {
    npx codeceptjs run $env:UITestParams
    CheckLastExitCode
}
finally {
    Write-Host "==== Post Build phase ===="
    Set-Location $slnDir
    xcopy /Y /E WebApplication\ClientApp\output\* output\report\errorScreenShots\
    xcopy /Y /E WebApplication\ClientApp\coverage\* output\report\coverage\

    Write-Host "**** Shutting down the server ****"
    Stop-Process $serverProcess
}