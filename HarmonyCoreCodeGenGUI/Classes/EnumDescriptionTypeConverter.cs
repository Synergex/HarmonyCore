using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace HarmonyCoreCodeGenGUI.Classes
{
    /// <summary>
    /// Converts enum values into their descriptions
    /// <para>Usage: [Description("DescriptionHere")]</para>
    /// <para>Note: Implementation of this class is derived from http://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/</para>
    /// </summary>
    public class EnumDescriptionTypeConverter : EnumConverter
    {
        public EnumDescriptionTypeConverter(Type type) : base(type) { }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
                    if (fieldInfo != null)
                    {
                        DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        return ((attributes.Length > 0) && (!string.IsNullOrEmpty(attributes[0].Description))) ? attributes[0].Description : value.ToString();
                    }
                }

                return string.Empty;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
