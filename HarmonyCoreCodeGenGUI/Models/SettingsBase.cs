using HarmonyCoreGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCoreCodeGenGUI.Models
{
    public class SettingsBase
    {
        public virtual bool IsEnabled(Solution solution)
        {
            return true;
        }
    }
}
