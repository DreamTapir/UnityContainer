using UnityEngine;

namespace UnityValidator.Samples
{
    public class ColorProvider : MonoBehaviour, IColorProvider 
    {
        public Color Color => _color;
        [SerializeField] private Color _color;
    }
}