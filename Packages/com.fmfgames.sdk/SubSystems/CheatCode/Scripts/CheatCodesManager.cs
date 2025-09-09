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
			public string CheatCode;

			/// <summary>
			/// What device is this cheat for? Keyboard? Gamepad? Mouse?
			/// The name of the device must match the name of the device layout provided by the New InputSystem
			/// </summary>
			public string DeviceName;

			/// <summary>
			/// What action to perform when the cheat code is successfully triggered?
			/// </summary>
			public UnityEvent OnTriggeredAction;
		}
		#endregion

		#region Inspector Variables
		[SerializeField] private InputActionReference m_CheatCodesAction;

		/// <summary>
		/// All the cheat codes in the game, for all devices. Currently only supported
		/// for inspector, can be made public to be set up by code.
		/// </summary>
		[InfoBox("The CheatCode system works with cheats that are not subsets of each other. For example, if you have cheat codes \"BigB\" and \"BigBang\", then only BigB will get triggered, and not BigBang.", InfoMessageType.Warning)]
		[InfoBox("There is no time limit for consecutive key presses. If the 1st character of the cheat was pressed at time 10, and 2nd character was pressed at 100, it would still consider it as the same attempt. You can add a timer and remove the items from attemptedCheats if you so wish.", InfoMessageType.Warning)]
		[SerializeField] private List<CheatCodeData> m_AllCheatCodes = new();

		/// <summary>
		/// Should cheats expire after a short duration of no input?
		/// </summary>
		[Title("Options"), ToggleLeft, SerializeField] private bool m_UseTimeout = false;

		/// <summary>
		/// Time delay before inactivity will flush the entered cheats.
		/// </summary>
		[ShowIf(nameof(m_UseTimeout)), MinValue(0.1f), SerializeField] private float m_CheatTimeoutSeconds = 3f;
		#endregion Inspector Variables

		#region Variables & Constants
		/// <summary>
		/// All attempted codes which are valid. For e.g, cheat "BigBigBigWin" would have "Big", "BigBig" and "BigBigBig" as valid attempts.
		/// Once the W for Win has been pressed, "Big" and "BigBig" will no longer be stored in this list.
		/// </summary>
		private readonly List<string> m_AttemptedCheats = new();

		/// <summary>
		/// Time when the last key was pressed
		/// </summary>
		private float m_LastKeyTime;
		#endregion Variables & Constants

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

			var keyName = context.control.name;

			if (m_UseTimeout && Time.time - m_LastKeyTime > m_CheatTimeoutSeconds)
				m_AttemptedCheats.Clear();
			m_LastKeyTime = Time.time;

			//Update the existing attempts
			m_AttemptedCheats.For(i => m_AttemptedCheats[i] += keyName);
			m_AttemptedCheats.Add(keyName);

			//Filter out the cheats for the specific input device
			var allCheatsForDevice = m_AllCheatCodes.Where(x => InputSystem.IsFirstLayoutBasedOnSecond(context.control.device.layout, x.DeviceName)).ToList();

			//Cycle through all attempted cheats, to remove invalid ones
			for (var i = 0; i < m_AttemptedCheats.Count; i++) {
				var attempt = m_AttemptedCheats[i];
				var valid = false;

				foreach (var cheat in allCheatsForDevice) {
					if (cheat.CheatCode.Equals(attempt, StringComparison.OrdinalIgnoreCase)) {
						cheat.OnTriggeredAction?.Invoke();
						m_AttemptedCheats.RemoveAt(i);
						i--;
						valid = true;
						break;
					}

					if (cheat.CheatCode.StartsWith(attempt, StringComparison.OrdinalIgnoreCase)) {
						valid = true;
						break;
					}
				}

				if (!valid) {
					m_AttemptedCheats.RemoveAt(i);
					i--;
				}
			}
		}
		#endregion
	}
}