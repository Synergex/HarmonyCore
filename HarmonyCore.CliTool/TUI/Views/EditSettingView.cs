using HarmonyCore.CliTool.TUI.Models;
using Microsoft.Build.Tasks;
using NStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class EditSettingView : Dialog
    {
        private Func<object> _getResult;
        IHasNavigationResult _navigationResult;
        public EditSettingView(string title, IHasNavigationResult navigationObject) : base(title)
        {
            _navigationResult = navigationObject;
            var oldValue = navigationObject.Model;
            var ok = new Button("Ok", is_default: true);
            ok.Clicked += OkPressed;
            var cancel = new Button("Cancel");
            cancel.Clicked += CancelPressed;

            AddButton(ok);
            AddButton(cancel);

            var lbl = new Label()
            {
                X = 0,
                Y = 1,
                Text = oldValue.Prompt
            };

            if (oldValue is IMultiItemSettingsBase)
            {
                var multiItemView = new MultiItemSettingsView(oldValue as IMultiItemSettingsBase)
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill() - 1, //leave room for the buttons at the bottom
                };
                Add(lbl, multiItemView);
                multiItemView.SetFocus();
                _getResult = () => oldValue;
            }
            else if (oldValue is ISingleItemSettings)
            {
                var singleItemView = new SingleItemSettingsView(oldValue as ISingleItemSettings)
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill() - 1, //leave room for the buttons at the bottom
                };
                Add(lbl, singleItemView);
                singleItemView.SetFocus();
                _getResult = () => oldValue;
            }
            else
            {
                var options = navigationObject.Context.ExtractValueOptionsFromProperty(oldValue);
                bool allowsMultiSelection = navigationObject.Context.AllowMultiSelectionForProperty(oldValue.Source);
                if (options.Count == 0)
                {
                    var tf = new TextView()
                    {
                        Text = oldValue.Value?.ToString() ?? "",
                        X = 0,
                        Y = 2,
                        Width = Dim.Fill(),
                        Height = Dim.Fill() - 3,
                        ReadOnly = false,
                        WordWrap = true,
                        AllowsTab = true,
                        AllowsReturn = true,
                    };
                    Add(lbl, tf);
                    tf.SetFocus();
                    _getResult = () => tf.Text.ToString();
                }
                else if (options.Count == 3 && oldValue.Source.PropertyType == typeof(Nullable<bool>))
                {
                    lbl.GetCurrentWidth(out var labelWidth);
                    var tscb = new TriStateCheckBox()
                    {
                        X = labelWidth + 3,
                        Y = 1,
                        Height = Dim.Fill() - 1, //leave room for the buttons at the bottom
                        Checked = oldValue.Value as Nullable<bool>
                    };
                    Add(lbl, tscb);
                    tscb.SetFocus();
                    _getResult = () => navigationObject.Context.ExtractValueFromProperty(oldValue.Source, (object)tscb.Checked).ToString();
                }
                else if (options.Count < 6 && !allowsMultiSelection)
                {
                    var rg = new RadioGroup()
                    {
                        X = 0,
                        Y = 2,
                        Width = Dim.Fill(),
                        Height = Dim.Fill() - 5, //leave room for the buttons at the bottom
                        RadioLabels = options.Select(obj => ustring.Make(obj.ToString())).ToArray(),
                        SelectedItem = options.IndexOf(oldValue.Value),
                    };
                    var helpText = "Click an item to select it, or use arrow keys to move to an item," 
                                 + "and press the spacebar to select it. Then click Ok or press Enter.";
                    var affordance = new TextView() 
                    {
                        Text = helpText,
                        X = 2,
                        Y = 11,
                        Width = Dim.Fill() - 4,
                        Height = 3,
                        ReadOnly = true,
                        WordWrap = true,
                    };
                    Add(lbl, rg, affordance);
                    ok.Enabled = false;
                    rg.SetFocus();
                    rg.SelectedItemChanged += item =>
                    {
                        //If a radio button is selected (e.g., with mouse click), enable the OK button.
                        ok.Enabled = true;
                    };
                    rg.KeyPress += key =>
                    {
                        //If a key is pressed and a radio button is selected, enable OK.
                        if (rg.SelectedItem >= 0)
                        {
                            ok.Enabled = true;
                        }
                    };
                    _getResult = () => options[rg.SelectedItem].ToString();
                }
                else if (!allowsMultiSelection)
                {
                    var cf = new ComboBox()
                    {
                        X = 0,
                        Y = 2,
                        Width = Dim.Fill(),
                        Height = Dim.Fill() - 1, //leave room for the buttons at the bottom
                        Source = new ListWrapper(options)
                    };
                    cf.SelectedItem = options.IndexOf(oldValue.Value);
                    cf.KeyPress += key =>
                    {
                        //if the user presses enter while focused on the combobox they want to accept that value
                        //switch focus to the ok button
                        if (key.KeyEvent.Key == Key.Enter)
                            ok.SetFocus();
                    };
                    Add(lbl, cf);
                    cf.SetFocus();
                    cf.Expand();
                    _getResult = () => cf.Text.ToString();
                }
                else
                {
                    var delimiter = navigationObject.Context.MultiSelectionDelimiterForProperty(oldValue.Source);
                    var listSource = new ListWrapper(options);

                    if (oldValue.Value is string stringValue)
                    {
                        var splitItems = stringValue.Split(delimiter);
                        foreach (var item in splitItems)
                        {
                            var foundIndex = options.FindIndex(itm => (string)itm == item);
                            if (foundIndex != -1)
                                listSource.SetMark(foundIndex, true);
                        }
                    }

                    var lv = new ListView(listSource)
                    {
                        X = 0,
                        Y = 2,
                        Width = Dim.Fill(),
                        Height = Dim.Fill() - 1, //leave room for the buttons at the bottom
                        AllowsMarking = true
                    };
                    Add(lbl, lv);
                    lv.SetFocus();
                    _getResult = () =>
                    {
                        //get all marked items
                        //join using delimiter
                        var result = new List<string>();
                        for (int i = 0; i < listSource.Count; i++)
                        {
                            if (listSource.IsMarked(i))
                            {
                                result.Add(options[i].ToString());
                            }
                        }
                        return string.Join(delimiter, result);
                    };
                }
            }
        }

        private void Rg_SelectedItemChanged(SelectedItemChangedArgs obj)
        {
            throw new NotImplementedException();
        }
        void OkPressed()
        {
            try
            {
                _navigationResult.Result = _navigationResult.Context.UpdateSettingValue(_navigationResult.Model, _getResult());
                _navigationResult.Success = true;
            }

            catch (Exception ex)
            {
                _navigationResult.Success = false;
                MessageBox.ErrorQuery(60, 20, "Failed to set text", ex.Message, "Ok");
            }
            Application.RequestStop(this);
        }

        void CancelPressed()
        {
            _navigationResult.Success = false;
            Application.RequestStop(this);
        }

        public static Task<IHasNavigationResult> PushEditSettingsView(string title, IHasNavigationResult navResult, bool fullscreen)
        {
            var tcs = new TaskCompletionSource<IHasNavigationResult>();
            var settingsView = new EditSettingView(title, navResult);
            if (fullscreen)
            {
                settingsView.X = 0;
                settingsView.Y = 1;
                settingsView.Width = Dim.Fill();
                settingsView.Height = Dim.Fill();
                settingsView.ColorScheme = Colors.TopLevel;
            }

            settingsView.Closed += (view) => tcs.TrySetResult(navResult);
            Application.Run(settingsView);
            return tcs.Task;
        }
    }
}
