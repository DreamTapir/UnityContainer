using UnityEngine;

namespace UnityContainer.Samples
{
    public class ColorProvider : MonoBehaviour, IColorProvider 
    {
        public Color Color => _color;
        [SerializeField] private Color _color;
    }
}