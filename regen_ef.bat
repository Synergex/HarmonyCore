@echo off
setlocal
pushd %~dp0

rem ===================================================================================================================================================
rem FITST TIME SETUP
rem
rem 1. Install the DOTNET tooling for EF Core
rem
rem    dotnet tool install --global dotnet-ef
rem
rem 2. Add a C# class library to your Harmony Core solution: 
rem 
rem    dotnet new classlib -n Services.Models.CS
rem    cd Services.Models.CS
rem
rem 3. Add the C# project to your Harmony Core solution
rem
rem    Open the solution in VS and use File > Add > Existing Project
rem    Set the C# project to .NET Core 3.1
rem    Set the default namespace to Services.Models
rem    Delete Class.cs
rem
rem 4. Add NuGet packages to the Services.Models.CS project
rem
rem    Add package Pomelo.EntityFrameworkCore.MySql (version 3.2.4) to the Services.Models.CS project
rem    Add package Microsoft.EntityFrameworkCore.Tools (version 3.1.8) to the Services.Models.CS project
rem
rem 5. Add package references to Services.Models.CS to:
rem
rem    Services
rem    Services.Controllers
rem    Services.Test (if present)
rem
rem ===================================================================================================================================================
rem GENERATING EF CODE FROM MySQL
rem
rem 1. Use the dotnet ef dbcontext scaffold command to generate code (see below)
rem 
rem 2. Each time you generate code:
rem
rem    Edit the ForwardOfficeContext.cs and
rem    a. Remove the default constructor
rem    b. Replace the #warning with a //TODO: 
rem
rem ===================================================================================================================================================
rem 
rem Usage: dotnet ef dbcontext scaffold [arguments] [options]
rem 
rem Arguments:
rem  <CONNECTION>  The connection string to the database.
rem   <PROVIDER>    The provider to use. (E.g. Microsoft.EntityFrameworkCore.SqlServer)
rem 
rem Options:
rem   -d|--data-annotations                  Use attributes to configure the model (where possible). If omitted, only the fluent API is used.
rem   -c|--context <NAME>                    The name of the DbContext. Defaults to the database name.
rem   --context-dir <PATH>                   The directory to put the DbContext file in. Paths are relative to the project directory.
rem   -f|--force                             Overwrite existing files.
rem   -o|--output-dir <PATH>                 The directory to put files in. Paths are relative to the project directory.
rem   --schema <SCHEMA_NAME>...              The schemas of tables to generate entity types for.
rem   -t|--table <TABLE_NAME>...             The tables to generate entity types for.
rem   --use-database-names                   Use table and column names directly from the database.
rem   --json                                 Show JSON output. Use with --prefix-output to parse programatically.
rem   -n|--namespace <NAMESPACE>             The namespace to use. Matches the directory by default.
rem   --context-namespace <NAMESPACE>        The namespace of the DbContext class. Matches the directory by default.
rem   --no-onconfiguring                     Don't generate DbContext.OnConfiguring.
rem   --no-pluralize                         Don't use the pluralizer.
rem   -p|--project <PROJECT>                 The project to use. Defaults to the current working directory.
rem   -s|--startup-project <PROJECT>         The startup project to use. Defaults to the current working directory.
rem   --framework <FRAMEWORK>                The target framework. Defaults to the first one in the project.
rem   --configuration <CONFIGURATION>        The configuration to use.
rem   --runtime <RUNTIME_IDENTIFIER>         The runtime to use.
rem   --msbuildprojectextensionspath <PATH>  The MSBuild project extensions path. Defaults to "obj".
rem   --no-build                             Don't build the project. Intended to be used when the build is up-to-date.
rem   -h|--help                              Show help information
rem   -v|--verbose                           Show verbose output.
rem   --no-color                             Don't colorize output.
rem   --prefix-output                        Prefix output with level.
rem 

set MYSQL_SERVER=localhost
set MYSQL_DATABASE=forwardoffice
set MYSQL_USERNAME=root
set MYSQL_PASSWORD=p@ssw0rd
set MYSQL_TABLES=-t consignment_statistics -t supplier_costs
set MYSQL_LIBRARY=Pomelo.EntityFrameworkCore.MySql
set PROJECT_FOLDER=Services.Models.CS
set PROJECT_NAMESPACE=Services.Models
set DBCONTEXT_CLASS=ForwardOfficeContext

pushd %PROJECT_FOLDER%
dotnet ef dbcontext scaffold "Server=%MYSQL_SERVER%;Database=%MYSQL_DATABASE%;User=%MYSQL_USERNAME%;Password=%MYSQL_PASSWORD%" %MYSQL_LIBRARY% --namespace %PROJECT_NAMESPACE% --force --data-annotations --context %DBCONTEXT_CLASS% --no-build --no-onconfiguring --verbose %MYSQL_TABLES%
popd

popd
endlocal