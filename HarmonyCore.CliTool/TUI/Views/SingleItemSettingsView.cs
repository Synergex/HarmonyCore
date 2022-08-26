using HarmonyCore.CliTool.TUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using System.Data;
using NStack;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class SingleItemSettingsView : View
    {
        ISingleItemSettings _settings;
        TableView _tableView;
        ScrollBarView _scrollBar;
        DataTable _dataSource;
        DataColumn _promptColumn;
        DataColumn _valueColumn;
        string _titleContext;
        public SingleItemSettingsView(ISingleItemSettings settings, string titleContext = null)
        {
            Width = Dim.Fill();
            Height = Dim.Fill();
            _settings = settings;

            _dataSource = new DataTable();
            _promptColumn = new DataColumn(" ", typeof(String));
            _valueColumn = new DataColumn("  ", typeof(object));
            _dataSource.Columns.Add(_promptColumn);
            _dataSource.Columns.Add(_valueColumn);

            var alignRight = new TableView.ColumnStyle()
            {
                AlignmentGetter = (obj) => TextAlignment.Right
            };

            string Truncate(string value, int maxChars)
            {
                return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
            }

            var singleItemSetting = new TableView.ColumnStyle()
            {
                RepresentationGetter = (obj) =>
                {
                    var typedObj = obj as PropertyItemSetting;
                    if (!GetCurrentWidth(out var currentWidth))
                        currentWidth = 40;
                    else
                        currentWidth /= 2;

                    return Truncate(typedObj.Value?.ToString() ?? "-", currentWidth);
                }
            };

            _tableView = new TableView(_dataSource)
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            _tableView.FullRowSelect = true;
            _tableView.Style.ShowHorizontalHeaderOverline = false;
            _tableView.Style.ShowHorizontalHeaderUnderline = false;
            _tableView.Style.ShowVerticalHeaderLines = false;
            _tableView.Style.ShowVerticalCellLines = false;
            _tableView.Style.ColumnStyles.Add(_promptColumn, alignRight);
            _tableView.Style.ColumnStyles.Add(_valueColumn, singleItemSetting);
            Add(_tableView);
            SetupScrollBar();
            foreach (var item in _settings.DisplayProperties)
            {
                _dataSource.Rows.Add(item.Prompt, item);
            }
            _tableView.CellActivated += EditCurrentCell;
            _titleContext=titleContext ?? string.Empty;
        }

        private void EditCurrentCell(TableView.CellActivatedEventArgs e)
        {
            if (e.Table == null)
                return;

            var editValue = new EditablePropertyItem(_settings, e.Table.Rows[e.Row][1] as PropertyItemSetting);
            var isfullScreen = editValue.Model is IMultiItemSettingsBase;
            var prompt = string.IsNullOrWhiteSpace(_titleContext) ? editValue.Model.Prompt : _titleContext + " > " + editValue.Model.Prompt;
            EditSettingView.PushEditSettingsView(isfullScreen ? prompt : "Enter new value", editValue, isfullScreen);

            if (editValue.Success)
            {
                e.Table.Rows[e.Row][1] = editValue.Result;

                _tableView.Update();
            }
        }

        private void SetupScrollBar()
        {
            _scrollBar = new ScrollBarView(_tableView, true);

            _scrollBar.ChangedPosition += () => {
                _tableView.RowOffset = _scrollBar.Position;
                if (_tableView.RowOffset != _scrollBar.Position)
                {
                    _scrollBar.Position = _tableView.RowOffset;
                }
                _tableView.SetNeedsDisplay();
            };

            _tableView.DrawContent += (e) => {
                _scrollBar.Size = _tableView.Table?.Rows?.Count ??0;
                _scrollBar.Position = _tableView.RowOffset;
                _scrollBar.Refresh();
            };

        }
    }
}
