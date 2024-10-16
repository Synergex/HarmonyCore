using CodeGen.Engine;
using HarmonyCore.CliTool.Commands;
using HarmonyCore.CliTool.TUI.Helpers;
using HarmonyCore.CliTool.TUI.Models;
using HarmonyCore.CliTool.TUI.ViewModels;
using HarmonyCoreGenerator.Model;
using Microsoft.Build.Tasks;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool
{
    internal class CommonCommands
    {
        private Action<string> CallerLogger { get; set; } = (str) => Console.WriteLine(str);
        private ProjectInfo utProj;
        private ProjectInfo gvProj;
        private GenerationEvents events;
                
        public CommonCommands(GenerationEvents events)
        {
            this.events = events;
        }

        public async Task LoadTestProjects(SolutionInfo _solutionInfo)
        {
            events?.StatusUpdate?.Invoke("Adding unit test project template");
            if (events?.StatusUpdate == null)
            {
                CallerLogger("Adding unit test project template");
            }
            if (!await DotnetTool.AddTemplateToSolution("harmonycore-ut",
                    Path.Combine(_solutionInfo.SolutionDir, "Services.Test"), _solutionInfo.SolutionPath, events?.Message))
            {
                events?.Message("Template creation failed for Services.Test");
                if (events?.Message == null)
                {
                    CallerLogger("Template creation failed for Services.Test");
                }
                throw new InvalidOperationException("Template creation failed for Services.Test");
            }

            events?.StatusUpdate?.Invoke("Adding unit test value generator template");
            if (events?.StatusUpdate == null)
            {
                CallerLogger("Adding unit test value generator template");
            }
            if (!await DotnetTool.AddTemplateToSolution("harmonycore-utg",
                    Path.Combine(_solutionInfo.SolutionDir, "Services.Test.GenerateValues"), _solutionInfo.SolutionPath,
                    events?.Message))
            {
                events.Message("Template creation failed for Services.Test.GenerateValues");
                if (events?.Message == null)
                {
                    CallerLogger("Template creation failed for Services.Test.GenerateValues");
                }
                throw new InvalidOperationException("Template creation failed for Services.Test.GenerateValues");
            }

            events?.StatusUpdate?.Invoke("Loading new projects");
            if (events?.StatusUpdate == null)
            {
                CallerLogger("Loading new projects");
            }
            utProj = _solutionInfo.LoadProject(Path.Combine(_solutionInfo.SolutionDir, "Services.Test",
                "Services.Test.synproj"));
            gvProj = _solutionInfo.LoadProject(Path.Combine(_solutionInfo.SolutionDir, "Services.Test.GenerateValues",
                "Services.Test.GenerateValues.synproj"));
            _solutionInfo.Projects.Add(utProj);
            _solutionInfo.Projects.Add(gvProj);

            _solutionInfo.CodeGenSolution.GenerateUnitTests = true;
            _solutionInfo.CodeGenSolution.UnitTestProject = "Services.Test";
            _solutionInfo.CodeGenSolution.UnitTestFolder = "Services.Test";
            _solutionInfo.CodeGenSolution.UnitTestsBaseNamespace = "Services.Test";
            _solutionInfo.CodeGenSolution.UnitTestsNamespace = "Services.Test.UnitTests";
        }

        public async Task RunRegen(SolutionInfo solutionInfo)
        {
            var regenEvents = events != null ? new GenerationEvents(events, null) : new GenerationEvents();

            Regen(solutionInfo, regenEvents.ProgressUpdate, regenEvents.StatusUpdate, regenEvents.Message ?? CallerLogger, regenEvents.OnLoaded, (proj, addedFiles) =>
            {
                events?.StatusUpdate?.Invoke("Adding new files");
                if (events?.StatusUpdate == null)
                {
                    CallerLogger("Adding new files");
                }
                if (proj.EndsWith("Services.Test.synproj", StringComparison.CurrentCultureIgnoreCase))
                {
                    utProj.AddRemoveFiles(addedFiles, Enumerable.Empty<string>());
                    utProj.MSBuildProject.Save();
                }
                else if (proj.EndsWith("Services.Test.GenerateValues.synproj", StringComparison.CurrentCultureIgnoreCase))
                {
                    gvProj.AddRemoveFiles(addedFiles, Enumerable.Empty<string>());
                    gvProj.MSBuildProject.Save();
                }
            }, (proj, remvedFiles) => { }, regenEvents.CancelToken);
            await regenEvents.LoadedAsync;
        }

        public async void Regen(SolutionInfo solutionInfo, Action<string, float> progressUpdate, Action<string> statusUpdate, Action<string> message, Action loaded,
            Action<string, IEnumerable<string>> addedFiles, Action<string, IEnumerable<string>> removedFiles, CancellationToken cancelToken)
        {
            try
            {
                await Task.Run(() =>
                {
                    CodeGenTaskSet runningTaskset = null;
                    int runPartsCompleted = 0;
                    var regenCommand = new RegenCommand(() => Task.FromResult(solutionInfo)) { CallerLogger = message };
                    regenCommand.GenerationEvents.GenerationStarted = (tsk) =>
                    {
                        runPartsCompleted = 5;
                        runningTaskset = tsk;
                    };
                    if (regenCommand.GenerationEvents.TaskStarted != null)
                    {
                        regenCommand.GenerationEvents.TaskStarted = tsk => progressUpdate($"Generating {tsk.Templates.FirstOrDefault()}",
                        (1.0f / (runningTaskset.Tasks.Count + 5)) * runPartsCompleted);
                    }
                    if (regenCommand.GenerationEvents.TaskComplete != null)
                    {
                        regenCommand.GenerationEvents.TaskComplete = tsk =>
                        {
                            runPartsCompleted++;
                        };
                    }

                    regenCommand.CancelToken = cancelToken;
                    Dictionary<string, HashSet<string>> syncAddedFiles = new Dictionary<string, HashSet<string>>();
                    Dictionary<string, HashSet<string>> syncRemovedFiles = new Dictionary<string, HashSet<string>>();
                    regenCommand.Run(new RegenOptions { Generators = Enumerable.Empty<string>(), Interfaces = Enumerable.Empty<string>(), Structures = Enumerable.Empty<string>() }, syncAddedFiles, syncRemovedFiles);

                    foreach (var syncTpl in syncAddedFiles)
                        addedFiles(syncTpl.Key, syncTpl.Value);

                    foreach (var syncTpl in syncRemovedFiles)
                        removedFiles(syncTpl.Key, syncTpl.Value);

                    loaded();
                }, cancelToken);
            }
            catch (OperationCanceledException)
            {
                message("Cancelled");
            }
            //TODO show messages interactively
        }

        public async Task RunUpgradeLatest(string? zipPath = null)
        {
            var defaultLoader = () => Program.LoadSolutionInfo((str) =>
            {
                events?.Message(str);
                if (events?.Message == null)
                {
                    CallerLogger(str);
                }
            });
            var _loader = new Lazy<Task<SolutionInfo>>(defaultLoader);
            var solutionInfo = _loader.Value.Result;
            events?.StatusUpdate?.Invoke("Checking for current version info");
            if (events?.StatusUpdate == null)
            {
                CallerLogger("Checking for current version info");
            }
            var versionInfo = Program.LoadVersionInfoSync(true);
            var upgradeLatestOptions = new UpgradeLatestOptions();
            await Program.UpgradeLatest(solutionInfo, versionInfo, upgradeLatestOptions.OverrideTemplateVersion, upgradeLatestOptions.OverrideTemplateUrl, events, zipPath);
        }

        public async Task CollectTestData(SolutionInfo _solutionInfo)
        {
            var generateValuesProjectName = _solutionInfo.CodeGenSolution.UnitTestProject + ".GenerateValues";
            events?.StatusUpdate.Invoke("Generating Test Values");
            if (events?.StatusUpdate == null)
            {
                CallerLogger("Generating Test Values");
            }
            events?.Message($"Compiling and running {generateValuesProjectName}, this may take a while...");
            if (events?.Message == null)
            {
                CallerLogger($"Compiling and running {generateValuesProjectName}, this may take a while...");
            }

            if (!await DotnetTool.RunProject(_solutionInfo.SolutionDir,
                    Path.Combine(_solutionInfo.SolutionDir, generateValuesProjectName,
                        generateValuesProjectName + ".synproj"), events?.Message))
            {
                events?.Message("Failed to run GenerateValues");
                if (events?.Message == null)
                {
                    CallerLogger("Failed to run GenerateValues");
                }
                throw new InvalidOperationException("Failed to run GenerateValues");
            }
        }

        public async Task AddTraditionalBridge(SolutionInfo _solutionInfo)
        {
            await DotnetTool.AddTemplateToSolution("harmonycore-tb",
                Path.Combine(_solutionInfo.SolutionDir, "TraditionalBridge"), _solutionInfo.SolutionPath, events?.Message ?? CallerLogger);
            events?.Message("Instantiated and added Traditional Bridge project to solution");
            _solutionInfo.CodeGenSolution.TraditionalBridge = new TraditionalBridge() { EnableSampleDispatchers = true };
            events?.Message("Saved initial feature settings to Harmony Core configuration file");
            events?.Message("Completed");
            if (events?.Message == null)
            {
                CallerLogger("Instantiated and added Traditional Bridge project to solution");
                CallerLogger("Saved initial feature settings to Harmony Core configuration file");
                CallerLogger("Completed");
            }
        }

        public async Task AddSmc(SolutionInfo _solutionInfo, string smcPath)
        {
            if (_solutionInfo.CodeGenSolution.TraditionalBridge == null)
                _solutionInfo.CodeGenSolution.TraditionalBridge = new TraditionalBridge();

            _solutionInfo.CodeGenSolution.TraditionalBridge.EnableXFServerPlusMigration = true;
            _solutionInfo.CodeGenSolution.TraditionalBridge.XFServerSMCPath = smcPath;
            if (Directory.Exists(Path.Combine(_solutionInfo.SolutionDir, "TraditionalBridge", "Source")))
                _solutionInfo.CodeGenSolution.TraditionalBridge.GenerateIntoSourceFolder = true;

            events?.Message("\nSetting up SMC import (enabling import and saving SMC path)...");
            events?.Message("SMC setup completed.\n\nSCM path (relative to SolutionDir):");
            events?.Message(smcPath);
            if (events?.Message == null)
            {
                CallerLogger("\nSetting up SMC import (enabling import and saving SMC path)...");
                CallerLogger("SMC setup completed.\n\nSCM path (relative to SolutionDir):");
                CallerLogger(smcPath);
            }
        }
    }
}
