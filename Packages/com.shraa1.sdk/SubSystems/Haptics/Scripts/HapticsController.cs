//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
		protected virtual void Awake() {
			DontDestroy = true;
		}
		#endregion

		#region Helper Methods
		//Remove the #if wrapper if you want this in the game. Not sure why, but why not ;)
#if UNITY_EDITOR
		/// <summary>
		/// Place the controller in your hand or your back and use it as a massager ;)
		/// </summary>
		public void Massage() => Vibrate(float.MaxValue, 1f, false);
#endif

		/// <summary>
		/// Start the operation for vibration of the current Gamepad
		/// </summary>
		/// <param name="duration">How long should the vibration stay active?</param>
		/// <param name="strength">How strong should the vibration be? Ranges from 0 to 1</param>
		/// <param name="cancelAllPreviousVibrations">If there was a previously long vibration already active, and it a new shorter one is needed to be played, should the old vibration continue to play after the new one? Or should all vibrations stop when the new one has finished playing? If everything should stop, use true, else use false</param>
		public void Vibrate(float duration, float strength, bool cancelAllPreviousVibrations = false) {
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
		protected virtual IEnumerator CheckToPlayHaptics() {
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

#if UNITY_EDITOR
namespace BaseSDK.EditorScripts {
	using BaseSDK.Controllers;
	using UnityEditor;
	
	[CustomEditor(typeof(HapticsController))]
	public class HapticsControllerEditor : Editor {
		private UnityEditor.PackageManager.Requests.AddRequest addPackage_Result;

		protected void OnEnable() => EditorApplication.update += Update;

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
#endif