@echo off

codegen -smc %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\InterfaceDispatcher -o %SolutionDir%\%PROJECT%\Dispatcher -n %NAMESPACE% -ut MODELS_NAMESPACE=%NAMESPACE%.Models -e -r -lf
if ERRORLEVEL 1 goto error

codegen -smc %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\InterfaceMethodDispatchers -o %SolutionDir%\%PROJECT%\MethodDispatchers -n %NAMESPACE% -ut MODELS_NAMESPACE=%NAMESPACE%.Models -e -r -lf
if ERRORLEVEL 1 goto error

rem Generate the request and response models for the service class methods (.NET side)
codegen -smcstrs %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\TraditionalBridgeInterfaceServiceModels -i %TEMPLATEROOT% -o %SolutionDir%\TraditionalBridge.TestClient\Client -n %TESTPROJECT% -ut MODELS_NAMESPACE=TraditionalBridge.Models -e -r -lf
if ERRORLEVEL 1 goto bypass

rem Generate the service class (.NET side)
codegen -smcstrs %SMCROOT%\%SMCNAME% -interface %TESTNAME% -ms -t %SolutionDir%\%TEMPLATEROOT%\InterfaceService -i %TEMPLATEROOT% -o %SolutionDir%\TraditionalBridge.TestClient\Client -n %TESTPROJECT% -ut MODELS_NAMESPACE=TraditionalBridge.Models -e -r -lf
if ERRORLEVEL 1 goto bypass

codegen -smcstrs %SMCROOT%\%SMCNAME% -ms -t %SolutionDir%\%TEMPLATEROOT%\InterfaceDispatcherData -o %SolutionDir%\%PROJECT%\DispatcherData -n %NAMESPACE% -ut SMC_INTERFACE=%TESTNAME% -e -r -lf
if ERRORLEVEL 1 goto bypass

rem Generate model and metadata classes
codegen -smcstrs %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\ODataModel %SolutionDir%\%TEMPLATEROOT%\ODataMetaData -i %TEMPLATEROOT% -o %SolutionDir%\TraditionalBridge.Models\Models -n TraditionalBridge.Models -e -r -lf
if ERRORLEVEL 1 goto bypass

goto done

:bypass
exit /b 0

:error
echo *** CODE GENERATION INCOMPLETE ***

:done
