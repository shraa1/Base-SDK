namespace BaseSDK {
	/// <summary>
	/// Hides the property in the normal inspector, but can be seen in the debug view (unlike in HideInInspector, which hides the variable in both views)
	/// </summary>
	public class HideInNormalInspectorAttribute : UnityEngine.PropertyAttribute { }
}