@echo off
pushd %~dp0
setlocal
set SolutionDir=%~dp0

rem ================================================================================================================================
rem Specify the names of the projects to generate code into:

set ServicesProject=Services
set ModelsProject=Services.Models
set ControllersProject=Services.Controllers
set HostProject=Services.Host
set TestProject=Services.Test

rem ================================================================================================================================
rem Specify the names of the repository structures to generate code from:

set DATA_STRUCTURES=CUSTOMERS ITEMS ORDERS ORDER_ITEMS VENDORS
set DATA_ALIASES=%DATA_STRUCTURES%

set FILE_STRUCTURES=%DATA_STRUCTURES%
set FILE_ALIASES=%DATA_ALIASES%

set CUSTOM_STRUCTURES=
set CUSTOM_ALIASES=%CUSTOM_STRUCTURES%

rem DATA_STRUCTURES     Is a list all structures that you wish to generate models, metadata and
rem                     controllers for. In other words it declares all of the "entities"
rem                     that are being represented and exposed by the environment. The DbContext
rem                     and EdmBuilder classes will be aware of the types associated with These
rem                     structures.
rem
rem FILE_STRUCTURES     If you don't have multi-record format files then this should be the
rem                     same as DATA_STRUCTURES. But if you do then FILE_STRUCTURES should
rem                     only list ONE of the structures assigned to each file, so this list
rem                     will be a subset of DATA_STRUCTURES.
rem
rem CUSTOM_STRUCTURES	Is a list of structures that you wish to generate models and metadata
rem                     for, but which will NOT be exposed to the Entity Framework provider.
rem                     These classes are intended for use only by custom code-based endpoints
rem                     and the DbContext and EdmBuilder classes will know nothing about them.

rem ================================================================================================================================
rem Specify optional "system parameter file" structure

set PARAMSTR=SYSPARAMS

rem In the sammple environment the system parameter file is a relative file that contains
rem "next available record number" data for use in conjunction with POST (create with automated
rem primary key assignment) operaitons. Naming the structure associated with that file here
rem ensures that a copy of that file will be made available in the sample self-host environment
rem along with other data files in the sample data folder. This mechanism will require customization
rem for use in other environments.

rem ================================================================================================================================
rem Comment or uncomment the following lines to enable or disable optional features:

rem Note that the ENABLE_SWAGGER_DOCS and ENABLE_API_VERSIONING are mutually exclusive.

set ENABLE_GET_ALL=-define ENABLE_GET_ALL
set ENABLE_GET_ONE=-define ENABLE_GET_ONE
set ENABLE_SELF_HOST_GENERATION=YES
set ENABLE_CREATE_TEST_FILES=-define ENABLE_CREATE_TEST_FILES
set ENABLE_SWAGGER_DOCS=-define ENABLE_SWAGGER_DOCS
rem set ENABLE_API_VERSIONING=-define ENABLE_API_VERSIONING
set ENABLE_POSTMAN_TESTS=YES
set ENABLE_ALTERNATE_KEYS=-define ENABLE_ALTERNATE_KEYS
set ENABLE_COUNT=-define ENABLE_COUNT
set ENABLE_PROPERTY_ENDPOINTS=-define ENABLE_PROPERTY_ENDPOINTS
rem set ENABLE_PROPERTY_VALUE_DOCS=-define ENABLE_PROPERTY_VALUE_DOCS
set ENABLE_SELECT=-define ENABLE_SELECT
set ENABLE_FILTER=-define ENABLE_FILTER
set ENABLE_ORDERBY=-define ENABLE_ORDERBY
set ENABLE_TOP=-define ENABLE_TOP
set ENABLE_SKIP=-define ENABLE_SKIP
set ENABLE_RELATIONS=-define ENABLE_RELATIONS
set ENABLE_PUT=-define ENABLE_PUT
set ENABLE_POST=-define ENABLE_POST
set ENABLE_PATCH=-define ENABLE_PATCH
set ENABLE_DELETE=-define ENABLE_DELETE
set ENABLE_SPROC=-define ENABLE_SPROC
set ENABLE_ADAPTER_ROUTING=-define ENABLE_ADAPTER_ROUTING
rem set ENABLE_AUTHENTICATION=-define ENABLE_AUTHENTICATION
rem set ENABLE_CUSTOM_AUTHENTICATION=-define ENABLE_CUSTOM_AUTHENTICATION
rem set ENABLE_FIELD_SECURITY=-define ENABLE_FIELD_SECURITY
set ENABLE_UNIT_TEST_GENERATION=YES
rem set ENABLE_CASE_SENSITIVE_URL=-define ENABLE_CASE_SENSITIVE_URL
rem set ENABLE_CORS=-define ENABLE_CORS
rem set ENABLE_IIS_SUPPORT=-define ENABLE_IIS_SUPPORT
set ENABLE_OVERLAYS=-f o
rem set ENABLE_ALTERNATE_FIELD_NAMES=-af
rem set ENABLE_READ_ONLY_PROPERTIES=-define ENABLE_READ_ONLY_PROPERTIES

