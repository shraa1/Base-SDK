//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using BaseSDK.Extension;
using UnityEngine.Events;
using UnityEngine;
using IS = UnityEngine.InputSystem.InputSystem;
using UnityEditor;

namespace BaseSDK.Controllers {
	/// <summary>
	/// Manages all cheat code related functionality.
	/// Cheats can be set up in the inspector.
	/// </summary>
	public class CheatCodesManager : Singleton<CheatCodesManager> {

		#region Custom DataTypes
		/// <summary>
		/// Data required for any cheat codes
		/// </summary>
		[Serializable]
		public class CheatCodeData {
			/// <summary>
			/// The cheat code itself. Press the keys on the device in the correct order to trigger the cheat
			/// </summary>
			public string cheatCode;
			/// <summary>
			/// What device is this cheat for? Keyboard? Gamepad? Mouse?
			/// The name of the device must match the name of the device layout provided by the New InputSystem
			/// </summary>
			public string deviceName;
			/// <summary>
			/// What action to perform when the cheat code is successfully triggered?
			/// </summary>
			public UnityEvent onTriggeredAction;
		}
		#endregion

		#region Variables & Constants
#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// All the cheat codes in the game, for all devices. Currently only supported
		/// for inspector, can be made public to be set up by code.
		/// </summary>
		[SerializeField] private List<CheatCodeData> allCheatCodes = new List<CheatCodeData>();

		/// <summary>
		/// All attempted codes which are valid. For e.g, cheat "BigBigBigWin" would have "Big", "BigBig" and "BigBigBig" as valid attempts.
		/// Once the W for Win has been pressed, "Big" and "BigBig" will no longer be stored in this list.
		/// </summary>
		private List<string> attemptedCheats = new List<string>();
#endif
		#endregion

		#region Unity Methods
		/// <summary>
		/// Awake. Add Listeners
		/// </summary>
		protected virtual void Awake() {
			DontDestroy = true;

#if ENABLE_INPUT_SYSTEM
			InputMasterController.InputMaster.CheatCodes.KeyPress.performed += CheatCodeKeyPressPerformed;
#else
			Debug.LogWarning("CheatCodes currently only work with the new InputSystem, not the legacy InputManager");
#endif
		}

#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// OnEnable. Activate the cheat code system.
		/// </summary>
		protected virtual void OnEnable() => InputMasterController.InputMaster.CheatCodes.Enable();

		/// <summary>
		/// OnDisable. Deactivate the cheat code system.
		/// </summary>
		protected virtual void OnDisable() => InputMasterController.InputMaster.CheatCodes.Disable();
#endif
		#endregion

		#region Helper Methods
#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// Cheatcode InputAction's KeyPress occurred. Operate here.
		/// Note: Instead of using InputSystem's Keyboard class's onTextInput event, using CallbackContext instead.
		/// This is because the Gamepad class does not have a similar event to hook to.
		/// If you plan to use only Keyboard cheatcodes, you can modify the method below and hook it up to Keyboard.onTextInput instead.
		/// </summary>
		/// <param name="context"></param>
		private void CheatCodeKeyPressPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
			//If certain keys which have sensitive values, like sticks/shoulders on controllers, ignore them till they are completely triggered
			if (context.ReadValue<float>() < 1f)
				return;

			//IF YOU WANT TO CHECK THE KEYCODE NAMES THAT SHOULD BE USED FOR GAMEPADS/MOUSE, ETC., JUST UNCOMMENT THESE DEBUG.LOG AND CONCAT IT WHEN SETTING UP IN THE INSPECTOR
			//Debug.LogError(context.control.name);
			//CHECK DEVICE NAME HERE
			//Debug.LogError(context.control.device.layout);
			//Debug.LogError(UnityEngine.InputSystem.InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, "Keyboard"));
			//Debug.LogError(UnityEngine.InputSystem.InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, "Gamepad"));
			//Debug.LogError(UnityEngine.InputSystem.InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, "Mouse"));

