using UnityEngine;

namespace UnityValidator.Samples
{
    public class IntProvider : MonoBehaviour, IIntProvider
    {
        public int Value => _value;
        [SerializeField] private int _value;
    }
}