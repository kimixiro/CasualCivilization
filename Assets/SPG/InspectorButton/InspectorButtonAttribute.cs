using System;
using UnityEngine;

namespace SPG.InspectorButton
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public string[] MethodNames { get; private set; }

        public InspectorButtonAttribute(params string[] methodNames)
        {
            MethodNames = methodNames;
        }
    }
}