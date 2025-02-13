using UnityEngine;

namespace UnityContainer.Samples
{
    public interface IIntProvider
    {
        int Value { get; }
    }

    public interface IFloatProvider
    {
        float Value { get; }
    }

    public interface IColorProvider
    {
        Color Color { get; }
    }
}