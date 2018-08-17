@echo off
setlocal

set PATH=C:\Program Files\7-Zip;%PATH%

set SolutionDir=%~dp0
pushd %SolutionDir%

if not exist distribution\linux\debug\.   mkdir distribution\linux\debug
if not exist distribution\linux\release\. mkdir distribution\linux\release

pushd SampleServices.Test
echo Building debug distribution
dotnet publish -c Debug   -r linux-x64 -o ..\distribution\linux\debug
echo Building release distribution
dotnet publish -c Release -r linux-x64 -o ..\distribution\linux\release
popd

pushd distribution
if exist HarmonyCoreSample_linux64.zip del HarmonyCoreSample_linux64.zip
pushd linux
7z a -r ..\HarmonyCoreSample_linux64.zip *
popd
popd

popd
endlocal
pause
