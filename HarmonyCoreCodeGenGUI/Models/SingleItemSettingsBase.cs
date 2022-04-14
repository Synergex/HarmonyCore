using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCoreCodeGenGUI.Models
{
    public class SingleItemSettingsBase : SettingsBase
    {
        public class SingleItemSetting
        {
            public string Prompt { get; set; }
            public PropertyInfo Source { get; set; }
            public object Value { get; set; }
        }
        protected SingleItemSettingsBase()
        {
            var thisType = this.GetType();
            DisplayPropertyBacking.AddRange(thisType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));
        }
        protected List<PropertyInfo> DisplayPropertyBacking { get; set; } = new List<PropertyInfo>();
        public IEnumerable<SingleItemSetting> DisplayProperties
        {
            get
            {
                var result = new List<SingleItemSetting>();
                foreach(var property in DisplayPropertyBacking)
                {
                    result.Add(new SingleItemSetting
                    {
                        Prompt = ExtractPromptFromProperty(property),
                        Value = ExtractValueFromProperty(property),
                        Source = property
                    });
                }
                return result;
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

        private string ExtractPromptFromProperty(PropertyInfo property)
        {
            var promptAttribute = property.GetCustomAttribute<PromptAttribute>(true);
            if (promptAttribute != null)
                return promptAttribute.Value;
            else
                return property.Name;
        }
    }
}
