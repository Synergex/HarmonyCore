@echo off
pushd %~dp0
setlocal

set CODEGEN_TPLDIR=Templates
set PROJECT=SampleServices
set OPTS=-e -r -lf

set STRUCTURES=CUSTOMERS ORDERS PLANTS VENDORS

rem Generate model classes
codegen -s %STRUCTURES%   -t DataObject -n %PROJECT%.Models -o %PROJECT%\Models %OPTS%

rem Generate controller classes
codegen -s %STRUCTURES%    -t ODataController -n %PROJECT%.Controllers -o %PROJECT%\Controllers %OPTS% -ut DBCONTEXT_NAMESPACE=%PROJECT%

rem Generate the DbContext, EdmBuilder and Startup classes
codegen -s %STRUCTURES%    -ms -t ODataDbContext ODataEdmBuilder ODataStartup -n %PROJECT% -o %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models %OPTS%

rem Generate unit tests
codegen -s %STRUCTURES%   -t ODataUnitTests -n %PROJECT%.Test -o %PROJECT%.Test -ut MODELS_NAMESPACE=%PROJECT%.Models %OPTS%

rem ================================================================================================================================

set FILE_STRUCTURES=CUSTOMERS ORDERS PLANTS

rem Generate the unit test environment class
codegen -s %FILE_STRUCTURES% -ms -t ODataUnitTestEnvironment -n %PROJECT%.Test -o %PROJECT%.Test -ut SERVICES_NAMESPACE=%PROJECT% %OPTS%

endlocal
popd