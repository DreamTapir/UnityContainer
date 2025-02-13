using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityContainer
{
    [DefaultExecutionOrder(-9999)]
    public class SceneContainer : MonoBehaviour, IContainer
    {
        #region Private
        [SerializeField] private bool _includeInactive = false;
        [SerializeField] private bool _autoValidateOnAwake = true;

        private ConcurrentDictionary<Type, Instance> _typeInstancePairs = new();

        private IEnumerable<Component> GetComponentsInScene(bool includeInactive)
        {
            var scenePath = gameObject.scene.path;
#if UNITY_2022_2_OR_NEWER
            return FindObjectsByType<Component>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(c => c.gameObject.scene.path == scenePath);
#else
            return FindObjectsOfType<Component>(includeInactive).Where(c => c.gameObject.scene.path == scenePath);
#endif
        }

        private void RegisterAndValidateInSceneObjects()
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
                RegisterAndValidateInSceneObjects();
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (var pair in _typeInstancePairs)
            {
                pair.Value.Dispose();
            }
            _typeInstancePairs.Clear();
            _typeInstancePairs = null;
        }
        #endregion

        #region IContainer
        public void Register(object obj)
        {
            _typeInstancePairs.RegisterSelfAndBaseType(obj);
            _typeInstancePairs.RegisterInterface(obj);
        }

        public void Unregister(object obj)
        {
            if (_typeInstancePairs.ContainsKey(obj.GetType()))
            {
                _typeInstancePairs.TryRemove(obj.GetType(), out var instance);
                instance?.Remove(obj);
            }
        }

        public object Resolve(Type type)
        {
            object obj = null;

            if (_typeInstancePairs.ContainsKey(type))
            {
                obj = _typeInstancePairs[type].GetInstance();
            }

            if (obj == null)
            {
                _typeInstancePairs.TryGetList(type, out obj);
            }

            if (obj == null)
            {
                _typeInstancePairs.TryGetArray(type, out obj);
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