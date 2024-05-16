FROM mcr.microsoft.com/dotnet/framework/sdk:4.8

SHELL ["powershell.exe", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ADD InstallNode.ps1 .
RUN ./InstallNode.ps1

ENV NUGET_PACKAGES=c:\\sdra\\nuget

ADD FontsToAdd.tar /Fonts/

WORKDIR /Fonts/
# RUN .\\Add-Font.ps1 Fonts

WORKDIR /
RUN Remove-Item Fonts -Recurse