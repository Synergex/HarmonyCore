using HarmonyCore.CliTool.TUI.Models;
using HarmonyCore.CliTool.TUI.ViewModels;
using HarmonyCore.CliTool.TUI.Views;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;

namespace HarmonyCore.CliTool.Commands
{
    internal class GUICommand
    {
        private SolutionInfo _solutionInfo;

        public GUICommand(SolutionInfo solutionInfo) => this._solutionInfo = solutionInfo;

        public int Run(GUIOptions options)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;
            Application.Init();
            Application.HeightAsBuffer = false;
            
            Application.Run(new MainView(_solutionInfo));
            return 0;
            //var guiPath = Path.Combine(System.AppContext.BaseDirectory, "gui", "HarmonyCore.CliTool.TUI.exe");
            //if (!File.Exists(guiPath))
            //{
            //    guiPath = Path.Combine(System.AppContext.BaseDirectory, "HarmonyCore.CliTool.TUI.exe");
            //}
            //if (File.Exists(guiPath))
            //{
            //    Process process = Process.Start(guiPath, Path.Combine(this._solutionInfo.SolutionDir, "Harmony.Core.CodeGen.json"));
            //    process.WaitForExit();
            //    return process.ExitCode;
            //}
            //else
            //{
            //    Console.WriteLine("failed to locate harmony core gui executable");
            //    return -1;
            //}
        }
    }
}
