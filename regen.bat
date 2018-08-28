@echo off
pushd %~dp0
setlocal
set SolutionDir=%~dp0
rem ================================================================================================================================
rem Enable or disable optional features:

set ENABLE_SWAGGER_DOCS=-define ENABLE_SWAGGER_DOCS
set ENABLE_ALTERNATE_KEYS=-define ENABLE_ALTERNATE_KEYS
set ENABLE_COUNT=-define ENABLE_COUNT
set ENABLE_PROPERTY_ENDPOINTS=-define ENABLE_PROPERTY_ENDPOINTS
set ENABLE_SELECT=-define ENABLE_SELECT
set ENABLE_FILTER=-define ENABLE_FILTER
set ENABLE_ORDERBY=-define ENABLE_ORDERBY
set ENABLE_TOP=-define ENABLE_TOP
set ENABLE_SKIP=-define ENABLE_SKIP
set ENABLE_RELATIONS=-define ENABLE_RELATIONS
set ENABLE_PUT=-define ENABLE_PUT
set ENABLE_PATCH=-define ENABLE_PATCH
set ENABLE_DELETE=-define ENABLE_DELETE
rem set ENABLE_AUTHENTICATION=-define ENABLE_AUTHENTICATION
rem set ENABLE_CASE_SENSITIVE_URL=-define ENABLE_CASE_SENSITIVE_URL
rem set ENABLE_CORS=-define ENABLE_CORS
rem set ENABLE_IIS_SUPPORT=-define ENABLE_IIS_SUPPORT
set ENABLE_CREATE_TEST_FILES=-define ENABLE_CREATE_TEST_FILES

set ENABLE_POSTMAN_TESTS=YES
set ENABLE_SELF_HOST_GENERATION=YES
set ENABLE_UNIT_TEST_GENERATION=YES

if not "NONE%ENABLE_SELECT%%ENABLE_FILTER%%ENABLE_ORDERBY%%ENABLE_TOP%%ENABLE_SKIP%%ENABLE_RELATIONS%"=="NONE" (
  set PARAM_OPTIONS_PRESENT=-define PARAM_OPTIONS_PRESENT
)

rem ================================================================================================================================
rem Specify which repository structures we are going to be processing and which projects we're generating into
rem DataStructures is used when processing all of the individual data structures
rem FileStructures may be different if data for multiple structures is stored in a single file. In this case only one of those structures needs to be processed.

set ServicesProject=SampleServices
set HostProject=SampleServices.Host
set TestProject=SampleServices.Test

set DataStructures=CUSTOMERS ITEMS ORDERS ORDER_ITEMS VENDORS
set FileStructures=CUSTOMERS ITEMS ORDERS ORDER_ITEMS

rem ================================================================================================================================
rem Configure standard command line options and the CodeGen environment

set NoReplaceOpts=-e -lf -u %SolutionDir%UserDefinedTokens.tkn %ENABLE_AUTHENTICATION% %ENABLE_PROPERTY_ENDPOINTS% %ENABLE_CASE_SENSITIVE_URL% %ENABLE_CREATE_TEST_FILES% %ENABLE_CORS% %ENABLE_IIS_SUPPORT% %ENABLE_DELETE% %ENABLE_PUT% %ENABLE_PATCH% %ENABLE_ALTERNATE_KEYS% %ENABLE_SWAGGER_DOCS% %ENABLE_RELATIONS% %ENABLE_SELECT% %ENABLE_FILTER% %ENABLE_ORDERBY% %ENABLE_COUNT% %ENABLE_TOP% %ENABLE_SKIP% %PARAM_OPTIONS_PRESENT% -i %SolutionDir%Templates -rps %RPSMFIL% %RPSTFIL%
set StdOpts=%NoReplaceOpts% -r

rem ================================================================================================================================
rem Generate a Web API / OData CRUD environment

rem Generate model, metadata and controller classes
codegen -s %DataStructures% -t ODataModel ODataMetaData ODataController -tf -o %SolutionDir%%ServicesProject% -n %ServicesProject% %StdOpts%
if ERRORLEVEL 1 goto error

rem Generate the DbContext and EdmBuilder and Startup classes
codegen -s %DataStructures% -ms -t ODataDbContext ODataEdmBuilder ODataStartup -o %SolutionDir%%ServicesProject% -n %ServicesProject% %StdOpts%
if ERRORLEVEL 1 goto error

rem ================================================================================
rem Self hosting

if DEFINED ENABLE_SELF_HOST_GENERATION (
  codegen -s %FileStructures% -ms -t ODataStandAloneSelfHost -o %SolutionDir%%HostProject% -n %HostProject% %StdOpts%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Swagger documentation and Postman tests

rem Generate a Swagger file
if DEFINED ENABLE_SWAGGER_DOCS (
  codegen -s %DataStructures% -ms -t ODataSwaggerYaml -o %SolutionDir%%ServicesProject%\wwwroot %StdOpts%
  if ERRORLEVEL 1 goto error
)

rem Generate Postman Tests
if DEFINED ENABLE_POSTMAN_TESTS (
  codegen -s %DataStructures% -ms -t ODataPostManTests -o %SolutionDir% %StdOpts%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================================================================
rem Unit testing project

if DEFINED ENABLE_UNIT_TEST_GENERATION (

  rem Generate OData client model, data loader and unit test classes
  codegen -s %DataStructures% -t ODataClientModel ODataTestDataLoader ODataTestDataGenerator ODataUnitTests -tf -o %SolutionDir%%TestProject% -n %TestProject% %StdOpts%
  if ERRORLEVEL 1 goto error

  rem Generate data generator classes; one time, not replaced
  codegen -s %DataStructures% -t ODataClientModel ODataTestDataLoader ODataTestDataGenerator ODataUnitTests -tf -o %SolutionDir%%TestProject% -n %TestProject% %NoReplaceOpts%
  if ERRORLEVEL 1 goto error

  rem Generate the test environment and unit test environment classes, and the self-hosting program
  codegen -s %FileStructures% -ms -t ODataTestEnvironment ODataUnitTestEnvironment ODataSelfHost -o %SolutionDir%%TestProject% -n %TestProject% %StdOpts%
  if ERRORLEVEL 1 goto error

  rem Generate the unit test constants properties classes
  codegen -s %DataStructures% -ms -t ODataTestConstantsProperties -o %SolutionDir%%TestProject% -n %TestProject% %StdOpts%
  if ERRORLEVEL 1 goto error

  rem Generate unit test constants values class; one time, not replaced
  codegen -s %DataStructures% -ms -t ODataTestConstantsValues -o %SolutionDir%%TestProject% -n %TestProject% %NoReplaceOpts%
  if ERRORLEVEL 1 goto error

)

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