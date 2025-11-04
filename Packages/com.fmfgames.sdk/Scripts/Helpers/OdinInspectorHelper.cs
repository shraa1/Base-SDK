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

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
	public class InfoBoxAttribute : Attribute {
		public InfoBoxAttribute(string str, InfoMessageType infoMessageType = InfoMessageType.Info, string visibleIfMemberName = null) { }
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
	public class TitleAttribute : Attribute {
		public TitleAttribute(string title, string subtitle = null, TitleAlignments titleAlignment = TitleAlignments.Left, bool horizontalLine = true, bool bold = true) { }
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
	public class ToggleLeftAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
	public sealed class ShowIfAttribute : Attribute {
		public ShowIfAttribute(string condition, bool animate = true) { }
		public ShowIfAttribute(string condition, object optionalValue, bool animate = true) { }
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true), Conditional("UNITY_EDITOR")]
	public sealed class MinValueAttribute : Attribute {
		public MinValueAttribute(double minValue) { }
		public MinValueAttribute(string expression) { }
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true), Conditional("UNITY_EDITOR")]
	public sealed class OnValueChangedAttribute : Attribute {
		public string Action;
		public bool IncludeChildren;
		public bool InvokeOnUndoRedo = true;
		public bool InvokeOnInitialize;
		public OnValueChangedAttribute(string action, bool includeChildren = false) { }
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true), Conditional("UNITY_EDITOR")]
	public sealed class ReadOnlyAttribute : Attribute { }

	public enum TitleAlignments {
		Left, Centered, Right, Split
	}

	public enum InfoMessageType {
		None, Warning, Error, Info,
	}
}