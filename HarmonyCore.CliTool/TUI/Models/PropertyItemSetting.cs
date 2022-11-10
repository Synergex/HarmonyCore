using System.Reflection;

namespace HarmonyCore.CliTool.TUI.Models
{
    public interface IPropertyItemSetting
    {
        public string Prompt { get; set; }
        public PropertyInfo Source { get; set; }
        public object Value { get; set; }
    }

    public class GeneratedPropertyItemSetting : IPropertyItemSetting
    {
        public string Prompt { get; set; }
        public PropertyInfo Source { get; set; }
        public object Value { get; set; }
    }
}
