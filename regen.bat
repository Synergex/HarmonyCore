@echo off
pushd %~dp0
setlocal
set SolutionDir=%~dp0
rem ================================================================================================================================
rem Enable or disable optional features:

set ENABLE_CREATE_TEST_FILES=-define ENABLE_CREATE_TEST_FILES
set ENABLE_SWAGGER_DOCS=-define ENABLE_SWAGGER_DOCS
set ENABLE_ALTERNATE_KEYS=-define ENABLE_ALTERNATE_KEYS
set ENABLE_RELATIONS=-define ENABLE_RELATIONS
set ENABLE_PROPERTY_ENDPOINTS=-define ENABLE_PROPERTY_ENDPOINTS
set ENABLE_PUT=-define ENABLE_PUT
set ENABLE_PATCH=-define ENABLE_PATCH
set ENABLE_DELETE=-define ENABLE_DELETE
rem set ENABLE_AUTHENTICATION=-define ENABLE_AUTHENTICATION
rem set ENABLE_CASE_SENSITIVE_URL=-define ENABLE_CASE_SENSITIVE_URL
rem set ENABLE_CORS=-define ENABLE_CORS
rem set ENABLE_IIS_SUPPORT=-define ENABLE_IIS_SUPPORT

rem ================================================================================================================================
rem Specify which repository structures we are going to be processing and which projects we're generating into
rem DataStructures is used when processing all of the individual data structures
rem FileStructures may be different if data for multiple structures is stored in a single file. In this case only one of those structures needs to be processed.

set DataStructures=CUSTOMERS ITEMS ORDERS ORDER_ITEMS VENDORS
set FileStructures=CUSTOMERS ITEMS ORDERS ORDER_ITEMS

set ServicesProject=SampleServices
set UnitTestProject=SampleServices.Test
set SelfHostProject=SampleServices.Host

rem ================================================================================================================================
rem Configure standard command line options and the CodeGen environment

set NoReplaceOpts=-e -lf -u UserDefinedTokens.tkn %ENABLE_AUTHENTICATION% %ENABLE_PROPERTY_ENDPOINTS% %ENABLE_CASE_SENSITIVE_URL% %ENABLE_CREATE_TEST_FILES% %ENABLE_CORS% %ENABLE_IIS_SUPPORT% %ENABLE_DELETE% %ENABLE_PUT% %ENABLE_PATCH% %ENABLE_ALTERNATE_KEYS% %ENABLE_SWAGGER_DOCS% %ENABLE_RELATIONS% -i %SolutionDir%Templates -rps %RPSMFIL% %RPSTFIL%
set StdOpts=%NoReplaceOpts% -r

rem ================================================================================================================================
rem Generate a Web API / OData CRUD environment

rem Generate model and metadata classes
codegen -s %DataStructures% -t ODataModel -o %SolutionDir%%ServicesProject%\Models -n %ServicesProject%.Models %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate controller classes
codegen -s %DataStructures% -t ODataController -o %SolutionDir%%ServicesProject%\Controllers -n %ServicesProject%.Controllers %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate the DbContext and EdmBuilder and Startup classes
codegen -s %DataStructures% -ms -t ODataDbContext ODataEdmBuilder ODataStartup -o %SolutionDir%%ServicesProject% -n %ServicesProject% %StdOpts%
if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate the test environment

rem Generate OData model classes for client side use
codegen -s %DataStructures% -t ODataClientModel -o %SolutionDir%%UnitTestProject%\Models -n %UnitTestProject%.Models %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate unit tests
codegen -s %DataStructures% -t ODataUnitTests -o %SolutionDir%%UnitTestProject%\UnitTests -n %UnitTestProject%.UnitTests %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate test context data
codegen -s %DataStructures% -ms -t ODataTestConstantsProperties -o %SolutionDir%%UnitTestProject% -n %UnitTestProject% %StdOpts%
if ERRORLEVEL 1 goto error
rem One time, not replaced!
codegen -s %DataStructures% -ms -t ODataTestConstantsValues -o %SolutionDir%%UnitTestProject% -n %UnitTestProject% %NoReplaceOpts%
if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate documentation and external test environments

rem Generate Postman Tests
codegen -s %DataStructures% -ms -t ODataPostManTests -o %SolutionDir% %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate a Swagger file
if DEFINED ENABLE_SWAGGER_DOCS (
  codegen -s %DataStructures% -ms -t ODataSwaggerYaml -o %SolutionDir%%ServicesProject%\wwwroot %StdOpts%
  if ERRORLEVEL 1 goto error
)

rem Generate the test environment and unit test environment classes, and the self-hosting program
codegen -s %FileStructures% -ms -t ODataTestEnvironment ODataUnitTestEnvironment ODataSelfHost -o %SolutionDir%%UnitTestProject% -n %UnitTestProject% %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate the data loader classes
codegen -s %FileStructures% -t ODataTestDataLoader -o %SolutionDir%%UnitTestProject%\DataGenerators -n %UnitTestProject%.DataGenerators %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate the data generator classes - one time, not replaced!
codegen -s %FileStructures% -t ODataTestDataGenerator -o %SolutionDir%%UnitTestProject%\DataGenerators -n %UnitTestProject%.DataGenerators %NoReplaceOpts%
if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate code for a standalong self-hosting environment

codegen -s %FileStructures% -ms -t ODataStandAloneSelfHost -o %SolutionDir%%SelfHostProject% -n %SelfHostProject% %StdOpts%

rem ================================================================================================================================
rem Generate code for the TraditionalBridge sample environment

rem set CODEGEN_TPLDIR=Templates\TraditionalBridge
rem set PROJECT=TraditionalBridge.Test
rem set SMC_INTERFACE=SampleXfplEnv
rem set XFPL_SMCPATH=

rem Generate model classes
rem codegen -s %DataStructures% -t ODataModel -o %SolutionDir%%PROJECT%\Models -n %PROJECT%.Models -e -r -lf
rem if ERRORLEVEL 1 goto error

rem Generate method dispatcher classes
rem codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceDispatcher -o %SolutionDir%%PROJECT% -n %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models -e -r -lf
rem if ERRORLEVEL 1 goto error
rem codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceMethodDispatchers -o %SolutionDir%%PROJECT% -n %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models -e -r -lf
rem if ERRORLEVEL 1 goto error

rem codegen -s %DataStructures% -ms -t InterfaceDispatcherData -o %SolutionDir%%PROJECT% -n %PROJECT% -ut SMC_INTERFACE=%SMC_INTERFACE% -e -r -lf
rem if ERRORLEVEL 1 goto error

rem ================================================================================================================================
rem Generate OData action return data models

rem set CODEGEN_TPLDIR=Templates\TraditionalBridge
rem set PROJECT=SampleServices.Models
rem set SMC_INTERFACE=SampleXfplEnv
rem set XFPL_SMCPATH=

rem codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t ODataActionModels -o %SolutionDir%%PROJECT% -n %PROJECT% -e -r -lf
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