if not "NONE%ENABLE_SELECT%%ENABLE_FILTER%%ENABLE_ORDERBY%%ENABLE_TOP%%ENABLE_SKIP%%ENABLE_RELATIONS%"=="NONE" (
  set PARAM_OPTIONS_PRESENT=-define PARAM_OPTIONS_PRESENT
)

rem ================================================================================================================================
rem Configure standard command line options and the CodeGen environment

if "%COMPUTERNAME%"=="SIVES" (
  set USERTOKENFILE=UserDefinedTokensSteve.tkn
) else (
  set USERTOKENFILE=UserDefinedTokens.tkn
)
echo.
echo User token file is %USERTOKENFILE%

set NOREPLACEOPTS=-e -lf -u %SolutionDir%%USERTOKENFILE% %ENABLE_GET_ALL% %ENABLE_GET_ONE% %ENABLE_OVERLAYS% %ENABLE_ALTERNATE_FIELD_NAMES% %ENABLE_AUTHENTICATION% %ENABLE_CUSTOM_AUTHENTICATION% %ENABLE_FIELD_SECURITY% %ENABLE_PROPERTY_ENDPOINTS% %ENABLE_PROPERTY_VALUE_DOCS% %ENABLE_CASE_SENSITIVE_URL% %ENABLE_CREATE_TEST_FILES% %ENABLE_CORS% %ENABLE_IIS_SUPPORT% %ENABLE_DELETE% %ENABLE_PUT% %ENABLE_POST% %ENABLE_PATCH% %ENABLE_ALTERNATE_KEYS% %ENABLE_SWAGGER_DOCS% %ENABLE_API_VERSIONING% %ENABLE_RELATIONS% %ENABLE_SELECT% %ENABLE_FILTER% %ENABLE_ORDERBY% %ENABLE_COUNT% %ENABLE_TOP% %ENABLE_SKIP% %ENABLE_SPROC% %ENABLE_ADAPTER_ROUTING% %ENABLE_READ_ONLY_PROPERTIES% %PARAM_OPTIONS_PRESENT% -i %SolutionDir%Templates -rps %RPSMFIL% %RPSTFIL%
set STDOPTS=%NOREPLACEOPTS% -r

rem ================================================================================================================================
rem Generate a Web API / OData CRUD environment

rem Generate model and metadata classes
codegen -s %DATA_STRUCTURES% %CUSTOM_STRUCTURES% ^
        -a %DATA_ALIASES% %CUSTOM_ALIASES% ^
        -t ODataModel ODataMetaData ^
        -o %SolutionDir%%ModelsProject% ^
        -n %ModelsProject% ^
           %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate controller classes
codegen -s %DATA_STRUCTURES% ^
        -a %DATA_ALIASES% ^
        -t ODataController ^
        -o %SolutionDir%%ControllersProject% ^
        -n %ControllersProject% ^
           %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate the DbContext class
codegen -s %DATA_STRUCTURES% -ms ^
        -a %DATA_ALIASES% ^
        -t ODataDbContext ^
        -o %SolutionDir%%ModelsProject% ^
        -n %ModelsProject% ^
           %STDOPTS%
if ERRORLEVEL 1 goto error

rem Generate the EdmBuilder and Startup classes
codegen -s %DATA_STRUCTURES% -ms ^
        -a %DATA_ALIASES% ^
        -t ODataEdmBuilder ODataStartup ^
        -o %SolutionDir%%ServicesProject% ^
        -n %ServicesProject% ^
        -ut CONTROLLERS_NAMESPACE=%ControllersProject% MODELS_NAMESPACE=%ModelsProject% ^
           %STDOPTS%
if ERRORLEVEL 1 goto error

rem ================================================================================
rem Self hosting

