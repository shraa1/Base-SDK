using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;

namespace BaseSDK.Controllers {
	/// <summary>
	/// Easy way to manage haptics/vibrations of your gamepads
	/// </summary>
	public class HapticsController : Singleton<HapticsController> {
		#region Custom DataTypes
		/// <summary>
		/// Class to store necessary data for haptics
		/// </summary>
		[System.Serializable]
		public class HapticsControllerData {
			public float str;
			public float startTime;
			public float endTime;
			public bool cancelAllPreviousVibrations;
		}
		#endregion

		#region Variables & Constants
		/// <summary>
		/// Stored list of haptics data.
		/// </summary>
		private List<HapticsControllerData> hapticControllerDatas = new List<HapticsControllerData>();
		#endregion

		#region Unity Methods
		/// <summary>
		/// Awake. Dont Destroy this gameObject.
		/// </summary>
		protected virtual void Awake () {
			DontDestroy = true;
		}
		#endregion

		#region Helper Methods
		//Remove the #if wrapper if you want this in the game. Not sure why, but why not ;)
#if UNITY_EDITOR
		/// <summary>
		/// Place the controller in your hand or your back and use it as a massager ;)
		/// </summary>
		public void Massage () => Vibrate(float.MaxValue, 1f, false);
#endif

		/// <summary>
		/// Start the operation for vibration of the current Gamepad
		/// </summary>
		/// <param name="duration">How long should the vibration stay active?</param>
		/// <param name="strength">How strong should the vibration be? Ranges from 0 to 1</param>
		/// <param name="cancelAllPreviousVibrations">If there was a previously long vibration already active, and it a new shorter one is needed to be played, should the old vibration continue to play after the new one? Or should all vibrations stop when the new one has finished playing? If everything should stop, use true, else use false</param>
		public void Vibrate (float duration, float strength, bool cancelAllPreviousVibrations = false) {
#if !ENABLE_INPUT_SYSTEM
			Debug.LogWarning("Haptics Controller uses the New InputSystem to use the Gamepad class.");
			return;
#endif

			//Add new data to the list, for the newest request to vibrate the controller
			hapticControllerDatas.Add(new HapticsControllerData() {
				str = strength,
				startTime = Time.time,
				endTime = Time.time + duration,
				cancelAllPreviousVibrations = cancelAllPreviousVibrations
			});
			StartCoroutine(CheckToPlayHaptics());
		}

		/// <summary>
		/// Check if there is any vibration left to play, if so, play it. Else, early return.
		/// </summary>
		protected virtual IEnumerator CheckToPlayHaptics () {
			//Remove all items which have already expired.
			hapticControllerDatas.RemoveAll(x => x.endTime <= Time.time);
			//Sort the items descendigly, based on the strength.
			hapticControllerDatas = hapticControllerDatas.OrderByDescending(y => y.str).ToList();

			//Get the item with the strongest vibration strength if available.
			var hapticControllerDataToUse = hapticControllerDatas.FirstOrDefault();
			//Early exit.
			if (hapticControllerDataToUse == null)
				yield break;

			//How long is left to vibrate this item.
			var dur = hapticControllerDataToUse.endTime - Time.time;

#if ENABLE_INPUT_SYSTEM
			//Actual vibration here
			Gamepad.current?.SetMotorSpeeds(hapticControllerDataToUse.str, hapticControllerDataToUse.str);

			//Wait enough here.
			yield return new WaitForSeconds(dur);
			yield return GameConstants.EOF;

			//Stop vibration here.
			Gamepad.current?.ResetHaptics();

			//Cancel any remaining vibrations if needed
			if (hapticControllerDataToUse.cancelAllPreviousVibrations) {
				hapticControllerDatas.Clear();
				yield break;
			}
#else
			yield return null;
#endif

			//After stopping the previous vibrations, check if we want to resume any previous ones.
			StartCoroutine(CheckToPlayHaptics());

			//Filter out the old items again.
			hapticControllerDatas.RemoveAll(x => x.endTime <= Time.time);
		}
		#endregion
	}
}