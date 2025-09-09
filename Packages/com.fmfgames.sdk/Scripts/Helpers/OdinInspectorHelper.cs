using System;
using System.Diagnostics;

namespace BaseSDK.SirenixHelper {
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
    public class FoldoutGroupAttribute : Attribute {
        public FoldoutGroupAttribute(string groupName, bool expanded = false, float order = 0f) { }
    }

    [AttributeUsage(AttributeTargets.All), Conditional("UNITY_EDITOR")]
    public class DrawWithUnityAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true), Conditional("UNITY_EDITOR")]
    public class PreviewFieldAttribute : Attribute {
        public int Height;
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
    public class HorizontalGroupAttribute : Attribute {
        public HorizontalGroupAttribute(float width = 0f, int marginLeft = 0, int marginRight = 0, float order = 0f) { }
        public HorizontalGroupAttribute(string group, float width = 0f, int marginLeft = 0, int marginRight = 0, float order = 0f) { }
    }

    [AttributeUsage(AttributeTargets.All), Conditional("UNITY_EDITOR")]
    public class HideLabelAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.All), Conditional("UNITY_EDITOR")]
	public class ShowInInspectorAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class InfoBoxAttribute : Attribute {
        public InfoBoxAttribute(string str, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIfMemberName = null) { }
    }

    public enum InfoMessageType {
        None, Warning, Error, Info,
    }
}