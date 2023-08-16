using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NStack;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class ConfirmationDialog : Dialog
    {
        public TextField confirmaiton;
               
        public ConfirmationDialog(CancellationTokenSource cts)
        {
            Title = "Confirm to proceed";

            TextView infoLabel = new TextView()
            {
                Text = "This utility will make significant changes to projects and other source files in your Harmony Core development environment. Before running this tool we recommend checking the current state of your development environment into your source code repository, taking a backup copy of the environment if you don't use source code control.",
                Y = 1,
                Width = Dim.Fill(),
                Height = 3,
                WordWrap = true,
                ReadOnly = true,
            };
            
            var inputLabel = new Label()
            {
                Text = "Type YES to proceed",
                Y = Pos.Bottom(infoLabel) + 1
            };
            confirmaiton = new TextField("")
            {
                X = Pos.Right(inputLabel) + 1,
                Y = Pos.Bottom(infoLabel) + 1,
                Width = Dim.Fill(),
            };
            var _okButton = new Button()
            {
                Text = "OK",
                Y = Pos.Bottom(inputLabel) + 1,
                X = Pos.Center(),
                IsDefault = true
            };
            var _cancelButton = new Button()
            {
                Text = "Cancel",
                Y = Pos.Bottom(inputLabel) + 1,
                X = Pos.Right(_okButton) + 1,
            };

            _cancelButton.Clicked += () =>
            {
                cts.Cancel();
                Application.RequestStop();
            };

            _okButton.Clicked += () =>
            {
                if (confirmaiton.Text.ToLower() == "yes")
                {
                    Application.RequestStop();
                }
                else
                {
                    cts.Cancel();
                    Application.RequestStop();
                }
            };

            Add(infoLabel, inputLabel, confirmaiton, _okButton, _cancelButton);
        }
    }
}