			//ALSO, THE SYSTEM USES THE CONTROL'S NAME DEFAULT, YOU CAN USE DISPLAYNAME INSTEAD IF YOU PREFER THAT. JUST CHANGE THE VALUE BEING ADDED TO ATTEMPTEDCHEATS HERE ->
			//Update the existing attempts
			attemptedCheats.For(i => attemptedCheats[i] += context.control.name);
			attemptedCheats.Add(context.control.name);

			//Filter out the cheats for the specific input device
			var allCheatsForDevice = allCheatCodes.FindAll(x => IS.IsFirstLayoutBasedOnSecond(context.control.device.layout, x.deviceName))?.Select(x => x.cheatCode).ToList();

			//Cycle through all attempted cheats, to remove invalid ones
			for (var i = 0; i < attemptedCheats.Count; i++) {
				//Was there a valid attempt?
				var used = false;
				//Used to verify if this attempt completed the cheat code.
				var completedAttempt = string.Empty;

				//Only verify against cheats from this device.
				for (var j = 0; j < allCheatsForDevice.Count; j++) {
					//Is a valid attempt, and the entire cheat code was completed.
					if (allCheatsForDevice[j].Equals(attemptedCheats[i], StringComparison.OrdinalIgnoreCase)) {
						completedAttempt = allCheatsForDevice[j];
						used = true;
						break;
					}

					//Is a valid attempt, but the entire cheat code was not completed. More key presses are required to complete.
					if (allCheatsForDevice[j].StartsWith(attemptedCheats[i], StringComparison.OrdinalIgnoreCase)) {
						used = true;
						break;
					}
				}

				//There a valid attempt.
				if (used) {
					//Fire and remove the cheat code from attempted list only if it was completed
					//Check the demo scene -> LeftRightLeftRightLeftRight cheatcode for Mouse, will trigger again when LeftRight is pressed for the 4th time.
					//To avoid this, inside the if condition, clear the list and break the for loop. But this is not the experience intended, so keeping it here like this ;)
					if (!completedAttempt.IsNullOrEmpty()) {
						//Trigger the Action for the completed cheat code
						allCheatCodes.Find(x => x.cheatCode.Equals(completedAttempt, StringComparison.OrdinalIgnoreCase))?.onTriggeredAction?.Invoke();

						//Triggered the cheat code, so now remove it from the attempted cheats list
						attemptedCheats.Remove(attemptedCheats[i]);
						i--;
					}

					//If not completed, then just continue to the next attempted cheat code
					continue;
				}
				//Remove the attempt if it was invalid
				else {
					attemptedCheats.Remove(attemptedCheats[i]);
					i--;
				}
			}
		}
#endif
		#endregion
	}
}


#if UNITY_EDITOR
namespace BaseSDK.EditorScripts {
	[CustomEditor(typeof(Controllers.CheatCodesManager))]
	public class CheatCodesManagerEditor : Editor {
		private SerializedProperty m_ActionProperty;
		private UnityEditor.PackageManager.Requests.AddRequest addPackage_Result;

		protected void OnEnable() {
			m_ActionProperty = serializedObject.FindProperty("allCheatCodes");
			EditorApplication.update += Update;
		}

		private void Update() {
			if (addPackage_Result != null && addPackage_Result.IsCompleted && addPackage_Result.Status == UnityEditor.PackageManager.StatusCode.Success) {
				addPackage_Result = null;
				PackageDependenciesHelper.ChangeInputSystemPlayerSettings();
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("Restart", "The Input settings for the game will be modified to use the new Input System. Unity will be restarted to use the updated settings without any issues. If you have any scene changes which are unsaved, you will get a chance to save them.", "Okay");

				EditorApplication.OpenProject(System.IO.Directory.GetCurrentDirectory());
			}
		}

		public override void OnInspectorGUI() {
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
#endif