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
            Projects = projectPaths.Select(path => new ProjectInfo(path, targetVersion)).ToList();
            var codegenProjectPath = Path.Combine(SolutionDir, "Harmony.Core.CodeGen.json");
            var regenPath = Path.Combine(SolutionDir, "regen.bat");
            var regenConfigPath = Path.Combine(SolutionDir, "regen_config.bat");
            var userTokenFile = Path.Combine(SolutionDir, "UserDefinedTokens.tkn");
            try
            {

                var instances = MSBuildLocator.QueryVisualStudioInstances().ToList();
                var msbuildDeploymentToUse = instances.FirstOrDefault();

                // Calling Register methods will subscribe to AssemblyResolve event. After this we can
                // safely call code that use MSBuild types (in the Builder class).
                
                Console.WriteLine($"Using MSBuild from path: {msbuildDeploymentToUse.MSBuildPath}");
                Console.WriteLine();

                if(!MSBuildLocator.IsRegistered)
                    MSBuildLocator.RegisterMSBuildPath(msbuildDeploymentToUse.MSBuildPath);

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