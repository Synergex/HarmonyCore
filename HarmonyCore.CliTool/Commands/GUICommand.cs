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
            Process process = Process.Start(Path.Combine(System.AppContext.BaseDirectory, "gui", "HarmonyCoreCodeGenGUI.exe"), Path.Combine(this._solutionInfo.SolutionDir, "Harmony.Core.CodeGen.json"));
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
