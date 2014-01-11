@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=.\..\.nuget\nuget.exe

msbuild /t:BuildRelease .\..\build.proj /p:AsmVersion=%VERSION%
%NUGET% pack brutile.nuspec -Version %VERSION% -outputdirectory .\..\Release
%NUGET% pack brutile.desktop.nuspec -Version %VERSION% -outputdirectory .\..\Release
%NUGET% push .\..\Release\brutile.%VERSION%.nupkg 
%NUGET% push .\..\Release\brutile.desktop.%VERSION%.nupkg 





