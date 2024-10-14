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
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HarmonyCore.CliTool.TUI.Helpers;

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
        [Option('z', "zip-path")]
        public string CurrentTemplates { get; set; }
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
        [Option('p', Required = false, HelpText = "Sync project files after regen")]
        public bool Sync { get; set; } = false;
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
        [Option("tb", SetName = "rps")]
        public bool Tb { get; set; }
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

    [Verb("features")]
    class FeaturesOptions
    {
        [Option("add-tb", HelpText = "Add support for running traditional Synergy code")]
        public bool TraditionalBridge { get; set; }

        [Option("smc", HelpText = "Provide path to SMC file to enable xfServerPlus")]
        public string EnableSMCImport { get; set; }

        [Option("add-unit-tests", HelpText = "Add support for running unit tests")]
        public bool AddUnitTests { get; set; }

        [Option("collect-test-data", HelpText = "Collect test data for your Unit Tests")]
        public bool CollectTestData { get; set; }
    }

    public class VersionTargetingInfo
    {
        public class Root
        {
            public List<KnownVersion> KnownVersions { get; set; }
        }
        public class KnownVersion
        {
            public string HCVersion { get; set; }
            public string TemplatesVersionPrefix { get; set; }
            public string TargetFramework { get; set; }
            public string BuildPackageVersion { get; set; }
            public List<string> HCRegenRequiredVersions { get; set; }
            public Dictionary<string, string> NugetVersions { get; set; }
            public List<string> RemoveReferences { get; set; }
        }
        private KnownVersion _knownVersion;

        private VersionTargetingInfo(KnownVersion version)
        {
            _knownVersion = version;
        }

        public static async Task<VersionTargetingInfo> GetVersionTargetingInfo(int majorVersionTarget, bool skipCache)
        {
            switch (majorVersionTarget)
            {
                case 3:
                    return new VersionTargetingInfo(await LoadKnownVersion("netcoreapp3.1", skipCache));
                case 6:
                    return new VersionTargetingInfo(await LoadKnownVersion("net6.0", skipCache));
                case 8:
                default:
                    return new VersionTargetingInfo(await LoadKnownVersion("net8.0", skipCache));
            }
        }

        public static async Task<KnownVersion> LoadKnownVersion(string prefix, bool skipCache)
        {
            var toolVersion = await GitHubRelease.GetCliToolVersions(skipCache);
            var knownVersions = JsonConvert.DeserializeObject<Root>(toolVersion).KnownVersions;
            return knownVersions.First(ver => ver.TargetFramework.StartsWith(prefix));
        }

        public Dictionary<string, string> NugetReferences => _knownVersion.NugetVersions;
        public string BuildPackageVersion => _knownVersion.BuildPackageVersion;
        public string HCBuildVersion => _knownVersion.HCVersion;
        public string TargetFramework => _knownVersion.TargetFramework ?? "net8";
        public List<string> HCRegenRequiredVersions => _knownVersion.HCRegenRequiredVersions ?? new List<string>();
        public List<string> RemoveNugetReferences => _knownVersion.RemoveReferences ?? new List<string>();
    }

    class Program
    {
        public const int RELOAD_SOLUTION = 1337;
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

        public static string AppFolder;
        public static Dictionary<string, string> AppSettings = new Dictionary<string, string>();

        public static async Task<SolutionInfo> LoadSolutionInfo(Action<string> logger)
        {
            var rawSolDir = Environment.GetEnvironmentVariable("SolutionDir");
            var solutionDir = string.IsNullOrEmpty(rawSolDir) ? string.Empty : Environment.ExpandEnvironmentVariables(rawSolDir);

            if(!Directory.Exists(solutionDir))
            {
                logger($"SolutionDir '{solutionDir}' is not set. Using the current working directory.");
                solutionDir = string.Empty;
            }

            if (string.IsNullOrWhiteSpace(solutionDir))
            {
                var startingDirectory = Environment.CurrentDirectory;
                while(!Directory.EnumerateFiles(startingDirectory, "*.sln").Any())
                {
                    var nextPath = Path.GetFullPath(Path.Combine(startingDirectory, ".." + Path.DirectorySeparatorChar));
                    if (nextPath == startingDirectory)
                    {
                        logger("Failed to locate solution starting from current working directory, move to the target folder or set the 'SolutionDir' environment variable to point to your target solution");
                        throw new InvalidOperationException("failure during initialization, see logged message");
                    }
                    else
                    {
                        startingDirectory = nextPath;
                    }
                }

                solutionDir = startingDirectory;
            }
            else
                Environment.SetEnvironmentVariable("SolutionDir", solutionDir);

            logger($"Scanning '{solutionDir}' for HarmonyCore project files");
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
                logger($"error while searching for project files: {ex.Message}");
                return null;
            }

            //VisualStudioInstanceQueryOptions
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

            return await SolutionInfo.LoadSolutionInfoAsync(synprojFiles, solutionDir, logger);
        }

        static void Main(string[] args)
        {
            //clear CODEGEN_TPLDIR to prevent unexpected codegen templates being used
            Environment.SetEnvironmentVariable("CODEGEN_TPLDIR", null);
            Environment.SetEnvironmentVariable("CODEGEN_OUTDIR", null);
            var (handle, mode) = GetConsoleState();
            //force synrnt to load
            Synergex.SynergyDE.AlphaDesc.notPassedAlpha.CheckPassed();

            ResetConsoleMode(handle, mode);

            AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify), "Harmony.Core.CLITool");
            // Ensure the directory and all its parents exist.
            Directory.CreateDirectory(AppFolder);

            var currentDir = Environment.CurrentDirectory;
            var slnIsInCurrDir = Directory.EnumerateFiles(currentDir, "*.sln").Any();

            var solutionDir = Environment.ExpandEnvironmentVariables("%SolutionDir%") ?? "";
            var slnIsInSolutionDir = Directory.Exists(solutionDir) && Directory.EnumerateFiles(solutionDir, "*.sln").Any();

            if (!slnIsInCurrDir && !slnIsInSolutionDir)
            {
                var loadErrorMessage = "Solution Load Error: No solution file (.sln) can be found.\n"
                                     + "Open the Harmony Core CLI tool in a directory with the .sln\n"
                                     + "file for the target solution, or set the SolutionDir\n"
                                     + "environment variable to the directory with the solution\n"
                                     + "file before opening the CLI tool.";
                Console.WriteLine(loadErrorMessage);
                return;
            }

            var configData = Path.Combine(AppFolder, "config.json");
            if (File.Exists(configData))
            {
                AppSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(configData));
            }
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => File.WriteAllText(configData, JsonConvert.SerializeObject(AppSettings));

            

            while(ProcessCommandArgs(args) == RELOAD_SOLUTION)
            {
                var defaultLoader = () => LoadSolutionInfo((str) => Console.WriteLine(str));
                ReloadBatFile(defaultLoader);
            }
        }

        private static int ProcessCommandArgs(string[] args)
        {
            var defaultLoader = () => LoadSolutionInfo((str) => Console.WriteLine(str));
            return Parser.Default.ParseArguments<UpgradeLatestOptions, CodegenListOptions, CodegenAddOptions, CodegenRemoveOptions, RpsOptions, RegenOptions, XMLGenOptions, GUIOptions, ReloadBatOptions, FeaturesOptions>(args)
                        .MapResult(

                          (UpgradeLatestOptions opts) =>
                          {
                              Console.WriteLine("This utility will make significant changes to projects and other source files in your Harmony Core development environment. Before running this tool we recommend checking the current state of your development environment into your source code repository, taking a backup copy of the environment if you don't use source code control.\n\n");
                              var zipPath = opts.CurrentTemplates;


                              Console.WriteLine("Type YES to proceed: ");
                              if (string.Compare(Console.ReadLine(), "YES", true) != 0)
                              {
                                  Console.WriteLine("exiting without changes");
                                  return 1;
                              }

                              Console.WriteLine("Checking for current version info");
                              var versionInfo = zipPath != null ? LoadVersionInfoSync(false) : LoadVersionInfoSync(true);
                              
                              if (opts.ProjectOnly)
                                  UpgradeProjects(defaultLoader().Result, versionInfo);
                              else
                                  UpgradeLatest(defaultLoader().Result, versionInfo, opts.OverrideTemplateVersion, opts.OverrideTemplateUrl, null, zipPath).Wait();
                              return 0;
                          },
                          (Func<CodegenListOptions, int>)new CodegenCommand(defaultLoader).List,
                          (Func<CodegenAddOptions, int>)new CodegenCommand(defaultLoader).Add,
                          (Func<CodegenRemoveOptions, int>)new CodegenCommand(defaultLoader).Remove,
                          (Func<RpsOptions, int>)new RPSCommand(defaultLoader).Run,
                          (Func<RegenOptions, int>)new RegenCommand(defaultLoader).Run,
                          (Func<XMLGenOptions, int>)new XMLGenCommand().Run,
                          (Func<GUIOptions, int>)new GUICommand(LoadSolutionInfo).Run,
                          (Func<FeaturesOptions, int>)new FeaturesCommand(defaultLoader).Run,
                          (ReloadBatOptions opts) =>
                          {
                              ReloadBatFile(defaultLoader);
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

        private static void ReloadBatFile(Func<Task<SolutionInfo>> defaultLoader)
        {
            var solutionInfo = defaultLoader().Result;
            var regenPath = Path.Combine(solutionInfo.SolutionDir, "regen.bat");
            var regenConfigPath = Path.Combine(solutionInfo.SolutionDir, "regen_config.bat");
            var userTokenFile = Path.Combine(solutionInfo.SolutionDir, "UserDefinedTokens.tkn");
            solutionInfo.LoadFromBat(solutionInfo.SolutionDir, File.Exists(regenPath) ? regenPath : regenConfigPath, userTokenFile);
        }

        public static VersionTargetingInfo LoadVersionInfoSync(bool skipCache)
        {
            return Task.Run(async () =>
            {
                var versionOverride = Environment.GetEnvironmentVariable("HC_VERSION");
                if (int.TryParse(versionOverride, out var version))
                    return await VersionTargetingInfo.GetVersionTargetingInfo(version, skipCache);
                else
                    return await VersionTargetingInfo.GetVersionTargetingInfo(8, skipCache);
            }).Result;
        }

        public static void UpgradeProjects(SolutionInfo solution, VersionTargetingInfo versionInfo)
        {
            foreach (var project in solution.Projects)
            {
                project.PatchKnownIssues(versionInfo);
                project.PatchNugetVersions(versionInfo);
                project.Save();
            }
        }

        public static async Task UpgradeLatest(SolutionInfo solution, VersionTargetingInfo versionInfo, string overrideTemplateUrl, string overrideTemplateVersion, GenerationEvents? events, string zipPath)
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

                string msg = string.Format("Updating template files in {0}", distinctTemplateFolders.First());
                events?.StatusUpdate?.Invoke(msg);
                if (events?.StatusUpdate == null)
                {
                    Console.WriteLine(msg);
                }
                foreach (var distinctFolder in distinctTemplateFolders.Skip(1))
                {
                    msg = string.Format("Found template files in {0}", distinctFolder);
                    events?.Message(msg);
                    if (events?.Message == null)
                    {
                        Console.WriteLine(msg);
                    }
                }
            }

            if (hasTraditionalBridge)
            {
                string msg = string.Format("Updating traditional bridge files in {0}", traditionalBridgeFolder);
                events?.Message(msg);
                if (events?.Message == null)
                {
                    Console.WriteLine(msg);
                }
            }

            await GitHubRelease.GetAndUnpackLatest(hasTraditionalBridge, traditionalBridgeFolder, distinctTemplateFolders, solution, overrideTemplateVersion, overrideTemplateUrl, zipPath);

            UpgradeProjects(solution, versionInfo);
        }
    }
}