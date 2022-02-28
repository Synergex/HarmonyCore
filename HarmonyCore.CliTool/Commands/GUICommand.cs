using System;
using System.Diagnostics;
using System.IO;

namespace HarmonyCore.CliTool.Commands
{
    internal class GUICommand
    {
        private SolutionInfo _solutionInfo;

        public GUICommand(SolutionInfo solutionInfo) => this._solutionInfo = solutionInfo;

        public int Run(GUIOptions options)
        {
            var guiPath = Path.Combine(System.AppContext.BaseDirectory, "gui", "HarmonyCoreCodeGenGUI.exe");
            if (!File.Exists(guiPath))
            {
                guiPath = Path.Combine(System.AppContext.BaseDirectory, "HarmonyCoreCodeGenGUI.exe");
            }
            if (File.Exists(guiPath))
            {
                Process process = Process.Start(guiPath, Path.Combine(this._solutionInfo.SolutionDir, "Harmony.Core.CodeGen.json"));
                process.WaitForExit();
                return process.ExitCode;
            }
            else
            {
                Console.WriteLine("failed to locate harmony core gui executable");
                return -1;
            }
        }
    }
}
