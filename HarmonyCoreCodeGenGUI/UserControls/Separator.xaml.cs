using System.Windows;
using System.Windows.Controls;

namespace HarmonyCoreCodeGenGUI.UserControls
{
    /// <summary>
    /// Interaction logic for Separator.xaml
    /// </summary>
    public partial class Separator : UserControl
    {
        public static readonly DependencyProperty SeparatorTitleProperty = DependencyProperty.Register("SeparatorTitle", typeof(string), typeof(Separator));

        public string SeparatorTitle
        {
            get
            {
                return (string)GetValue(SeparatorTitleProperty);
            }
            set
            {
                SetValue(SeparatorTitleProperty, value);
            }
        }

        public Separator()
        {
            InitializeComponent();

            (Content as FrameworkElement).DataContext = this;
        }
    }
}