if DEFINED ENABLE_SELF_HOST_GENERATION (
  codegen -s %FILE_STRUCTURES% %PARAMSTR% -ms ^
          -a %FILE_ALIASES% ^
          -t ODataSelfHost ODataSelfHostEnvironment ^
          -o %SolutionDir%%HostProject% ^
          -n %HostProject% ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Swagger documentation and Postman tests

rem Generate a Swagger files
if DEFINED ENABLE_SWAGGER_DOCS (
  codegen -s %DATA_STRUCTURES% -ms ^
          -a %DATA_ALIASES% ^
          -t ODataSwaggerYaml ^
          -o %SolutionDir%%ServicesProject%\wwwroot ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error

  codegen -s %DATA_STRUCTURES% ^
          -a %DATA_ALIASES% ^
          -t ODataSwaggerType ^
          -o %SolutionDir%%ServicesProject%\wwwroot ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem Generate Postman Tests
if DEFINED ENABLE_POSTMAN_TESTS (
  codegen -s %DATA_STRUCTURES% -ms ^
          -a %DATA_ALIASES% ^
          -t ODataPostManTests ^
          -o %SolutionDir% ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Unit testing project

if DEFINED ENABLE_UNIT_TEST_GENERATION (

  rem Generate OData client model, data loader and unit test classes
  codegen -s %DATA_STRUCTURES% ^
          -a %DATA_ALIASES% ^
          -t ODataClientModel ODataTestDataLoader ODataUnitTests ^
          -o %SolutionDir%%TestProject% -tf ^
          -n %TestProject% ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the test environment
  codegen -s %FILE_STRUCTURES% %PARAMSTR% -ms ^
          -a %FILE_ALIASES% ^
          -t ODataTestEnvironment ^
          -o %SolutionDir%%TestProject% ^
          -n %TestProject% ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the unit test environment class, and the unit test hosting program
  codegen -s %FILE_STRUCTURES% -ms ^
          -a %FILE_ALIASES% ^
          -t ODataUnitTestEnvironment ODataUnitTestHost ^
          -o %SolutionDir%%TestProject% ^
          -n %TestProject% ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the unit test constants properties classes
  codegen -s %DATA_STRUCTURES% -ms ^
          -a %DATA_ALIASES% ^
          -t ODataTestConstantsProperties ^
          -o %SolutionDir%%TestProject% ^
          -n %TestProject% ^
             %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate unit test constants values class; one time, not replaced
  codegen -s %DATA_STRUCTURES% -ms ^
          -a %DATA_ALIASES% ^
          -t ODataTestConstantsValues ^
          -o %SolutionDir%%TestProject% ^
          -n %TestProject% ^
             %NOREPLACEOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Generate code for the TraditionalBridge sample environment

set CODEGEN_TPLDIR=Templates\TraditionalBridge
set PROJECT=TraditionalBridge.Test
set SMC_INTERFACE=SampleXfplEnv
set XFPL_SMCPATH=

rem Generate model classes
codegen -s %DATA_STRUCTURES% ^
        -t ODataModel ^
        -o %SolutionDir%%PROJECT%\Models ^
        -n %PROJECT%.Models ^
        -e -r -lf
if ERRORLEVEL 1 goto error

rem Generate method dispatcher classes
codegen -smc XfplEnvironment\smc.xml ^
        -interface %SMC_INTERFACE% ^
        -t InterfaceDispatcher ^
        -o %SolutionDir%%PROJECT% ^
        -n %PROJECT% ^
        -ut MODELS_NAMESPACE=%PROJECT%.Models ^
        -e -r -lf
if ERRORLEVEL 1 goto error

codegen -smc XfplEnvironment\smc.xml ^
        -interface %SMC_INTERFACE% ^
        -t InterfaceMethodDispatchers ^
        -o %SolutionDir%%PROJECT% ^
        -n %PROJECT% ^
        -ut MODELS_NAMESPACE=%PROJECT%.Models ^
        -e -r -lf
if ERRORLEVEL 1 goto error

codegen -s %DATA_STRUCTURES% -ms ^
        -t InterfaceDispatcherData ^
        -o %SolutionDir%%PROJECT% ^
        -n %PROJECT% ^
        -ut SMC_INTERFACE=%SMC_INTERFACE% ^
        -e -r -lf
if ERRORLEVEL 1 goto error

rem ================================================================================
rem Generate OData action return data models

rem set CODEGEN_TPLDIR=Templates\TraditionalBridge
rem set PROJECT=Services.Models
rem set SMC_INTERFACE=SampleXfplEnv
rem set XFPL_SMCPATH=

rem codegen -smc XfplEnvironment\smc.xml ^
rem         -interface %SMC_INTERFACE% ^
rem         -t ODataActionModels ^
rem         -o %SolutionDir%%PROJECT% ^
rem         -n %PROJECT% ^
rem         -e -r -lf
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