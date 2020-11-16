@echo off
set SolutionDir=%cd%

rem ===========================================================================
rem Specify the names of the repository structures to generate code from:

set DATA_STRUCTURES=ADDRESS BOOLEANSTR COERCESTRUCTURE COERCE_TEST CUSTOMER DATATABLESTR DATESTRU DATETEBLESTR2 DATETIMEARY DATETIMESTR DBAUTHOR DBBAG DBBOOK DBORDER DBORDERDTL DBPUBLISHER DNETDATETIME GPC3 GRFAFILESTRUCT IMPLIEDDECIMALTEST INTEGERS INTEGERTESTS MANAGE_FUNDS MSC1 NULLDTARY NULLDTSTR PERFSTRUCT PMAUTHOR_RD PMBAG_RD PMBOOK_INFO PMBOOK_RD PRO_40 PRO_41 REFCOUNT SINGLEIMPLIEDDECIMAL STROPTSINNER STRTEST1 STRTEST12 STRTEST14 STRTEST16 STRTEST18 STRTEST19 STRTEST2 STRTEST23 STRTEST24 STRTEST25 STRTEST27 STRTEST3 STRTEST30 STRTEST31 STRTEST4 STRTEST7 STRUCTURETEST STRU_A STRU_B STRU_C STRU_D STRU_E TESTSTRUCT TIMEKEY TIMESTRU TR_SYNINS_INP TR_SYNINS_RET USERDATES USERSTRU V93REPOSOPTS

rem ===========================================================================
rem Codegen stuff
set TEMPLATEROOT=Templates\TraditionalBridge
set SMCROOT=xfplSMC
set PROJECT=TraditionalBridge.Test
set NAMESPACE=TraditionalBridge.Test
set TESTPROJECT=TraditionalBridge.TestClient

rem Models
codegen -s %DATA_STRUCTURES% -t TraditionalModel TraditionalMetaData -i %TEMPLATEROOT% -o %PROJECT%\Models -n %NAMESPACE%.Models -e -r -lf
if ERRORLEVEL 1 goto error

rem smc
set TESTNAME=smc
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem AutoTime
set TESTNAME=AutoTime
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem BinaryTransfer
set TESTNAME=BinaryTransfer
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem data64k
set TESTNAME=data64k
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem Encrypt
set TESTNAME=Encrypt
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem IF913
set TESTNAME=IF913
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem LrgPkts
set TESTNAME=LrgPkts
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem NoParms
set TESTNAME=NoParms
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem pooltests
set SMCNAME=pooltests.xml
set TESTNAME=Pool1
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error
set TESTNAME=Pool2
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error
set TESTNAME=Pool3
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error
set TESTNAME=Pool4
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error
set TESTNAME=Pool5
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error
set TESTNAME=Pool6
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem strtests
set TESTNAME=strtests
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem syntst
set TESTNAME=syntst
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem TestDate
set TESTNAME=TestDate
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem UserData
set TESTNAME=UserData
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem V93IF
set TESTNAME=V93IF
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem WCFields
set TESTNAME=WCFields
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

rem ZDateTime
set TESTNAME=ZDateTime
set SMCNAME=%TESTNAME%.xml
call %SMCROOT%\gencode.bat
if ERRORLEVEL 1 goto error

goto done

:error
echo *** CODE GENERATION INCOMPLETE ***

:done
