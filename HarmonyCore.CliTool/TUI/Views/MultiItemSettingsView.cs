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
            _currentItemFrame = new FrameView(" ");

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

        public void AttachStatusBar(StatusBar target)
        {
            _statusBar = target;
            if (_settings.CanAddItems)
            {
                _statusBar.AddItemAt(0, new StatusItem(Key.CtrlMask | Key.R, "~^R~ Remove " + _settings.Name.ToLower(), OnRemoveThing));
                _statusBar.AddItemAt(0, new StatusItem(Key.CtrlMask | Key.A, "~^A~ Add " + _settings.Name.ToLower(), OnAddThing));
            }
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
                //need to run a wizzard after selecting the structure or possible as part of selecting the structure
                //must at the least populate the enabled generators. Would like to multi select structures.
                _settings.AddItem(picker.Result);
                _structureListView.SetSource(_settings.Items.Select(itm => itm.Name).ToList());
            }
        }

        private void OnRemoveThing()
        {
            throw new NotImplementedException();
        }

        public void DetachStatusBar()
        {
            if (_settings.CanAddItems)
            {
                _statusBar.RemoveItem(0);
                _statusBar.RemoveItem(0);
            }
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
