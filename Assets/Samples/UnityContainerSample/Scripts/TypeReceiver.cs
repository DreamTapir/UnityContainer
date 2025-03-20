using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityContainer.Samples
{
    public class TypeReceiver : MonoBehaviour
    {
        [SerializeField] private List<IntProvider> _intProviders;
        [Inject] private FloatProvider _floatProvider;
        [Inject] private ColorProvider _colorProvider;

        [Inject]
        public void Inject(List<IntProvider> intProviders)
        {
            _intProviders = intProviders;

            Debug.Log($"[{gameObject.name}] IntProvider Count:{_intProviders.Count()}");
        }

        private void Start()
        {
            Debug.Log($"[{gameObject.name}] IFloatProvider.Value:{_floatProvider?.Value}");
            Debug.Log($"[{gameObject.name}] IColorProvider.Color:{_colorProvider?.Color}");
        }
    }
}