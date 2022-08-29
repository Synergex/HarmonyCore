using HarmonyCoreGenerator.Model;
using Microsoft.Build.Locator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HarmonyCore.CliTool
{
    public class SolutionInfo
    {
        public SolutionInfo(IEnumerable<string> projectPaths, string solutionDir, VersionTargetingInfo targetVersion)
        {
            SolutionDir = solutionDir;
            
            var codegenProjectPath = Path.Combine(SolutionDir, "Harmony.Core.CodeGen.json");
            var regenPath = Path.Combine(SolutionDir, "regen.bat");
            var regenConfigPath = Path.Combine(SolutionDir, "regen_config.bat");
            var userTokenFile = Path.Combine(SolutionDir, "UserDefinedTokens.tkn");
            try
            {
                var basePath = Solution.GetDotnetBasePath();
                var projectOptions = new Microsoft.Build.Definition.ProjectOptions();
                var evalContext = Microsoft.Build.Evaluation.Context.EvaluationContext.Create(Microsoft.Build.Evaluation.Context.EvaluationContext.SharingPolicy.Isolated);

                projectOptions.EvaluationContext = evalContext;
                projectOptions.GlobalProperties = new Dictionary<String, String>();
                projectOptions.GlobalProperties.Add("SolutionDir", SolutionDir + "\\");
                projectOptions.GlobalProperties.Add("Configuration", "Debug");
                projectOptions.GlobalProperties.Add("Platform", "AnyCPU");
                projectOptions.GlobalProperties.Add("NuGetRestoreTargets", Path.Combine(basePath, "Nuget.targets"));

                Projects = projectPaths.Select(path => new ProjectInfo(path, targetVersion, TryLoadProject(path, projectOptions))).ToList();

                if (File.Exists(codegenProjectPath))
                {
                    CodeGenSolution = Solution.LoadSolution(codegenProjectPath, solutionDir);
                }
                else if (File.Exists(regenPath))
                {
                    LoadFromBat(solutionDir, regenPath, userTokenFile);
                }
                else if (File.Exists(regenConfigPath))
                {
                    LoadFromBat(solutionDir, regenConfigPath, userTokenFile);
                }
                else
                {
                    Console.WriteLine("Unable to locate codegen project file or regen.bat");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("WARNING: Exception while synthesizing codegen project information: {0}", ex);
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
                foreach(var import in imports)
                {
                    import.ParentNode.RemoveChild(import);
                }

                return Microsoft.Build.Evaluation.Project.FromXmlReader(new System.Xml.XmlNodeReader(projectDoc), options);
            }
        }

        public void LoadFromBat(string solutionDir, string regenPath, string userTokenFile)
        {
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
                            Program.TargetVersion.BuildPackageVersion, "Repository");
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

        public List<ProjectInfo> Projects { get; }
        public string SolutionDir { get; set; }
        public Solution CodeGenSolution { get; set; }
    }
}