using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    interface IHasNavigationResult
    {
        ISingleItemSettings Context { get; }
        IPropertyItemSetting Model { get; }
        bool Success { set; }
        IPropertyItemSetting Result { set; }
    }
}
