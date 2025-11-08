#if UNITY_EDITOR
using BaseSDK.Helper;
using BaseSDK.Extension;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace BaseSDK {
	public class GeneralHelperEditors : MonoBehaviour {
		[MenuItem("Tools/Open Persistent Data Path Folder")]
		public static void OpenPDP () {
			var procInfo = new ProcessStartInfo { UseShellExecute = true, FileName = Application.persistentDataPath };
			var pp = new Process() { StartInfo = procInfo };
			try { pp.Start(); }
			catch { }
		}

		[MenuItem("Tools/Clear Console &#%c")]
		public static void ClearConsole () {
			//TODO FIXME need to find the exact version where this was changed
#if UNITY_2017_1_OR_NEWER
			Assembly.GetAssembly(typeof(ActiveEditorTracker)).GetType("UnityEditor.LogEntries").GetMethod("Clear").Invoke(null, null);
#else
			Assembly.GetAssembly(typeof(ActiveEditorTracker)).GetType("UnityEditorInternal.LogEntries").GetMethod("Clear").Invoke(null, null);
#endif
		}

		[MenuItem("Tools/Delete PlayerPrefs %#&DEL")]
		public static void Delete () {
			var hadKey = PlayerPrefs.HasKey("PlayFromScene0");

			PlayerPrefsManager.DeleteAll();

			if (hadKey)
				PlayerPrefs.SetString("PlayFromScene0", string.Empty);
		}
		
		[MenuItem("Tools/Delete PlayerPrefs + Game Save Files %#DEL", priority = 50)]
		public static void DeleteWithGameFiles () {
			var hadKey = PlayerPrefs.HasKey("PlayFromScene0");

			PlayerPrefsManager.DeleteAll();

			if (hadKey)
				PlayerPrefs.SetString("PlayFromScene0", string.Empty);

			Path.Combine(Application.dataPath, "Saved Game Files", "FirstLaunch.playedAlready").DeleteSafely();
			Path.Combine(Application.dataPath, "Saved Game Files", "FirstLaunch.playedAlready.meta").DeleteSafely();

			if (EditorApplication.isPlaying) {
				Path.Combine(Application.dataPath, "Saved Game Files", $"{GameConstants.GameName()}.sav").DeleteSafely();
				Path.Combine(Application.dataPath, "Saved Game Files", $"{GameConstants.GameName()}.sav.meta").DeleteSafely();
				Path.Combine(Application.dataPath, "Saved Game Files", $"{GameConstants.GameName()}_Settings.json").DeleteSafely();
				Path.Combine(Application.dataPath, "Saved Game Files", $"{GameConstants.GameName()}_Settings.json.meta").DeleteSafely();
			}
			else {
				Path.Combine(Application.dataPath, "Saved Game Files", $"{PlayerSettings.productName}.sav").DeleteSafely();
				Path.Combine(Application.dataPath, "Saved Game Files", $"{PlayerSettings.productName}.sav.meta").DeleteSafely();
				Path.Combine(Application.dataPath, "Saved Game Files", $"{PlayerSettings.productName}_Settings.json").DeleteSafely();
				Path.Combine(Application.dataPath, "Saved Game Files", $"{PlayerSettings.productName}_Settings.json.meta").DeleteSafely();
			}
			AssetDatabase.Refresh();
		}
	}
}
#endif