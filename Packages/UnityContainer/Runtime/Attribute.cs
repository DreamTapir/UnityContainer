using System;
using UnityEngine;

namespace UnityContainer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public class ValidateAttribute : PropertyAttribute
    {
        public bool IsRequired { get; private set; } = true;

        public ValidateAttribute(bool isRequired = true)
        {
            IsRequired = isRequired;
        }
    }
}