using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool
{
    class Program
    {
        public static string CurrentVersionTag = "release-v3.1";
        public static string BuildPackageVersion = "11.1.1030.2704";
        public static string CodeDomProviderVersion = "1.0.7";
        private static Dictionary<string, string> LatestNugetReferences = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase)
        {
            {"Harmony.Core", "3.1.17"},
            {"HarmonyCore.CodeDomProvider", CodeDomProviderVersion},
            {"Harmony.Core.EF", "3.1.17"},
            {"Harmony.Core.OData", "3.1.17"},
            {"Harmony.Core.AspNetCore", "3.1.17"},
            {"Synergex.SynergyDE.synrnt", "11.1.1030"},
            {"Synergex.SynergyDE.Build", BuildPackageVersion},
            {"Microsoft.AspNetCore.Mvc.NewtonsoftJson", "3.1.1"},
            {"Microsoft.Extensions.Logging.Console", "3.1.1"},
            {"Microsoft.AspNetCore.SignalR.Client", "3.1.2"},
            {"Microsoft.EntityFrameworkCore", "3.1.1"},
            {"Microsoft.AspNetCore.OData", "7.3.0"},
            {"Microsoft.OData.Core", "7.6.2"},
            {"Microsoft.OData.Edm", "7.6.2"},
            {"Microsoft.Spatial", "7.6.2"},
            {"Swashbuckle.AspNetCore", "5.0.0"},
            {"SSH.NET", "2016.1.0"},
            {"Microsoft.AspNetCore.Mvc.Versioning", "4.1.1"},
            {"Microsoft.AspNetCore.OData.Versioning.ApiExplorer", "4.1.1"},
            {"Nito.AsyncEx", "5.0.0"},
            {"System.Linq.Dynamic.Core", "1.0.20"},
            {"system.text.encoding.codepages", "4.7.0"},
        };
        
        static void Main(string[] args)
        {
            var solutionDir = Environment.GetEnvironmentVariable("SolutionDir") ?? Environment.CurrentDirectory;
            Console.WriteLine("Scanning '{0}' for HarmonyCore project files", solutionDir);
            var synprojFiles = Directory.GetFiles(
                solutionDir,
                "*.synproj",
                SearchOption.AllDirectories);

            var solutionInfo = new SolutionInfo(synprojFiles, solutionDir);
            
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "upgrade-latest":
                    {
                        UpgradeLatest(solutionInfo).Wait();
                        break;
                    }
                }
            }
        }

        static async Task UpgradeLatest(SolutionInfo solution)
        {
            var traditionalBridgeFolder = Path.Combine(solution.SolutionDir, "TraditionalBridge");
            var hasTraditionalBridge = Directory.Exists(traditionalBridgeFolder);
        
            var client = new HttpClient();
            var sourceDistStream = await client.GetStreamAsync($"https://github.com/Synergex/HarmonyCore/releases/download/{CurrentVersionTag}/HarmonyCoreSourceDist.zip");
            using(var zip = new ZipArchive(sourceDistStream, ZipArchiveMode.Read))
            {
                foreach(var entry in zip.Entries)
                {
                    if (entry.CompressedLength > 0 && entry.FullName.Contains("Templates"))
                    {
                        var targetFileName = Path.Combine(solution.SolutionDir,"Templates", entry.FullName.Replace("SourceDist/Templates/HCTemplates/", "").Replace("/", "\\"));
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFileName));
                        if(File.Exists(targetFileName))
                            File.Delete(targetFileName);
                        
                        entry.ExtractToFile(targetFileName);
                    }
                    else if (entry.CompressedLength > 0 && hasTraditionalBridge && entry.FullName.Contains("TraditionalBridge"))
                    {
                        var targetFileName = Path.Combine(traditionalBridgeFolder, Path.GetFileName(entry.FullName));
                        if(File.Exists(targetFileName))
                            File.Delete(targetFileName);
                        entry.ExtractToFile(targetFileName);
                    }
                }
            }
            //download templates and traditional bridge source
            //replace templates and traditional bridge source
            foreach (var project in solution.Projects)
            {
                project.PatchKnownIssues();
                project.PatchNugetVersions(LatestNugetReferences);
                project.Save();
            }
        }
    }
}