@echo off
pushd %~dp0
setlocal

set CODEGEN_TPLDIR=Templates
set PROJECT=SampleServices
set OPTS=-e -r -lf

set STRUCTURES=CUSTOMERS ORDERS PLANTS CONTRACT

rem Generate model classes
codegen -s %STRUCTURES%   -t DataObject -n %PROJECT%.Models -o %PROJECT%\Models %OPTS%

rem Generate the DbContext class
codegen -s %STRUCTURES%    -ms -t ODataDbContext -n %PROJECT%.Data -o %PROJECT%\Data -ut MODELS_NAMESPACE=%PROJECT%.Models %OPTS%

rem Generate controller classes
codegen -s %STRUCTURES%    -t ODataController -n %PROJECT%.Controllers -o %PROJECT%\Controllers %OPTS% -ut DBCONTEXT_NAMESPACE=%PROJECT%.Data

endlocal
popd