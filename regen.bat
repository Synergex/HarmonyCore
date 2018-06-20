@echo off
pushd %~dp0
setlocal

set OPTS=-e -r -lf

set CODEGEN_TPLDIR=Templates
set PROJECT=SampleServices

rem ================================================================================================================================

set STRUCTURES=CUSTOMERS ORDERS PLANTS VENDORS

rem Generate model classes
codegen -s %STRUCTURES%     -t DataObject -n %PROJECT%.Models -o %PROJECT%\Models %OPTS%

rem Generate controller classes
codegen -s %STRUCTURES%     -t ODataController -n %PROJECT%.Controllers -o %PROJECT%\Controllers %OPTS% -ut DBCONTEXT_NAMESPACE=%PROJECT%

rem Generate the DbContext, EdmBuilder and Startup classes
codegen -s %STRUCTURES% -ms -t ODataDbContext ODataEdmBuilder ODataStartup -n %PROJECT% -o %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models %OPTS%

rem Generate unit tests
codegen -s %STRUCTURES%     -t ODataUnitTests -n %PROJECT%.Test -o %PROJECT%.Test -ut SERVICES_NAMESPACE=%PROJECT% %OPTS%

rem Generate OData model classes for client side use
codegen -s %STRUCTURES%     -t ODataModel -n %PROJECT%.Test.Models -o %PROJECT%.Test\Models %OPTS%

rem Generate Postman Tests
codegen -s %STRUCTURES% -ms -t ODataPostManTests -o .\ %OPTS%

rem ================================================================================================================================
rem The test environment has slightly different requirements, because we need to generate code based on structures, but when tags
rem are used to indicate that multiple structures are associated with a single ISAM file, we only need to generate from one of The
rem structures associated with each file.

set FILE_STRUCTURES=CUSTOMERS ORDERS PLANTS

rem Generate the test environment class
codegen -s %FILE_STRUCTURES% -ms -t ODataTestEnvironment -n %PROJECT% -o %PROJECT% %OPTS%

rem Generate the unit test environment class
codegen -s %FILE_STRUCTURES% -ms -t ODataUnitTestEnvironment -n %PROJECT%.Test -o %PROJECT%.Test -ut SERVICES_NAMESPACE=%PROJECT% %OPTS%

rem ================================================================================================================================
rem Generate code for the TraditionalBridge sample environment

set CODEGEN_TPLDIR=Templates\TraditionalBridge
set PROJECT=TraditionalBridge.Test
set XFPL_SMCPATH=

rem Generate model classes
codegen -s %STRUCTURES%     -t DataObject -n %PROJECT%.Models -o %PROJECT%\Models %OPTS%

rem Generate method dispatcher classes
codegen -smc SampleXfplEnvironment\smc.xml -interface SampleXfplEnv -t DispatcherMethods MethodDispatchers -n %PROJECT% -o %PROJECT% %OPTS% -ut MODELS_NAMESPACE=%PROJECT%.Models -define VERBOSE
codegen -s %STRUCTURES% -ms -t DispatcherStructures -n %PROJECT% -o %PROJECT% %OPTS%

endlocal
popd