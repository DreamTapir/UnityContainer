using System;
using UnityEngine;

namespace UnityContainer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class InjectAttribute : PropertyAttribute
    {
        public bool IsRequired { get; private set; } = true;

        public InjectAttribute(bool isRequired = true)
        {
            IsRequired = isRequired;
        }
    }
}