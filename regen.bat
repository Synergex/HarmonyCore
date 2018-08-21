@echo off
pushd %~dp0
setlocal
rem ================================================================================================================================
rem Configure optional Harmony Core capabilities

rem Comment or uncomment the following lines to enable or disable optional capabilities as required:

rem set DO_AUTHENTICATION=-define AUTHENTICATION
set DO_PROPERTY_ENDPOINTS=-define PROPERTY_ENDPOINTS
rem set DO_CASE_SENSITIVE_URL=-define CASE_SENSITIVE_URL
set DO_CREATE_TEST_FILES=-define CREATE_TEST_FILES
rem set DO_ENABLE_CORS=-define ENABLE_CORS
rem set DO_IIS_SUPPORT=-define IIS_SUPPORT

rem ================================================================================================================================
rem Configure standard command line options and the VodeGen environment

set NOREPLACEOPTS=-e -lf -u UserDefinedTokens.tkn %DO_AUTHENTICATION% %DO_PROPERTY_ENDPOINTS% %DO_CASE_SENSITIVE_URL% %DO_CREATE_TEST_FILES% %DO_ENABLE_CORS% %DO_IIS_SUPPORT%
set STDOPTS=%NOREPLACEOPTS% -r
set CODEGEN_TPLDIR=Templates

rem ================================================================================================================================
rem Generate a Web API / OData CRUD environment

set PROJECT=SampleServices
set STRUCTURES=CUSTOMERS ORDERS ORDER_ITEMS PLANTS VENDORS

rem Generate model and metadata classes
codegen -s %STRUCTURES% -t ODataModel -n %PROJECT%.Models -o %PROJECT%\Models %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate the DbContext and EdmBuilder classes
codegen -s %STRUCTURES% -ms -t ODataDbContext ODataEdmBuilder -n %PROJECT% -o %PROJECT% %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate controller classes
codegen -s %STRUCTURES% -t ODataController -n %PROJECT%.Controllers -o %PROJECT%\Controllers %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate the Startup class
codegen -s %STRUCTURES% -ms -t ODataStartup -n %PROJECT% -o %PROJECT% %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate unit tests
codegen -s %STRUCTURES% -t ODataUnitTests -n %PROJECT%.Test.UnitTests -o %PROJECT%.Test\UnitTests %STDOPTS%
if ERRORLEVEL 1 goto error
codegen -s %STRUCTURES% -ms -t ODataTestContext -n %PROJECT%.Test -o %PROJECT%.Test %STDOPTS%
if ERRORLEVEL 1 goto error

rem One time, not replaced!
codegen -s %STRUCTURES% -ms -t ODataTestData -n %PROJECT%.Test -o %PROJECT%.Test %NOREPLACEOPTS%
if ERRORLEVEL 1 goto error

rem Generate OData model classes for client side use
codegen -s %STRUCTURES% -t ODataClientModel -n %PROJECT%.Test.Models -o %PROJECT%.Test\Models %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate Postman Tests
codegen -s %STRUCTURES% -ms -t ODataPostManTests -o .\ %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate a Swagger file
codegen -s %STRUCTURES% -ms -t ODataSwaggerJson -o %PROJECT%\wwwroot %STDOPTS%
if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem The test environment has slightly different requirements, because we need to generate code based on structures, but when tags
rem are used to indicate that multiple structures are associated with a single ISAM file, we only need to generate from one of The
rem structures associated with each file.

set FILE_STRUCTURES=CUSTOMERS ORDERS ORDER_ITEMS PLANTS

rem Generate the test environment and unit test environment classes
codegen -s %FILE_STRUCTURES% -ms -t ODataTestEnvironment ODataUnitTestEnvironment ODataSelfHost -n %PROJECT%.Test -o %PROJECT%.Test %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate the data loader and generatror classes
codegen -s %FILE_STRUCTURES% -t ODataTestDataLoader    -n %PROJECT%.Test -o %PROJECT%.Test\DataGenerators %STDOPTS%
if ERRORLEVEL 1 goto error
rem One time, not replaced!
codegen -s %FILE_STRUCTURES% -t ODataTestDataGenerator -n %PROJECT%.Test -o %PROJECT%.Test\DataGenerators %NOREPLACEOPTS%
if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate code for the TraditionalBridge sample environment

rem set CODEGEN_TPLDIR=Templates\TraditionalBridge
rem set PROJECT=TraditionalBridge.Test
rem set SMC_INTERFACE=SampleXfplEnv
rem set XFPL_SMCPATH=

rem Generate model classes
rem codegen -s %STRUCTURES% -t ODataModel -n %PROJECT%.Models -o %PROJECT%\Models -e -r -lf
rem if ERRORLEVEL 1 goto error

rem Generate method dispatcher classes
rem codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceDispatcher -n %PROJECT% -o %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models -e -r -lf
rem if ERRORLEVEL 1 goto error
rem codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceMethodDispatchers -n %PROJECT% -o %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models -e -r -lf
rem if ERRORLEVEL 1 goto error

rem codegen -s %STRUCTURES% -ms -t InterfaceDispatcherData -n %PROJECT% -o %PROJECT% -ut SMC_INTERFACE=%SMC_INTERFACE% -e -r -lf
rem if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate OData action return data models

rem set CODEGEN_TPLDIR=Templates
rem set CODEGEN_OUTDIR=SampleServices\Models
rem set PROJECT=SampleServices.Models
rem set SMC_INTERFACE=SampleXfplEnv
rem set XFPL_SMCPATH=

rem codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t ODataActionModels -n %PROJECT% -e -r -lf
rem if ERRORLEVEL 1 goto error

echo.
echo DONE!
echo.
goto done

:error
echo *** CODE GENERATION INCOMPLETE ***

:done
endlocal
popd