using UnityEngine;

namespace UnityValidator.Samples
{
    public class ColorFloatProvider : ColorProvider, IFloatProvider
    {
        public float Value => _value;
        [SerializeField] private float _value;
    }
}
