using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HarmonyCoreCodeGenGUI.Classes
{
    public class SettingsTemplateSelector : DataTemplateSelector
    {
        public ResourceDictionary Resources { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
}
