using System.Runtime;
using System.Windows;

namespace HarmonyCoreCodeGenGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            GCSettings.LatencyMode = GCLatencyMode.Interactive;
        }
    }
}
