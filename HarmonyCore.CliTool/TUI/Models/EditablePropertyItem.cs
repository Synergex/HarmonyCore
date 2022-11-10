using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    class EditablePropertyItem : IHasNavigationResult
    {
        public EditablePropertyItem(ISingleItemSettings context, IPropertyItemSetting model)
        {
            Context = context;
            Model = model;
        }
        public ISingleItemSettings Context { get; set; }

        public IPropertyItemSetting Model { get; set; }

        public bool Success { get; set; }
        public IPropertyItemSetting Result { get; set; }
    }
}
