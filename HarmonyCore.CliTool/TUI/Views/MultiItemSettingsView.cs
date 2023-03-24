using HarmonyCore.CliTool.TUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class MultiItemSettingsView : View
    {
        SingleItemSettingsView _currentItemView;
        IMultiItemSettingsBase _settings;
        ListView _structureListView;
        ScrollBarView _structureListScrollBarView;
        FrameView _currentItemFrame;
        FrameView _leftFrame;
        StatusBar _statusBar;
        public MultiItemSettingsView(IMultiItemSettingsBase settings)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            _settings = settings;

            _structureListView = new ListView(new ListWrapper(_settings.Items.Select(itm => itm.Name).ToList()))
            {
                Y = 0,
                X = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            _leftFrame = new FrameView(settings.Name);
            _currentItemFrame = new FrameView($"To add {_settings.Name.ToLower()}, select \"Add {_settings.Name.ToLower()}\" in the status bar");

            _leftFrame.Add(_structureListView);
            _structureListView.SelectedItemChanged += _structureListView_SelectedItemChanged;

            var currentItem = _settings.Items.FirstOrDefault();
            if (currentItem != null)
            {
                _currentItemView = new SingleItemSettingsView(currentItem, currentItem.Name);
                _currentItemFrame.Add(_currentItemView);
                _currentItemFrame.Title = currentItem.Name;
            }

            _structureListScrollBarView = new ScrollBarView(_structureListView, true);

            _structureListScrollBarView.ChangedPosition += () =>
            {
                _structureListView.TopItem = _structureListScrollBarView.Position;
                if (_structureListView.TopItem != _structureListScrollBarView.Position)
                {
                    _structureListScrollBarView.Position = _structureListView.TopItem;
                }
                _structureListView.SetNeedsDisplay();
            };

            _structureListScrollBarView.OtherScrollBarView.ChangedPosition += () =>
            {
                _structureListView.LeftItem = _structureListScrollBarView.OtherScrollBarView.Position;
                if (_structureListView.LeftItem != _structureListScrollBarView.OtherScrollBarView.Position)
                {
                    _structureListScrollBarView.OtherScrollBarView.Position = _structureListView.LeftItem;
                }
                _structureListView.SetNeedsDisplay();
            };

            _structureListScrollBarView.DrawContent += (e) =>
            {
                _structureListScrollBarView.Size = Math.Max(_structureListView.Source.Count - 1, 1);
                _structureListScrollBarView.Position = _structureListView.TopItem;
                _structureListScrollBarView.OtherScrollBarView.Size = _structureListView.Maxlength - 1;
                _structureListScrollBarView.OtherScrollBarView.Position = _structureListView.LeftItem;
                _structureListScrollBarView.Refresh();
            };

            Add(_leftFrame, _currentItemFrame, _structureListScrollBarView);
        }


        private List<ISingleItemSettings> _findContext;
        private string _findContextSearchTerm;
        private int _findContextIndex;

        //return true if this is a new context
        //false if this is already setup
        private bool SetFindContext(string searchTerm)
        {
            if (_findContextSearchTerm == searchTerm)
            {
                return false;
            }
            else
            {
                _findContextSearchTerm = searchTerm;
                _findContext = _settings.FindMatchingItems(searchTerm).ToList();
                _findContextIndex = 0;
                return true;
            }
        }

        public void FindNext(string searchTerm)
        {
            SetFindContext(searchTerm);

            if (_findContext.Count == 0)
                return;

            if (_findContextIndex >= _findContext.Count)
                _findContextIndex = 0;

            SelectItem(_findContext[_findContextIndex]);

            if (!_currentItemView.FindNext(searchTerm))
                _findContextIndex++;
        }

        public void FindPrev(string searchTerm)
        {
            SetFindContext(searchTerm);

            if (_findContext.Count == 0)
                return;

            if (_findContextIndex < 0)
                _findContextIndex = _findContext.Count - 1;

            SelectItem(_findContext[_findContextIndex]);

            if (!_currentItemView.FindPrev(searchTerm))
                _findContextIndex--;
        }

        public void SelectItem(ISingleItemSettings targetSetting)
        {
            var itemModels = _structureListView.Source.ToList();
            for (int i = 0; i < itemModels.Count; i++)
            {
                var row = itemModels[i] as string;
                if (row == targetSetting.Name)
                {
                    _structureListView.SelectedItem = i;
                }
            }
            _structureListView.SetNeedsDisplay();
        }

        public void AttachStatusBar(StatusBar target)
        {
            _statusBar = target;
            if (_settings.CanAddItems)
                _statusBar.AddItemAt(0, new StatusItem(Key.CtrlMask | Key.A, "~^A~ Add " + _settings.Name.ToLower(), OnAddThing));
            if (_settings.Items.Count > 0 && _statusBar.Items.Length < 2)
                _statusBar.AddItemAt(1, new StatusItem(Key.CtrlMask | Key.R, "~^R~ Remove " + _settings.Name.ToLower(), OnRemoveThing));
        }

        private class ThingPicker : IHasNavigationResult
        {
            public ISingleItemSettings Context { get; set; }

            public IPropertyItemSetting Model { get; set; }

            public bool Success { get; set; }
            public IPropertyItemSetting Result { get; set; }

            public ThingPicker(IMultiItemSettingsBase multiItemContext)
            {
                (Context, Model) = multiItemContext.GetInitialProperty();
            }
        }

        private async void OnAddThing()
        {
            //show structure/interface picker
            var picker = (await EditSettingView.PushEditSettingsView("Add " + _settings.Name.ToLower(), new ThingPicker(_settings), false)) as ThingPicker;
            if(picker.Success)
            {
                //need to run a wizard after selecting the structure or possible as part of selecting the structure
                //must at the least populate the enabled generators. Would like to multi select structures.
                _settings.AddItem(picker.Result);
                _structureListView.SetSource(_settings.Items.Select(itm => itm.Name).ToList());
                if (_statusBar.Items.Length < 2)
                    _statusBar.AddItemAt(1, new StatusItem(Key.CtrlMask | Key.R, "~^R~ Remove " + _settings.Name.ToLower(), OnRemoveThing));
                SelectItem(_settings.Items.Last());
            }
        }

        private void OnRemoveThing()
        {
            var selectedItem = _settings.Items.ElementAtOrDefault(_structureListView.SelectedItem);
            var n = MessageBox.Query("Remove item", $"Are you sure you want to remove {selectedItem.Name}?", "Yes", "No");
            if (n != 0) return;
            if (_settings is IRemovableItem removableSettings)
                removableSettings.RemoveItem(selectedItem);
            UpdateViewsWhenLastItemRemoved();
            _structureListView.SetSource(_settings.Items.Select(itm => itm.Name).ToList());
            SelectItem(_settings.Items.LastOrDefault());
        }

        private void UpdateViewsWhenLastItemRemoved()
        {
            if (_settings.Items.Count < 1)
            {
                _statusBar.RemoveItem(1);
                _currentItemFrame.Title = $"To add {_settings.Name.ToLower()}, select \"Add {_settings.Name.ToLower()}\" in the status bar";
                _currentItemFrame.RemoveAll();
            }
        }

        public void DetachStatusBar()
        {
            if (_settings.CanAddItems)
                while (_statusBar.Items.Length > 0)
                    _statusBar.RemoveItem(0);
            _statusBar = null;
        }

        public override void LayoutSubviews()
        {
            Application.Top.GetCurrentWidth(out var applicationWidth);
            if (applicationWidth < 120)
            {
                _currentItemFrame.X = 0;
                _currentItemFrame.Y = Pos.Percent(50);
                _currentItemFrame.Height = Dim.Fill();
                _currentItemFrame.Width = Dim.Fill();

                _leftFrame.Height = Dim.Percent(50, true);
                _leftFrame.Width = Dim.Fill();
                _leftFrame.X = 0;
                _leftFrame.Y = 0;
            }
            else
            {
                _currentItemFrame.X = Pos.Percent(25);
                _currentItemFrame.Y = 0;
                _currentItemFrame.Width = Dim.Fill();
                _currentItemFrame.Height = Dim.Fill();

                _leftFrame.Width = Dim.Percent(25);
                _leftFrame.Height = Dim.Fill();
                _leftFrame.X = 0;
                _leftFrame.Y = 0;
            }

            base.LayoutSubviews();
        }

        private void _structureListView_SelectedItemChanged(ListViewItemEventArgs obj)
        {
            if(obj.Item >= 0 && obj.Item < _settings.Items.Count)
            {
                var currentItem = _settings.Items[obj.Item];
                _currentItemView = new SingleItemSettingsView(currentItem, currentItem.Name);
                _currentItemFrame.Add(_currentItemView);
                _currentItemFrame.Title = currentItem.Name;
            }
            else if(_currentItemView != null)
            {
                _currentItemFrame.Clear();
                _currentItemView = null;
            }
            
            
        }
    }
}
