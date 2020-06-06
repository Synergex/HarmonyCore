@echo off
setlocal

set SolutionDir=%~dp0

pushd "%SolutionDir%"

if not defined RPSMFIL (
    echo RPSMFIL is not defined!
    goto error
)

if not defined RPSTFIL (
    echo RPSTFIL is not defined!
    goto error
)

rem ================================================================================================================================
rem Specify the names of the projects to generate code into:

set ServicesProject=Services
set ModelsProject=Services.Models
set ControllersProject=Services.Controllers
set HostProject=Services.Host
set TestProject=Services.Test
set TraditionalBridgeProject=TraditionalBridge

rem ================================================================================================================================
rem Specify the names of the repository structures to generate code from:

set DATA_STRUCTURES=CUSTOMERS CUSTOMER_NOTES ITEMS ORDERS ORDER_ITEMS VENDORS
set DATA_ALIASES=%DATA_STRUCTURES%
set DATA_FILES=%DATA_STRUCTURES%

set FILE_STRUCTURES=%DATA_STRUCTURES%
set FILE_ALIASES=%DATA_ALIASES%
set FILE_FILES=%DATA_FILES%

set CUSTOM_STRUCTURES=
set CUSTOM_ALIASES=%CUSTOM_STRUCTURES%

set BRIDGE_STRUCTURES=
set BRIDGE_ALIASES=%BRIDGE_STRUCTURES%

rem DATA_STRUCTURES     Is a list all structures that you wish to generate models, metadata and
rem                     controllers for. In other words it declares all of the "entities"
rem                     that are being represented and exposed by the OData environment. The
rem                     DbContextand EdmBuilder classes will be aware of the types associated
rem                     with These structures.
rem
rem DATA_ALIASES        Is a list of alias names for the structures listed in DATA_STRUCTURES.
rem                     If you wish to provide alternate names for the structures being exposed
rem                     then list them here. Specify an alias name for each structure.
rem
rem DATA_FILES          Is a list of the repository file definition names that are associated
rem                     with each structure listed in DATA_STRUCTURES. If you have a one to ONE
rem                     mapping from structures to files then you can leave the setting to
rem                     default to the same value as DATA_STRUCTURES, but if your structure and
rem                     file definitions are different, especially if you have structures that
rem                     are assigned to multiple file definitions, then it is important to list
rem                     the correct file definition assignment for each structure.
rem
rem FILE_STRUCTURES     If you don't have multi-record format files then this should be the
rem                     same as DATA_STRUCTURES. But if you do then FILE_STRUCTURES should
rem                     only list ONE of the structures assigned to each file, so this list
rem                     will be a subset of DATA_STRUCTURES.
rem
rem FILE_ALIASES        Optional aliases for the structures listed in FILE_STRUCTURES
rem
rem FILE_FILES          Repository file assignments for the structures listed in FILE_STRUCTURES
rem
rem CUSTOM_STRUCTURES	Is a list of structures that you wish to generate models and metadata
rem                     for, but which will NOT be exposed to the Entity Framework provider.
rem                     These classes are intended for use only by custom code-based endpoints
rem                     and the DbContext and EdmBuilder classes will know nothing about them.
rem
rem CUSTOM_ALIASES      Optional aliases for the structures listed in CUSTOM_STRUCTURES

rem BRIDGE_STRUCTURES	Is a list of structures that you wish to generate models and metadata
rem                     for use with a Traditional Bridge environment. These types will NOT
rem                     be exposed to the Entity Framework provider.
rem
rem BRIDGE_ALIASES      Optional aliases for the structures listed in BRIDGE_STRUCTURES

rem ================================================================================================================================
rem Comment or uncomment the following lines to enable or disable optional features:

