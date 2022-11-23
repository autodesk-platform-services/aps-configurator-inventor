$userIndex=$Env:CODEBUILD_BUILD_NUMBER % $Env:UserCount
echo "Using test user index $userIndex"
$APS_CLIENT_ID=(Get-ChildItem -Path Env:\APS_CLIENT_ID$userIndex).Value
echo "Client ID is $APS_CLIENT_ID"
$APS_CLIENT_SECRET=(Get-ChildItem -Path Env:\APS_CLIENT_SECRET$userIndex).Value
(Get-Content -path .\AWS-CICD\build-specs\appsettings.Local.json -Raw) -replace '<YOUR_CLIENT_ID>',"$APS_CLIENT_ID" -replace '<YOUR_CLIENT_SECRET>',"$APS_CLIENT_SECRET" -replace '<BUCKET_KEY_SUFFIX>',"$Env:CODEBUILD_BUILD_NUMBER" | Set-Content -Path WebApplication\appsettings.Local.json
