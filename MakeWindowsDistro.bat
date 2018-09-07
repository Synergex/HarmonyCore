@echo off
setlocal

set PATH=C:\Program Files\7-Zip;%PATH%

set SolutionDir=%~dp0
pushd %SolutionDir%

if not exist distribution\windows\debug\.   mkdir distribution\windows\debug
rem if not exist distribution\windows\release\. mkdir distribution\windows\release

pushd Services.Test
echo Building debug distribution
dotnet publish -c Debug   -r win7-x64 -o ..\distribution\windows\debug
rem echo Building release distribution
rem dotnet publish -c Release -r win7-x64 -o ..\distribution\windows\release
popd

rem pushd distribution
rem if exist HarmonyCoreSample_windows64.zip del HarmonyCoreSample_windows64.zip
rem pushd windows
rem 7z a -r ..\HarmonyCoreSample_windows64.zip *
rem popd
rem popd

popd
endlocal
pause
