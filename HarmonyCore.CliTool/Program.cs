using CommandLine;
using HarmonyCore.CliTool.Commands;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HarmonyCore.CliTool
{
    [Verb("gui", false)]
    internal class GUIOptions
    {
    }
    [Verb("reload-bat", false)]
    internal class ReloadBatOptions
    {
    }
    [Verb("upgrade-latest")]
    class UpgradeLatestOptions
    {
        [Option('p', "project")]
        public bool ProjectOnly { get; set; }
        [Option('t', "template-url")]
        public string OverrideTemplateUrl { get; set; }
        [Option('v', "template-version")]
        public string OverrideTemplateVersion { get; set; }
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
        [Option('s',Default = null, Required = false, Separator = ',', HelpText = "Specify the list of structures, separated by a comma")]
        public IEnumerable<string> Structures { get; set; }
        [Option('i', Default = null, Required = false, Separator = ',', HelpText = "Specify the list of interfaces, separated by a comma")]
        public IEnumerable<string> Interfaces { get; set; }
        [Option('g', Default = null, Required = false, Separator = ',', HelpText = "Specify the list of generators, separated by a comma")]
        public IEnumerable<string> Generators { get; set; }
    }

    [Verb("xmlgen")]
    class XMLGenOptions
    {
        [Option('s', Required = true, Separator = ',', HelpText = "Specify the list of structures, separated by a comma")]
        public IEnumerable<string> Structures { get; set; }

        [Option('x', Required = true, HelpText = "Specify the location of the XMLFolder to generate from.")]
        public string XMLFolder { get; set; }
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

    public class VersionTargetingInfo
    {
        public VersionTargetingInfo(int majorVersionTarget)
        {
            switch (majorVersionTarget)
            {
                case 3:
                    HCBuildVersion = "3.1.463";
                    BuildPackageVersion = "11.1.1070.3107";
                    HCRegenRequiredVersions = new List<string>
                    {
                        "3.1.156"
                    };
                    NugetReferences = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
                    {
                        {"Harmony.Core", HCBuildVersion},
                        {"Harmony.Core.EF", HCBuildVersion},
                        {"Harmony.Core.OData", HCBuildVersion},
                        {"Harmony.Core.AspNetCore", HCBuildVersion},
                        {"Synergex.SynergyDE.synrnt", "11.1.1070"},
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
                    TargetFramework = "netcoreapp3.1";
                    break;

                case 6:
                    HCBuildVersion = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                    BuildPackageVersion = "22.8.1287";
                    HCRegenRequiredVersions = new List<string>
                    {
                        "3.1.156",
                        "3.1.999"
                    };
                    NugetReferences = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
                    {
                        {"Harmony.Core", HCBuildVersion},
                        {"Harmony.Core.EF", HCBuildVersion},
                        {"Harmony.Core.OData", HCBuildVersion},
                        {"Harmony.Core.AspNetCore", HCBuildVersion},
                        {"Synergex.SynergyDE.synrnt", "12.1.1.3278"},
                        {"Synergex.SynergyDE.Build", BuildPackageVersion},
                        {"Microsoft.AspNetCore.Mvc.NewtonsoftJson", "6.0.2"},
                        {"Microsoft.AspNetCore.Mvc.Testing", "6.0.2"},
                        {"Microsoft.Extensions.DependencyInjection", "6.0.0"},
                        {"Microsoft.Extensions.Logging.Console", "6.0.0"},
                        {"Microsoft.Extensions.Primitives", "6.0.0" },
                        {"Microsoft.AspNetCore.SignalR.Client", "6.0.2"},
                        {"Microsoft.EntityFrameworkCore", "6.0.3"},
                        {"IdentityServer4.AccessTokenValidation", "3.0.1"},
                        {"Microsoft.AspNetCore.OData", "8.0.8"},
                        {"Microsoft.OData.Core", "7.10.0"},
                        {"Microsoft.AspNetCore.JsonPatch", "6.0.2"},
                        {"Microsoft.VisualStudio.Threading", "17.1.46"},
                        {"StreamJsonRpc", "2.10.44"},
                        {"IdentityModel", "6.0.0" },
                        {"Microsoft.OData.Edm", "7.10.0"},
                        {"Microsoft.Spatial", "7.10.0 "},
                        {"Swashbuckle.AspNetCore", "6.2.3"},
                        {"SSH.NET", "2020.0.2"},
                        {"Nito.AsyncEx", "5.1.2"},
                        {"System.Linq.Dynamic.Core", "1.2.18"},
                        {"system.text.encoding.codepages", "6.0.0"},
                        {"Microsoft.IdentityModel.Tokens", "6.16.0"},
                        {"Newtonsoft.Json", "13.0.1"},
                        {"System.ComponentModel.Annotations", "5.0.0" }
                    };
                    TargetFramework = "net6.0";
                    RemoveNugetReferences = new List<string>
                    {
                        "Microsoft.AspNetCore.OData.Versioning.ApiExplorer",
                        "Microsoft.AspNetCore.Mvc.Versioning",
                        "Microsoft.AspNetCore.OData.Versioning",
                        "Microsoft.AspNetCore.SignalR"
                    };
                    break;
                default:
                    throw new Exception("Invalid version specified");
            }
        }
        public Dictionary<string, string> NugetReferences;
        public string BuildPackageVersion = "11.1.1070.3107";
        public string HCBuildVersion;
        public string TargetFramework = "net6.0";
        public List<string> HCRegenRequiredVersions;
        public List<string> RemoveNugetReferences = new List<string>();
    }

    class Program
    {
        public static VersionTargetingInfo TargetVersion;

        const int STD_INPUT_HANDLE = -10;
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        static (IntPtr stdout, uint consoleMode) GetConsoleState()
        {
            try
            {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var handle = GetStdHandle(STD_INPUT_HANDLE);
                    uint currentMode = 0;
                    GetConsoleMode(handle, out currentMode);
                    return (handle, currentMode);
                }
            }
            catch
            {

            }

            return (IntPtr.Zero, 0);
        }

        static void ResetConsoleMode(IntPtr stdout, uint consoleMode)
        {
            if (stdout != IntPtr.Zero)
            {
                SetConsoleMode(stdout, consoleMode);
            }
        }

        public static Dictionary<string, string> AppSettings = new Dictionary<string, string>();

        static SolutionInfo LoadSolutionInfo(Action<string> logger)
        {
            var solutionDir = Environment.GetEnvironmentVariable("SolutionDir") ?? Environment.CurrentDirectory;
            logger(string.Format("Scanning '{0}' for HarmonyCore project files", solutionDir));
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
                logger(string.Format("error while searching for project files: {0}", ex.Message));
                return null;
            }

            var instances = MSBuildLocator.QueryVisualStudioInstances().ToList();
            var msbuildDeploymentToUse = instances.FirstOrDefault();
            if (msbuildDeploymentToUse == null)
            {
                logger("Failed to locate MSBuild path");
                return null;
            }
            // Calling Register methods will subscribe to AssemblyResolve event. After this we can
            // safely call code that use MSBuild types (in the Builder class).

            logger($"Using MSBuild from path: {msbuildDeploymentToUse.MSBuildPath}");
            logger("");

            if (!MSBuildLocator.IsRegistered)
                MSBuildLocator.RegisterMSBuildPath(msbuildDeploymentToUse.MSBuildPath);

            return new SolutionInfo(synprojFiles, solutionDir, TargetVersion);
        }

        static void Main(string[] args)
        {
            var (handle, mode) = GetConsoleState();
            var versionOverride = Environment.GetEnvironmentVariable("HC_VERSION");
            if (int.TryParse(versionOverride, out var version))
                TargetVersion = new VersionTargetingInfo(version);
            else
                TargetVersion = new VersionTargetingInfo(6);

            //force synrnt to load
            Synergex.SynergyDE.AlphaDesc.notPassedAlpha.CheckPassed();

            ResetConsoleMode(handle, mode);

            string appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Harmony.Core.CLITool");
            // Ensure the directory and all its parents exist.
            Directory.CreateDirectory(appData);

            var configData = Path.Combine(appData, "config.json");
            if (File.Exists(configData))
            {
                AppSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configData));
            }
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => File.WriteAllText(configData, JsonConvert.SerializeObject(AppSettings));

            var defaultLoader = () => LoadSolutionInfo((str) => Console.WriteLine(str));

            _ = Parser.Default.ParseArguments<UpgradeLatestOptions, CodegenListOptions, CodegenAddOptions, CodegenRemoveOptions, RpsOptions, RegenOptions, XMLGenOptions, GUIOptions, ReloadBatOptions>(args)
            .MapResult<UpgradeLatestOptions, CodegenListOptions, CodegenAddOptions, CodegenRemoveOptions, RpsOptions, RegenOptions, XMLGenOptions, GUIOptions, ReloadBatOptions, int>(

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
                      UpgradeProjects(defaultLoader());
                  else
                      UpgradeLatest(defaultLoader(), opts.OverrideTemplateVersion, opts.OverrideTemplateUrl).Wait();
                  return 0;
              },
              new CodegenCommand(defaultLoader).List,
              new CodegenCommand(defaultLoader).Add,
              new CodegenCommand(defaultLoader).Remove,
              new RPSCommand(defaultLoader).Run,
              new RegenCommand(defaultLoader).Run,
              new XMLGenCommand().Run,
              new GUICommand(LoadSolutionInfo).Run,
              (ReloadBatOptions opts) =>
              {
                  var solutionInfo = defaultLoader();
                  var regenPath = Path.Combine(solutionInfo.SolutionDir, "regen.bat");
                  var regenConfigPath = Path.Combine(solutionInfo.SolutionDir, "regen_config.bat");
                  var userTokenFile = Path.Combine(solutionInfo.SolutionDir, "UserDefinedTokens.tkn");
                  solutionInfo.LoadFromBat(solutionInfo.SolutionDir, File.Exists(regenPath) ? regenPath : regenConfigPath, userTokenFile);
                  return 0;
              },
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
                project.PatchKnownIssues(TargetVersion.RemoveNugetReferences);
                project.PatchNugetVersions(TargetVersion.NugetReferences);
                project.Save();
            }
        }

        static async Task UpgradeLatest(SolutionInfo solution, string overrideTemplateUrl, string overrideTemplateVersion)
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

            await GitHubRelease.GetAndUnpackLatest(hasTraditionalBridge, traditionalBridgeFolder, distinctTemplateFolders, solution, overrideTemplateVersion, overrideTemplateUrl);

            UpgradeProjects(solution);
        }
    }
}