set ENABLE_ODATA_ENVIRONMENT=YES
set ENABLE_SELF_HOST_GENERATION=YES
set ENABLE_CREATE_TEST_FILES=-define ENABLE_CREATE_TEST_FILES
rem set DO_NOT_SET_FILE_LOGICALS=-define DO_NOT_SET_FILE_LOGICALS
set ENABLE_GET_ALL=-define ENABLE_GET_ALL
set ENABLE_GET_ONE=-define ENABLE_GET_ONE
set ENABLE_POSTMAN_TESTS=YES
set ENABLE_ALTERNATE_KEYS=-define ENABLE_ALTERNATE_KEYS
set ENABLE_COUNT=-define ENABLE_COUNT
rem set ENABLE_PROPERTY_ENDPOINTS=-define ENABLE_PROPERTY_ENDPOINTS
set ENABLE_SELECT=-define ENABLE_SELECT
set ENABLE_FILTER=-define ENABLE_FILTER
set ENABLE_ORDERBY=-define ENABLE_ORDERBY
set ENABLE_TOP=-define ENABLE_TOP
set ENABLE_SKIP=-define ENABLE_SKIP
set ENABLE_RELATIONS=-define ENABLE_RELATIONS
set ENABLE_RELATIONS_VALIDATION=-define ENABLE_RELATIONS_VALIDATION
set ENABLE_PUT=-define ENABLE_PUT
set ENABLE_POST=-define ENABLE_POST
set ENABLE_PATCH=-define ENABLE_PATCH
set ENABLE_DELETE=-define ENABLE_DELETE
set ENABLE_SPROC=-define ENABLE_SPROC
set ENABLE_ADAPTER_ROUTING=-define ENABLE_ADAPTER_ROUTING
rem set ENABLE_AUTHENTICATION=-define ENABLE_AUTHENTICATION
rem set ENABLE_CUSTOM_AUTHENTICATION=-define ENABLE_CUSTOM_AUTHENTICATION
rem set ENABLE_FIELD_SECURITY=-define ENABLE_FIELD_SECURITY
rem set ENABLE_SIGNALR=-define ENABLE_SIGNALR
set ENABLE_UNIT_TEST_GENERATION=YES
rem set ENABLE_CASE_SENSITIVE_URL=-define ENABLE_CASE_SENSITIVE_URL
rem set ENABLE_CORS=-define ENABLE_CORS
rem set ENABLE_IIS_SUPPORT=-define ENABLE_IIS_SUPPORT
set ENABLE_OVERLAYS=-f o
rem set ENABLE_ALTERNATE_FIELD_NAMES=-af
rem set ENABLE_READ_ONLY_PROPERTIES=-define ENABLE_READ_ONLY_PROPERTIES
rem set ENABLE_TRADITIONAL_BRIDGE_GENERATION=YES
rem set ENABLE_XFSERVERPLUS_MIGRATION=YES
rem set ENABLE_XFSERVERPLUS_MODEL_GENERATION=YES
rem set ENABLE_BRIDGE_SAMPLE_DISPATCHERS=-define ENABLE_BRIDGE_SAMPLE_DISPATCHERS
rem set ENABLE_BRIDGE_OPTIONAL_PARAMETERS=YES
set ENABLE_NEWTONSOFT=-define ENABLE_NEWTONSOFT

if not "NONE%ENABLE_SELECT%%ENABLE_FILTER%%ENABLE_ORDERBY%%ENABLE_TOP%%ENABLE_SKIP%%ENABLE_RELATIONS%"=="NONE" (
  set PARAM_OPTIONS_PRESENT=-define PARAM_OPTIONS_PRESENT
)

rem ================================================================================================================================
rem Configure standard command line options and the CodeGen environment

set NOREPLACEOPTS=-e -lf -u %SolutionDir%UserDefinedTokens.tkn %ENABLE_GET_ALL% %ENABLE_GET_ONE% %ENABLE_OVERLAYS% %DO_NOT_SET_FILE_LOGICALS% %ENABLE_ALTERNATE_FIELD_NAMES% %ENABLE_AUTHENTICATION% %ENABLE_CUSTOM_AUTHENTICATION% %ENABLE_SIGNALR% %ENABLE_FIELD_SECURITY% %ENABLE_PROPERTY_ENDPOINTS% %ENABLE_CASE_SENSITIVE_URL% %ENABLE_CREATE_TEST_FILES% %ENABLE_CORS% %ENABLE_IIS_SUPPORT% %ENABLE_DELETE% %ENABLE_PUT% %ENABLE_POST% %ENABLE_PATCH% %ENABLE_ALTERNATE_KEYS% %ENABLE_RELATIONS% %ENABLE_RELATIONS_VALIDATION% %ENABLE_SELECT% %ENABLE_FILTER% %ENABLE_ORDERBY% %ENABLE_COUNT% %ENABLE_TOP% %ENABLE_SKIP% %ENABLE_SPROC% %ENABLE_ADAPTER_ROUTING% %ENABLE_READ_ONLY_PROPERTIES% %ENABLE_NEWTONSOFT% %PARAM_OPTIONS_PRESENT% -rps %RPSMFIL% %RPSTFIL%
set STDOPTS=%NOREPLACEOPTS% -r

