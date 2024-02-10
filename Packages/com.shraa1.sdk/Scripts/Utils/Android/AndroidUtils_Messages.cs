using BaseSDK.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AJO = UnityEngine.AndroidJavaObject;
using AJC = UnityEngine.AndroidJavaClass;
using System.Linq;

#pragma warning disable CS0162 // Unreachable code detected

#if UNITY_ANDROID
namespace BaseSDK.Utils.Android {
	public partial class AndroidExtensions {
		public static void Toast(string message, int duration = 1) {
#if UNITY_EDITOR
			Debug.Log(message);
			return;
#endif
			AJC toastClass = new AJC("android.widget.Toast");
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
				AJO toastObject = toastClass.CallStatic<AJO>("makeText", currentActivity, message, duration);
				toastObject.Call("show");

				toastClass.Dispose();
			}));
		}

		public static void ShowAlert(string title, string message, params ShowAlertButton[] showAlertButtons) {
#if UNITY_EDITOR
			var actions = showAlertButtons.Select(x => x.onButtonClickedAction).ToList();
			MessageBox.Show(title, message,
				(sbyte val) => actions[val]?.Invoke(),
				showAlertButtons.Select(x => x.buttonText).ToArray());
			return;
#endif
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
				var builder = new AJO("android.app.AlertDialog$Builder", currentActivity);

				AJO dialog = builder.Call<AJO>("setMessage", message)
					.Call<AJO>("setTitle", title)
					.Call<AJO>("setCancelable", false);

				if (showAlertButtons.Length > 0) {
					dialog.Call<AJO>("setNeutralButton", showAlertButtons[0].buttonText, alertButtonListner);
					onNeutralButtonListener = showAlertButtons[0].onButtonClickedAction;
				}
				if (showAlertButtons.Length > 1) {
					dialog.Call<AJO>("setNegativeButton", showAlertButtons[1].buttonText, alertButtonListner);
					onNegativeButtonListener = showAlertButtons[1].onButtonClickedAction;
				}
				if (showAlertButtons.Length > 2) {
					dialog.Call<AJO>("setPositiveButton", showAlertButtons[2].buttonText, alertButtonListner);
					onPositiveButtonListener = showAlertButtons[2].onButtonClickedAction;
				}

				dialog.Call<AJO>("create").Call("show");
			}));
		}
	}

	public struct ShowAlertButton {
		public Action onButtonClickedAction;
		public string buttonText;

		public ShowAlertButton(string buttonText, Action onButtonClickedAction) {
			this.buttonText = buttonText;
			this.onButtonClickedAction = onButtonClickedAction;
		}
	}
}
#endif