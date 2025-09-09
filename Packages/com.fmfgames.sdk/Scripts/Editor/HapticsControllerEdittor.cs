using BaseSDK.Controllers;
using UnityEditor;
using UnityEngine;

namespace BaseSDK.EditorScripts {
	[CustomEditor(typeof(HapticsController))]
	public class HapticsControllerEditor : Editor {
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			if (GUILayout.Button("Massage"))
				(target as HapticsController).Massage();
		}
	}
}