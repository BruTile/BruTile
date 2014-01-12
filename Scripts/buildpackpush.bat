@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=.\..\.nuget\nuget.exe

buildpack %VERSION%
%NUGET% push .\..\Release\brutile.%VERSION%.nupkg 
%NUGET% push .\..\Release\brutile.desktop.%VERSION%.nupkg 

