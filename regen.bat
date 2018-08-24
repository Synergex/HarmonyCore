@echo off
pushd %~dp0
setlocal
rem ================================================================================================================================
rem Comment or uncomment the following lines to enable or disable optional features:

set ENABLE_CREATE_TEST_FILES=-define ENABLE_CREATE_TEST_FILES
rem set ENABLE_AUTHENTICATION=-define ENABLE_AUTHENTICATION
rem set ENABLE_CASE_SENSITIVE_URL=-define ENABLE_CASE_SENSITIVE_URL
rem set ENABLE_CORS=-define ENABLE_CORS
rem set ENABLE_IIS_SUPPORT=-define ENABLE_IIS_SUPPORT
set ENABLE_ALTERNATE_KEYS=-define ENABLE_ALTERNATE_KEYS
set ENABLE_PROPERTY_ENDPOINTS=-define ENABLE_PROPERTY_ENDPOINTS
set ENABLE_PUT=-define ENABLE_PUT
set ENABLE_PATCH=-define ENABLE_PATCH
set ENABLE_DELETE=-define ENABLE_DELETE
set ENABLE_SWAGGER_DOCS=-define ENABLE_SWAGGER_DOCS

rem ================================================================================================================================
rem Configure standard command line options and the CodeGen environment

set NOREPLACEOPTS=-e -lf -u UserDefinedTokens.tkn %ENABLE_AUTHENTICATION% %ENABLE_PROPERTY_ENDPOINTS% %ENABLE_CASE_SENSITIVE_URL% %ENABLE_CREATE_TEST_FILES% %ENABLE_CORS% %ENABLE_IIS_SUPPORT% %ENABLE_DELETE% %ENABLE_PUT% %ENABLE_PATCH% %ENABLE_ALTERNATE_KEYS% %ENABLE_SWAGGER_DOCS%
set STDOPTS=%NOREPLACEOPTS% -r
set CODEGEN_TPLDIR=Templates

rem ================================================================================================================================
rem Generate a Web API / OData CRUD environment

set PROJECT=SampleServices
set STRUCTURES=CUSTOMERS ITEMS ORDERS ORDER_ITEMS VENDORS

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

rem ================================================================================================================================
rem Generate the test environment

rem Generate OData model classes for client side use
codegen -s %STRUCTURES% -t ODataClientModel -n %PROJECT%.Test.Models -o %PROJECT%.Test\Models %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate unit tests
codegen -s %STRUCTURES% -t ODataUnitTests -n %PROJECT%.Test.UnitTests -o %PROJECT%.Test\UnitTests %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate test context data
codegen -s %STRUCTURES% -ms -t ODataTestConstantsProperties -n %PROJECT%.Test -o %PROJECT%.Test %STDOPTS%
if ERRORLEVEL 1 goto error
rem One time, not replaced!
codegen -s %STRUCTURES% -ms -t ODataTestConstantsValues -n %PROJECT%.Test -o %PROJECT%.Test %NOREPLACEOPTS%
if ERRORLEVEL 1 goto error

rem Generate Postman Tests
codegen -s %STRUCTURES% -ms -t ODataPostManTests -o .\ %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate a Swagger file
if DEFINED ENABLE_SWAGGER_DOCS (
  codegen -s %STRUCTURES% -ms -t ODataSwaggerJson ODataSwaggerYaml -o %PROJECT%\wwwroot %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================================================================
rem The test environment has slightly different requirements, because we need to generate code based on structures, but when tags
rem are used to indicate that multiple structures are associated with a single ISAM file, we only need to generate from one of The
rem structures associated with each file.

set FILE_STRUCTURES=CUSTOMERS ITEMS ORDERS ORDER_ITEMS

rem Generate the test environment and unit test environment classes
codegen -s %FILE_STRUCTURES% -ms -t ODataTestEnvironment ODataUnitTestEnvironment ODataSelfHost -n %PROJECT%.Test -o %PROJECT%.Test %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate the data loader classes
codegen -s %FILE_STRUCTURES% -t ODataTestDataLoader -n %PROJECT%.Test.DataGenerators -o %PROJECT%.Test\DataGenerators %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate the data loader classes - one time, not replaced!
codegen -s %FILE_STRUCTURES% -t ODataTestDataGenerator -n %PROJECT%.Test.DataGenerators -o %PROJECT%.Test\DataGenerators %NOREPLACEOPTS%
if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate code for the TraditionalBridge sample environment

set CODEGEN_TPLDIR=Templates\TraditionalBridge
set PROJECT=TraditionalBridge.Test
set SMC_INTERFACE=SampleXfplEnv
set XFPL_SMCPATH=

Generate model classes
codegen -s %STRUCTURES% -t ODataModel -n %PROJECT%.Models -o %PROJECT%\Models -e -r -lf
if ERRORLEVEL 1 goto error

Generate method dispatcher classes
codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceDispatcher -n %PROJECT% -o %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models -e -r -lf
if ERRORLEVEL 1 goto error
codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceMethodDispatchers -n %PROJECT% -o %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models -e -r -lf
if ERRORLEVEL 1 goto error

codegen -s %STRUCTURES% -ms -t InterfaceDispatcherData -n %PROJECT% -o %PROJECT% -ut SMC_INTERFACE=%SMC_INTERFACE% -e -r -lf
if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate OData action return data models

set CODEGEN_TPLDIR=Templates\TraditionalBridge
set PROJECT=SampleServices.Models
set SMC_INTERFACE=SampleXfplEnv
set XFPL_SMCPATH=

codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t ODataActionModels -o SampleServices\Models -n %PROJECT% -e -r -lf
if ERRORLEVEL 1 goto error

echo.
echo DONE!
echo.
goto done

:error
echo *** CODE GENERATION INCOMPLETE ***

:done
endlocal
popd