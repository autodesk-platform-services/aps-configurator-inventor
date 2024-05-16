$nodeVer = "20.12.2"

Invoke-WebRequest -OutFile nodejs.zip -UseBasicParsing "https://nodejs.org/dist/v${nodeVer}/node-v${nodeVer}-win-x64.zip"

Expand-Archive nodejs.zip -DestinationPath C:\

Rename-Item "C:\\node-v${nodeVer}-win-x64" c:\nodejs

[Environment]::SetEnvironmentVariable("Path", $env:Path + ";c:\nodejs", [EnvironmentVariableTarget]::Machine)