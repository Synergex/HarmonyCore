using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCoreCodeGenGUI.Models
{
    internal class MultiItemSettingsBase : SettingsBase
    {
        public ObservableCollection<object> Items { get; protected set; }
    }
}
