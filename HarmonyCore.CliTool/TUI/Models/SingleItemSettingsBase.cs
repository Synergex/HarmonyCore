using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    public interface ISingleItemSettings : ISettingsBase
    {
        public IEnumerable<IPropertyItemSetting> FindMatchingProperties(string searchTerm)
        {
            List<IPropertyItemSetting> result = new List<IPropertyItemSetting>();
            foreach (var prop in DisplayProperties)
            {
                if (prop.Prompt.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ||
                    (prop.Value?.ToString()?.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase) ?? false))
                {
                    result.Add(prop);
                }
            }

            return result;
        }
        public List<PropertyInfo> DisplayPropertyBacking { get; }
        public SolutionInfo Context { get; }
        public IEnumerable<IPropertyItemSetting> DisplayProperties
        {
            get
            {
                var result = new List<IPropertyItemSetting>();
                foreach (var property in DisplayPropertyBacking)
                {
                    result.Add(MakeItemSetting(property));
                }
                return result;
            }
        }

        internal void LoadDisplayPropertyBacking(Type targetType)
        {
            DisplayPropertyBacking
                .AddRange(targetType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(prop => prop.GetCustomAttribute<IgnorePropertyAttribute>() == null));
        }
        internal void LoadDisplayPropertyBacking<T>()
        {
            LoadDisplayPropertyBacking(typeof(T));
        }

        internal void LoadSameProperties(object wrapping)
        {
            var wrappingProperties = wrapping.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var wrappingProperty in wrappingProperties)
            {
                var foundProperty = DisplayPropertyBacking.FirstOrDefault(prop => prop.Name == wrappingProperty.Name);
                if (foundProperty != null && wrappingProperty.PropertyType == foundProperty.PropertyType)
                    foundProperty.SetValue(this, wrappingProperty.GetValue(wrapping));
            }
        }

        internal void SaveSameProperties(object wrapping)
        {
            var wrappingProperties = wrapping.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var wrappingProperty in wrappingProperties)
            {
                var foundProperty = DisplayPropertyBacking.FirstOrDefault(prop => prop.Name == wrappingProperty.Name);
                if (foundProperty != null && wrappingProperty.PropertyType == foundProperty.PropertyType)
                    wrappingProperty.SetValue(wrapping, foundProperty.GetValue(this));
            }
        }

        private IPropertyItemSetting MakeItemSetting(PropertyInfo property)
        {
            if (property.PropertyType.IsAssignableTo(typeof(IPropertyItemSetting)))
            {
                var typedResult = property.GetValue(this) as IPropertyItemSetting;
                typedResult.Prompt = ExtractPromptFromProperty(property);
                typedResult.Value = ExtractValueFromProperty(property);
                typedResult.Source = property;
                return typedResult;
            }
            else
            {
                return new GeneratedPropertyItemSetting
                {
                    Prompt = ExtractPromptFromProperty(property),
                    Value = ExtractValueFromProperty(property),
                    Source = property
                };
            }
        }

        private object ExtractValueFromProperty(PropertyInfo property)
        {
            var valueExtractorAttribute = property.GetCustomAttribute<ValueExtractorBaseAttribute>(true);
            if (valueExtractorAttribute != null)
                return valueExtractorAttribute.BindValue(property, this);
            else
                return property.GetValue(this);
        }

        public object ExtractValueFromProperty(PropertyInfo property, object actualValue)
        {
            var valueExtractorAttribute = property.GetCustomAttribute<ValueExtractorBaseAttribute>(true);
            if (valueExtractorAttribute != null)
                return valueExtractorAttribute.BindValue(actualValue);
            else
                return actualValue;
        }

        public bool AllowMultiSelectionForProperty(PropertyInfo property)
        {
            var allowMultiSelection = property.GetCustomAttribute<AllowMultiSelectionAttribute>(true);
            if (allowMultiSelection != null)
                return true;
            else
                return false;
        }

        public string MultiSelectionDelimiterForProperty(PropertyInfo property)
        {
            var enumerableExtractor = property.GetCustomAttribute<IEnumerableExtractorAttribute>(true);
            if (enumerableExtractor != null)
                return enumerableExtractor.Delimiter;
            else
                return ",";
        }

        public List<object> ExtractValueOptionsFromProperty(IPropertyItemSetting setting)
        {
            var valueExtractorAttribute = setting.Source.GetCustomAttribute<ValueOptionsExtractorBaseAttribute>(true);
            if (valueExtractorAttribute != null)
                return valueExtractorAttribute.BindValue(setting.Source, setting.Source.GetValue(this), this, Context);
            else
                return new List<object>();
        }

        public IPropertyItemSetting UpdateSettingValue(IPropertyItemSetting setting, object value)
        {
            var valueExtractorAttribute = setting.Source.GetCustomAttribute<ValueInjectorBaseAttribute>(true);
            if (valueExtractorAttribute != null)
                valueExtractorAttribute.BindValue(setting.Source, this, value);
            else
                setting.Source.SetValue(this, value);

            return new GeneratedPropertyItemSetting
            {
                Prompt = setting.Prompt,
                Value = ExtractValueFromProperty(setting.Source),
                Source = setting.Source
            };
        }

        private static string SplitCamelCase(string str)
        {
            var result = Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            ).ToLower();
            return char.ToUpper(result[0]) + result.Substring(1);
        }

        private string ExtractPromptFromProperty(PropertyInfo property)
        {
            var promptAttribute = property.GetCustomAttribute<PromptAttribute>(true);
            if (promptAttribute != null)
                return promptAttribute.Value;
            else
                return SplitCamelCase(property.Name);
        }
    }
    public partial class SingleItemSettingsBase : ISingleItemSettings
    {
        protected SingleItemSettingsBase(SolutionInfo context)
        {
            Context = context;
            BaseInterface.LoadDisplayPropertyBacking(this.GetType());
        }

        protected ISingleItemSettings BaseInterface => this;

        public SolutionInfo Context { get; set; }

        public string Name { get; set; }

        public List<PropertyInfo> DisplayPropertyBacking { get; } = new List<PropertyInfo>();

        public virtual void Save(SolutionInfo context)
        {
            throw new NotImplementedException();
        }
    }
}
