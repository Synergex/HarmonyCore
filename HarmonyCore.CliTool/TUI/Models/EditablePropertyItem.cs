using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    class EditablePropertyItem : IHasNavigationResult
    {
        public EditablePropertyItem(ISingleItemSettings context, PropertyItemSetting model)
        {
            Context = context;
            Model = model;
        }
        public ISingleItemSettings Context { get; set; }

        public PropertyItemSetting Model { get; set; }

        public bool Success { get; set; }
        public PropertyItemSetting Result { get; set; }
    }
}
