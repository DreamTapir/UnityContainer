using UnityEngine;

namespace UnityContainer.Samples
{
    public class ColorFloatProvider : ColorProvider, IFloatProvider
    {
        public float Value => _value;
        [SerializeField] private float _value;
    }
}
