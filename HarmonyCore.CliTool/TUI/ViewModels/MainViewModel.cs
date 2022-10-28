using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private readonly Lazy<SolutionInfo> _loader;
        SolutionInfo _context => _loader.Value;

        Dictionary<string, ISettingsBase> DynamicSettings { get; set; }
        public List<string> InactiveSettings { get; } = new List<string>();
        public List<ISettingsBase> ActiveSettings { get; } = new List<ISettingsBase>();

        public MainViewModel(Func<SolutionInfo> contextLoader)
        {
            _loader = new Lazy<SolutionInfo>(contextLoader);
        }

        public async void EnsureSolutionLoad(Func<string> getFileName, Action<string> statusUpdate, Action<string> error, Action loaded)
        {
            try
            {
                var synthesizedPath = Path.Combine(_context.SolutionDir, "Harmony.Core.CodeGen.json");
                await Task.Yield();
                await Task.Run(async () => await LoadSolutionFile(File.Exists(synthesizedPath) ? synthesizedPath : getFileName(), statusUpdate, error));
                loaded();
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

        internal void Regen()
        {
            Save();
            var regenCommand = new RegenCommand(() => _context) { CallerLogger = (str) => { } };
            regenCommand.Run(new RegenOptions());
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
                    return new TraditionalBridgeSettings(_context);
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

        internal void SyncVS()
        {
            throw new NotImplementedException();
        }
    }
}
