@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=.\..\.nuget\nuget.exe

msbuild updateversionnumber.proj /p:AsmVersion=%VERSION%
msbuild build.proj 
%NUGET% pack brutile.nuspec -Version %VERSION% -outputdirectory .\..\Release
%NUGET% pack brutile.desktop.nuspec -Version %VERSION% -outputdirectory .\..\Release






