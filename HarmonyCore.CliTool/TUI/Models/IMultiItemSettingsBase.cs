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
        ISingleItemSettings AddItem(IPropertyItemSetting initSetting);
        (ISingleItemSettings, IPropertyItemSetting) GetInitialProperty();
        void ISettingsBase.Save(SolutionInfo context)
        {
            foreach (ISettingsBase item in Items)
            {
                item.Save(context);
            }
        }

        public IEnumerable<ISingleItemSettings> FindMatchingItems(string searchTerm)
        {
            List<ISingleItemSettings> result = new List<ISingleItemSettings>();
            foreach (var item in Items)
            {
                var matchingProperties = item.FindMatchingProperties(searchTerm);
                if (matchingProperties.Any() || item.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Add(item);
                }
            }

            return result;
        }
    }
}
