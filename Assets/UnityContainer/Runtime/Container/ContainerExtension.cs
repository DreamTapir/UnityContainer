using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace UnityContainer
{
    public static class ContainerExtension
    {
        #region Register
        public static void RegisterSelfAndBaseType(this ConcurrentDictionary<Type, Instance> typeInstancePairs, object self)
        {
            var type = self.GetType();
            while (type != null)
            {
                typeInstancePairs.TryGetValue(type, out var instance);
                if (instance == null)
                {
                    instance = new Instance();
                }
                instance.Add(self);
                typeInstancePairs[type] = instance;
                type = type.BaseType;
            }
        }

        public static void RegisterInterface(this ConcurrentDictionary<Type, Instance> typeInstancePairs, object self)
        {
            foreach (var type in self.GetType().GetInterfaces())
            {
                typeInstancePairs.TryGetValue(type, out var instance);
                if (instance == null)
                {
                    instance = new Instance();
                }
                instance.Add(self);
                typeInstancePairs[type] = instance;
            }
        }
        #endregion

        #region Resolve
        public static bool TryGetList(this ConcurrentDictionary<Type, Instance> typeInstancePairs, Type type, out object obj)
        {
            obj = null;
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var itemType = type.GenericTypeArguments[0];

                if (typeInstancePairs.ContainsKey(itemType))
                {
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var list = Activator.CreateInstance(listType);

                    foreach (var item in typeInstancePairs[itemType].GetInstances())
                    {
                        listType.GetMethod("Add")?.Invoke(list, new[] { item });
                    }

                    obj = list;
                }
            }

            return obj != null;
        }

        public static bool TryGetArray(this ConcurrentDictionary<Type, Instance> typeInstancePairs, Type type, out object obj)
        {
            obj = null;
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (typeInstancePairs.ContainsKey(elementType))
                {
                    var instances = typeInstancePairs[elementType].GetInstances();
                    var array = Array.CreateInstance(elementType, instances.Count);

                    for (var i = 0; i < instances.Count; i++)
                    {
                        array.SetValue(instances[i], i);
                    }
                    obj = array;
                }
            }

            return obj != null;
        }
        #endregion
    }
}