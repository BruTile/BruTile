@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=.\..\tools\nuget\nuget.exe

CALL buildpack %VERSION%
%NUGET% push .\..\Release\brutile.%VERSION%.nupkg -source nuget.org
%NUGET% push .\..\Release\brutile.desktop.%VERSION%.nupkg -source nuget.org
%NUGET% push .\..\Release\brutile.mbtiles.%VERSION%.nupkg -source nuget.org
git commit -m %VERSION% -a
git tag %VERSION%
git push origin %VERSION%
git push

