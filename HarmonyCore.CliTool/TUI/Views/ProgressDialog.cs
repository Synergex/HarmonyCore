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
    internal class ProgressDialog : Dialog
    {
        private TextView _loadView;
        private View _progressText;
        private ProgressBar _progressView;
        private Timer _progressTimer;
        private Button _okButton;
        private Button _cancelButton;
        private Action<ProgressDialog> _operation;
        bool _showProgress;

        public async void EndProgressOperation()
        {
            _progressView.Fraction = 100f;
            _okButton.Enabled = true;
            Remove(_cancelButton);
            _progressTimer?.Dispose();
            _progressText.Text = "Finished";
            _okButton.SetFocus();
        }

        public void ShowProgress(string status, float percent)
        {
            if(_showProgress)
                _progressView.Fraction = percent;

            _progressText.Text = status;
        }

        public void ShowMessage(string message)
        {
            _loadView.Text = _loadView.Text + message + "\n";
            _loadView.ProcessKey(new KeyEvent(Key.End, new KeyModifiers() { Ctrl = true }));
        }

        public void ShowProgress(string message)
        {
            _progressText.Text = message;
        }

        public override async void OnLoaded()
        {
            base.OnLoaded();
            _operation(this);
        }

        public ProgressDialog(string operation, bool fractionProgress, bool hasOk, CancellationTokenSource cts, Action<ProgressDialog> opAction)
        {
            _showProgress = fractionProgress;
            _operation = opAction;
            _okButton = new Button("Ok") { Enabled = false };
            _cancelButton = new Button("Cancel");
            _cancelButton.Clicked += () =>
            {
                cts.Cancel();
                Application.RequestStop();
            };

            _okButton.Clicked += () =>
            {
                Application.RequestStop();
            };

            if(hasOk)
                AddButton(_okButton);

            AddButton(_cancelButton);
            Title = operation;

            _loadView = new TextView()
            {
                X = Pos.Center(),
                Y = 4,
                Width = Dim.Fill(1),
                Height = Dim.Fill(1),
                Text = "",
                ReadOnly = true,
                WordWrap = true
            };

            _progressText = new View()
            {
                X = Pos.Center(),
                Y = 1,
                Width = Dim.Fill(1),
                Height = 1,
                Text = operation
            };

            _progressView = new ProgressBar()
            {
                X = Pos.Center(),
                Y = 3,
                ProgressBarStyle = fractionProgress ? ProgressBarStyle.Blocks : ProgressBarStyle.MarqueeContinuous,
                Width = Dim.Fill(1)
            };

            Add(_progressText, _progressView, _loadView);

            if (!fractionProgress)
            {
                _progressTimer = new Timer((_) =>
                {
                    _progressView.Pulse();
                    Application.MainLoop.Driver.Wakeup();
                }, null, 0, 300);
            }
        }
    }
}
