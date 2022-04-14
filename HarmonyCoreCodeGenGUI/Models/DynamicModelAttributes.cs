using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCoreCodeGenGUI.Models
{
    public class PromptAttribute : Attribute
    {
        public PromptAttribute(string value) { Value = value; }
        public string Value;
    }

    public abstract class ValueExtractorBaseAttribute : Attribute
    {
        public virtual object BindValue(PropertyInfo property, object source)
        {
            return BindValue(property.GetValue(source));
        }
        public abstract object BindValue(object value);
    }
    //Add this attribute to a string property to tell the gui
    //that field must match a repository structure
    public class StructNameOptionsAttribute : Attribute
    {

    }

    //Add this attribute to a string property to tell the gui
    //that field must match a repository structure field
    public class StructFieldNameOptionsAttribute : Attribute
    {

    }

    //Add this attribute to a string property to tell the gui
    //that field must match an smc interface
    public class InterfaceNameOptionsAttribute : Attribute
    {

    }
}
