using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HarmonyCoreCodeGenGUI.Views
{
    /// <summary>
    /// Interaction logic for DynamicSettingsTab.xaml
    /// </summary>
    public partial class DynamicSettingsTab : UserControl
    {
        public DynamicSettingsTab()
        {
            InitializeComponent();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
        }
    }
}
