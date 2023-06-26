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

namespace HarmonyCore.CliTool.Commands
{
    internal class FeaturesCommand
    {
        /*private readonly Lazy<Task<SolutionInfo>> _loader;
        SolutionInfo _solutionInfo => _loader.Value.Result;
        public FeaturesCommand(Func<Task<SolutionInfo>> solutionInfo)
        {
            _loader = new Lazy<Task<SolutionInfo>>(solutionInfo);
        }

        public async Task<int> RunAsync(FeaturesOptions opts)
        {
            if (opts.TraditionalBridgeFeature)
            {

            }
            else if (opts.TraditionalBridgeSMCFeature)
            {

            }
            else if (opts.AddUnitTests)
            {
                 await HarmonyCore.CliTool.TUI.ViewModels.MainViewModel.AddUnitTests();
            }
            else if (opts.CollectTestData)
            {

            }
            else
            {
                Console.WriteLine("invalid arguments");
                return -1;
            }
            _solutionInfo.SaveSolution();
            return 0;

        */


        private readonly Lazy<Task<SolutionInfo>> _loader;
        SolutionInfo _solutionInfo => _loader.Value.Result;
        public FeaturesCommand(Func<Task<SolutionInfo>> solutionInfo)
        {
            _loader = new Lazy<Task<SolutionInfo>>(solutionInfo);
            GenerationEvents = new Solution.SolutionGenerationEvents() { Message = Logger, Error = Logger };
        }

        public Solution.SolutionGenerationEvents GenerationEvents { get; set; }

        public Action<string> CallerLogger { get; set; } = (str) => Console.WriteLine(str);
        private List<string> AddedFiles { get; } = new List<string>();
        private List<string> UpdatedFiles { get; } = new List<string>();
        public CancellationToken CancelToken { get; set; }


        public int Run(FeaturesOptions options)
        {
            GenerationEvents events = new GenerationEvents();
            
            var t = AddUnitTestsAsync(events);

            t.Wait();

            return 0;
        }

        public async Task<int> AddUnitTestsAsync(GenerationEvents events)
        {
            try
            {

                Console.WriteLine("Adding unit test project template");
                if (!await DotnetTool.AddTemplateToSolution("harmonycore-ut",
                        Path.Combine(_solutionInfo.SolutionDir, "Services.Test"), _solutionInfo.SolutionPath, events.Message))
                {
                    Console.WriteLine("Template creation failed for Services.Test");
                    return 1;
                }

                Console.WriteLine("Adding unit test value generator template");
                if (!await DotnetTool.AddTemplateToSolution("harmonycore-utg",
                        Path.Combine(_solutionInfo.SolutionDir, "Services.Test.GenerateValues"), _solutionInfo.SolutionPath, events.Message))
                {
                    Console.WriteLine("Template creation failed for Services.Test.GenerateValues");
                    return 1;
                }

                Console.WriteLine("Loading new projects");
                var utProj = _solutionInfo.LoadProject(Path.Combine(_solutionInfo.SolutionDir, "Services.Test",
                    "Services.Test.synproj"));
                var gvProj = _solutionInfo.LoadProject(Path.Combine(_solutionInfo.SolutionDir, "Services.Test.GenerateValues",
                    "Services.Test.GenerateValues.synproj"));
                _solutionInfo.Projects.Add(utProj);
                _solutionInfo.Projects.Add(gvProj);

                _solutionInfo.CodeGenSolution.GenerateUnitTests = true;
                _solutionInfo.CodeGenSolution.UnitTestProject = "Services.Test";
                _solutionInfo.CodeGenSolution.UnitTestFolder = "Services.Test";
                _solutionInfo.CodeGenSolution.UnitTestsBaseNamespace = "Services.Test";
                _solutionInfo.CodeGenSolution.UnitTestsNamespace = "Services.Test.UnitTests";

                //Save();
                _solutionInfo.SaveSolution();

                // call regen


                //await CollectTestData(events);
            }
            catch
            {
                Console.WriteLine("Operation failed!");
                return 1;
            }
            finally
            {
                Console.WriteLine("All done!");
            }   

            //RunRegen
            //CollectTestData
            return 0;
        }

        private void Logger(CodeGenTask tsk, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                CallerLogger(string.Format("{0} : {1}", string.Join(',', tsk.Templates), message));
        }
    }
}
