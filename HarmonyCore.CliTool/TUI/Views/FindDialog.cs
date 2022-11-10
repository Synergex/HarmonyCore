using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace HarmonyCore.CliTool.TUI.Views
{
    internal class FindDialog : Window
    {
        public Action<string> FindNext;
        public Action<string> FindPrevious;
        private string _textToFind;
        public FindDialog()
        {
            X = Application.Top.Bounds.Width - 30;
            Y = 0;
            ColorScheme = Colors.TopLevel;

			DrawContent += (e) => {
				foreach (var v in Subviews)
				{
					v.SetNeedsDisplay();
				}
			};

			var lblWidth = "Replace:".Length;

			var label = new Label("Find:")
			{
				Y = 1,
				Width = lblWidth,
				TextAlignment = TextAlignment.Right,
				AutoSize = false
			};
			Add(label);

			var txtToFind = new TextField()
			{
				X = Pos.Right(label) + 1,
				Y = Pos.Top(label),
				Width = 20
			};

			Add(txtToFind);

			var btnFindNext = new Button("Find _Next")
			{
				X = Pos.Right(txtToFind) + 1,
				Y = Pos.Top(label),
				Width = 20,
				Enabled = !txtToFind.Text.IsEmpty,
				TextAlignment = TextAlignment.Centered,
				IsDefault = true,
				AutoSize = false
			};
			btnFindNext.Clicked += () => FindNext(_textToFind);
			Add(btnFindNext);

			var btnFindPrevious = new Button("Find _Previous")
			{
				X = Pos.Right(txtToFind) + 1,
				Y = Pos.Top(btnFindNext) + 1,
				Width = 20,
				Enabled = !txtToFind.Text.IsEmpty,
				TextAlignment = TextAlignment.Centered,
				AutoSize = false
			};
			btnFindPrevious.Clicked += () => FindPrevious(_textToFind);
			Add(btnFindPrevious);

			txtToFind.TextChanged += (e) => {
				_textToFind = txtToFind.Text.ToString();
                btnFindNext.Enabled = !txtToFind.Text.IsEmpty;
				btnFindPrevious.Enabled = !txtToFind.Text.IsEmpty;
			};

            txtToFind.KeyPress += (e) =>
            {
                if (e.KeyEvent.Key == Key.Enter)
                {
                    FindNext(_textToFind);
					e.Handled = true;
                }
            };

            var btnCancel = new Button("Cancel")
			{
				X = Pos.Right(txtToFind) + 1,
				Y = Pos.Top(btnFindPrevious) + 2,
				Width = 20,
				TextAlignment = TextAlignment.Centered,
				AutoSize = false
			};
			btnCancel.Clicked += () => {
                Application.Top.Remove(this);
			};
			Add(btnCancel);

            Width = label.Width + txtToFind.Width + btnFindNext.Width + 2;
			Height = btnFindNext.Height + btnFindPrevious.Height + btnCancel.Height + 4;

            this.KeyPress += (args) =>
            {
                if (args.KeyEvent.Key == Key.Esc)
                {
                    args.Handled = true;
                    Application.Top.Remove(this);
                }
				else if (args.KeyEvent.Key == Key.F3)
                {
                    FindNext(_textToFind);
                }
            };
        }
    }
}
