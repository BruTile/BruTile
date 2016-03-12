@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=.\..\tools\nuget\nuget.exe

CALL buildpack %VERSION%
%NUGET% push .\..\Release\brutile.%VERSION%.nupkg 
%NUGET% push .\..\Release\brutile.desktop.%VERSION%.nupkg 
git commit -m %VERSION%
git tag %VERSION%
git push

