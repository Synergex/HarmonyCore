using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace Harmony.Core.EF.Extensions.Internal
{
    static class TypeHelper
    {
        private static readonly Dictionary<Type, object> _commonTypeDictionary = new Dictionary<Type, object>
        {
            {
                typeof(int),
                (object)0
            },
            {
                typeof(Guid),
                (object)default(Guid)
            },
            {
                typeof(DateTime),
                (object)default(DateTime)
            },
            {
                typeof(DateTimeOffset),
                (object)default(DateTimeOffset)
            },
            {
                typeof(long),
                (object)0L
            },
            {
                typeof(bool),
                (object)false
            },
            {
                typeof(double),
                (object)0.0
            },
            {
                typeof(short),
                (object)(short)0
            },
            {
                typeof(float),
                (object)0f
            },
            {
                typeof(byte),
                (object)(byte)0
            },
            {
                typeof(char),
                (object)'\0'
            },
            {
                typeof(uint),
                (object)0u
            },
            {
                typeof(ushort),
                (object)(ushort)0
            },
            {
                typeof(ulong),
                (object)0uL
            },
            {
                typeof(sbyte),
                (object)(sbyte)0
            }
        };

        public static Type UnwrapNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        public static bool IsNullableType(this Type type)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (typeInfo.IsValueType)
            {
                if (typeInfo.IsGenericType)
                {
                    return typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
                }
                return false;
            }
            return true;
        }

        public static bool IsValidEntityType(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }

        public static Type MakeNullable(this Type type)
        {
            if (!type.IsNullableType())
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            return type;
        }

        public static bool IsInteger(this Type type)
        {
            type = type.UnwrapNullableType();
            if (!(type == typeof(int)) && !(type == typeof(long)) && !(type == typeof(short)) && !(type == typeof(byte)) && !(type == typeof(uint)) && !(type == typeof(ulong)) && !(type == typeof(ushort)) && !(type == typeof(sbyte)))
            {
                return type == typeof(char);
            }
            return true;
        }

        public static PropertyInfo GetAnyProperty(this Type type, string name)
        {
            List<PropertyInfo> list = (from p in type.GetRuntimeProperties()
                                       where p.Name == name
                                       select p).ToList();
            if (list.Count > 1)
            {
                throw new AmbiguousMatchException();
            }
            return list.SingleOrDefault();
        }

        public static bool IsInstantiable(this Type type)
        {
            return TypeHelper.IsInstantiable(type.GetTypeInfo());
        }

        private static bool IsInstantiable(TypeInfo type)
        {
            if (!type.IsAbstract && !type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    return !type.IsGenericTypeDefinition;
                }
                return true;
            }
            return false;
        }

        public static Type UnwrapEnumType(this Type type)
        {
            bool flag = type.IsNullableType();
            Type type2 = flag ? type.UnwrapNullableType() : type;
            if (!type2.GetTypeInfo().IsEnum)
            {
                return type;
            }
            Type underlyingType = Enum.GetUnderlyingType(type2);
            if (!flag)
            {
                return underlyingType;
            }
            return underlyingType.MakeNullable();
        }

        public static Type GetSequenceType(this Type type)
        {
            Type type2 = type.TryGetSequenceType();
            if (type2 == (Type)null)
            {
                throw new ArgumentException();
            }
            return type2;
        }

        public static Type TryGetSequenceType(this Type type)
        {
            return type.TryGetElementType(typeof(IEnumerable<>)) ?? type.TryGetElementType(typeof(IAsyncEnumerable<>));
        }

        public static Type ForceSequenceType(this Type type)
        {
            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GenericTypeArguments.First();


            var implementsIEnumerable = type.GetInterfaces().FirstOrDefault(inter => inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (implementsIEnumerable != null)
                return implementsIEnumerable.GenericTypeArguments.First();
            else
                return type;
        }

        public static Type TryGetElementType(this Type type, Type interfaceOrBaseType)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                return null;
            }
            IEnumerable<Type> genericTypeImplementations = type.GetGenericTypeImplementations(interfaceOrBaseType);
            Type type2 = null;
            foreach (Type item in genericTypeImplementations)
            {
                if (type2 == (Type)null)
                {
                    type2 = item;
                    continue;
                }
                type2 = null;
                break;
            }
            if ((object)type2 == null)
            {
                return null;
            }
            return type2.GetTypeInfo().GenericTypeArguments.FirstOrDefault();
        }

        public static IEnumerable<Type> GetGenericTypeImplementations(this Type type, Type interfaceOrBaseType)
        {
            TypeInfo typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericTypeDefinition)
            {
                IEnumerable<Type> enumerable = interfaceOrBaseType.GetTypeInfo().IsInterface ? typeInfo.ImplementedInterfaces : type.GetBaseTypes();
                foreach (Type item in enumerable)
                {
                    if (item.GetTypeInfo().IsGenericType && item.GetGenericTypeDefinition() == interfaceOrBaseType)
                    {
                        yield return item;
                    }
                }
                if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == interfaceOrBaseType)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            type = type.GetTypeInfo().BaseType;
            while (type != (Type)null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        public static IEnumerable<Type> GetTypesInHierarchy(this Type type)
        {
            while (type != (Type)null)
            {
                yield return type;
                type = type.GetTypeInfo().BaseType;
            }
        }

        public static ConstructorInfo GetDeclaredConstructor(this Type type, Type[] types)
        {
            types = (types ?? Array.Empty<Type>());
            return type.GetTypeInfo().DeclaredConstructors.SingleOrDefault(delegate (ConstructorInfo c)
            {
                if (!c.IsStatic)
                {
                    return (from p in c.GetParameters()
                            select p.ParameterType).SequenceEqual(types);
                }
                return false;
            });
        }

        public static IEnumerable<PropertyInfo> GetPropertiesInHierarchy(this Type type, string name)
        {
            do
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                PropertyInfo declaredProperty = typeInfo.GetDeclaredProperty(name);
                if (declaredProperty != (PropertyInfo)null && !(declaredProperty.GetMethod ?? declaredProperty.SetMethod).IsStatic)
                {
                    yield return declaredProperty;
                }
                type = typeInfo.BaseType;
            }
            while (type != (Type)null);
        }

        public static IEnumerable<MemberInfo> GetMembersInHierarchy(this Type type)
        {
            do
            {
                foreach (PropertyInfo item in from pi in type.GetRuntimeProperties()
                                              where !(pi.GetMethod ?? pi.SetMethod).IsStatic
                                              select pi)
                {
                    yield return (MemberInfo)item;
                }
                foreach (FieldInfo item2 in from f in type.GetRuntimeFields()
                                            where !f.IsStatic
                                            select f)
                {
                    yield return (MemberInfo)item2;
                }
                type = type.BaseType;
            }
            while (type != (Type)null);
        }

        public static IEnumerable<MemberInfo> GetMembersInHierarchy(this Type type, string name)
        {
            return from m in type.GetMembersInHierarchy()
                   where m.Name == name
                   select m;
        }

        public static object GetDefaultValue(this Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
            {
                return null;
            }
            object result = default(object);
            if (!TypeHelper._commonTypeDictionary.TryGetValue(type, out result))
            {
                return Activator.CreateInstance(type);
            }
            return result;
        }

        public static IEnumerable<TypeInfo> GetConstructibleTypes(this Assembly assembly)
        {
            return assembly.GetLoadableDefinedTypes().Where(delegate (TypeInfo t)
            {
                if (!t.IsAbstract)
                {
                    return !t.IsGenericTypeDefinition;
                }
                return false;
            });
        }

        public static IEnumerable<TypeInfo> GetLoadableDefinedTypes(this Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                return (from t in ex.Types
                        where t != (Type)null
                        select t).Select(IntrospectionExtensions.GetTypeInfo);
            }
        }

        public static ConstantExpression GetDefaultValueConstant(this Type type)
            => (ConstantExpression)_generateDefaultValueConstantMethod
                .MakeGenericMethod(type).Invoke(null, Array.Empty<object>())!;

        private static readonly MethodInfo _generateDefaultValueConstantMethod =
            typeof(TypeHelper).GetTypeInfo().GetDeclaredMethod(nameof(GenerateDefaultValueConstant))!;

        private static ConstantExpression GenerateDefaultValueConstant<TDefault>()
            => Expression.Constant(default(TDefault), typeof(TDefault));
    }

    internal static class ExpressionExtensions
    {
        public static LambdaExpression UnwrapLambdaFromQuote(this Expression expression)
        {
            UnaryExpression unaryExpression = expression as UnaryExpression;
            return (LambdaExpression)((unaryExpression != null && expression.NodeType == ExpressionType.Quote) ? unaryExpression.Operand : expression);
        }

        public static bool IsNullConstantExpression(this Expression expression)
            => ExpressionExtensions.RemoveConvert(expression) is ConstantExpression constantExpression
                && constantExpression.Value == null;

        private static Expression RemoveConvert(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression
                && (expression.NodeType == ExpressionType.Convert
                    || expression.NodeType == ExpressionType.ConvertChecked))
            {
                return RemoveConvert(unaryExpression.Operand);
            }

            return expression;
        }

        public static Expression UnwrapTypeConversion(this Expression expression, out Type convertedType)
        {
            convertedType = null;
            while (true)
            {
                UnaryExpression unaryExpression = expression as UnaryExpression;
                if (unaryExpression == null || unaryExpression.NodeType != ExpressionType.Convert)
                {
                    break;
                }
                expression = unaryExpression.Operand;
                if (unaryExpression.Type != typeof(object) && !unaryExpression.Type.IsAssignableFrom(expression.Type))
                {
                    convertedType = unaryExpression.Type;
                }
            }
            return expression;
        }
    }
}
