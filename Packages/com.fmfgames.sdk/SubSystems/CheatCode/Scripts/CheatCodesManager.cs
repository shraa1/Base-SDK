using System;
using System.Collections.Generic;
using System.Linq;
using BaseSDK.Extension;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using BaseSDK.SirenixHelper;
#endif

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
		[InfoBox("The CheatCode system works with cheats that are not subsets of each other. For example, if you have cheat codes \"BigB\" and \"BigBang\", then only BigB will get triggered, and not BigBang.", InfoMessageType.Warning)]
		[InfoBox("There is no time limit for consecutive key presses. If the 1st character of the cheat was pressed at time 10, and 2nd character was pressed at 100, it would still consider it as the same attempt. You can add a timer and remove the items from attemptedCheats if you so wish.", InfoMessageType.Warning)]
		[SerializeField] private InputActionReference m_CheatCodesAction;

		/// <summary>
		/// All the cheat codes in the game, for all devices. Currently only supported
		/// for inspector, can be made public to be set up by code.
		/// </summary>
		[SerializeField] private List<CheatCodeData> m_AllCheatCodes = new();

		/// <summary>
		/// All attempted codes which are valid. For e.g, cheat "BigBigBigWin" would have "Big", "BigBig" and "BigBigBig" as valid attempts.
		/// Once the W for Win has been pressed, "Big" and "BigBig" will no longer be stored in this list.
		/// </summary>
		private readonly List<string> m_AttemptedCheats = new();
		#endregion

		#region Unity Methods
		/// <summary>
		/// Awake. Add Listeners
		/// </summary>
		protected virtual void Awake () {
			DontDestroy = true;

			m_CheatCodesAction.action.performed += CheatCodeKeyPressPerformed;
		}

		/// <summary>
		/// OnEnable. Activate the cheat code system.
		/// </summary>
		protected virtual void OnEnable () => m_CheatCodesAction.action.Enable();

		/// <summary>
		/// OnDisable. Deactivate the cheat code system.
		/// </summary>
		protected virtual void OnDisable () => m_CheatCodesAction.action.Disable();
		#endregion

		#region Helper Methods
		/// <summary>
		/// Cheatcode InputAction's KeyPress occurred. Operate here.
		/// Note: Instead of using InputSystem's Keyboard class's onTextInput event, using CallbackContext instead.
		/// This is because the Gamepad class does not have a similar event to hook to.
		/// If you plan to use only Keyboard cheatcodes, you can modify the method below and hook it up to Keyboard.onTextInput instead.
		/// </summary>
		/// <param name="context"></param>
		private void CheatCodeKeyPressPerformed (InputAction.CallbackContext context) {
			//If certain keys which have sensitive values, like sticks/shoulders on controllers, ignore them till they are completely triggered
			if (context.ReadValue<float>() < 1f)
				return;

			//IF YOU WANT TO CHECK THE KEYCODE NAMES THAT SHOULD BE USED FOR GAMEPADS/MOUSE, ETC., JUST UNCOMMENT THESE DEBUG.LOG AND CONCAT IT WHEN SETTING UP IN THE INSPECTOR
			//Debug.LogError(context.control.name);
			//CHECK DEVICE NAME HERE
			//Debug.LogError(context.control.device.layout);
			//Debug.LogError(InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, "Keyboard"));
			//Debug.LogError(InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, "Gamepad"));
			//Debug.LogError(InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, "Mouse"));

			//ALSO, THE SYSTEM USES THE CONTROL'S NAME DEFAULT, YOU CAN USE DISPLAYNAME INSTEAD IF YOU PREFER THAT. JUST CHANGE THE VALUE BEING ADDED TO ATTEMPTEDCHEATS HERE ->
			//Update the existing attempts
			m_AttemptedCheats.For(i => m_AttemptedCheats[i] += context.control.name);
			m_AttemptedCheats.Add(context.control.name);

			//Filter out the cheats for the specific input device
			var allCheatsForDevice = m_AllCheatCodes.FindAll(x => InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, x.deviceName))?.Select(x => x.cheatCode).ToList();

			//Cycle through all attempted cheats, to remove invalid ones
			for (var i = 0; i < m_AttemptedCheats.Count; i++) {
				//Was there a valid attempt?
				var used = false;
				//Used to verify if this attempt completed the cheat code.
				var completedAttempt = string.Empty;

				//Only verify against cheats from this device.
				for (var j = 0; j < allCheatsForDevice.Count; j++) {
					//Is a valid attempt, and the entire cheat code was completed.
					if (allCheatsForDevice[j].Equals(m_AttemptedCheats[i], StringComparison.OrdinalIgnoreCase)) {
						completedAttempt = allCheatsForDevice[j];
						used = true;
						break;
					}

					//Is a valid attempt, but the entire cheat code was not completed. More key presses are required to complete.
					if (allCheatsForDevice[j].StartsWith(m_AttemptedCheats[i], StringComparison.OrdinalIgnoreCase)) {
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
						m_AllCheatCodes.Find(x => x.cheatCode.Equals(completedAttempt, StringComparison.OrdinalIgnoreCase))?.onTriggeredAction?.Invoke();

						//Triggered the cheat code, so now remove it from the attempted cheats list
						m_AttemptedCheats.Remove(m_AttemptedCheats[i]);
						i--;
					}

					//If not completed, then just continue to the next attempted cheat code
					continue;
				}
				//Remove the attempt if it was invalid
				else {
					m_AttemptedCheats.Remove(m_AttemptedCheats[i]);
					i--;
				}
			}
		}
		#endregion
	}
}