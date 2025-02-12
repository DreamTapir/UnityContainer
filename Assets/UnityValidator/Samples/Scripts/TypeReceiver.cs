using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityValidator.Samples
{
    public class TypeReceiver : MonoBehaviour
    {
        [SerializeField] private List<IntProvider> _intProviders;
        [Validate] private FloatProvider _floatProvider;
        [Validate] private ColorProvider _colorProvider;

        [Validate]
        public void Validate(List<IntProvider> intProviders)
        {
            _intProviders = intProviders;

            Debug.Log($"[{gameObject.name}] IntProvider Count:{intProviders.Count()}");
        }

        private void Start()
        {
            Debug.Log($"[{gameObject.name}] IFloatProvider.Value:{_floatProvider?.Value}");
            Debug.Log($"[{gameObject.name}] IColorProvider.Color:{_colorProvider?.Color}");
        }
    }
}