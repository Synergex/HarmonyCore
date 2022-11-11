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
        private Func<Action<string>, Task<SolutionInfo>> _solutionLoader;

        public GUICommand(Func<Action<string>, Task<SolutionInfo>> solutionInfo) => this._solutionLoader = solutionInfo;

        public int Run(GUIOptions options)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;
            Application.Init();
            Application.HeightAsBuffer = false;
            
            Application.Run(new MainView(_solutionLoader), ex =>
            {
                Trace.WriteLine(ex);
                return false;
            });
            return 0;
        }
    }
}
