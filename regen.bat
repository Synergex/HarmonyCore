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
set TestValuesProject=Services.Test.GenerateValues
set TraditionalBridgeProject=TraditionalBridge
set RepositoryProject=HarmonyCore.Test.Repository\HarmonyCore.Test.Repository.synproj
set VsTestClientTestsDir=HttpTests

rem ================================================================================================================================
rem Specify the names of the repository structures to generate code from:

set DATA_STRUCTURES=CUSTOMERS CUSTOMER_NOTES ITEMS ORDERS ORDER_ITEMS VENDORS CUSTOMER_EX NONUNIQUEPK DIFFERENTPK TESTCAR TESTCARLOT TESTCAROWNER1 TESTCAROWNER2 TESTCAROWNER3
set DATA_ALIASES=%DATA_STRUCTURES%
set DATA_FILES=%DATA_STRUCTURES%

set FILE_STRUCTURES=%DATA_STRUCTURES%
set FILE_ALIASES=%DATA_ALIASES%
set FILE_FILES=%DATA_FILES%

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
rem ================================================================================================================================
rem Comment or uncomment the following lines to enable or disable optional features:

set ENABLE_ODATA_ENVIRONMENT=YES
rem set EF_PROVIDER_MYSQL=-define EF_PROVIDER_MYSQL
rem set NO_CUSTOM_PLURALIZATION=-ncp
set ENABLE_SELF_HOST_GENERATION=YES
set ENABLE_CREATE_TEST_FILES=-define ENABLE_CREATE_TEST_FILES
rem set DO_NOT_SET_FILE_LOGICALS=-define DO_NOT_SET_FILE_LOGICALS
set ENABLE_GET_ALL=-define ENABLE_GET_ALL
set ENABLE_GET_ONE=-define ENABLE_GET_ONE
set ENABLE_POSTMAN_TESTS=YES
set ENABLE_RESTCLIENT_TESTS=YES
set ENABLE_ALTERNATE_KEYS=-define ENABLE_ALTERNATE_KEYS
rem set ENABLE_PARTIAL_KEYS=-define ENABLE_PARTIAL_KEYS
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
rem set ENABLE_XFSERVERPLUS_MIGRATION=YES
rem set ENABLE_XFSERVERPLUS_MODEL_GENERATION=YES
rem set ENABLE_BRIDGE_OPTIONAL_PARAMETERS=YES
set ENABLE_NEWTONSOFT=-define ENABLE_NEWTONSOFT

if not "NONE%ENABLE_SELECT%%ENABLE_FILTER%%ENABLE_ORDERBY%%ENABLE_TOP%%ENABLE_SKIP%%ENABLE_RELATIONS%"=="NONE" (
  set PARAM_OPTIONS_PRESENT=-define PARAM_OPTIONS_PRESENT
)

rem ================================================================================================================================
rem Configure standard command line options and the CodeGen environment

rem set SHOW_CODEGEN_COMMANDS=-e

set NOREPLACEOPTS=%SHOW_CODEGEN_COMMANDS% -lf -u %SolutionDir%UserDefinedTokens.tkn %NO_CUSTOM_PLURALIZATION% %ENABLE_GET_ALL% %ENABLE_GET_ONE% %ENABLE_OVERLAYS% %DO_NOT_SET_FILE_LOGICALS% %ENABLE_ALTERNATE_FIELD_NAMES% %ENABLE_AUTHENTICATION% %ENABLE_CUSTOM_AUTHENTICATION% %ENABLE_SIGNALR% %ENABLE_FIELD_SECURITY% %ENABLE_PROPERTY_ENDPOINTS% %ENABLE_CASE_SENSITIVE_URL% %ENABLE_CREATE_TEST_FILES% %ENABLE_CORS% %ENABLE_IIS_SUPPORT% %ENABLE_DELETE% %ENABLE_PUT% %ENABLE_POST% %ENABLE_PATCH% %ENABLE_ALTERNATE_KEYS% %ENABLE_PARTIAL_KEYS% %ENABLE_RELATIONS% %ENABLE_RELATIONS_VALIDATION% %ENABLE_SELECT% %ENABLE_FILTER% %ENABLE_ORDERBY% %ENABLE_COUNT% %ENABLE_TOP% %ENABLE_SKIP% %ENABLE_SPROC% %ENABLE_ADAPTER_ROUTING% %ENABLE_READ_ONLY_PROPERTIES% %ENABLE_NEWTONSOFT% %PARAM_OPTIONS_PRESENT% %EF_PROVIDER_MYSQL% -rps %RPSMFIL% %RPSTFIL%
set STDOPTS=%NOREPLACEOPTS% -r

