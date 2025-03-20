using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityContainer
{
    /// <summary>
    /// Container for storing instances of the same Type or Interface
    /// </summary>
    public class InstanceContainer : IDisposable
    {
        private List<object> _instances = new List<object>();

        public void Add(object instance)
        {
            if (_instances.All(i => !i.Equals(instance)))
            {
                _instances.Add(instance);
            }
        }

        public void Remove(object instance)
        {
            _instances.Remove(instance);
        }

        public object GetInstance()
        {
            return _instances?.FirstOrDefault();
        }

        public List<object> GetInstances()
        {
            return _instances;
        }

        public void Dispose()
        {
            _instances.Clear();
        }
    }
}