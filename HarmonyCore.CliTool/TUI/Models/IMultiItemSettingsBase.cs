using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    public interface IMultiItemSettingsBase : ISettingsBase
    {
        bool CanAddItems { get; }
        List<ISingleItemSettings> Items { get; }
        ISingleItemSettings AddItem(PropertyItemSetting initSetting);
        (ISingleItemSettings, PropertyItemSetting) GetInitialProperty();
        void ISettingsBase.Save(SolutionInfo context)
        {
            foreach (ISettingsBase item in Items)
            {
                item.Save(context);
            }
        }

    }
}
