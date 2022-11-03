using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Helpers
{
    internal class DotnetTool
    {
        public static async Task<bool> UpdateTemplates(Action<string> errorLogger)
        {
            //var dotnetInfo = new Process();
            //dotnetInfo.StartInfo = new ProcessStartInfo("dotnet", "new install Harmony.Core.ProjectTemplates");
            //dotnetInfo.StartInfo.RedirectStandardOutput = true;
            //dotnetInfo.Start();
            //var dotnetInfoOutput = await dotnetInfo.StandardOutput.ReadToEndAsync();
            //if (dotnetInfo.HasExited && dotnetInfo.ExitCode != 0)
            //{
            //    errorLogger(dotnetInfoOutput);
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
            return true;
        }
        public static async Task<bool> InstantiateTemplate(string solutionPath, string templateName, string outputFolder, Action<string> logger, Action<string> fileWatcher)
        {
            if (await UpdateTemplates(logger))
            {
                Directory.CreateDirectory(outputFolder);
                using var fsw = new FileSystemWatcher(outputFolder, "*.synproj");
                fsw.Created += (sender, args) =>
                {
                    fileWatcher(args.FullPath);
                };
                fsw.IncludeSubdirectories = true;
                fsw.EnableRaisingEvents = true;

                var startInfo = new ProcessStartInfo("dotnet", $"new {templateName} -o {outputFolder}");
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
                startInfo.RedirectStandardInput = true;
                startInfo.Environment["SolutionDir"] = Path.GetDirectoryName(solutionPath) + "\\";
                startInfo.WorkingDirectory = outputFolder;
                var result = await ProcessAsyncHelper.RunAsync(startInfo, logger);
                return result.ExitCode == 0;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> AddProjectToSolution(string projectPath, string solutionPath, Action<string> logger)
        {
            var startInfo = new ProcessStartInfo("dotnet", $"sln add {projectPath}");
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.Environment["SolutionDir"] = Path.GetDirectoryName(solutionPath) + "\\";
            startInfo.WorkingDirectory = Path.GetDirectoryName(solutionPath);
            var result = await ProcessAsyncHelper.RunAsync(startInfo, logger);
            return result.ExitCode == 0;
            
        }

        public static async Task<bool> RemoveProjectFromSolution(string projectPath, string solutionPath, bool alsoDeleteFiles, Action<string> logger)
        {
            var startInfo = new ProcessStartInfo("dotnet", $"sln remove {projectPath}");
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = true;
            startInfo.Environment["SolutionDir"] = Path.GetDirectoryName(solutionPath) + "\\";
            startInfo.WorkingDirectory = Path.GetDirectoryName(solutionPath);
            var result = await ProcessAsyncHelper.RunAsync(startInfo, logger);
            if (result.ExitCode == 0)
            {
                if (alsoDeleteFiles)
                {
                    var projectDir = Path.GetDirectoryName(projectPath);
                    
                    if (!string.IsNullOrWhiteSpace(projectDir) && projectDir != Path.GetDirectoryName(solutionPath) && projectDir.Contains(solutionPath))
                    {
                        logger($"Deleting folder {projectDir} and its contents");
                        Directory.Delete(projectDir);
                    }
                    else
                    {
                        logger($"Error: not deleting folder {projectDir} because it's invalid with supplied solutionPath {solutionPath}");
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<bool> AddTemplateToSolution(string templateName, string outputFolder,
            string solutionPath, Action<string> logger)
        {
            var newProjectFiles = new List<string>();
            if (await InstantiateTemplate(solutionPath, templateName, outputFolder, logger,
                    (newProjFile) => newProjectFiles.Add(newProjFile)))
            {
                foreach (var projFile in newProjectFiles)
                {
                    if (!await AddProjectToSolution(projFile, solutionPath, logger))
                        return false;
                }

                return true;
            }

            return false;
        }

        public static async Task<bool> RunProject(string solutionDir, string projectName, Action<string> logger)
        {
            var startInfo = new ProcessStartInfo("dotnet", $"run {Path.GetFileName(projectName)}");
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            startInfo.Environment["SolutionDir"] = solutionDir + "\\";
            startInfo.WorkingDirectory = Path.GetDirectoryName(projectName);
            var result = await ProcessAsyncHelper.RunAsync(startInfo, logger);
            return result.ExitCode == 0;
        }
        
    }
}
