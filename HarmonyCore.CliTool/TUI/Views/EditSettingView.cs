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
                Text = oldValue.Prompt + ":"
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
                    var helpText = "There are no " + navigationObject.Context.Name.Substring(5) + "s to add."; //Substring() removes "Pick " from start of string. 
                    var hv = new TextView()
                    {
                        Text = helpText,
                        X = 3, Y = 1,
                        Width = Dim.Fill() - 4,
                        Height = 3,
                        ReadOnly = true,
                        WordWrap = true,
                        CanFocus = false,
                        ColorScheme = new ColorScheme() { Focus = Terminal.Gui.Attribute.Make(Color.Gray, Color.Gray) }
                    };
                    Add(hv);
                    ok.Enabled = false;
                    hv.SetFocus();
                    _getResult = () => hv.Text.ToString();
                }
                else if(allowsMultiSelection)
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
                else
                {
                    ok.Clicked -= OkPressed; //Handler removed but later re-added when an item is marked
                    var delimiter = navigationObject.Context.MultiSelectionDelimiterForProperty(oldValue.Source);
                    var lv = new ListView()
                    {
                        X = 8, Y = 3,
                        Width = Dim.Fill() - 6,
                        Height = Dim.Fill() - 2,
                        AllowsMarking = true,
                        AllowsMultipleSelection = true,
                        Source = new ListWrapper(options)
                    };
                    var searchLabel = new Label()
                    {
                        X = 2, Y = 4,
                        Text = "Search/filter:"
                    };
                    var searchField = new TextField()
                    {
                        X = 17, Y = 4,
                        Width = 10,
                    };
                    var helpText =  "Select (check) items by clicking or pressing the " + 
                                    "spacebar when an item is highlighted.";
                    var helpTextView = new TextView()
                    {
                        Text = helpText,
                        X = 2, Y = 1,
                        Width = Dim.Fill() - 4,
                        Height = 3,
                        ReadOnly = true,
                        WordWrap = true,
                        ColorScheme = new ColorScheme() { Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.Gray) }
                    };

                    ok.Enter += (e) =>
                    //When ok view gets focus...
                    {
                        ok.Clicked -= OkPressed;
                        //If anything in the list is selected
                        for (int i = 0; i < lv.Source.Count; i++)
                            if (lv.Source.IsMarked(i))
                            {
                                ok.Clicked += OkPressed;
                                ok.SetFocus(); 
                                break;
                            }
                    };

                    lv.KeyPress += (key) =>
                     {
                         ok.Clicked -= OkPressed;
                         //If anything in the list is selected...
                         for (int i = 0; i < lv.Source.Count; i++)
                             if (lv.Source.IsMarked(i))
                             {
                                 ok.Enabled = true;
                                 ok.Clicked += OkPressed;
                                 break;
                             }
                     };

                    searchField.Enter += (e) =>
                    //When search field gets focus...
                    {
                        searchField.ColorScheme = new ColorScheme() { Focus = Terminal.Gui.Attribute.Make(Color.Black, Color.Gray) };
                    };

                    searchField.TextChanged += (e) =>
                    //When text changes in search field, filter list
                    {
                        var searchString = searchField.Text.ToString().ToLower();
                        IListDataSource originalSource = new ListWrapper(options);
                        var lvItems = originalSource.ToList();
                        var newItems = lvItems
                            .Cast<object>()
                            .Select(item => item.ToString())
                            .Where(item => searchString == "" || item.ToLower().Contains(searchString))
                            .ToList();
                        lv.Source = new ListWrapper(newItems);
                        lv.SelectedItem = 0;
                    };

                    if (options.Count > 6)
                    {
                        lbl.Y = 6;
                        lv.Y = 6;
                        Add(lbl, searchLabel, searchField, lv, helpTextView);
                    }
                    else
                    {
                        lbl.Y = 4;
                        lv.Y = 4;
                        Add(lbl, lv, helpTextView);
                    }
                    lbl.X = 2;
                    ok.Enabled = true;
                    lv.SetFocus();

                    _getResult = () =>
                    {
                        //Get all marked items and join with delimiter
                        var result = new List<string>();
                        //If anything in the list is selected...
                        for (int i = 0; i < lv.Source.Count; i++)
                            if (lv.Source.IsMarked(i))
                                result.Add(lv.Source.ToList()[i].ToString());
                        return string.Join(delimiter, result);
                    };
                }
            }
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
