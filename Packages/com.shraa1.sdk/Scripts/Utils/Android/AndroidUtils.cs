using BaseSDK.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using AJO = UnityEngine.AndroidJavaObject;
using AJC = UnityEngine.AndroidJavaClass;

#if UNITY_ANDROID
namespace BaseSDK.Utils.Android {
	public partial class AndroidExtensions {

#pragma warning disable CS0162 // Unreachable code detected
#pragma warning disable IDE1006 // Naming Styles

#region Common Native Classes' C# references
		private static AJC _unityPlayer;
		private static AJC unityPlayer {
			get => _unityPlayer = _unityPlayer ??  new AJC("com.unity3d.player.UnityPlayer");
		}

		private static AJO _currentActivity;
		private static AJO currentActivity {
			get => _currentActivity = _currentActivity ?? unityPlayer.GetStatic<AJO>("currentActivity");
		}

		private static AJO _packageManager;
		private static AJO packageManager {
			get => _packageManager = _packageManager ?? currentActivity.Call<AJO>("getPackageManager");
		}

		private static AJO _applicationContext;
		private static AJO applicationContext {
			get => _applicationContext = _applicationContext ?? currentActivity.Call<AJO>("getApplicationContext");
		}
#endregion

#region Other Variables
		private static AlertButtonListner _alertButtonListner;
		private static AlertButtonListner alertButtonListner {
			get => _alertButtonListner = _alertButtonListner ??  new AlertButtonListner();
		}
#endregion

#region OnClick Listeners for ShowAlert
		private static Action onNegativeButtonListener = null, onPositiveButtonListener = null, onNeutralButtonListener = null;

		private class AlertButtonListner : AndroidJavaProxy {
			public AlertButtonListner() : base("android.content.DialogInterface$OnClickListener") {}

			public void onClick(AJO obj, int value) {
				if (value == -3)
					onNeutralButtonListener?.Invoke();
				if (value == -2)
					onNegativeButtonListener?.Invoke();
				if (value == -1)
					onPositiveButtonListener?.Invoke();

				onNegativeButtonListener = null;
				onPositiveButtonListener = null;
				onNeutralButtonListener = null;
			}
		}
#endregion


		//https://stackoverflow.com/questions/21544331/trying-open-a-specific-folder-in-android-using-intent
		//https://stackoverflow.com/questions/30446052/getlaunchintentforpackage-is-null-for-some-apps
		//https://answers.unity.com/questions/780406/androidunity-launching-activity-from-unity-activit.html
		//https://stackoverflow.com/questions/14608327/how-to-open-a-file-browser-in-android
		//https://answers.unity.com/questions/918210/storing-files-on-android-not-showing-up-on-windows.html
		//https://answers.unity.com/questions/1279669/how-can-i-browse-files-on-android-outside-of-the-u.html
		//https://stackoverflow.com/questions/45012523/how-to-install-android-apk-from-code-in-unity
		//https://forum.unity.com/threads/launch-another-unity-app-from-unity-app-param-problem.426168/
		//https://stackoverflow.com/questions/52590525/how-to-show-a-toast-message-in-unity-similar-to-one-in-android
		//http://blog.trsquarelab.com/2015/02/accessing-android-or-java-classes-from.html
		//https://stackoverflow.com/questions/35810968/weird-behaviour-of-AJO-in-unity-when-implemented-through-c-sharp?rq=1

		
		/// <summary>
		/// Untested
		/// </summary>
		public static void LocalNotification(string title, string[] messages) {
#if UNITY_EDITOR
			Debug.Log(title + " -> " + messages.Print());
			return;
#endif
			currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
				var fragmentClass = new AJC("androidx.fragment.app.ListFragment");
				AJO notificationIntent = new AJO("android.content.Intent", applicationContext, fragmentClass);
				notificationIntent.Call<AJO>("putExtra", "clicked", "Notification Clicked");
				notificationIntent.Call<AJO>("addFlags", 0x04000000 | 0x20000000);

				AJO mBuilder = new AJO("androidx.core.app.NotificationCompat$Builder", applicationContext);
				AJO uri = new AJC("android.media.RingtoneManager").Call<AJO>("getDefaultUri", 0x00000002);

				mBuilder = mBuilder.Call<AJO>("setContentTitle", "Reminder")
					.Call<AJO>("setContentText", "You have new Reminders.")
					.Call<AJO>("setTicker", "New Reminder Alert!")
					.Call<AJO>("setSound", uri)
					.Call<AJO>("setAutoCancel", true);

				AJO inboxStyle = new AJO("androidx.core.app.NotificationCompat$InboxStyle");

				inboxStyle = inboxStyle.Call<AJO>("setBigContentTitle", "You have Reminders:");

				// Moves events into the big view
				for (int i = 0; i < messages.Length; i++)
					inboxStyle = inboxStyle.Call<AJO>("addLine", messages[i]);


				AJO resultIntent = new AJO("android.content.Intent", applicationContext, fragmentClass);
				AJO stackBuilder = new AJC("android.app.TaskStackBuilder").CallStatic<AJO>("create", applicationContext);
				stackBuilder = stackBuilder.Call<AJO>("addParentStack", fragmentClass);


				stackBuilder = stackBuilder.Call<AJO>("addNextIntent", resultIntent);
				AJO resultPendingIntent = stackBuilder.Call<AJO>("getPendingIntent", 0, 0x10000000);

				mBuilder.Call("setContentIntent", resultPendingIntent);
				AJO mNotificationManager = applicationContext.Call<AJO>("getSystemService", "notification").Cast("android.app.NotificationManager");

				// notificationID allows you to update the notification later  on.
				mNotificationManager.Call<AJO>("notify", 999, mBuilder.Call<AJO>("build"));

				currentActivity.Dispose();
				notificationIntent.Dispose();
			}));
		}



		public static void OpenPersistentPath(params string[] additionalPaths) {
			string path = Application.persistentDataPath;
			foreach(var additionalPath in additionalPaths)
				path = Path.Combine(path, additionalPath);
#if UNITY_EDITOR
			Debug.Log("Starting intent with package name " + path);
			return;
#endif

			AJO launchIntent = packageManager.Call<AJO>("android.content.Intent", "android.content.Intent$ACTION_VIEW");
			AJO uri = Uri(path);
			launchIntent.Call<AJO>("setDataAndType", uri, "*/*");
			currentActivity.Call("startActivity", launchIntent);
		}

		public static void StartIntent(string package) {
#if UNITY_EDITOR
			Debug.Log("Starting intent with package name " + package);
			return;
#endif
			AJO launchIntent = packageManager.Call<AJO>("getLaunchIntentForPackage", package);
			currentActivity.Call("startActivity", launchIntent);
		}

		private static AJO Uri (string path) => new AJO("android.net.Uri").CallStatic<AJO>("parse", path);
	}


	public static class AndroidExtensionMethods {
		// Cast extension method
		public static AJO Cast(this AJO source, string destClass) {
			using AJO destClassAJC = ClassForName(destClass);
			return destClassAJC.Call<AJO>("cast", source);
		}

		private static AJO ClassForName(string className) {
			using var clazz = new AJC("java.lang.Class");
			return clazz.CallStatic<AJO>("forName", className);
		}
	}
}
#endif