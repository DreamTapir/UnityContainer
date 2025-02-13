using UnityEngine;

namespace UnityContainer.Samples
{
    public class FloatProvider : MonoBehaviour, IFloatProvider
    {
        public float Value => _value;
        [SerializeField] private float _value;
    }
}