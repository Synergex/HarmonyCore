using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    public interface IRemovableItem
    {
        bool CanRemoveItems { get; set; }
        void RemoveItem(ISingleItemSettings itemToRemove);

    }
}
