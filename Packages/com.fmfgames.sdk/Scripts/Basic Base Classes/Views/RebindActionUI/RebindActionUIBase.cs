using UnityEngine;

namespace BaseSDK.Settings.Rebind {
	/// <summary>
	/// A reusable component with a self-contained UI for rebinding a single action.
	/// </summary>
	public abstract class RebindActionUIBase : MonoBehaviour {
		public abstract void UpdateBindingDisplay();
		public abstract void ResetToDefault();
	}
}