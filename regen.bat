@echo off
pushd %~dp0
setlocal

set OPTS=-e -r -lf

set CODEGEN_TPLDIR=Templates
set PROJECT=SampleServices

rem Rem the following line in or out to enable or disable authentication code
rem set DO_AUTHENTICATION=-define AUTHENTICATION

rem ================================================================================================================================
rem Generate a Web API / OData CRUD environment

set STRUCTURES=CUSTOMERS ORDERS PLANTS VENDORS

rem Generate model classes
codegen -s %STRUCTURES%     -t ODataModel -n %PROJECT%.Models -o %PROJECT%\Models %OPTS%

rem Generate controller classes
codegen -s %STRUCTURES%     -t ODataController -n %PROJECT%.Controllers -o %PROJECT%\Controllers %OPTS% -ut MODELS_NAMESPACE=%PROJECT%.Models DBCONTEXT_NAMESPACE=%PROJECT% %DO_AUTHENTICATION%

rem Generate the DbContext, EdmBuilder and Startup classes
codegen -s %STRUCTURES% -ms -t ODataDbContext ODataEdmBuilder ODataStartup -n %PROJECT% -o %PROJECT% -ut MODELS_NAMESPACE=%PROJECT%.Models API_PAGE_TITLE="Harmony Core Sample API" %OPTS% %DO_AUTHENTICATION%

rem Generate unit tests
codegen -s %STRUCTURES%     -t ODataUnitTests -n %PROJECT%.Test -o %PROJECT%.Test -ut SERVICES_NAMESPACE=%PROJECT% %OPTS% %DO_AUTHENTICATION%

rem Generate OData model classes for client side use
codegen -s %STRUCTURES%     -t ODataClientModel -n %PROJECT%.Test.Models -o %PROJECT%.Test\Models %OPTS%

rem Generate Postman Tests
codegen -s %STRUCTURES% -ms -t ODataPostManTests -o .\ %OPTS%

rem Generate a Swagger file
codegen -s %STRUCTURES% -ms -t ODataSwaggerYaml ODataSwaggerJson -o %PROJECT%\wwwroot %OPTS% -ut API_DESCRIPTION="Harmony Core Sample API" API_VERSION=1.0.0 API_TITLE="Harmony Core Sample API" API_TERMS_URL=http://some.url

rem ================================================================================================================================
rem The test environment has slightly different requirements, because we need to generate code based on structures, but when tags
rem are used to indicate that multiple structures are associated with a single ISAM file, we only need to generate from one of The
rem structures associated with each file.

set FILE_STRUCTURES=CUSTOMERS ORDERS PLANTS

rem Generate the test environment and unit test environment classes
codegen -s %FILE_STRUCTURES% -ms -t ODataTestEnvironment ODataUnitTestEnvironment -n %PROJECT%.Test -o %PROJECT%.Test -ut SERVICES_NAMESPACE=%PROJECT% MODELS_NAMESPACE=%PROJECT%.Models %OPTS% %DO_AUTHENTICATION%

rem ================================================================================================================================
rem Generate code for the TraditionalBridge sample environment

set CODEGEN_TPLDIR=Templates\TraditionalBridge
set PROJECT=TraditionalBridge.Test
set SMC_INTERFACE=SampleXfplEnv
set XFPL_SMCPATH=

rem Generate model classes
codegen -s %STRUCTURES% -t ODataModel -n %PROJECT%.Models -o %PROJECT%\Models %OPTS%

rem Generate method dispatcher classes
codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceDispatcher        -n %PROJECT% -o %PROJECT% %OPTS% -ut MODELS_NAMESPACE=%PROJECT%.Models
codegen -smc SampleXfplEnvironment\smc.xml -interface %SMC_INTERFACE% -t InterfaceMethodDispatchers -n %PROJECT% -o %PROJECT% %OPTS% -ut MODELS_NAMESPACE=%PROJECT%.Models

codegen -s %STRUCTURES% -ms -t InterfaceDispatcherData -n %PROJECT% -o %PROJECT% %OPTS% -ut SMC_INTERFACE=%SMC_INTERFACE%

endlocal
popd