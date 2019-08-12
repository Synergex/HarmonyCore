@echo off
set SolutionDir=%cd%

rem ===========================================================================
rem Specify the names of the projects to generate code into:

set ServicesProject=Services
set ModelsProject=Services.Models
set ControllersProject=Services.Controllers
set HostProject=Services.Host
set TestProject=Services.Test

rem ===========================================================================
rem Specify the names of the repository structures to generate code from:

rem set DATA_STRUCTURES=ADDRESS BINARYTEST BOOLEANSTR COERCESTRUCTURE COERCE_TEST CUSTOMER DATASET DATATABLESTR DATESTRU DATETEBLESTR2 DATETIMEARY DATETIMESTR DBAUTHOR DBBAG DBBOOK DBORDER DBORDERDTL DBPUBLISHER DNETDATETIME ENUMSTRUCTURE FUENTES GPC GPC2 GPC3 GPC4 GPC5 GPC6 GPC7 GRFAFILESTRUCT GRFATEST IMPLIEDDECIMALTEST INTEGERS INTEGERTESTS MANAGE_FUNDS MSC1 MSC2 MSC3 MSC4 NULLDTARY NULLDTSTR PERFSTRUCT PMAUTHOR_RD PMBAG_RD PMBOOK_INFO PMBOOK_RD PRO_40 PRO_41 REFCOUNT SALESMAN SINGLEIMPLIEDDECIMAL STROPTSINNER STROPTSOUTTER STRTEST1 STRTEST12 STRTEST14 STRTEST16 STRTEST18 STRTEST19 STRTEST2 STRTEST23 STRTEST24 STRTEST25 STRTEST27 STRTEST3 STRTEST30 STRTEST31 STRTEST4 STRTEST7 STRUCTURETEST STRU_A STRU_B STRU_C STRU_D STRU_E TESTSTRUCT TIMEKEY TIMESTRU TR_SYNINS_INP TR_SYNINS_RET USERDATES USERSTRU V93REPOSOPTS
set DATA_STRUCTURES=ADDRESS BOOLEANSTR COERCESTRUCTURE COERCE_TEST CUSTOMER DATATABLESTR DATESTRU DATETEBLESTR2 DATETIMEARY DATETIMESTR DBAUTHOR DBBAG DBBOOK DBORDER DBORDERDTL DBPUBLISHER DNETDATETIME GPC3 GRFAFILESTRUCT IMPLIEDDECIMALTEST INTEGERS INTEGERTESTS MANAGE_FUNDS MSC1 NULLDTARY NULLDTSTR PERFSTRUCT PMAUTHOR_RD PMBAG_RD PMBOOK_INFO PMBOOK_RD PRO_40 PRO_41 REFCOUNT SINGLEIMPLIEDDECIMAL STROPTSINNER STRTEST1 STRTEST12 STRTEST14 STRTEST16 STRTEST18 STRTEST19 STRTEST2 STRTEST23 STRTEST24 STRTEST25 STRTEST27 STRTEST3 STRTEST30 STRTEST31 STRTEST4 STRTEST7 STRUCTURETEST STRU_A STRU_B STRU_C STRU_D STRU_E TESTSTRUCT TIMEKEY TIMESTRU TR_SYNINS_INP TR_SYNINS_RET USERDATES USERSTRU V93REPOSOPTS
set DATA_ALIASES=%DATA_STRUCTURES%
set DATA_FILES=%DATA_STRUCTURES%

set FILE_STRUCTURES=AUTH BAG BOOK ORD ORDDTL PUB
set FILE_ALIASES=%FILE_STRUCTURES%

rem DATA_STRUCTURES Is a list all of structures that you wish to generate models and
rem                 controllers for. In other words it declares all of the "entities"
rem                 that are being represented and exposed by the environment.
rem
rem FILE_STRUCTURES If you don't have multi-record format files then this should be the
rem                 same as DATA_STRUCTURES. But if you do then FILE_STRUCTURES should
rem                 only list ONE of the structures assigned to each file, so this list
rem                 will be a subset of DATA_STRUCTURES.

rem ===========================================================================
rem Specify optional "system parameter file" structure

set PARAMSTR=SYSPARAMS

rem In the sammple environment the system parameter file is a relative file that contains
rem "next available record number" data for use in conjunction with POST (create with automated
rem primary key assignment) operaitons. Naming the structure associated with that file here
rem ensures that a copy of that file will be made available in the sample self-host environment
rem along with other data files in the sample data folder. This mechanism will require customization
rem for use in other environments.

