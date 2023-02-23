using HarmonyCoreGenerator.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool
{
    public class SolutionInfo
    {
        public Func<string, ProjectInfo> LoadProject;

        public static async Task<SolutionInfo> LoadSolutionInfoAsync(IEnumerable<string> projectPaths,
            string solutionDir, Action<string> logger)
        {
            var result = new SolutionInfo();
            result.SolutionDir = solutionDir;

            var solutionFiles = Directory.EnumerateFiles(solutionDir, "*.sln", SearchOption.TopDirectoryOnly).ToList();
            if (solutionFiles.Count() > 1)
            {
                var bestSolutionName = Path.Combine(new DirectoryInfo(solutionDir).Name, ".sln");
                result.SolutionPath = solutionFiles.FirstOrDefault(file =>
                    file.EndsWith(bestSolutionName, StringComparison.CurrentCultureIgnoreCase));
            }

            if (string.IsNullOrWhiteSpace(result.SolutionPath))
            {
                result.SolutionPath = solutionFiles.First();
            }

            var codegenProjectPath = Path.Combine(result.SolutionDir, "Harmony.Core.CodeGen.json");
            var regenPath = Path.Combine(result.SolutionDir, "regen.bat");
            var regenConfigPath = Path.Combine(result.SolutionDir, "regen_config.bat");
            var userTokenFile = Path.Combine(result.SolutionDir, "UserDefinedTokens.tkn");
            try
            {
                var basePath = Solution.GetDotnetBasePath();
                var projectOptions = new Microsoft.Build.Definition.ProjectOptions();
                var evalContext = Microsoft.Build.Evaluation.Context.EvaluationContext.Create(Microsoft.Build.Evaluation.Context.EvaluationContext.SharingPolicy.Isolated);
                //make sure we dont have any leftovers before we start a new load operation
                Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.UnloadAllProjects();

                projectOptions.EvaluationContext = evalContext;
                projectOptions.GlobalProperties = new Dictionary<String, String>();
                projectOptions.GlobalProperties.Add("SolutionDir", result.SolutionDir + "\\");
                projectOptions.GlobalProperties.Add("Configuration", "Debug");
                projectOptions.GlobalProperties.Add("Platform", "AnyCPU");
                projectOptions.GlobalProperties.Add("NuGetRestoreTargets", Path.Combine(basePath, "Nuget.targets"));
                result.LoadProject = (path) => new ProjectInfo(path, result.TryLoadProject(path, projectOptions));
                result.Projects = projectPaths.Select(result.LoadProject).ToList();
                var commonEnvVars = result.Projects.FirstOrDefault(project => project.MSBuildProject.GetProperty("CommonEnvVars") != null)?.MSBuildProject?.GetProperty("CommonEnvVars");
                if (commonEnvVars?.EvaluatedValue != null)
                {
                    var splitVars = commonEnvVars.EvaluatedValue.Split(';');
                    foreach (var envVar in splitVars)
                    {
                        var parts = envVar.Split('=');
                        if (parts.Length == 2)
                            Environment.SetEnvironmentVariable(parts[0], parts[1]);
                        else if (parts.Length == 1)
                            Environment.SetEnvironmentVariable(parts[0], null);
                    }
                }

                if (File.Exists(codegenProjectPath))
                {
                    result.CodeGenSolution = await (await DynamicCodeGenerator.LoadDynamicConfig(Path.Combine(result.SolutionDir, "Config")))(codegenProjectPath, solutionDir, logger);
                }
                else if (File.Exists(regenPath))
                {
                    result.LoadFromBat(solutionDir, regenPath, userTokenFile);
                }
                else if (File.Exists(regenConfigPath))
                {
                    result.LoadFromBat(solutionDir, regenConfigPath, userTokenFile);
                }
                else
                {
                    Console.WriteLine("Unable to locate codegen project file or regen.bat");
                }
            }
            catch (Exception ex)
            {
                result.Projects = new List<ProjectInfo>();
                Console.WriteLine("WARNING: Exception while synthesizing codegen project information: {0}", ex);
            }

            return result;
        }

        private SolutionInfo()
        {
            
        }

        class VersionsResponse
        {
            public string[] Versions { get; set; }
        }

        public async Task<ValueTuple<bool, string, string>> UpToDateCheck()
        {
            var myVersion = System.Diagnostics.FileVersionInfo
                .GetVersionInfo(typeof(SolutionInfo).Assembly.Location)
                .FileVersion;
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(5000);
                var packageName = "Harmony.Core.CLITool";
                var url = $"https://api.nuget.org/v3-flatcontainer/{packageName}/index.json";
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url, cts.Token);
                var versionsResponseBytes = await response.Content.ReadAsStringAsync(cts.Token);
                var versionsResponse = JsonConvert.DeserializeObject<VersionsResponse>(versionsResponseBytes);
                var lastVersion = versionsResponse.Versions[^1]; //(length-1)
                return (Version.Parse(lastVersion) <= Version.Parse(myVersion), lastVersion, myVersion);
            }
            catch
            {

                return (true, myVersion, myVersion);
            }
        }

        private Microsoft.Build.Evaluation.Project TryLoadProject(string path, Microsoft.Build.Definition.ProjectOptions options)
        {
            try
            {
                return Microsoft.Build.Evaluation.Project.FromFile(path, options);
            }
            catch
            {
                var projectDoc = new System.Xml.XmlDocument { PreserveWhitespace = true };
                projectDoc.Load(path);
                var imports = projectDoc.GetElementsByTagName("Import").OfType<System.Xml.XmlNode>().Where(node => node.Attributes.GetNamedItem("Project")?.Value?.Contains("Synergex.SynergyDE.Traditional.targets") ?? false).ToList();
                var imports2 = projectDoc.GetElementsByTagName("Import").OfType<System.Xml.XmlNode>().Where(node => node.Attributes.GetNamedItem("Project")?.Value?.Contains("Synergex.SynergyDE.Traditional.UnitTest.targets") ?? false).ToList();
                foreach (var import in imports.Concat(imports2))
                {
                    import.ParentNode.RemoveChild(import);
                }

                try
                {
                    return Microsoft.Build.Evaluation.Project.FromXmlReader(new System.Xml.XmlNodeReader(projectDoc), options);
                }
                catch
                {
                    return null;
                }
            }
        }

        public void LoadFromBat(string solutionDir, string regenPath, string userTokenFile)
        {
            var targetVersion = Program.LoadVersionInfoSync(true);
            try
            {
                CodeGenSolution = Solution.LoadSolution(regenPath, userTokenFile, solutionDir);
            }
            catch (InvalidOperationException)
            {
                var targetRps = Projects.FirstOrDefault(pi => pi.FileName.EndsWith("Repository.synproj", StringComparison.OrdinalIgnoreCase));
                if (targetRps != null)
                {
                    Console.WriteLine("Unable to load repository project, likely due to missing nuget package. Fix project file automatically? (Y/N)");
                    var response = Console.ReadKey().KeyChar;
                    if (response == 'Y' || response == 'y')
                    {
                        ProjectInfo.FixRPS(targetRps.ProjectDoc,
                            targetVersion.BuildPackageVersion, "Repository");
                        targetRps.Save();
                        CodeGenSolution = Solution.LoadSolution(regenPath, userTokenFile, solutionDir);
                    }
                    else
                    {
                        Console.WriteLine("Exiting");
                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("Exiting");
                    throw;
                }
            }
            SaveSolution();
        }

        public void SaveSolution()
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };
            File.WriteAllText(Path.Combine(SolutionDir, "Harmony.Core.CodeGen.json"), JsonConvert.SerializeObject(CodeGenSolution, settings));
        }

        public List<ProjectInfo> Projects { get; private set; }
        public string SolutionDir { get; set; }
        public string SolutionPath { get; set; }
        public Solution CodeGenSolution { get; set; }
    }
}