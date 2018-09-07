@echo off
pushd %~dp0

rem Delete any existing file to make sure we get a new one
if exist smc.xml del /q smc.xml

rem Generate a new SMC XML file from our method sources
echo Processing source files...
dbl2xml *.dbl -out smc.xml
if ERRORLEVEL 1 goto parse_fail

rem Load the XML file into the SMC
echo Loading method catalog...
dbs DBLDIR:mdu -u smc.xml
if ERRORLEVEL 1 goto load_fail
echo Method catalog was loaded

goto done

:parse_fail
echo ERROR: Failed to extract SMC data from code
goto done

:load_fail
echo ERROR: Failed to load method catalog

:done
popd