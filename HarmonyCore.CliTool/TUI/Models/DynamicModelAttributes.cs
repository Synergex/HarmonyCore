using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCore.CliTool.TUI.Models
{
    public class PromptAttribute : Attribute
    {
        public PromptAttribute(string value) { Value = value; }
        public string Value;
    }

    public class IgnorePropertyAttribute : Attribute
    {
    }

    public class AllowMultiSelectionAttribute : Attribute
    {
    }

    public abstract class ValueExtractorBaseAttribute : Attribute
    {
        public virtual object BindValue(PropertyInfo property, object source)
        {
            return BindValue(property.GetValue(source));
        }
        public abstract object BindValue(object value);
    }

    public abstract class ValueInjectorBaseAttribute : Attribute
    {
        public virtual void BindValue(PropertyInfo property, object source, object value)
        {
            property.SetValue(source, BindValue(value));
        }
        public abstract object BindValue(object value);
    }

    public abstract class ValueOptionsExtractorBaseAttribute : Attribute
    {
        public abstract List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context);
    }

    public class IEnumerableInjectorAttribute : ValueInjectorBaseAttribute
    {
        Type _targetType;
        string _delimeter;
        public IEnumerableInjectorAttribute(Type targetType, string delimeter)
        {
            _targetType = targetType;
            _delimeter = delimeter;
        }
        public override object BindValue(object value)
        {
            if (value is string valueString)
            {
                if (_targetType == typeof(HashSet<string>))
                {
                    return new HashSet<string>(valueString.Split(_delimeter));
                }
                else if (_targetType == typeof(List<string>))
                {
                    return new List<string>(valueString.Split(_delimeter));
                }
            }
            throw new NotImplementedException();
        }
    }

    public class ComplexObjectExtractorAttribute : ValueExtractorBaseAttribute
    {
        public override object BindValue(object value)
        {
            if (value == null)
                return "-";
            else
                return "...";
        }
    }

    public class IEnumerableExtractorAttribute : ValueExtractorBaseAttribute
    {
        public string Delimiter { get; set; }
        public IEnumerableExtractorAttribute(string delimiter)
        {
            Delimiter = delimiter;
        }
        public override object BindValue(object value)
        {
            if (value is IEnumerable enumerable)
            {
                var result = string.Join(Delimiter, enumerable.OfType<object>().Select(obj => obj.ToString()));
                if (string.IsNullOrWhiteSpace(result))
                    return "-";
                else
                    return result;
            }
            else if (value == null)
                return "-";
            else
                throw new NotImplementedException();
        }
    }

    public class DictionaryExtractorAttribute : ValueExtractorBaseAttribute
    {
        string _elementDelimiter;
        string _keyDelimiter;
        public DictionaryExtractorAttribute(string keyDelimiter, string elementDelimiter)
        {
            _keyDelimiter = keyDelimiter;
            _elementDelimiter = elementDelimiter;
        }
        public override object BindValue(object value)
        {
            if (value is Dictionary<string, string> enumerable)
            {
                var result = string.Join(_elementDelimiter, enumerable.Select(kvp => kvp.Key + _keyDelimiter + kvp.Value));
                if (string.IsNullOrWhiteSpace(result))
                    return "-";
                else
                    return result;
            }
            else if (value == null)
                return "-";
            else
                throw new NotImplementedException();
        }
    }

    public class DisallowEdits : Attribute
    {

    }

    public class NullableBoolOptionsExtractorAttribute : ValueOptionsExtractorBaseAttribute
    {
        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context)
        {
            return new List<object>
            {
                "-",
                "yes",
                "no"
            };
        }
    }

    public class NullableBoolExtractorAttribute : ValueExtractorBaseAttribute
    {
        public override object BindValue(object value)
        {
            var typedValue = value as bool?;
            return typedValue.HasValue ? (typedValue.Value ? "yes" : "no" ): "-";
        }
    }

    public class NullableBoolInjectorAttribute : ValueInjectorBaseAttribute
    {
        public override object BindValue(object objValue)
        {
            var value = objValue as string;
            if (string.Compare(value, "-", true) == 0)
                return null;
            else if (string.Compare(value, "yes", true) == 0)
                return true;
            else if (string.Compare(value, "no", true) == 0)
                return false;
            else
                throw new InvalidOperationException($"failed to unformat {value}");
        }
    }

    //Add this attribute to a string property to tell the gui
    //that field must match a repository structure
    public class StructNameOptionsAttribute : ValueOptionsExtractorBaseAttribute
    {
        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context)
        {
            var unfiltered = context.CodeGenSolution.RPS.Structures.Select(str => (object)str.Name);
            if (parent is IContextWithFilter contextWithFilter)
            {
                return unfiltered.Where(itm => contextWithFilter.AllowItem(itm as string)).ToList();
            }
            else
                return unfiltered.ToList();
        }
    }

    public class GeneratorOptionsAttribute : ValueOptionsExtractorBaseAttribute
    {
        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context)
        {
            var dynamicItems = DynamicCodeGenerator.LoadDynamicGenerators(Path.Combine(context.SolutionDir, "Generators", "Enabled")).Result.Select(kvp => kvp.Key);
            var baseItems = new List<string> { "SignalRGenerator", "ODataGenerator", "ModelGenerator", "TraditionalBridgeGenerator", "EFCoreGenerator" };
            return dynamicItems.Concat(baseItems).Distinct().OfType<object>().ToList();
        }
    }

    public class StructKeyOptionsAttribute : ValueOptionsExtractorBaseAttribute
    {
        public StructKeyOptionsAttribute(string dynamicStructContext = null)
        {
            _dynamicStructContext = dynamicStructContext;
        }

        private string _dynamicStructContext;
        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context)
        {
            if (!string.IsNullOrWhiteSpace(_dynamicStructContext))
            {
                //the dynamic struct context must be a sibling property for our current type/value
                var targetProperty = property.DeclaringType.GetProperty(_dynamicStructContext);
                var targetStructName = targetProperty?.GetValue(parent) as string;
                if (string.IsNullOrWhiteSpace(targetStructName))
                    throw new Exception(string.Format("Invalid dynamic structure target for -> {0}", property.Name));
                var actualTargetStruct = context.CodeGenSolution.RPS.GetStructure(targetStructName);
                if (actualTargetStruct == null)
                    throw new Exception(string.Format("Invalid dynamic structure target was {0}", targetStructName));

                return actualTargetStruct.Keys.Select(key => (object)key.Name).ToList();
            }
            else if (parent is IContextWithStructure contextWithStructure)
            {
                return contextWithStructure.StructureContext.Keys.Select(key => (object)key.Name).ToList();
            }
            else
                throw new NotImplementedException();
        }
    }

    //Add this attribute to a string property to tell the gui
    //that field must match a repository structure field
    public class StructFieldNameOptionsAttribute : ValueOptionsExtractorBaseAttribute
    {
        public StructFieldNameOptionsAttribute(string dynamicStructContext = null)
        {
            _dynamicStructContext = dynamicStructContext;
        }

        private string _dynamicStructContext;

        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context)
        {
            if (!string.IsNullOrWhiteSpace(_dynamicStructContext))
            {
                //the dynamic struct context must be a sibling property for our current type/value
                var targetProperty = property.DeclaringType.GetProperty(_dynamicStructContext);
                var targetStructName = targetProperty?.GetValue(parent) as string;
                if (string.IsNullOrWhiteSpace(targetStructName))
                    throw new Exception(string.Format("Invalid dynamic structure target for -> {0}", property.Name));
                var actualTargetStruct = context.CodeGenSolution.RPS.GetStructure(targetStructName);
                if (actualTargetStruct == null)
                    throw new Exception(string.Format("Invalid dynamic structure target was {0}", targetStructName));

                return actualTargetStruct.Fields.Select(key => (object)key.Name).ToList();
            }
            else if (parent is IContextWithStructure contextWithStructure)
            {
                return contextWithStructure.StructureContext.Fields.Select(fld => (object)fld.Name).ToList();
            }
            else
                throw new NotImplementedException();
        }
    }

    //Add this attribute to a string property to tell the gui
    //that field must match an smc interface
    public class InterfaceNameOptionsAttribute : ValueOptionsExtractorBaseAttribute
    {
        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context) 
        {
            var unfiltered = context.CodeGenSolution.TraditionalBridge.Smc.Interfaces.Select(str => (object)str.Name);
            if (parent is IContextWithFilter contextWithFilter)
            {
                return unfiltered.Where(itm => contextWithFilter.AllowItem(itm as string)).ToList();
            }
            else
                return unfiltered.ToList();
        }
    }

    public class EnumOptionsAttribute : ValueOptionsExtractorBaseAttribute
    {
        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context)
        {
            if (!(property?.PropertyType?.IsEnum ?? false))
                throw new Exception("Cant apply enum options to a non enum type");

            return property.PropertyType.GetEnumNames().OfType<object>().ToList();
        }
    }

    public class StaticOptionsAttribute : ValueOptionsExtractorBaseAttribute
    {
        public StaticOptionsAttribute(string optionsString)
        {
            _options = optionsString.Split("|");
        }

        private string[] _options;

        public override List<object> BindValue(PropertyInfo property, object source, ISingleItemSettings parent, SolutionInfo context)
        {
            return _options.OfType<object>().ToList();
        }
    }
}
