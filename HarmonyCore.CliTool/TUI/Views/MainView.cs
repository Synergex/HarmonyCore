using HarmonyCore.CliTool.TUI.Models;
using HarmonyCore.CliTool.TUI.ViewModels;
using HarmonyCoreGenerator.Model;
using Microsoft.Build.Locator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NStack;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class MainView : Toplevel
    {
        MainViewModel _mainViewModel;
        TabView _tabView;
        private View _loadView;
        private ProgressBar _progressView;
        private Timer _progressTimer;
        public MainView(Func<Action<string>, SolutionInfo> context)
        {
            StatusBar = new StatusBar();

            _loadView = new View()
            {
                X = Pos.Center(),
                Y = Pos.Center(),
                Width = Dim.Percent(75f),
                Height = Dim.Percent(75f),
                Text = ""
            };

            _progressView = new ProgressBar()
            {
                X = Pos.Center(),
                Y = 2,
                ProgressBarStyle = ProgressBarStyle.MarqueeContinuous,
                Width = 30
            };



            _mainViewModel = new MainViewModel(() => context((str) => Application.MainLoop.Invoke(() =>
            {
                _loadView.GetCurrentWidth(out var currentWidth);
                var wrappedText = TextFormatter.WordWrap(str, currentWidth);
                _loadView.Text += (string.Join(Environment.NewLine, wrappedText.Select(txt => txt.ToString()).ToArray()) + Environment.NewLine);
            })));

            MenuBar = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem("_File", new MenuItem[]
                {
                    new MenuItem("_Save", "Save Harmony Core Customization file", _mainViewModel.Save),
                    new MenuItem("_Import", "Import regen.bat settings into the current Harmony Core Customization file", _mainViewModel.Save),
                    new MenuItem("_Validate", "Validate customization scripts and settings", _mainViewModel.Validate),
                    new MenuItem("_Quit", "Exit the program", Quit)
                }),
                new MenuBarItem("_Codegen", new MenuItem[]
                {
                    new MenuItem("_Regen", "Run CodeGen based off your Harmony Core customization file", _mainViewModel.Regen),
                    new MenuItem("_Sync VS", "Add/Remove generated files from your Visual Studio Solution", _mainViewModel.SyncVS),
                })
            });

            Add(_loadView, _progressView, MenuBar, StatusBar);

            _mainViewModel.EnsureSolutionLoad(PromptForSolutionFile, UpdateStatus, ShowLoadError, FinishedLoad);

            _progressTimer = new Timer((_) => {
                _progressView.Pulse();
                Application.MainLoop.Driver.Wakeup();
            }, null, 0, 300);
        }

        private void Quit()
        {
            Application.RequestStop(this);
        }

        void UpdateStatus(string status)
        {
            Application.MainLoop.Invoke(() => StatusBar.Text = status);
        }

        void ShowLoadError(string error)
        {
            Application.MainLoop.Invoke(() =>
            {
                MessageBox.ErrorQuery("Load error", error, "Ok");
                Application.RequestStop(this);
            });
        }

        void FinishedLoad()
        {
            Application.MainLoop.Invoke(async () =>
            {
                _progressTimer?.Dispose();
                _progressView.Fraction = 100f;
                _loadView.Text += "Loaded";

                await Task.Delay(2000);
                
                Remove(_loadView);
                //load up tab views 
                _tabView = new TabView()
                {
                    X = 1,
                    Y = 1,
                    Width = Dim.Fill(),
                    Height = Dim.Fill() - 1
                };


                foreach (var setting in _mainViewModel.ActiveSettings)
                {
                    if (setting is SingleItemSettingsBase singleItemSetting)
                        _tabView.AddTab(new TabView.Tab(setting.Name, new SingleItemSettingsView(singleItemSetting)),
                            setting == _mainViewModel.ActiveSettings.First());
                    else if (setting is IMultiItemSettingsBase multiItemSetting)
                        _tabView.AddTab(new TabView.Tab(setting.Name, new MultiItemSettingsView(multiItemSetting)),
                            setting == _mainViewModel.ActiveSettings.First());
                }

                _tabView.SelectedTabChanged += tabView_SelectedTabChanged;
                Add(_tabView);
            });
        }

        private void tabView_SelectedTabChanged(object sender, TabView.TabChangedEventArgs e)
        {
            var typedOldTab = e.OldTab?.View as MultiItemSettingsView;
            var typedNewTab = e.NewTab?.View as MultiItemSettingsView;

            typedOldTab?.DetachStatusBar();
            typedNewTab?.AttachStatusBar(StatusBar);
        }

        string PromptForSolutionFile()
        {
            var openFileDialog = new OpenDialog("Settings Load", "Pick a Harmony Core json settings file",
                    new List<string> { ".json", ".*" }, OpenDialog.OpenMode.File);
            Application.Run(openFileDialog);
            if (File.Exists(openFileDialog.FilePath.ToString()))
            {
                return openFileDialog.FilePath.ToString();
            }
            else
                throw new FileNotFoundException();
        }
    }
}
