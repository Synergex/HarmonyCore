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
        public SolutionInfo(IEnumerable<string> projectPaths, string solutionDir)
        {
            SolutionDir = solutionDir;
            Projects = projectPaths.Select(path => new ProjectInfo(path)).ToList();
            var codegenProjectPath = Path.Combine(SolutionDir, "Harmony.Core.CodeGen.json");
            var regenPath = Path.Combine(SolutionDir, "regen.bat");
            var userTokenFile = Path.Combine(SolutionDir, "UserDefinedTokens.tkn");
            try
            {

                var instances = MSBuildLocator.QueryVisualStudioInstances().ToList();
                var msbuildDeploymentToUse = instances.FirstOrDefault();

                // Calling Register methods will subscribe to AssemblyResolve event. After this we can
                // safely call code that use MSBuild types (in the Builder class).
                
                Console.WriteLine($"Using MSBuild from path: {msbuildDeploymentToUse.MSBuildPath}");
                Console.WriteLine();

                MSBuildLocator.RegisterMSBuildPath(msbuildDeploymentToUse.MSBuildPath);
                


                if (File.Exists(codegenProjectPath))
                {
                    CodeGenSolution = Solution.LoadSolution(codegenProjectPath, solutionDir);
                }
                else if (File.Exists(regenPath))
                {
                    CodeGenSolution = Solution.LoadSolution(regenPath, userTokenFile, solutionDir);
                    SaveSolution();
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
         
        public void SaveSolution()
        {
            var settings = new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };
            File.WriteAllText(Path.Combine(SolutionDir, "Harmony.Core.CodeGen.json"), JsonConvert.SerializeObject(CodeGenSolution, settings));
        }

        public List<ProjectInfo> Projects { get; }
        public string SolutionDir { get; }
        public Solution CodeGenSolution { get; }
    }
}