rem ===========================================================================
rem Comment or uncomment the following lines to enable or disable optional features:

set ENABLE_GET_ALL=-define ENABLE_GET_ALL
set ENABLE_GET_ONE=-define ENABLE_GET_ONE
set ENABLE_SELF_HOST_GENERATION=YES
set ENABLE_CREATE_TEST_FILES=-define ENABLE_CREATE_TEST_FILES
set ENABLE_SWAGGER_DOCS=-define ENABLE_SWAGGER_DOCS
set ENABLE_POSTMAN_TESTS=YES
set ENABLE_ALTERNATE_KEYS=-define ENABLE_ALTERNATE_KEYS
set ENABLE_COUNT=-define ENABLE_COUNT
set ENABLE_PROPERTY_ENDPOINTS=-define ENABLE_PROPERTY_ENDPOINTS
set ENABLE_PROPERTY_VALUE_DOCS= 
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
set ENABLE_AUTHENTICATION= 
set ENABLE_FIELD_SECURITY= 
set ENABLE_UNIT_TEST_GENERATION=YES
set ENABLE_CASE_SENSITIVE_URL= 
set ENABLE_CORS= 
set ENABLE_IIS_SUPPORT= 
set ENABLE_OVERLAYS=-f o
set ENABLE_ALTERNATE_FIELD_NAMES= 
set ENABLE_READ_ONLY_PROPERTIES= 
set RPSDAT=xfplSMC
set RPSMFIL=xfplSMC\rpsmain.ism
set RPSTFIL=xfplSMC\rpstext.ism

if not "NONE%ENABLE_SELECT%%ENABLE_FILTER%%ENABLE_ORDERBY%%ENABLE_TOP%%ENABLE_SKIP%%ENABLE_RELATIONS%"=="NONE" (
  set PARAM_OPTIONS_PRESENT=-define PARAM_OPTIONS_PRESENT
)

rem ===========================================================================
rem Configure standard command line options and the CodeGen environment

if "%COMPUTERNAME%"=="SIVES" (
  set USERTOKENFILE=UserDefinedTokensSteve.tkn
) else (
  set USERTOKENFILE=C:\Users\devadm\Desktop\HarmonyCore\UserDefinedTokens.tkn
)
echo.
echo User token file is %USERTOKENFILE%

set NOREPLACEOPTS=-e -lf -u %USERTOKENFILE% %ENABLE_GET_ALL% %ENABLE_GET_ONE% %ENABLE_OVERLAYS% %ENABLE_ALTERNATE_FIELD_NAMES% %ENABLE_AUTHENTICATION% %ENABLE_FIELD_SECURITY% %ENABLE_PROPERTY_ENDPOINTS% %ENABLE_PROPERTY_VALUE_DOCS% %ENABLE_CASE_SENSITIVE_URL% %ENABLE_CREATE_TEST_FILES% %ENABLE_CORS% %ENABLE_IIS_SUPPORT% %ENABLE_DELETE% %ENABLE_PUT% %ENABLE_POST% %ENABLE_PATCH% %ENABLE_ALTERNATE_KEYS% %ENABLE_SWAGGER_DOCS% %ENABLE_RELATIONS% %ENABLE_SELECT% %ENABLE_FILTER% %ENABLE_ORDERBY% %ENABLE_COUNT% %ENABLE_TOP% %ENABLE_SKIP% %ENABLE_SPROC% %ENABLE_ADAPTER_ROUTING% %ENABLE_READ_ONLY_PROPERTIES% %PARAM_OPTIONS_PRESENT% -i Templates -rps %RPSMFIL% %RPSTFIL%
set STDOPTS=%NOREPLACEOPTS% -r

rem ===========================================================================
rem Codegen stuff
set TEMPLATEROOT=Templates\TraditionalBridge
set SMCROOT=xfplSMC
set PROJECT=TraditionalBridge.Test
set NAMESPACE=TraditionalBridge.Test
set TESTPROJECT=TraditionalBridge.TestClient

rem Generate a Web API / OData CRUD environment
rem Generate model and metadata classes
codegen -s %DATA_STRUCTURES% -t ODataModel ODataMetaData -i %TEMPLATEROOT% -o %SolutionDir%TraditionalBridge.Models -n TraditionalBridge.Models -e -r -lf
if ERRORLEVEL 1 goto error

rem Unit testing project
rem Generate OData client model, data loader and unit test classes


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
call %SolutionDir%\%SMCROOT%\gencode.bat
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