rem ================================================================================================================================
rem Generate a Web API / OData CRUD environment

if DEFINED ENABLE_ODATA_ENVIRONMENT (

  rem Generate model and metadata classes
  codegen -s  %DATA_STRUCTURES% %CUSTOM_STRUCTURES% ^
          -a  %DATA_ALIASES% %CUSTOM_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataModel ODataMetaData ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ModelsProject% ^
          -n  %ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
  
  rem Generate controller classes
  codegen -s  %DATA_STRUCTURES% ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataController ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ControllersProject% ^
          -n  %ControllersProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
  
if DEFINED ENABLE_PROPERTY_ENDPOINTS (
  rem Generate partial controller class for individual property endpoints

  codegen -s  %DATA_STRUCTURES% ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataControllerPropertyEndpoints ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ControllersProject% ^
          -n  %ControllersProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)
  rem Generate the DbContext class
  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataDbContext ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ModelsProject% ^
          -n  %ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
  
  rem Generate the EdmBuilder and Startup classes
  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataEdmBuilder ODataStartup ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ServicesProject% ^
          -n  %ServicesProject% ^
          -ut CONTROLLERS_NAMESPACE=%ControllersProject% MODELS_NAMESPACE=%ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Self hosting

if DEFINED ENABLE_SELF_HOST_GENERATION (

  codegen -s  %FILE_STRUCTURES% -ms ^
          -a  %FILE_ALIASES% ^
		  -fo %FILE_FILES% ^
          -t  ODataSelfHost ODataSelfHostEnvironment ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%HostProject% ^
          -n  %HostProject% ^
          -ut SERVICES_NAMESPACE=%ServicesProject% MODELS_NAMESPACE=%ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Postman tests

rem Generate Postman Tests
if DEFINED ENABLE_POSTMAN_TESTS (
  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataPostManTests ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Custom Authentication Example

if DEFINED ENABLE_CUSTOM_AUTHENTICATION (

  rem Generate AuthenticationModels.dbl
  codegen -t  ODataCustomAuthModels ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ModelsProject% ^
          -n  %ModelsProject% ^
              %NOREPLACEOPTS%
  echo Note: 1 file failed can be normal here, the file exists and should not be replaced
  if ERRORLEVEL 1 goto error

  rem Generate AuthenticationController.dbl and AuthenticationTools.dbl
  codegen -t  ODataCustomAuthController ODataCustomAuthTools ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ControllersProject% ^
          -n  %ControllersProject% ^
              %NOREPLACEOPTS%
  echo Note: 2 files failed can be normal here, the files exists and should not be replaced
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Unit testing project

if DEFINED ENABLE_UNIT_TEST_GENERATION (

  rem Generate OData client model, data loader and unit test classes
  codegen -s  %DATA_STRUCTURES% ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataClientModel ODataTestDataLoader ODataUnitTests ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestProject% -tf ^
          -n  %TestProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the unit test environment class, and the unit test hosting program
  codegen -s  %FILE_STRUCTURES% -ms ^
          -a  %FILE_ALIASES% ^
		  -fo %FILE_FILES% ^
          -t  ODataUnitTestEnvironment ODataUnitTestHost ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestProject% ^
          -n  %TestProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the unit test constants properties classes
  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataTestConstantsProperties ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestProject% ^
          -n  %TestProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate unit test constants values class; one time, not replaced
  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
		  -fo %DATA_FILES% ^
          -t  ODataTestConstantsValues ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestProject% ^
          -n  %TestProject% ^
              %NOREPLACEOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Generate code for the TraditionalBridge sample environment

if DEFINED ENABLE_TRADITIONAL_BRIDGE_GENERATION (

  rem Generate model classes
  codegen -s %BRIDGE_STRUCTURES% ^
          -a %BRIDGE_ALIASES% ^
          -t ODataModel ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\source ^
          -n %TraditionalBridgeProject% ^
          -e -r -lf
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Generate code for the TraditionalBridge sample environment

set SMC_XML_FILE=
set SMC_INTERFACE=
rem set BRIDGE_DISPATCHER_TEMPLATE=InterfaceMethodDispatchers
set XFPL_SMCPATH=
set NOREPLACEOPTS=-e -lf -rps %RPSMFIL% %RPSTFIL% %ENABLE_AUTHENTICATION% %ENABLE_BRIDGE_SAMPLE_DISPATCHERS%
set STDOPTS=%NOREPLACEOPTS% -r

if DEFINED ENABLE_BRIDGE_OPTIONAL_PARAMETERS (
  set BRIDGE_DISPATCHER_TEMPLATE=OptionalParameterMethodDispatchers
  ) else (
  set BRIDGE_DISPATCHER_TEMPLATE=InterfaceMethodDispatchers
)

if DEFINED ENABLE_XFSERVERPLUS_MIGRATION (
  call :GenerateCodeForInterface %SMC_INTERFACE%
)

rem ================================================================================
rem Generate code for the Traditional Bridge SignalR sample environment

if DEFINED ENABLE_SIGNALR (
  call :GenerateCodeForSignalR %SMC_INTERFACE%
)

echo.
echo DONE!
echo.
goto done

:error
echo *** CODE GENERATION INCOMPLETE ***

:done
popd
endlocal
goto :eof

:GenerateCodeForInterface

  echo Generating Traditional Bridge code for interface %1...

  rem Generate dispatcher classes for all methods in in interface (TRADITIONAL SIDE)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t %BRIDGE_DISPATCHER_TEMPLATE% ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\Dispatchers ^
          -n %TraditionalBridgeProject%.Dispatchers ^
          -ut MODELS_NAMESPACE=%TraditionalBridgeProject%.Models ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the main dispatcher class (TRADITIONAL SIDE)
  
  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceDispatcher ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\Dispatchers ^
          -n %TraditionalBridgeProject%.Dispatchers ^
          -ut MODELS_NAMESPACE=%TraditionalBridgeProject%.Models ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate model classes (TRADITIONAL SIDE)

  codegen -smcstrs %SMC_XML_FILE% ^
          -interface %1 ^
          -t TraditionalModel TraditionalMetadata ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\Models ^
          -n %TraditionalBridgeProject%.Models ^
          %STDOPTS%

  rem Generate model classes (.NET side)
  rem Ideally the same data classes are shared between OData and Traditional Bridge
  rem environments. But if OData is not being used, enable this to generate Models
  rem in the web service based on SMC content.

  if defined ENABLE_XFSERVERPLUS_MODEL_GENERATION (
    codegen -smcstrs %SMC_XML_FILE% ^
            -interface %SMC_INTERFACE% ^
            -t ODataModel ODataMetaData ^
            -i %SolutionDir%Templates\TraditionalBridge ^
            -o %SolutionDir%%ModelsProject% ^
            -n %ModelsProject% ^
            %STDOPTS%
  )

  rem Generate request and response models for the service class methods (.NET side)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceServiceModels ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%ModelsProject% ^
          -n %SMC_INTERFACE% ^
          -ut MODELS_NAMESPACE=%ModelsProject% ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the service class (.NET side)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceService ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%ControllersProject% ^
          -n %ControllersProject% ^
          -ut MODELS_NAMESPACE=%ModelsProject% DTOS_NAMESPACE=%SMC_INTERFACE% ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  rem Generate the Web API controller (.NET side)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceController ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%ControllersProject% ^
          -n %ControllersProject% ^
          -ut MODELS_NAMESPACE=%ModelsProject% DTOS_NAMESPACE=%SMC_INTERFACE% ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  if DEFINED ENABLE_POSTMAN_TESTS (

  rem Generate the Postman tests for the Interface

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfacePostmanTests ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir% ^
            %STDOPTS%
    if ERRORLEVEL 1 goto error
  )

  if DEFINED ENABLE_UNIT_TEST_GENERATION (

    rem Generate a unit test class for the Interface
    codegen -smc %SMC_XML_FILE% ^
            -interface %1 ^
            -t  InterfaceUnitTests InterfaceUnitTestValues ^
            -i  %SolutionDir%Templates\TraditionalBridge ^
            -o  %SolutionDir%%TestProject% -tf ^
            -n  %TestProject% ^
            -ut CLIENT_MODELS_NAMESPACE=%TestProject%.Models DTOS_NAMESPACE=%SMC_INTERFACE% ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error
 
  )

GOTO:eof

:GenerateCodeForSignalR

  echo Generating SignalR code for interface %1...

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t SignalRHub ^
          -i %SolutionDir%Templates\SignalR ^
          -o %SolutionDir%%ControllersProject% ^
          -n %ControllersProject% ^
          -ut MODELS_NAMESPACE=%ModelsProject% DTOS_NAMESPACE=%SMC_INTERFACE% ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error  

GOTO:eof