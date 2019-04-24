@echo off

codegen -smc %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\InterfaceDispatcher -o %SolutionDir%\%PROJECT%\Dispatcher -n %NAMESPACE% -ut MODELS_NAMESPACE=%NAMESPACE%.Models -e -r -lf

codegen -smc %SMCROOT%\%SMCNAME% -interface %TESTNAME% -t %SolutionDir%\%TEMPLATEROOT%\InterfaceMethodDispatchers -o %SolutionDir%\%PROJECT%\MethodDispatchers -n %NAMESPACE% -ut MODELS_NAMESPACE=%NAMESPACE%.Models -e -r -lf

codegen -s %DATA_STRUCTURES% -ms -t %SolutionDir%\%TEMPLATEROOT%\InterfaceDispatcherData -o %SolutionDir%\%PROJECT%\DispatcherData -n %NAMESPACE% -ut SMC_INTERFACE=%TESTNAME% -e -r -lf
