@ECHO OFF
SETLOCAL
SET VERSION=%1
SET NUGET=.\..\tools\nuget\nuget.exe

CALL buildpack %VERSION%
REM %NUGET% push .\..\Release\brutile.%VERSION%.nupkg 
REM %NUGET% push .\..\Release\brutile.desktop.%VERSION%.nupkg 
git commit -m ''%VERSION%''
REM git tag %VERSION%
git push

