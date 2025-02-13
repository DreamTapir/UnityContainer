using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace UnityContainer
{
    public static class ContainerExtension
    {
        #region Register
        public static void RegisterSelfAndBaseType(this ConcurrentDictionary<Type, InstanceContainer> typeContainerPairs, object self)
        {
            var type = self.GetType();
            while (type != null)
            {
                typeContainerPairs.TryGetValue(type, out var instance);
                if (instance == null)
                {
                    instance = new InstanceContainer();
                }
                instance.Add(self);
                typeContainerPairs[type] = instance;
                type = type.BaseType;
            }
        }

        public static void RegisterInterface(this ConcurrentDictionary<Type, InstanceContainer> typeContainerPairs, object self)
        {
            foreach (var type in self.GetType().GetInterfaces())
            {
                typeContainerPairs.TryGetValue(type, out var instance);
                if (instance == null)
                {
                    instance = new InstanceContainer();
                }
                instance.Add(self);
                typeContainerPairs[type] = instance;
            }
        }
        #endregion

        #region Resolve
        public static bool TryGetInstance(this ConcurrentDictionary<Type, InstanceContainer> typeContainerPairs, Type type, out object instance)
        {
            instance = null;
            if (typeContainerPairs.ContainsKey(type))
            {
                instance = typeContainerPairs[type].GetInstance();
            }
            return instance != null;
        }

        public static bool TryGetList(this ConcurrentDictionary<Type, InstanceContainer> typeContainerPairs, Type type, out object instance)
        {
            instance = null;
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>) || type.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                var itemType = type.GenericTypeArguments[0];

                if (typeContainerPairs.ContainsKey(itemType))
                {
                    var listType = typeof(List<>).MakeGenericType(itemType);
                    var list = Activator.CreateInstance(listType);

                    foreach (var item in typeContainerPairs[itemType].GetInstances())
                    {
                        listType.GetMethod("Add")?.Invoke(list, new[] { item });
                    }

                    instance = list;
                }
            }

            return instance != null;
        }

        public static bool TryGetArray(this ConcurrentDictionary<Type, InstanceContainer> typeContainerPairs, Type type, out object instance)
        {
            instance = null;
            if (type.IsArray)
            {
                var elementType = type.GetElementType();

                if (typeContainerPairs.ContainsKey(elementType))
                {
                    var instances = typeContainerPairs[elementType].GetInstances();
                    var array = Array.CreateInstance(elementType, instances.Count);

                    for (var i = 0; i < instances.Count; i++)
                    {
                        array.SetValue(instances[i], i);
                    }
                    instance = array;
                }
            }

            return instance != null;
        }
        #endregion
    }
}