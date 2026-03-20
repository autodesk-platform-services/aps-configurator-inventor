. ./cicd/common.ps1

Write-Output "**** Building ****"
msbuild -restore /property:Configuration=Release
CheckLastExitCode
msbuild /property:Configuration=Release /Target:Publish
CheckLastExitCode
