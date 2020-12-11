@echo off

codegen -smc %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\InterfaceDispatcher -o %SolutionDir%\%PROJECT%\Dispatcher -n %NAMESPACE% -ut MODELS_NAMESPACE=%NAMESPACE%.Models DTOS_NAMESPACE=%NAMESPACE%.Models -e -r -lf
if ERRORLEVEL 1 goto error

codegen -smc %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\InterfaceMethodDispatchers -o %SolutionDir%\%PROJECT%\MethodDispatchers -n %NAMESPACE% -ut MODELS_NAMESPACE=%NAMESPACE%.Models DTOS_NAMESPACE=%NAMESPACE%.Models -e -r -lf
if ERRORLEVEL 1 goto error

rem Generate the request and response models for the service class methods (.NET side)
codegen -smcstrs %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\MultiInterfaceServiceModels -i %TEMPLATEROOT% -o %SolutionDir%\TraditionalBridge.TestClient\Client -n %TESTPROJECT% -ut MODELS_NAMESPACE=TraditionalBridge.Models DTOS_NAMESPACE=TraditionalBridge.TestClient.%TESTNAME% -e -r -lf
if ERRORLEVEL 1 goto bypass

rem Generate the service class (.NET side)
codegen -smcstrs %SMCROOT%\%SMCNAME% -interface %TESTNAME% -ms -t %SolutionDir%\%TEMPLATEROOT%\InterfaceService -i %TEMPLATEROOT% -o %SolutionDir%\TraditionalBridge.TestClient\Client -n %TESTPROJECT% -ut MODELS_NAMESPACE=TraditionalBridge.Models DTOS_NAMESPACE=TraditionalBridge.TestClient.%TESTNAME% -e -r -lf
if ERRORLEVEL 1 goto bypass

codegen -smcstrs %SMCROOT%\%SMCNAME% -ms -t %SolutionDir%\%TEMPLATEROOT%\InterfaceDispatcherCustom -o %SolutionDir%\%PROJECT%\DispatcherCustom -n %NAMESPACE% -ut SMC_INTERFACE=%TESTNAME% DTOS_NAMESPACE=%NAMESPACE% -e -r -lf
if ERRORLEVEL 1 goto bypass

rem Generate model and metadata classes
codegen -smcstrs %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\ODataModel %SolutionDir%\%TEMPLATEROOT%\ODataMetaData -i %TEMPLATEROOT% -o %SolutionDir%\TraditionalBridge.Models\Models -n TraditionalBridge.Models -ut DTOS_NAMESPACE=TraditionalBridge.Models -e -r -lf
if ERRORLEVEL 1 goto bypass

goto done

:bypass
exit /b 0

:error
echo *** CODE GENERATION INCOMPLETE ***

:done
