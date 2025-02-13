using UnityEngine;

namespace UnityContainer.Samples
{
    public class IntProvider : MonoBehaviour, IIntProvider
    {
        public int Value => _value;
        [SerializeField] private int _value;
    }
}