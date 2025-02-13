using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityContainer
{
    /// <summary>
    /// Container for storing instances in Scene
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class SceneContainer : MonoBehaviour, IContainer
    {
        #region Private
        [SerializeField] private bool _includeInactive = false;
        [SerializeField] private bool _autoValidateOnAwake = true;

        private ConcurrentDictionary<Type, InstanceContainer> _typeContainerPairs = new();

        private IEnumerable<Component> GetComponentsInScene(bool includeInactive)
        {
            var scenePath = gameObject.scene.path;
#if UNITY_2022_2_OR_NEWER
            return FindObjectsByType<Component>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(c => c.gameObject.scene.path == scenePath);
#else
            return FindObjectsOfType<Component>(includeInactive).Where(c => c.gameObject.scene.path == scenePath);
#endif
        }

        private void RegisterAndValidateObjectsInScene()
        {
            var components = GetComponentsInScene(_includeInactive);
            foreach (var component in components.Where(c => c != this))
            {
                Register(component);
            }
            foreach (var component in components)
            {
                component.Validate(this);
            }
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            if (_autoValidateOnAwake)
            {
                RegisterAndValidateObjectsInScene();
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

        #region IContainer
        public void Register(object obj)
        {
            _typeContainerPairs.RegisterSelfAndBaseType(obj);
            _typeContainerPairs.RegisterInterface(obj);
        }

        public void Unregister(object obj)
        {
            if (_typeContainerPairs.ContainsKey(obj.GetType()))
            {
                _typeContainerPairs.TryRemove(obj.GetType(), out var instance);
                instance?.Remove(obj);
            }
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
    }
}