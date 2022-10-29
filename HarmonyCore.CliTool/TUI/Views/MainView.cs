using HarmonyCore.CliTool.TUI.Models;
using HarmonyCore.CliTool.TUI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class MainView : Toplevel
    {
        MainViewModel _mainViewModel;
        TabView _tabView;
       
        public MainView(Func<Action<string>, SolutionInfo> context)
        {
            StatusBar = new StatusBar();
            Add(StatusBar);
            FinishInit(context);
        }

        private async void FinishInit(Func<Action<string>, SolutionInfo> context)
        {
            await Task.Yield();
            var cts = new CancellationTokenSource();
            var dialog = new ProgressDialog("Loading solution", true, false, cts);
            Application.Top.Add(dialog);
            _mainViewModel = new MainViewModel(() => context(GetInvoker<string>(dialog.ShowMessage)));

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
                    new MenuItem("_Regen", "Run CodeGen based off your Harmony Core customization file", Regen),
                    new MenuItem("_Sync VS", "Generate and Add/Remove generated files from your Visual Studio Solution", SyncVS),
                })
            });

            Add(MenuBar);

            _mainViewModel.EnsureSolutionLoad(PromptForSolutionFile, GetInvoker<string>(UpdateStatus), GetInvoker<string>(ShowLoadError), GetInvoker(
                async () =>
                {
                    FinishedLoad();
                    await Task.Delay(1500);
                    Application.Top.Remove(dialog);
                }));
        }

        private void Regen()
        {
            var cts = new CancellationTokenSource();
            var dialog = new ProgressDialog("Regen", true, true, cts);
            Application.Top.Add(dialog);
            _mainViewModel.Regen(GetInvoker<string, float>(dialog.ShowProgress), GetInvoker<string>(UpdateStatus), GetInvoker<string>(dialog.ShowMessage), GetInvoker(dialog.EndProgressOperation), cts.Token);
            
        }

        private void Invoke<T>(T arg, Action<T> callme)
        {
            Application.MainLoop.Invoke(() => callme(arg));
        }

        private Action GetInvoker(Action callme)
        {
            return () => Application.MainLoop.Invoke(callme);
        }

        private Action<T> GetInvoker<T>(Action<T> callme)
        {
            return (arg) => Application.MainLoop.Invoke(() => callme(arg));
        }

        private Action<T, T2> GetInvoker<T, T2>(Action<T, T2> callme)
        {
            return (arg, arg2) => Application.MainLoop.Invoke(() => callme(arg, arg2));
        }

        private void SyncVS()
        {
            var cts = new CancellationTokenSource();
            var dialog = new ProgressDialog("Synchronize generated files", true, true, cts);
            Application.Top.Add(dialog);
            //TODO: register for file change report at the end
            _mainViewModel.Regen(GetInvoker<string, float>(dialog.ShowProgress), GetInvoker<string>(UpdateStatus), GetInvoker<string>(dialog.ShowMessage), GetInvoker(dialog.EndProgressOperation), cts.Token);
        }

        private void Quit()
        {
            Application.RequestStop(this);
        }

        void UpdateStatus(string status)
        {
            StatusBar.Text = status;
            Application.MainLoop.Driver.Wakeup();
        }

        void ShowLoadError(string error)
        {
            MessageBox.ErrorQuery("Load error", error, "Ok");
            Application.RequestStop(this);
        }

        void FinishedLoad()
        {
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
                    _tabView.AddTab(new TabView.Tab(setting.Name, new SingleItemSettingsView(singleItemSetting) {X = Pos.Center()}),
                        setting == _mainViewModel.ActiveSettings.First());
                else if (setting is IMultiItemSettingsBase multiItemSetting)
                    _tabView.AddTab(new TabView.Tab(setting.Name, new MultiItemSettingsView(multiItemSetting)),
                        setting == _mainViewModel.ActiveSettings.First());
            }

            _tabView.SelectedTabChanged += tabView_SelectedTabChanged;
            Add(_tabView);
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
