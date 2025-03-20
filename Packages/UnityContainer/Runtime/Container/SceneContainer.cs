using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityContainer
{
    /// <summary>
    /// Container for storing instances in Scene
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class SceneContainer : MonoBehaviour, IContainer
    {
        #region Private Fields
        [SerializeField] private bool _includeInactive = false;
        [SerializeField] private bool _autoInjectOnAwake = true;

        private ConcurrentDictionary<Type, InstanceContainer> _typeContainerPairs = new();
        #endregion

        #region IContainer
        public void Register(object obj)
        {
            _typeContainerPairs.RegisterSelfAndBaseType(obj);
            _typeContainerPairs.RegisterInterface(obj);
        }

        public object Resolve(Type type)
        {
            object obj;

            if (!_typeContainerPairs.TryGetInstance(type, out obj) &&
                !_typeContainerPairs.TryGetList(type, out obj) &&
                !_typeContainerPairs.TryGetArray(type, out obj))
            {
                Debug.LogError($"[{gameObject.name}] Not register " + type);
            }

            return obj;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
        #endregion

        #region Private Methods
        private IEnumerable<Component> GetComponentsInScene(bool includeInactive)
        {
            var scenePath = gameObject.scene.path;
#if UNITY_2022_2_OR_NEWER
            return FindObjectsByType<Component>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(c => c.gameObject.scene.path == scenePath);
#else
            return FindObjectsOfType<Component>(includeInactive).Where(c => c.gameObject.scene.path == scenePath);
#endif
        }

        private IEnumerable<FieldInfo> GetFieldsOfAttribute<T>(object obj, BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
        {
            return obj.GetType().GetFields(flags).Where(f => Attribute.IsDefined(f, typeof(T)));
        }

        private IEnumerable<MethodInfo> GetMethodsOfAttribute<T>(object obj, BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
        {
            return obj.GetType().GetMethods(flags).Where(m => Attribute.IsDefined(m, typeof(T)));
        }

        private void Inject(object obj)
        {
            // Inject Field
            foreach (var f in GetFieldsOfAttribute<InjectAttribute>(obj))
            {
                var value = f.GetValue(obj);
                var attribute = f.GetCustomAttribute<InjectAttribute>();
                var type = f.FieldType;

                if (value != null)
                    continue;

                value = Resolve(type);

                if (value == null && attribute.IsRequired == false)
                    continue;

                f.SetValue(obj, value);
            }

            // Inject Method
            foreach (var m in GetMethodsOfAttribute<InjectAttribute>(obj))
            {
                var attribute = m.GetCustomAttribute<InjectAttribute>();
                var parameters = m.GetParameters();
                object[] values = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterType = parameter.ParameterType;

                    values[i] = Resolve(parameterType);

                    if (values[i] == null && attribute.IsRequired == false)
                        continue;
                }

                m.Invoke(obj, values);
            }
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            if (_autoInjectOnAwake)
            {
                var components = GetComponentsInScene(_includeInactive);
                foreach (var component in components)
                {
                    Register(component);
                }
                foreach (var component in components)
                {
                    Inject(component);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (var pair in _typeContainerPairs)
            {
                pair.Value.Dispose();
            }
            _typeContainerPairs.Clear();
            _typeContainerPairs = null;
        }
        #endregion
    }
}