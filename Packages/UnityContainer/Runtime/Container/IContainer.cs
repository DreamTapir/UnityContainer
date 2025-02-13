using System;

namespace UnityContainer
{
    public interface IContainer
    {
        void Register(object obj);
        void Unregister(object obj);
        object Resolve(Type type);

        T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
    }
}