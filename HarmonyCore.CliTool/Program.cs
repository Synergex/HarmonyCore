using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool
{
    class Program
    {
        [Verb("upgrade-latest")]
        class UpgradeLatestOptions
        {
            [Option('p', "project")]
            public bool ProjectOnly { get; set; }
        }
        public static string CurrentVersionTag = "release-v3.1.5";
        public static string BuildPackageVersion = "11.1.1030.2704";
        public static string CodeDomProviderVersion = "1.0.7";
        public static string HCBuildVersion = "3.1.90";
        public static Dictionary<string, string> LatestNugetReferences = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            {"Harmony.Core", HCBuildVersion},
            {"HarmonyCore.CodeDomProvider", CodeDomProviderVersion},
            {"Harmony.Core.EF", HCBuildVersion},
            {"Harmony.Core.OData", HCBuildVersion},
            {"Harmony.Core.AspNetCore", HCBuildVersion},
            {"Synergex.SynergyDE.synrnt", "11.1.1031"},
            {"Synergex.SynergyDE.Build", BuildPackageVersion},
            {"Microsoft.AspNetCore.Mvc.NewtonsoftJson", "3.1.3"},
            {"Microsoft.Extensions.Logging.Console", "3.1.3"},
            {"Microsoft.AspNetCore.SignalR.Client", "3.1.3"},
            {"Microsoft.EntityFrameworkCore", "3.1.3"},
            {"IdentityServer4.AccessTokenValidation", "3.0.1"},
            {"Microsoft.AspNetCore.OData", "7.3.0"},
            {"Microsoft.OData.Core", "7.6.3"},
            {"Microsoft.AspNetCore.JsonPatch", "3.1.3"},
            {"Microsoft.VisualStudio.Threading", "16.5.132"},
            {"StreamJsonRpc", "2.3.103"},
            {"IdentityModel", "4.1.1" },
            {"Microsoft.OData.Edm", "7.6.3"},
            {"Microsoft.Spatial", "7.6.3"},
            {"Swashbuckle.AspNetCore", "5.2.1"},
            {"SSH.NET", "2016.1.0"},
            {"Microsoft.AspNetCore.Mvc.Versioning", "4.1.1"},
            {"Microsoft.AspNetCore.OData.Versioning.ApiExplorer", "4.1.1"},
            {"Nito.AsyncEx", "5.0.0"},
            {"System.Linq.Dynamic.Core", "1.0.22"},
            {"system.text.encoding.codepages", "4.7.0"},
        };
        
        static void Main(string[] args)
        {
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

            CommandLine.Parser.Default.ParseArguments<UpgradeLatestOptions, object>(args)
            .MapResult(
              (UpgradeLatestOptions opts) =>
              {
                  Console.WriteLine("This utility will make significant changes to projects and other source files in your Harmony Core development environment. Before running this tool we recommend checking the current state of your development environment into your source code repository, taking a backup copy of the environment if you don't use source code control.\n\n");


                  Console.WriteLine("Type YES to proceed: ");
                  if(string.Compare(Console.ReadLine(), "YES", true) != 0)
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
              (object opts) =>
              {
                  Console.WriteLine("no arguments passed, exiting");
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

            var client = new HttpClient();
            var targeturl = $"https://github.com/Synergex/HarmonyCore/archive/{CurrentVersionTag}.zip";
            var sourceDistStream = await client.GetStreamAsync(targeturl);
            var normalizer = new Regex(@"\r\n|\n\r|\n|\r", RegexOptions.Compiled);
            using (var zip = new ZipArchive(sourceDistStream, ZipArchiveMode.Read))
            {
                foreach(var entry in zip.Entries)
                {
                    if (entry.CompressedLength > 0 && entry.FullName.Contains("Templates"))
                    {
                        if (distinctTemplateFolders.Count > 0)
                        {
                            var targetFileName = Path.Combine(distinctTemplateFolders.First(), entry.FullName.Replace($"HarmonyCore-{CurrentVersionTag}/Templates/", "").Replace("/", "\\").Replace("\\\\", "\\"));

                            if (targetFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                                continue;

                            Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                            if (File.Exists(targetFileName))
                                File.Delete(targetFileName);

                            using (var stream = entry.Open())
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    File.WriteAllText(targetFileName, normalizer.Replace(reader.ReadToEnd(), "\r\n"));
                                }
                            }
                        }
                    }
                    else if (entry.CompressedLength > 0 && hasTraditionalBridge && entry.FullName.StartsWith($"HarmonyCore-{CurrentVersionTag}/TraditionalBridge/"))
                    {
                        var targetFileName = Path.Combine(traditionalBridgeFolder, Path.GetFileName(entry.FullName.Replace($"HarmonyCore-{CurrentVersionTag}", "").Replace("/", "\\").Replace("\\\\", "\\")));
                        if(File.Exists(targetFileName))
                            File.Delete(targetFileName);

                        using (var stream = entry.Open())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                File.WriteAllText(targetFileName, normalizer.Replace(reader.ReadToEnd(), "\r\n"));
                            }
                        }
                    }
                }
            }

            UpgradeProjects(solution);
        }
    }
}