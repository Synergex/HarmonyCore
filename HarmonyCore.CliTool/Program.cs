using CodeGen.RepositoryAPI;
using CommandLine;
using HarmonyCore.CliTool.Commands;
using HarmonyCoreGenerator.Generator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool
{
    [Verb("upgrade-latest")]
    class UpgradeLatestOptions
    {
        [Option('p', "project")]
        public bool ProjectOnly { get; set; }
    }
    [Verb("rps")]
    class RpsOptions
    {
        [Option("ls", Required = false, HelpText = "List structures contained in the specified repository")]
        public bool ListStructures { get; set; }

        [Option("lf", Required = false, HelpText = "List fields contained in the selected structure")]
        public bool ListFields { get; set; }

        [Option("lr", Required = false, HelpText = "List relations involving the selected structure")]
        public bool ListRelations { get; set; }

        [Option("lk", Required = false, HelpText = "List keys in the selected structure")]
        public bool ListKeys { get; set; }

        [Option('s', Required = false, HelpText = "Specify structure name to be operated on")]
        public string Structure { get; set; }

        [Option('f', Required = false, HelpText = "Specify field name to be operated on, use the fields fully qualified name or specify the structure using the -s option")]
        public string Field { get; set; }

        [Option('p', Required = false, HelpText =
@"Add/Remove property syntax is key:value with spaces between pairs
Known structure properties:
    Alias - string
	Files - Comma delimited values
	EnableRelations - true/false
	EnableRelationValidation - true/false
	EnableGetAll - true/false
	EnableGetOne - true/false
	EnableAltGet - true/false 
	EnablePut - true/false
	EnablePost - true/false
	EnablePatch - true/false
	EnableDelete - true/false
	ControllerAuthorization - true/false or comma delimited role names
	PostAuthorization - true/false or comma delimited role names
	PutAuthorization - true/false or comma delimited role names
	PatchAuthorization - true/false or comma delimited role names
	DeleteAuthorization - true/false or comma delimited role names
	GetAuthorization - true/false or comma delimited role names
	ODataQueryOptions - string", Separator = ' ')]
        public IEnumerable<string> Properties { get; set; }

        [Option('r', Required = false, HelpText = "Remove properties instead of adding them")]
        public bool RemoveProperties { get; set; }
    }

    [Verb("smc")]
    class SmcOptions
    {
        [Option("li", Required = false, HelpText = "List interfaces")]
        public bool ListInterfaces { get; set; }

        [Option("lm", Required = false, HelpText = "List methods in selected interface")]
        public bool ListMethods { get; set; }

        [Option("lp", Required = false, HelpText = "List parameter info in selected interface/method")]
        public bool ListParameters { get; set; }

        [Option("ls", Required = false, HelpText = "List structures used as parameters in selected interface/method")]
        public bool ListStructures { get; set; }

        [Option('i', Required = false, HelpText = "Specify interface name to be operated on")]
        public string Interface { get; set; }

        [Option('m', Required = false, HelpText = "Specify method name to be operated on")]
        public string Method { get; set; }
    }

    [Verb("regen")]
    class RegenOptions
    {
    }

    [Verb("codegen-list")]
    class CodegenListOptions
    {
        [Option('i', "interface")]
        public bool Interface { get; set; }
        [Option('s', "structure")]
        public bool Structure { get; set; }
    }

    [Verb("codegen-add")]
    class CodegenAddOptions
    {
        [Option("interface", SetName = "smc")]
        public bool Interface { get; set; }

        [Option("webapi", SetName = "smc")]
        public bool TBWebApi { get; set; }

        [Option("signalr", SetName = "smc")]
        public bool TBSignalR { get; set; }

        [Option("structure", SetName = "rps")]
        public bool Structure { get; set; }

        [Option("odata", SetName = "rps")]
        public bool OData { get; set; }
        [Option("ef", SetName = "rps")]
        public bool Ef { get; set; }
        [Option("custom", SetName = "rps")]
        public bool Custom { get; set; }
        [Option('i', Required = true, Separator = ' ')]
        public IEnumerable<string> Items { get; set; }
    }

    [Verb("codegen-remove")]
    class CodegenRemoveOptions
    {
        [Option("interface", SetName = "smc")]
        public bool Interface { get; set; }

        [Option("structure", SetName = "rps")]
        public bool Structure { get; set; }

        [Option('i', Required = true, Separator = ' ')]
        public IEnumerable<string> Items { get; set; }
    }

    class Program
    {
        public static string CurrentVersionTag = "release-v3.1.11";
        public static string BuildPackageVersion = "11.1.1050.2769";
        public static string CodeDomProviderVersion = "1.0.7";
        public static string HCBuildVersion = "3.1.156";
        public static List<string> HCRegenRequiredVersions = new List<string>
        {
            "3.1.156"
        };
        public static Dictionary<string, string> LatestNugetReferences = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            {"Harmony.Core", HCBuildVersion},
            {"HarmonyCore.CodeDomProvider", CodeDomProviderVersion},
            {"Harmony.Core.EF", HCBuildVersion},
            {"Harmony.Core.OData", HCBuildVersion},
            {"Harmony.Core.AspNetCore", HCBuildVersion},
            {"Synergex.SynergyDE.synrnt", "11.1.1060"},
            {"Synergex.SynergyDE.Build", BuildPackageVersion},
            {"Microsoft.AspNetCore.Mvc.NewtonsoftJson", "3.1.6"},
            {"Microsoft.AspNetCore.Mvc.Testing", "3.1.6"},
            {"Microsoft.Extensions.DependencyInjection", "3.1.6"},
            {"Microsoft.Extensions.Logging.Console", "3.1.6"},
            {"Microsoft.AspNetCore.SignalR.Client", "3.1.6"},
            {"Microsoft.EntityFrameworkCore", "3.1.6"},
            {"IdentityServer4.AccessTokenValidation", "3.0.1"},
            {"Microsoft.AspNetCore.OData", "7.4.1"},
            {"Microsoft.OData.Core", "7.7.0"},
            {"Microsoft.AspNetCore.JsonPatch", "3.1.6"},
            {"Microsoft.VisualStudio.Threading", "16.6.13"},
            {"StreamJsonRpc", "2.4.48"},
            {"IdentityModel", "4.1.1" },
            {"Microsoft.OData.Edm", "7.7.0"},
            {"Microsoft.Spatial", "7.7.0"},
            {"Swashbuckle.AspNetCore", "5.5.1"},
            {"SSH.NET", "2016.1.0"},
            {"Microsoft.AspNetCore.Mvc.Versioning", "4.1.1"},
            {"Microsoft.AspNetCore.OData.Versioning.ApiExplorer", "4.1.1"},
            {"Nito.AsyncEx", "5.0.0"},
            {"System.Linq.Dynamic.Core", "1.1.8"},
            {"system.text.encoding.codepages", "4.7.1"},
        };

        public static List<string> HCRegenRequiredVersions = new List<string>
        {
            "3.1.156"
        };

        const int STD_INPUT_HANDLE = -10;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);


        static void Main(string[] args)
        {
            var handle = GetStdHandle(STD_INPUT_HANDLE);
            uint currentMode = 0;
            GetConsoleMode(handle, out currentMode);

            var solutionDir = Environment.GetEnvironmentVariable("SolutionDir") ?? Environment.CurrentDirectory;
            Console.WriteLine("Scanning '{0}' for HarmonyCore project files", solutionDir);
            string[] synprojFiles = new string[0];
            try
            {
                synprojFiles = Directory.GetFiles(
                    solutionDir,
                    "*.synproj",
                    SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error while searching for project files: {0}", ex.Message);
                return;
            }
            var solutionInfo = new SolutionInfo(synprojFiles, solutionDir);

            if (!SetConsoleMode(handle, currentMode))
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }


            CommandLine.Parser.Default.ParseArguments<UpgradeLatestOptions, CodegenListOptions, CodegenAddOptions, CodegenRemoveOptions, RpsOptions, RegenOptions>(args)
            .MapResult<UpgradeLatestOptions, CodegenListOptions, CodegenAddOptions, CodegenRemoveOptions, RpsOptions, RegenOptions, int>(

              (UpgradeLatestOptions opts) =>
              {
                  Console.WriteLine("This utility will make significant changes to projects and other source files in your Harmony Core development environment. Before running this tool we recommend checking the current state of your development environment into your source code repository, taking a backup copy of the environment if you don't use source code control.\n\n");


                  Console.WriteLine("Type YES to proceed: ");
                  if (string.Compare(Console.ReadLine(), "YES", true) != 0)
                  {
                      Console.WriteLine("exiting without changes");
                      return 1;
                  }


                  if (opts.ProjectOnly)
                      UpgradeProjects(solutionInfo);
                  else
                      UpgradeLatest(solutionInfo).Wait();
                  return 0;
              },
              new CodegenCommand(solutionInfo).List,
              new CodegenCommand(solutionInfo).Add,
              new CodegenCommand(solutionInfo).Remove,
              new RPSCommand(solutionInfo).Run,
              new RegenCommand(solutionInfo).Run,
              errs =>
              {
                  foreach (var error in errs)
                  {
                      Console.WriteLine(error);
                  }
                  return 1;
              });
        }

        static void UpgradeProjects(SolutionInfo solution)
        {
            foreach (var project in solution.Projects)
            {
                project.PatchKnownIssues();
                project.PatchNugetVersions(LatestNugetReferences);
                project.Save();
            }
        }

        static async Task UpgradeLatest(SolutionInfo solution)
        {
            //download templates and traditional bridge source
            //replace templates and traditional bridge source
            var traditionalBridgeFiles = Directory.GetFiles(
                    solution.SolutionDir,
                    "RoutineDispatcher.dbl",
                    SearchOption.AllDirectories);

            var templateFiles = Directory.GetFiles(
                    solution.SolutionDir,
                    "*.tpl",
                    SearchOption.AllDirectories);

            var hasTraditionalBridge = (traditionalBridgeFiles?.Length ?? 0) > 0;
            var traditionalBridgeFolder = hasTraditionalBridge ? Path.GetDirectoryName(traditionalBridgeFiles[0]) : string.Empty;

            var distinctTemplateFolders = templateFiles.Select(fileName => Path.GetDirectoryName(fileName)).Distinct().OrderBy(folder => folder.Length).ToList();
            if (distinctTemplateFolders.Count > 0)
            {
                Console.WriteLine("Updating template files in {0}", distinctTemplateFolders.First());
                foreach (var distinctFolder in distinctTemplateFolders.Skip(1))
                {
                    Console.WriteLine("Found template files in {0}", distinctFolder);
                }
            }

            if (hasTraditionalBridge)
            {
                Console.WriteLine("Updating traditional bridge files in {0}", traditionalBridgeFolder);
            }

            await GitHubRelease.GetAndUnpackLatest(hasTraditionalBridge, traditionalBridgeFolder, distinctTemplateFolders, solution);

            UpgradeProjects(solution);
        }
    }
}