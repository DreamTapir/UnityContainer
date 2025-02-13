using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityContainer.Samples
{
    public class InterfaceReceiver : MonoBehaviour
    {
        private IIntProvider _intProvider;
        private IFloatProvider _floatProvider;
        private IColorProvider _colorProvider;

        [Validate]
        public void Validate(IIntProvider[] intProviders, List<IFloatProvider> floatProviders, IEnumerable<IColorProvider> colorProviders)
        {
            _intProvider = intProviders.Where(i => i.Value > 10).FirstOrDefault();
            _floatProvider = floatProviders.Where(f => f.Value > 0).FirstOrDefault();
            _colorProvider = colorProviders.Where(c => c.Color.g == 1f).FirstOrDefault();

            Debug.Log($"[{gameObject.name}] IIntProvider Count:{intProviders.Length}, Selected Provider Value:{_intProvider?.Value}");
            Debug.Log($"[{gameObject.name}] IFloatProvider Count:{floatProviders.Count}, Selected Provider Value:{_floatProvider?.Value}");
            Debug.Log($"[{gameObject.name}] IColorProvider Count:{colorProviders.Count()}, Selected Provider Color:{_colorProvider?.Color}");
        }
    }
}