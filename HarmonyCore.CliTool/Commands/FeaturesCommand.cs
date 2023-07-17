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
using System.Drawing.Printing;

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
            var hasTraditionalBridge = _solutionInfo.CodeGenSolution.TraditionalBridge != null;
            var hasUnitTests = _solutionInfo.Projects.Any(proj => proj.FileName.EndsWith("Services.Test.synproj", StringComparison.CurrentCultureIgnoreCase));
            if (options.AddUnitTests)
            {
                if (!hasUnitTests)
                {
                    var addUnitTests = AddUnitTests();
                    addUnitTests.Wait();
                }
                else
                {
                    Console.WriteLine("Services.Test already exists. Run --collect-test-data to collect data for Unit Tests.");
                }
            }
            else if (options.CollectTestData)
            {
                if (hasUnitTests) 
                {
                    Console.WriteLine("Collect Test Data");
                    var commonCommands = new CommonCommands(null);
                    try
                    {
                        Task collectTestData = commonCommands.CollectTestData(_solutionInfo);
                        collectTestData.Wait();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    Console.WriteLine("Finished");
                }
                else
                {
                    Console.WriteLine("Run --add-unit-tests to add support for running unit tests.");
                }
            }
            else if (options.TraditionalBridge)
            {
                if (!hasTraditionalBridge)
                {
                    Console.WriteLine("Add Traditional Bridge");
                    var commonCommands = new CommonCommands(null);
                    try
                    {
                        Task addtb = commonCommands.AddTraditionalBridge(_solutionInfo);
                        addtb.Wait();
                        _solutionInfo.SaveSolution();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Traditional Bridge was already added.");
                }
            }
            else if (options.EnableSMCImport)
            {
                if (!hasTraditionalBridge)
                {
                    Console.WriteLine("You must run --add-tb prior to enabling SMC import.");
                }
                else if (_solutionInfo.CodeGenSolution.TraditionalBridge?.EnableXFServerPlusMigration == true)
                {
                    Console.WriteLine("The option is not available.");
                }
                else
                {
                    Console.WriteLine("Enable xfServerPlus import and select SMC");
                    var commonCommands = new CommonCommands(null);
                    try
                    {
                        // get smc path
                        Console.WriteLine("Enter a path to an SMC file to import interfaces from: ");
                        string smcPath = Console.ReadLine();
                        if (string.IsNullOrEmpty(smcPath) || !smcPath.Contains(".xml"))
                        {
                            Console.WriteLine("\nSMC selection canceled.");
                            return 1;
                        }
                        smcPath = Path.GetRelativePath(_solutionInfo.SolutionDir + "\\", smcPath);
                        if(File.Exists(smcPath))
                        {
                            Task enableSMC = commonCommands.AddSmc(_solutionInfo, smcPath);
                            enableSMC.Wait();
                            _solutionInfo.SaveSolution();
                        }
                        else
                        {
                            throw new FileNotFoundException("The SMC file was not found.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                Console.WriteLine("invalid arguments");
                return -1;
            }
            _solutionInfo.SaveSolution();
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
                    await commonCommands.CollectTestData(_solutionInfo);
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
