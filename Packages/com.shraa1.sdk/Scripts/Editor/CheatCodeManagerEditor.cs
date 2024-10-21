using UnityEditor;

using UnityEngine;

namespace BaseSDK.EditorScripts {
	[CustomEditor(typeof(Controllers.CheatCodesManager))]
	public class CheatCodesManagerEditor : Editor {
		private SerializedProperty m_ActionProperty;
		private UnityEditor.PackageManager.Requests.AddRequest addPackage_Result;

		protected void OnEnable () {
			m_ActionProperty = serializedObject.FindProperty("allCheatCodes");
			EditorApplication.update += Update;
		}

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
#if ENABLE_INPUT_SYSTEM
			if (PlayerPrefs.HasKey("RemoveWarningsCCM") && GUILayout.Button("Remove warnings forever"))
				PlayerPrefs.SetInt("RemoveWarningsCCM", 1);

			if (PlayerPrefs.GetInt("RemoveWarningsCCM", 0) != 1) {
				EditorGUILayout.HelpBox("The CheatCode system works with cheats that are not subsets of each other. For example, if you have cheat codes \"BigB\" and \"BigBang\", then only BigB will get triggered, and not BigBang. You can modify the code to fit your needs if you'd like.", MessageType.Warning);
				EditorGUILayout.HelpBox("Multiple Input Devices can not make up 1 single cheat code, like Keyboard + Mouse. It can either be Keyboard or Mouse.", MessageType.Warning);
				EditorGUILayout.HelpBox("Only alphanumeric cheatcodes allowed for Keyboard.", MessageType.Warning);
				EditorGUILayout.HelpBox("There is no time limit for consecutive key presses. If the 1st character of the cheat was pressed at time 10, and 2nd character was pressed at 100, it would still consider it as the same attempt. You can add a timer and remove the items from attemptedCheats if you so wish", MessageType.Warning);
			}

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(m_ActionProperty);

			if (EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
#else
			EditorGUILayout.HelpBox("CheatCode System does not work with the legacy InputManager. Switch to the New InputSystem to use it", MessageType.Error);
			if (GUILayout.Button("Switch to new InputSystem"))
				PackageDependenciesHelper.ChangeInputSystemPlayerSettings();
#endif
		}
	}
}