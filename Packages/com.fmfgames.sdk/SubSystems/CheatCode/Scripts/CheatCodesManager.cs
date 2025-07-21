using System;
using System.Collections.Generic;
using System.Linq;

using BaseSDK.Extension;

using UnityEngine;
using UnityEngine.Events;

using IS = UnityEngine.InputSystem.InputSystem;

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
		protected virtual void Awake () {
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
		protected virtual void OnEnable () => InputMasterController.InputMaster.CheatCodes.Enable();

		/// <summary>
		/// OnDisable. Deactivate the cheat code system.
		/// </summary>
		protected virtual void OnDisable () => InputMasterController.InputMaster.CheatCodes.Disable();
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
		private void CheatCodeKeyPressPerformed (UnityEngine.InputSystem.InputAction.CallbackContext context) {
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