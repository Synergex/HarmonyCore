using HarmonyCore.CliTool.TUI.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Locator;

using System.Threading;
using CodeGen.Engine;
using HarmonyCore.CliTool.Commands;
using HarmonyCore.CliTool.TUI.Helpers;
using HarmonyCore.CliTool.TUI.Models;
using HarmonyCore.CliTool.TUI.ViewModels;
using HarmonyCoreGenerator.Model;
using HarmonyCore.CliTool.TUI.Views;
using Microsoft.Build.Evaluation;
using HarmonyCore.CliTool;

namespace HarmonyCore.CliTool.Commands
{
    internal class FeaturesCommand
    {
        private readonly Lazy<Task<SolutionInfo>> _loader;
        SolutionInfo _solutionInfo => _loader.Value.Result;

        public FeaturesCommand(Func<Task<SolutionInfo>> solutionInfo)
        {
            _loader = new Lazy<Task<SolutionInfo>>(solutionInfo);
        }

        public int Run(FeaturesOptions options)
        {
            var addUnitTests = AddUnitTests();
            addUnitTests.Wait();
            return 0;
        }

        public async Task AddUnitTests()
        {
            Console.WriteLine("This utility will make significant changes to projects and other source files in your Harmony Core development environment. Before running this tool we recommend checking the current state of your development environment into your source code repository, taking a backup copy of the environment if you don't use source code control.\n\n");
            Console.WriteLine("Type YES to proceed: ");
            if (string.Compare(Console.ReadLine(), "YES", true) != 0)
            {
                Console.WriteLine("exiting without changes");
            }
            else
            {
                var commonCommands = new CommonCommands(null);
                try
                {
                    await commonCommands.LoadTestProjects(_solutionInfo);
                    _solutionInfo.SaveSolution();
                    await commonCommands.RunRegen(_solutionInfo);
                    await commonCommands.RunUpgradeLatest();
                    await commonCommands.CollectCollectTestData(_solutionInfo);
                    Console.WriteLine("Finished");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
