using System.Reflection;

namespace HarmonyCore.CliTool.TUI.Models
{
    public class PropertyItemSetting
    {
        public string Prompt { get; set; }
        public PropertyInfo Source { get; set; }
        public virtual object Value { get; set; }
    }
}
