using BaseSDK.Controllers;

using UnityEditor;

using UnityEngine;

namespace BaseSDK.EditorScripts {
	[CustomEditor(typeof(HapticsController))]
	public class HapticsControllerEditor : Editor {
		private UnityEditor.PackageManager.Requests.AddRequest addPackage_Result;

		protected void OnEnable () => EditorApplication.update += Update;

		private void Update () {
			if (addPackage_Result != null && addPackage_Result.IsCompleted && addPackage_Result.Status == UnityEditor.PackageManager.StatusCode.Success) {
				addPackage_Result = null;
				PackageDependenciesHelper.ChangeInputSystemPlayerSettings();
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("Restart", "The Input settings for the game will be modified to use the new Input System. Unity will be restarted to use the updated settings without any issues. If you have any scene changes which are unsaved, you will get a chance to save them.", "Okay");

				EditorApplication.OpenProject(System.IO.Directory.GetCurrentDirectory());
			}
		}

		public override void OnInspectorGUI () {
			base.OnInspectorGUI();

#if ENABLE_INPUT_SYSTEM
			if (GUILayout.Button("Massage"))
				(target as HapticsController).Massage();
#else
			EditorGUILayout.HelpBox("Haptics System does not work with the legacy InputManager. Switch to the New InputSystem to use it", MessageType.Error);
			if (GUILayout.Button("Switch to new InputSystem"))
				PackageDependenciesHelper.ChangeInputSystemPlayerSettings();
#endif
		}
	}
}