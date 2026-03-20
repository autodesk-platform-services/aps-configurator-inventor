. ./cicd/common.ps1

& node (Join-Path $PSScriptRoot "validate-package-lock-registry.js")
CheckLastExitCode

Write-Output "**** Building ****"
msbuild -restore /property:Configuration=Release
CheckLastExitCode
msbuild /property:Configuration=Release /Target:Publish
CheckLastExitCode
