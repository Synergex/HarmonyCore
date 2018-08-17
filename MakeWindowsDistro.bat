@echo off
setlocal

set PATH=C:\Program Files\7-Zip;%PATH%

set SolutionDir=%~dp0
pushd %SolutionDir%

if not exist distribution\windows\debug\.   mkdir distribution\windows\debug
if not exist distribution\windows\release\. mkdir distribution\windows\release

pushd SampleServices.Test
echo Building debug distribution
dotnet publish -c Debug   -r win7-x64 -o ..\distribution\windows\debug
echo Building release distribution
dotnet publish -c Release -r win7-x64 -o ..\distribution\windows\release
popd

pushd distribution
if exist HarmonyCoreSample_windows64.zip del HarmonyCoreSample_windows64.zip
pushd windows
7z a -r ..\HarmonyCoreSample_windows64.zip *
popd
popd

popd
endlocal
pause
