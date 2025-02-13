using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace UnityContainer
{
    public static class ObjectExtension
    {
        public static IEnumerable<FieldInfo> FieldsOfAttribute<T>(this object obj, BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
        {
            return FieldsOfAttribute<T>(obj.GetType(), flags);
        }

        public static IEnumerable<FieldInfo> FieldsOfAttribute<T>(this Type type, BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
        {
            return type.GetFields(flags).Where(f => Attribute.IsDefined(f, typeof(T)));
        }

        public static IEnumerable<MethodInfo> MethodsOfAttribute<T>(this object obj)
        {
            return MethodsOfAttribute<T>(obj.GetType());
        }

        public static IEnumerable<MethodInfo> MethodsOfAttribute<T>(this Type type)
        {
            return type.GetMethods().Where(m => Attribute.IsDefined(m, typeof(T)));
        }

        public static void Validate(this object obj, IContainer targetContainer)
        {
            // Fields
            foreach (var f in obj.FieldsOfAttribute<ValidateAttribute>())
            {
                var value = f.GetValue(obj);
                var attribute = f.GetCustomAttribute<ValidateAttribute>();
                var type = f.FieldType;

                if (value != null)
                    continue;

                value = targetContainer.Resolve(type);

                if (value == null && attribute.IsRequired == false)
                    continue;

                Debug.Assert(value != null, "[Field] Can not find " + f.Name + " in " + obj);
                f.SetValue(obj, value);
            }

            // Methods
            foreach (var m in obj.MethodsOfAttribute<ValidateAttribute>())
            {
                var attribute = m.GetCustomAttribute<ValidateAttribute>();
                var parameters = m.GetParameters();
                object[] values = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;

                    values[i] = targetContainer.Resolve(parameterType);

                    if (values[i] == null && attribute.IsRequired == false)
                        continue;

                    Debug.Assert(values[i] != null, "[Method] Can not find " + parameter.Name + " in " + obj);
                }

                m.Invoke(obj, values);
            }
        }
    }
}