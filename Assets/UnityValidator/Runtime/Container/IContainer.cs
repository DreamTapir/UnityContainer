using System;

namespace UnityValidator
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