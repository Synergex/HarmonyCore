using System.Windows;
using System.Windows.Controls;

namespace HarmonyCoreCodeGenGUI.UserControls
{
    /// <summary>
    /// Interaction logic for TextBox.xaml
    /// </summary>
    public partial class TextBox : UserControl
    {
        public static readonly DependencyProperty TextBoxTitleProperty = DependencyProperty.Register("TextBoxTitle", typeof(string), typeof(TextBox));

        public string TextBoxTitle
        {
            get
            {
                return (string)GetValue(TextBoxTitleProperty);
            }
            set
            {
                SetValue(TextBoxTitleProperty, value);
            }
        }


        public static readonly DependencyProperty TextBoxTextProperty = DependencyProperty.Register("TextBoxText", typeof(string), typeof(TextBox));
        public string TextBoxText
        {
            get
            {
                return (string)GetValue(TextBoxTextProperty);
            }
            set
            {
                SetValue(TextBoxTextProperty, value);
            }
        }

        public TextBox()
        {
            InitializeComponent();

            (Content as FrameworkElement).DataContext = this;
        }
    }
}
