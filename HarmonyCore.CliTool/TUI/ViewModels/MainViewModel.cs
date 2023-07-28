using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeGen.Engine;
using HarmonyCore.CliTool.Commands;
using HarmonyCore.CliTool.TUI.Helpers;
using HarmonyCore.CliTool.TUI.Models;
using HarmonyCore.CliTool.TUI.ViewModels;
using HarmonyCoreGenerator.Model;
using Microsoft.Build.Locator;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.ViewModels
{
    internal class MainViewModel
    {

        private TraditionalBridgeSettings _traditionalBridgeSettings;

        private readonly Lazy<Task<SolutionInfo>> _loader;
        SolutionInfo _context => _loader.Value.Result;

        Dictionary<string, ISettingsBase> DynamicSettings { get; set; }
        public List<string> InactiveSettings { get; } = new List<string>();
        public List<ISettingsBase> ActiveSettings { get; } = new List<ISettingsBase>();

        public MainViewModel(Func<Task<SolutionInfo>> contextLoader)
        {
            _loader = new Lazy<Task<SolutionInfo>>(contextLoader);
        }

        public async void EnsureSolutionLoad(Func<string> getFileName, Action<string> statusUpdate, Action<string> error, Action loaded)
        {
            try
            {
                var synthesizedPath = Path.Combine(_context.SolutionDir, "Harmony.Core.CodeGen.json");
                await Task.Yield();
                await Task.Run(async () => await LoadSolutionFile(File.Exists(synthesizedPath) ? synthesizedPath : getFileName(), statusUpdate, error));
                loaded();
                if (!Program.AppSettings.TryGetValue("LastUpToDateCheck", out var lastChecked) || DateTime.Parse(lastChecked).AddDays(5) < DateTime.Now)
                {
                    var (upToDate, nugetVersion, myVersion) = await _context.UpToDateCheck();
                    Program.AppSettings["LastUpToDateCheck"] = DateTime.Now.ToString();
                    if (!upToDate)
                    {
                        MessageBox.ErrorQuery("Update needed",
                            $"The most recent version available on nuget is {nugetVersion} the currently running version {myVersion}");
                    }
                }
            }
            catch(FileNotFoundException)
            {
                error("Cancelled Load");
            }
            catch(Exception ex)
            {
                error(ex.ToString());
            }
        }

        internal async void SyncVS(Action<string, float> progressUpdate, Action<string> statusUpdate,
            Action<string> message, Action loaded, CancellationToken cancelToken)
        {
            Regen(progressUpdate, statusUpdate, message, loaded, (proj, addedFiles) =>
            {
                _context.Projects.First(projInfo => string.Compare(projInfo.FileName, proj) == 0).AddRemoveFiles(addedFiles, Enumerable.Empty<string>());
            }, (proj, removedFiles) =>
            {
                _context.Projects.First(projInfo => string.Compare(projInfo.FileName, proj) == 0).AddRemoveFiles(Enumerable.Empty<string>(), removedFiles);
            }, cancelToken);
        }

        internal async void Regen(Action<string, float> progressUpdate, Action<string> statusUpdate, Action<string> message, Action loaded, 
            Action<string, IEnumerable<string>> addedFiles, Action<string, IEnumerable<string>> removedFiles, CancellationToken cancelToken)
        {
            Save();
            try
            {
                await Task.Run(() =>
                {
                    CodeGenTaskSet runningTaskset = null;
                    int runPartsCompleted = 0;
                    var regenCommand = new RegenCommand(() => Task.FromResult(_context)) { CallerLogger = message };
                    regenCommand.GenerationEvents.GenerationStarted = (tsk) =>
                    {
                        runPartsCompleted = 5;
                        runningTaskset = tsk;
                    };
                    regenCommand.GenerationEvents.TaskStarted = tsk => progressUpdate($"Generating {tsk.Templates.FirstOrDefault()}",
                        (1.0f / (runningTaskset.Tasks.Count + 5)) * runPartsCompleted);

                    regenCommand.GenerationEvents.TaskComplete = tsk =>
                    {
                        runPartsCompleted++;
                    };

                    regenCommand.CancelToken = cancelToken;
                    Dictionary<string, HashSet<string>> syncAddedFiles = new Dictionary<string, HashSet<string>>();
                    Dictionary<string, HashSet<string>> syncRemovedFiles = new Dictionary<string, HashSet<string>>();
                    regenCommand.Run(new RegenOptions { Generators = Enumerable.Empty<string>(), Interfaces = Enumerable.Empty<string>(), Structures = Enumerable.Empty<string>()}, syncAddedFiles, syncRemovedFiles);
                    
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

        internal void Save()
        {
            foreach(var setting in ActiveSettings)
            {
                setting.Save(_context);
            }
            _context.SaveSolution();
        }

        public ISettingsBase SettingsLoader(string settingsName)
        {
            switch (settingsName.ToLower())
            {
                case "odata":
                    return new ODataSettings(_context);
                case "structures":
                    return new StructureSettings(_context);
                case "interfaces":
                    return new InterfaceSettings(_context);
                case "traditionalbridge":
                    _traditionalBridgeSettings = new TraditionalBridgeSettings(_context);
                    return _traditionalBridgeSettings;
                case "settings":
                    return new SolutionSettings(_context);
                default:
                    if (DynamicSettings.TryGetValue(settingsName, out var settingsBase))
                    {
                        return settingsBase;
                    }
                    throw new NotImplementedException();
            }

        }

        private async Task LoadSolutionFile(string fileName, Action<string> statusUpdate, Action<string> error)
        {
            statusUpdate("Loading...");

            // Calling Register methods will subscribe to AssemblyResolve event. After this we can
            // safely call code that use MSBuild types (in the Builder class).
            if (!MSBuildLocator.IsRegistered)
                MSBuildLocator.RegisterMSBuildPath(MSBuildLocator.QueryVisualStudioInstances().ToList().FirstOrDefault().MSBuildPath);
            var solution = _context.CodeGenSolution ?? Solution.LoadSolution(fileName, _context.SolutionDir);

            if (solution != null)
            {
                _context.CodeGenSolution = solution;
                //TODO report errors from here instead of just failing to load
                DynamicSettings = await DynamicSettingsLoader.LoadDynamicSettings(_context, Path.Combine(_context.SolutionDir, "Generators", "Settings"));

                //InstructionalTabTextBlockText = "Select a tab to continue.";

                // Determine visibility of tabs
                bool? hasOdata = solution.ExtendedStructures?.Any(k => k.EnabledGenerators.Contains("ODataGenerator"));
                bool? hasModels = solution.ExtendedStructures?.Any(k => k.EnabledGenerators.Contains("ModelGenerator"));
                bool? hasTraditionalBridge = solution.TraditionalBridge?.EnableXFServerPlusMigration;
                ActiveSettings.Clear();
                ActiveSettings.Add(SettingsLoader("Settings"));
                ActiveSettings.Add(SettingsLoader("OData"));
                ActiveSettings.Add(SettingsLoader("Structures"));
                ActiveSettings.Add(SettingsLoader("TraditionalBridge"));
                ActiveSettings.Add(SettingsLoader("Interfaces"));

                foreach (var kvp in DynamicSettings)
                {
                    if (kvp.Value.IsEnabled(_context.CodeGenSolution))
                        ActiveSettings.Add(kvp.Value);
                    else
                        InactiveSettings.Add(kvp.Key);
                }

                statusUpdate("Loaded successfully");
            }
            else
            {
                statusUpdate("Load failed");
                error($"Could not load the solution associated with this JSON file.{Environment.NewLine}{Environment.NewLine}Double check the paths inside the JSON file and try again. In addition, the JSON file must be placed at the root of the HarmonyCore solution.");
            }
        }

        internal async void Validate()
        {
            //pop compiling/output dialog
            //load all customization scripts to make sure they compile
            try
            {
                var generators = await DynamicCodeGenerator.LoadDynamicGenerators(Path.Combine(_context.SolutionDir, "Generators", "Enabled"));
                //log success for each generator
            }
            catch(Exception ex)
            {
                //log failure
            }

        }

        async Task AddTraditionalBridge(GenerationEvents events)
        {
            try
            {
                await DotnetTool.AddTemplateToSolution("harmonycore-tb",
                Path.Combine(_context.SolutionDir, "TraditionalBridge"), _context.SolutionPath, events.Message);
                events.Message("Instantiated and added Traditional Bridge project to solution");
                _context.CodeGenSolution.TraditionalBridge = new TraditionalBridge() { EnableSampleDispatchers = true };
                Save();
                events.Message("Saved initial feature settings to Harmony Core configuration file");
                events.Message("Completed");
                _traditionalBridgeSettings.LoadTraditionalBridgeSettings(_context);
                OnSmcAdded();
            }
            finally
            {
                events.OnLoaded();
            }
        }

        public event EventHandler SmcAdded;

        async Task AddTraditionalBridgeAndSmc(GenerationEvents events)
        {
            await AddTraditionalBridge(events);
            await AddSmc(events);
        }

        private string GetSmcPath(GenerationEvents events)
        {
            var openFileDialog = new OpenDialog("Load SMC", "Select a Synergy method catalog (SMC) with interfaces for the traditional Synergy routines you want Harmony Core to access.",
                    new List<string> { ".xml", ".*" }, OpenDialog.OpenMode.File);
            Application.Run(openFileDialog);

            if (string.IsNullOrEmpty(openFileDialog.FilePath.ToString()) || !openFileDialog.FilePath.Contains(".xml")) 
            {
                events.Message("\nSMC selection canceled.");
                return null;
            }
            try
            {
                return Path.GetRelativePath(_context.SolutionDir + "\\", openFileDialog.FilePath.ToString());
            }
            catch (FileNotFoundException ex) 
            {
                throw new FileNotFoundException("The selected SMC file was not found.", ex);
            }
        }

        async Task AddSmc(GenerationEvents events)
        {
            try
            {
                var smcPath = GetSmcPath(events);

                if (_context.CodeGenSolution.TraditionalBridge == null)
                    _context.CodeGenSolution.TraditionalBridge = new TraditionalBridge();

                _context.CodeGenSolution.TraditionalBridge.EnableXFServerPlusMigration = true;
                _context.CodeGenSolution.TraditionalBridge.XFServerSMCPath = smcPath;
                if (Directory.Exists(Path.Combine(_context.SolutionDir, "TraditionalBridge", "Source")))
                    _context.CodeGenSolution.TraditionalBridge.GenerateIntoSourceFolder = true;

                events.Message("\nSetting up SMC import (enabling import and saving SMC path)...");
                _context.SaveSolution();
                events.Message("SMC setup completed.\n\nSCM path (relative to SolutionDir):");
                events.Message(smcPath);

                _traditionalBridgeSettings.LoadTraditionalBridgeSettings(_context);
                OnSmcAdded();
            }
            finally
            {
                events.OnLoaded();
            }
        }

        protected virtual void OnSmcAdded()
        {
            SmcAdded?.Invoke(this, EventArgs.Empty);
        }

        async Task CollectTestData(GenerationEvents events)
        {
            //check for GenerateTestValues.dbl
            //if missing show error ask user to run regen first
            try
            {
                var generateValuesProjectName = _context.CodeGenSolution.UnitTestProject + ".GenerateValues";
                events?.StatusUpdate.Invoke("Generating Test Values");
                events.Message($"Compiling and running {generateValuesProjectName}, this may take a while...");
                
                if (!await DotnetTool.RunProject(_context.SolutionDir,
                        Path.Combine(_context.SolutionDir, generateValuesProjectName,
                            generateValuesProjectName + ".synproj"), events.Message))
                    events.Message("Failed to run GenerateValues");
            }
            finally
            {
                events.OnLoaded();
            }
        }

        async Task AddUnitTests(GenerationEvents events)
        {
            try
            {
                events?.StatusUpdate.Invoke("Adding unit test project template");
                if (!await DotnetTool.AddTemplateToSolution("harmonycore-ut",
                        Path.Combine(_context.SolutionDir, "Services.Test"), _context.SolutionPath, events.Message))
                {
                    events.Message("Template creation failed for Services.Test");
                    return;
                }

                events?.StatusUpdate.Invoke("Adding unit test value generator template");
                if (!await DotnetTool.AddTemplateToSolution("harmonycore-utg",
                        Path.Combine(_context.SolutionDir, "Services.Test.GenerateValues"), _context.SolutionPath,
                        events.Message))
                {
                    events.Message("Template creation failed for Services.Test.GenerateValues");
                    return;
                }

                events?.StatusUpdate.Invoke("Loading new projects");
                var utProj = _context.LoadProject(Path.Combine(_context.SolutionDir, "Services.Test",
                    "Services.Test.synproj"));
                var gvProj = _context.LoadProject(Path.Combine(_context.SolutionDir, "Services.Test.GenerateValues",
                    "Services.Test.GenerateValues.synproj"));
                _context.Projects.Add(utProj);
                _context.Projects.Add(gvProj);

                _context.CodeGenSolution.GenerateUnitTests = true;
                _context.CodeGenSolution.UnitTestProject = "Services.Test";
                _context.CodeGenSolution.UnitTestFolder = "Services.Test";
                _context.CodeGenSolution.UnitTestsBaseNamespace = "Services.Test";
                _context.CodeGenSolution.UnitTestsNamespace = "Services.Test.UnitTests";

                Save();

                //regen sets loaded status in the events but it will fire too soon so make it, its own event set
                var regenEvents = new GenerationEvents(events, null);
                Regen(regenEvents.ProgressUpdate, regenEvents.StatusUpdate, regenEvents.Message, regenEvents.OnLoaded, (proj, addedFiles) =>
                {
                    events?.StatusUpdate.Invoke("Adding new files");
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

                //Add Generated files to Services.Test and Services.Test.GenerateValues

                await regenEvents.LoadedAsync;

                await CollectTestData(events);
            }
            finally
            {
                events.OnLoaded();
            }
            //RunRegen
            //CollectTestData
        }

        public List<ValueTuple<string, string, Func<GenerationEvents, Task>>> GetFeatureItems()
        {
            var result = new List<ValueTuple<string, string, Func<GenerationEvents, Task>>>();

            var hasTraditionalBridge = _context.CodeGenSolution.TraditionalBridge != null; 
            var hasUnitTests = _context.Projects.Any(proj => proj.FileName.EndsWith("Services.Test.synproj", StringComparison.CurrentCultureIgnoreCase));

            if (!hasTraditionalBridge)
            {
                result.Add(("Add Traditional Bridge", "Add support for running traditional Synergy routines", AddTraditionalBridge));
                result.Add(("Add Traditional Bridge and SMC", "Add components for Traditional Bridge and xfServerPlus migration", AddTraditionalBridgeAndSmc));
            }

            if (hasTraditionalBridge && _context.CodeGenSolution.TraditionalBridge?.EnableXFServerPlusMigration != true) 
            {
                result.Add(("Add xfServerPlus Migration", "Add components for xfServerPlus migration", AddSmc));
            }

            if (!hasUnitTests)
            {
                result.Add(("Add Unit Test", "Add support for running unit tests", AddUnitTests));
            }
            else
            {
                result.Add(("Collect Test Data", "Collect test data for your Unit Tests", CollectTestData));
            }
            
            return result;
        }
    }
}
