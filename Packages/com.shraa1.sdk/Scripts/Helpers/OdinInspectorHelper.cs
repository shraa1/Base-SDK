using System;
using System.Diagnostics;

namespace BaseSDK.SirenixHelper
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
    public class FoldoutGroupAttribute : Attribute
    {
        public FoldoutGroupAttribute(string groupName, bool expanded = false, float order = 0f) { }
    }
}