rem ================================================================================================================================

if DEFINED EF_PROVIDER_MYSQL (
  set TEMPLATESUBDIR=\MySQL
)

if DEFINED ENABLE_ODATA_ENVIRONMENT (

  echo .
  echo ************************************************************************
  echo Generating Web API/OData CRUD environment

  if NOT DEFINED EF_PROVIDER_MYSQL (

  echo.
  echo Generating model and metadata classes

    codegen -s  %DATA_STRUCTURES% ^
            -a  %DATA_ALIASES% ^
            -fo %DATA_FILES% ^
            -t  ODataModel ODataMetaData ^
            -i  %SolutionDir%Templates ^
            -o  %SolutionDir%%ModelsProject% ^
            -n  %ModelsProject% ^
                %STDOPTS%
    if ERRORLEVEL 1 goto error
  )

  echo.
  echo Generating controller classes

  codegen -s  %DATA_STRUCTURES% ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataController ^
          -i  %SolutionDir%Templates%TEMPLATESUBDIR% ^
          -o  %SolutionDir%%ControllersProject% ^
          -n  %ControllersProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
  
if DEFINED ENABLE_PROPERTY_ENDPOINTS (

  echo.
  echo Generating individual property endpoints

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
if NOT DEFINED EF_PROVIDER_MYSQL (

  echo.
  echo Generating EF DbContext class

  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataDbContext ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%ModelsProject% ^
          -n  %ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)
  echo.
  echo Generating OData EDM Builder class

  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataEdmBuilder ^
          -i  %SolutionDir%Templates%TEMPLATESUBDIR% ^
          -o  %SolutionDir%%ServicesProject% ^
          -n  %ServicesProject% ^
          -ut CONTROLLERS_NAMESPACE=%ControllersProject% MODELS_NAMESPACE=%ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating startup class

  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataStartup ^
          -i  %SolutionDir%Templates%TEMPLATESUBDIR% ^
          -o  %SolutionDir%%ServicesProject% ^
          -n  %ServicesProject% ^
          -ut CONTROLLERS_NAMESPACE=%ControllersProject% MODELS_NAMESPACE=%ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Postman tests

if DEFINED ENABLE_POSTMAN_TESTS (

  echo.
  echo Generating Postman tests for OData environment

  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataPostManTests ^
          -i  %SolutionDir%Templates%TEMPLATESUBDIR% ^
          -o  %SolutionDir% ^
              %NOREPLACEOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Visual Studio REST Client Tests

if DEFINED ENABLE_RESTCLIENT_TESTS (

  echo.
  echo Generating Visual Studio REST Client tests for OData environment

  codegen -s  %DATA_STRUCTURES% ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataVsRestClientTests ^
          -i  %SolutionDir%Templates%TEMPLATESUBDIR% ^
          -o  %VsTestClientTestsDir% ^
              %NOREPLACEOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Self hosting

if DEFINED ENABLE_SELF_HOST_GENERATION (

  echo.
  echo ************************************************************************
  echo Generating self-hosting code
  echo.
  echo Generating self-hosting environment class

  codegen -s  %FILE_STRUCTURES% -ms ^
          -a  %FILE_ALIASES% ^
          -fo %FILE_FILES% ^
          -t  ODataSelfHostEnvironment ^
          -i  %SolutionDir%Templates%TEMPLATESUBDIR% ^
          -o  %SolutionDir%%HostProject% ^
          -n  %HostProject% ^
          -ut SERVICES_NAMESPACE=%ServicesProject% MODELS_NAMESPACE=%ModelsProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating self-hosting program

  codegen -s  %FILE_STRUCTURES% ^
          -a  %FILE_ALIASES% ^
          -fo %FILE_FILES% ^
          -t  ODataSelfHost ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%HostProject% ^
          -n  %HostProject% ^
          -ut SERVICES_NAMESPACE=%ServicesProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Custom Authentication Example

if DEFINED ENABLE_CUSTOM_AUTHENTICATION (

  echo ************************************************************************
  echo Generating custom authentication code
    echo.

  if not exist "%SolutionDir%%ModelsProject%\AuthenticationModels.dbl" (
    echo Generating custom authentication data model class
    echo.

    codegen -t  ODataCustomAuthModels ^
            -i  %SolutionDir%Templates ^
            -o  %SolutionDir%%ModelsProject% ^
            -n  %ModelsProject% ^
                %NOREPLACEOPTS%
    if ERRORLEVEL 1 goto error
    echo.
  )

  if not exist "%SolutionDir%%ControllersProject%\AuthenticationController.dbl" (
    echo Generating custom authentication controller class
    echo.

    codegen -t  ODataCustomAuthController ^
            -i  %SolutionDir%Templates ^
            -o  %SolutionDir%%ControllersProject% ^
            -n  %ControllersProject% ^
                %NOREPLACEOPTS%
    if ERRORLEVEL 1 goto error
    echo.
  )

  if not exist "%SolutionDir%%ControllersProject%\AuthenticationTools.dbl" (
    echo Generating custom authentication tools class
    echo.

    codegen -t  ODataCustomAuthTools ^
            -i  %SolutionDir%Templates ^
            -o  %SolutionDir%%ControllersProject% ^
            -n  %ControllersProject% ^
                %NOREPLACEOPTS%
    if ERRORLEVEL 1 goto error
    echo.
  )
)

rem ================================================================================
rem Unit testing project

if DEFINED ENABLE_UNIT_TEST_GENERATION (

  echo ************************************************************************
  echo Generating unit test code

  echo.
  echo Generating client model, data loader and unit test classes
  
  codegen -s  %DATA_STRUCTURES% ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataClientModel ODataTestDataLoader ODataUnitTests ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestProject% -tf ^
          -n  %TestProject% ^
          -ut UNIT_TEST_NAMESPACE=%TestProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating unit test environment class and hosting program

  codegen -s  %FILE_STRUCTURES% -ms ^
          -a  %FILE_ALIASES% ^
          -fo %FILE_FILES% ^
          -t  ODataUnitTestEnvironment ODataUnitTestHost ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestProject% ^
          -n  %TestProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating unit test constants properties class

  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  ODataTestConstantsProperties ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestProject% ^
          -n  %TestProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating unit test key value generation program 

  codegen -s  %DATA_STRUCTURES% -ms ^
          -a  %DATA_ALIASES% ^
          -fo %DATA_FILES% ^
          -t  GenerateTestValues ^
          -i  %SolutionDir%Templates ^
          -o  %SolutionDir%%TestValuesProject% ^
          -n  %TestValuesProject% ^
          -ut UNIT_TEST_NAMESPACE=%TestProject% ^
              %STDOPTS%
  if ERRORLEVEL 1 goto error
)

rem ================================================================================
rem Generate TraditionalBridge / xfServerPlus Migration Code

rem Specify the path to a SMC export file and the method catalog location
set SMC_XML_FILE=TraditionalBridge\MethodCatalog\MethodDefinitions.xml
set XFPL_SMCPATH=TraditionalBridge\MethodCatalog

rem Specify the name of one or more interfaces defined in the SMC export file, space separated
set SMC_INTERFACES=BridgeSamples

rem CodeGen options
set NOREPLACEOPTS=%SHOW_CODEGEN_COMMANDS% -lf -u %SolutionDir%UserDefinedTokens.tkn -rps %RPSMFIL% %RPSTFIL% %ENABLE_AUTHENTICATION%
set STDOPTS=%NOREPLACEOPTS% -r

rem Optional parameters support is for use with xfServerPlue environments that were
rem used in conjunction with xfNetLink COM, which supported optional parameters.
if DEFINED ENABLE_BRIDGE_OPTIONAL_PARAMETERS (
  set BRIDGE_DISPATCHER_TEMPLATE=OptionalParameterMethodDispatchers
  ) else (
  set BRIDGE_DISPATCHER_TEMPLATE=InterfaceMethodDispatchers
)

if DEFINED ENABLE_XFSERVERPLUS_MIGRATION (

  rem Generate code for each interface
  for %%x in (%SMC_INTERFACES%) do (
    call :GenerateCodeForInterface %%x
  )

  rem Generate the main (multi-interface capable) dispatcher
  call :GenerateMainDispatcher %SMC_INTERFACES%

  rem Generate SignalR hub(s)
  if DEFINED ENABLE_SIGNALR (
    for %%x in (%SMC_INTERFACES%) do (
      call :GenerateSignalRHub %%x
    )
  )
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

rem ================================================================================
:GenerateCodeForInterface

  echo.
  echo ************************************************************************
  echo Generating Traditional Bridge code for interface %1
  echo.
  echo Generating interface method dispatcher classes (Traditional)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t %BRIDGE_DISPATCHER_TEMPLATE% ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\Dispatchers ^
          -n %TraditionalBridgeProject%.Dispatchers ^
          -ut MODELS_NAMESPACE=%TraditionalBridgeProject%.Models ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating interface dispatcher classes (Traditional)
  
  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceDispatcher ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\Dispatchers ^
          -n %TraditionalBridgeProject%.Dispatchers ^
          -ut MODELS_NAMESPACE=%TraditionalBridgeProject%.Models ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating interface data model classes (Traditional)

  codegen -smcstrs %SMC_XML_FILE% ^
          -interface %1 ^
          -t TraditionalModel TraditionalMetadata ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\Models ^
          -n %TraditionalBridgeProject%.Models ^
          %STDOPTS%

  if defined ENABLE_XFSERVERPLUS_MODEL_GENERATION (

    echo.
    echo Generating interface data model classes (.NET)
    rem Ideally the same data classes are shared between OData and Traditional Bridge
    rem environments. But if OData is not being used, enable this to generate Models
    rem in the web service based on SMC content.

    codegen -smcstrs %SMC_XML_FILE% ^
            -interface %1 ^
            -t ODataModel ODataMetaData ^
            -i %SolutionDir%Templates\TraditionalBridge ^
            -o %SolutionDir%%ModelsProject% ^
            -n %ModelsProject% ^
            %STDOPTS%
  )

  echo.
  echo Generating interface request/response DTO classes (.NET)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceServiceModels ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%ModelsProject% ^
          -n %1 ^
          -ut MODELS_NAMESPACE=%ModelsProject% ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating interface service classes (.NET)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceService ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%ControllersProject% ^
          -n %ControllersProject% ^
          -ut MODELS_NAMESPACE=%ModelsProject% DTOS_NAMESPACE=%1 ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  echo.
  echo Generating interface controller classes (.NET)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceController ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%ControllersProject% ^
          -n %ControllersProject% ^
          -ut MODELS_NAMESPACE=%ModelsProject% DTOS_NAMESPACE=%1 ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

  if DEFINED ENABLE_POSTMAN_TESTS (

  echo.
  echo Generating Postman tests for interface

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfacePostmanTests ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir% ^
            %NOREPLACEOPTS%
    if ERRORLEVEL 1 goto error
  )

  if DEFINED ENABLE_RESTCLIENT_TESTS (

  echo.
  echo Generating Visual Studio REST Client tests for interface

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t InterfaceVsRestClientTests ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %VsTestClientTestsDir% ^
            %NOREPLACEOPTS%
    if ERRORLEVEL 1 goto error
  )

  if DEFINED ENABLE_UNIT_TEST_GENERATION (

    echo.
    echo Generating interface unit tests (.NET)

    codegen -smc %SMC_XML_FILE% ^
            -interface %1 ^
            -t  InterfaceUnitTests InterfaceUnitTestValues ^
            -i  %SolutionDir%Templates\TraditionalBridge ^
            -o  %SolutionDir%%TestProject% -tf ^
            -n  %TestProject% ^
            -ut CLIENT_MODELS_NAMESPACE=%TestProject%.Models DTOS_NAMESPACE=%1 ^
          %STDOPTS%
    if ERRORLEVEL 1 goto error

  )

GOTO:eof

rem ================================================================================
:GenerateMainDispatcher

  echo.
  echo Generating multi-interface dispatcher tools class (Traditional)

  codegen -smc %SMC_XML_FILE% ^
          -iloop %* ^
          -t InterfaceSuperDispatcher  ^
          -i %SolutionDir%Templates\TraditionalBridge ^
          -o %SolutionDir%%TraditionalBridgeProject%\Dispatchers ^
          -n %TraditionalBridgeProject%.Dispatchers ^
          -ut MODELS_NAMESPACE=%TraditionalBridgeProject%.Models ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error

GOTO:eof

rem ================================================================================
:GenerateSignalRHub

  echo.
  echo Generating interface SignalR hub (.NET)

  codegen -smc %SMC_XML_FILE% ^
          -interface %1 ^
          -t SignalRHub ^
          -i %SolutionDir%Templates\SignalR ^
          -o %SolutionDir%%ControllersProject% ^
          -n %ControllersProject% ^
          -ut MODELS_NAMESPACE=%ModelsProject% DTOS_NAMESPACE=%1 ^
          %STDOPTS%
  if ERRORLEVEL 1 goto error  

GOTO:eof