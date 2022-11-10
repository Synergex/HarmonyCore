using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Helpers
{
    internal class Dbl
    {
        public static async Task<bool> RunTCompile(string targetDirectory, string fileName, Action<string> logger)
        {
            var dotnetInfo = new Process();
            dotnetInfo.StartInfo = new ProcessStartInfo("cmd", $"/c \"%SYNERGYDE64%\\dbl\\dblvars64.bat && dbl {fileName}\"");
            dotnetInfo.StartInfo.RedirectStandardOutput = true;
            dotnetInfo.StartInfo.RedirectStandardInput = true;
            dotnetInfo.StartInfo.WorkingDirectory = targetDirectory;
            dotnetInfo.Start();
            var dotnetInfoOutput = await dotnetInfo.StandardOutput.ReadToEndAsync();
            logger(dotnetInfoOutput);
            return dotnetInfo.HasExited && dotnetInfo.ExitCode == 0;
        }

        public static async Task<bool> RunTLink(string targetDirectory, string fileName, Action<string> logger)
        {
            var dotnetInfo = new Process();
            dotnetInfo.StartInfo = new ProcessStartInfo("cmd", $"/c \"%SYNERGYDE64%\\dbl\\dblvars64.bat && dblink {fileName}\"");
            dotnetInfo.StartInfo.RedirectStandardOutput = true;
            dotnetInfo.StartInfo.RedirectStandardInput = true;
            dotnetInfo.StartInfo.WorkingDirectory = targetDirectory;
            dotnetInfo.Start();
            var dotnetInfoOutput = await dotnetInfo.StandardOutput.ReadToEndAsync();
            logger(dotnetInfoOutput);
            return dotnetInfo.HasExited && dotnetInfo.ExitCode == 0;
        }

        public static async Task<bool> RunDBR(string targetDirectory, string fileName, Action<string> logger)
        {
            var dotnetInfo = new Process();
            dotnetInfo.StartInfo = new ProcessStartInfo("cmd", $"/c \"%SYNERGYDE64%\\dbl\\dblvars64.bat && dbr {fileName}\"");
            dotnetInfo.StartInfo.RedirectStandardOutput = true;
            dotnetInfo.StartInfo.RedirectStandardInput = true;
            dotnetInfo.StartInfo.WorkingDirectory = targetDirectory;
            dotnetInfo.Start();
            var dotnetInfoOutput = await dotnetInfo.StandardOutput.ReadToEndAsync();
            logger(dotnetInfoOutput);
            return dotnetInfo.HasExited && dotnetInfo.ExitCode == 0;
        }

        public static async Task<bool> RunTSource(string targetDirectory, string fileName, Action<string> logger)
        {
            if (await RunTCompile(targetDirectory, fileName, logger))
            {
                if (await RunTLink(targetDirectory, Path.GetFileNameWithoutExtension(fileName), logger))
                {
                    return await RunDBR(targetDirectory, Path.GetFileNameWithoutExtension(fileName), logger);
                }
            }

            return false;
        }
    }
}
