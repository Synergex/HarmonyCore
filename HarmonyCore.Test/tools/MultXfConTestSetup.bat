setlocal

rem %1 is a flag for setup or cleanup
rem %2 is port number
rem %3 is data path
rem %4 is xfserver env var

rem run dblvars
call "%SYNERGYDE64%dbl\dblvars64.bat"
if "X%1" == "Xsetup" goto setup

:cleanup
start /min /wait rsynd -x -c TestSrv -p %2
goto done 

:setup
start /min /wait rsynd -rs -n -c TestSrv -p %2
rem add xfserver env var
start /min /wait powershell Start-Process powershell -WindowStyle hidden -verb runAs -ArgumentList \"reg add 'HKEY_LOCAL_MACHINE\SOFTWARE\Synergex\Synergy xfServer\TestSrv\Synrc' /v %4 /d %3\"
start /min /wait rsynd -q -c TestSrv -p %2
start /min /wait rsynd -rs -n -c TestSrv -p %2

:done

endlocal