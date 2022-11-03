using HarmonyCore.CliTool.TUI.Models;
using HarmonyCore.CliTool.TUI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HarmonyCore.CliTool.TUI.Helpers;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class MainView : Toplevel
    {
        MainViewModel _mainViewModel;
        TabView _tabView;
        private static int MainThread;
        public MainView(Func<Action<string>, SolutionInfo> context)
        {
            MainThread = System.Threading.Thread.CurrentThread.ManagedThreadId;
            StatusBar = new StatusBar();
            Add(StatusBar);
            FinishInit(context);
        }

        private async void ProgressDialogOperation(string operation, bool fractionProgress, bool hasOk, Func<GenerationEvents, Task> op)
        {
            var cts = new CancellationTokenSource();
            var dialog = new ProgressDialog(operation, fractionProgress, hasOk, cts, (dialog) => op(new GenerationEvents()
            {
                CancelToken = cts.Token,
                Loaded = GetInvoker(dialog.EndProgressOperation),
                Message = GetInvoker<string>(dialog.ShowMessage),
                ProgressUpdate = GetInvoker<string, float>(dialog.ShowProgress),
                StatusUpdate = GetInvoker<string>(dialog.ShowProgress)
            }));
            Application.Run(dialog);
        }

        private void AddFeatures()
        {
            List<MenuItem> features = new List<MenuItem>();
            foreach (var (title, help, action) in _mainViewModel.GetFeatureItems())
            {
                features.Add(new MenuItem(title, help, () => ProgressDialogOperation(title, false, true, action)));
            }

            if (features.Count > 0)
            {
                var featuresMenu = new MenuBarItem("Features", features.ToArray());
                var menuBarItems = MenuBar.Menus;
                Array.Resize(ref menuBarItems, menuBarItems.Length + 1);
                menuBarItems[^1] = featuresMenu;
                Remove(MenuBar);
                MenuBar = new MenuBar(menuBarItems);
                Add(MenuBar);
            }
        }

        private async void FinishInit(Func<Action<string>, SolutionInfo> context)
        {
            await Task.Yield();
            var cts = new CancellationTokenSource();
            var dialog = new ProgressDialog("Loading solution", true, false, cts, (dialog) =>
            {
                _mainViewModel.EnsureSolutionLoad(PromptForSolutionFile, GetInvoker<string>(UpdateStatus), GetInvoker<string>(ShowLoadError), GetInvoker(
                    async () =>
                    {
                        FinishedLoad();
                        await Task.Delay(1500, cts.Token);
                        Application.RequestStop(dialog);
                    }));
            });
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
            Application.Run(dialog);
        }

        private void Regen()
        {
            var cts = new CancellationTokenSource();
            var dialog = new ProgressDialog("Regen", true, true, cts, (dialog) =>
            {
                _mainViewModel.Regen(GetInvoker<string, float>(dialog.ShowProgress), GetInvoker<string>(dialog.ShowProgress), GetInvoker<string>(dialog.ShowMessage), GetInvoker(dialog.EndProgressOperation),
                    (_, _) => { }, (_, _) => { }, cts.Token);
            });
            Application.Run(dialog);
        }

        private void Invoke<T>(T arg, Action<T> callme)
        {
            if (MainThread == System.Threading.Thread.CurrentThread.ManagedThreadId)
                callme(arg);
            else
                Application.MainLoop.Invoke(() => callme(arg));
        }

        private void Invoke<T, T2>(T arg, T2 arg2, Action<T, T2> callme)
        {
            if (MainThread == System.Threading.Thread.CurrentThread.ManagedThreadId)
                callme(arg, arg2);
            else
                Application.MainLoop.Invoke(() => callme(arg, arg2));
        }

        private void Invoke(Action callme)
        {
            if (MainThread == System.Threading.Thread.CurrentThread.ManagedThreadId)
                callme();
            else
                Application.MainLoop.Invoke(() => callme());
        }

        private Action GetInvoker(Action callme)
        {
            return () => Invoke(callme);
        }

        private Action<T> GetInvoker<T>(Action<T> callme)
        {
            return (arg) => Invoke(arg, callme);
        }

        private Action<T, T2> GetInvoker<T, T2>(Action<T, T2> callme)
        {
            return (arg, arg2) => Invoke(arg, arg2, callme);
        }

        private void SyncVS()
        {
            var cts = new CancellationTokenSource();
            var dialog = new ProgressDialog("Synchronize generated files", true, true, cts, (dialog) =>
            {
                _mainViewModel.SyncVS(GetInvoker<string, float>(dialog.ShowProgress), GetInvoker<string>(dialog.ShowProgress), GetInvoker<string>(dialog.ShowMessage), GetInvoker(dialog.EndProgressOperation), cts.Token);
            });
            Application.Run(dialog);

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
            AddFeatures();
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
