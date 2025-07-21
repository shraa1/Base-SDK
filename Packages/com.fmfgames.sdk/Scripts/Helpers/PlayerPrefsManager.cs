#pragma warning disable 0162

using UnityEngine;
using System.Collections;
using Microsoft.Win32;
using System.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using BaseSDK.Extension;
using Newtonsoft.Json;

namespace BaseSDK.Helper {
	public class PlayerPrefsManager : MonoBehaviour {

		private static string RegistrySubkey = "SOFTWARE\\BaseSDK" + Application.identifier;
		private const string PlayerPrefsFolderName = "PPs";
		private const string Arrow = "-->";

	#if USE_DATA_PATH_INSTEAD_OF_REGEDIT
		private const string DeletingFile = "Deleting file";
		private const string WhileDeletingFile = "while deleting file";
		private const string ReadingFromPath = "reading file contents for path";
		private const string WithError = "with error";
		private const string WhileReadingFile = "while reading file";
		private const string WritingContentsToFile = "writing contents to file";
		private const string With = "with";
	#endif
	#if USE_REGEDIT
		private const string UnableToDelete = "Unable to Delete All PlayerPref Keys & Values";
		private const string ReturningDefVal = "Returning default value";
		private const string DidntFindKey = "Didn't Find PlayerPref Keys";
		private const string CouldntSetKeys = "Couldn't Set PlayerPrefsManager Keys";
	#endif

		private static string PlayerPrefsPath {
			get {
				var path = $"{Application.persistentDataPath}{GameConstants.SLASH}{PlayerPrefsFolderName}{GameConstants.SLASH}";
				if (!Directory.Exists(path))
					Directory.CreateDirectory(path);
				return path;
			}
		}

		public static void DeleteKey(string key) {
	#if USE_DEFAULT_PLAYERPREFS
			PlayerPrefs.DeleteKey(key);
	#endif

	#if USE_ENCRYPTION
			key = key.Encrypt();
	#endif

	#if USE_DATA_PATH_INSTEAD_OF_REGEDIT
			var l = Directory.GetFiles (PlayerPrefsPath);
			foreach(var x in l) {
				try { if (x.EndsWith (key + GameConstants.TEXT_FILE_EXTENSION)) File.Delete (x); }
				catch (Exception e) { Debug.LogError ($"{GameConstants.ERROR}{GameConstants.SPACE}{DeletingFile}{GameConstants.SPACE}{key}{GameConstants.SPACE}{Arrow}{GameConstants.SPACE}{GameConstants.ERROR}{GameConstants.COLON}{GameConstants.SPACE}{e.Message}"); }
			}
	#endif

	#if USE_REGEDIT
			try
			{
				var regkey = Registry.CurrentUser.OpenSubKey(RegistrySubkey, true);
				if (regkey == null) {
					Registry.CurrentUser.CreateSubKey(RegistrySubkey);
					regkey = Registry.CurrentUser.OpenSubKey(RegistrySubkey, true);
				}
				regkey.GetSubKeyNames().ForEach (x => {
					if(x == key)
						regkey?.DeleteSubKey (x);
				});
			}
			catch (Exception e) {
				Debug.Log($"{UnableToDelete}{GameConstants.PERIOD}{GameConstants.SPACE}{ReturningDefVal}{GameConstants.PERIOD}{GameConstants.SPACE}{GameConstants.ERROR}{GameConstants.SPACE}{Arrow}{GameConstants.SPACE}{e.Message}");
			}
	#endif
		}

		public static void DeleteAll() {
	#if USE_DEFAULT_PLAYERPREFS
			PlayerPrefs.DeleteAll();
	#endif

	#if USE_REGEDIT
			try {
				var regkey = Registry.CurrentUser.OpenSubKey(RegistrySubkey, true);
				if (regkey == null)
					return;
				regkey.GetSubKeyNames().ForEach (x => regkey?.DeleteSubKey (x));
			}
			catch (Exception e) {
				Debug.Log($"{UnableToDelete}{GameConstants.PERIOD}{GameConstants.SPACE}{ReturningDefVal}{GameConstants.PERIOD}{GameConstants.SPACE}{GameConstants.ERROR}{GameConstants.SPACE}{Arrow}{GameConstants.SPACE}{e.Message}");
			}
	#endif

	#if USE_DATA_PATH_INSTEAD_OF_REGEDIT
			Directory.GetFiles(PlayerPrefsPath)?.ForEach(x => {
				try { File.Delete(x); }
				catch { Debug.LogError($"{GameConstants.ERROR}{GameConstants.SPACE}{WhileDeletingFile}{GameConstants.SPACE}{x}"); }
			});
	#endif
		}

		public static T Get<T>(string key, T defaultValue = default, bool useNewtonsoft = true) {
#if USE_ENCRYPTION
			key = key.Encrypt();
#endif

#if USE_DEFAULT_PLAYERPREFS
			var str = PlayerPrefs.GetString(key, string.Empty);
			if (str.IsNullOrEmpty())
				return defaultValue;


			#region Newtonsoft JsonConvert
			return useNewtonsoft ? JsonConvert.DeserializeObject<T>(
#if USE_ENCRYPTION
				str.Decrypt().DecryptInMemory<string>());
#else
				str)
#endif
			#endregion
			#region JsonUtility
				: JsonUtility.FromJson<T>(
#if USE_ENCRYPTION
				str.Decrypt().DecryptInMemory<string>());
#else
				str)
#endif
				;
			#endregion


#endif

#if USE_DATA_PATH_INSTEAD_OF_REGEDIT
			foreach (var filePath in Directory.GetFiles(PlayerPrefsPath)) {
				try {
					if (filePath.EndsWith (key + GameConstants.TEXT_FILE_EXTENSION)) {
						var fileContents = filePath.ReadFromFile (out var e);
						if (e != null) {
							Debug.LogError ($"{GameConstants.ERROR}{GameConstants.SPACE}{ReadingFromPath}{GameConstants.SPACE}{filePath}{GameConstants.SPACE}{WithError}{e.Message}");
							return defaultValue;
						}
						return JsonUtility.FromJson<T>(
						//return JsonConvert.DeserializeObject<T>(
#if USE_ENCRYPTION
							fileContents.Decrypt().DecryptInMemory<string>());
#else
							fileContents);
#endif
					}
				}
				catch (Exception e) {
					Debug.LogError ($"{GameConstants.ERROR}{GameConstants.SPACE}{WhileReadingFile}{GameConstants.SPACE}{filePath}{GameConstants.SPACE}{WithError}{GameConstants.SPACE}{e.Message}");
				}
			}
			return defaultValue;
#endif

#if USE_REGEDIT
			try {
				var regkey = Registry.CurrentUser.OpenSubKey(RegistrySubkey, true);
				if (regkey == null) {
					Registry.CurrentUser.CreateSubKey(RegistrySubkey);
					regkey = Registry.CurrentUser.OpenSubKey(RegistrySubkey, true);
				}
				var t = regkey?.GetValue(key, defaultValue)?.ToString();
#if USE_ENCRYPTION
				t = t.Decrypt().DecryptInMemory<string>();
#endif
				return JsonUtility.FromJson<T>(t);
				//return JsonConvert.DeserializeObject<T>(t);
			}
			catch (Exception e) {
				Debug.Log($"{DidntFindKey}{GameConstants.SPACE}{key}{GameConstants.PERIOD}{GameConstants.SPACE}{ReturningDefVal}{GameConstants.PERIOD}{GameConstants.SPACE}{GameConstants.ERROR}{GameConstants.SPACE}{Arrow}{GameConstants.SPACE}{e.Message}");
			}
			return defaultValue;
#endif

			Debug.LogError("THIS SHOULD NOT HAPPEN, MANAGE SCRIPTING DEFINES, EITHER DIRECTLY OR USING THE SCRIPTINGDEFINE EDITOR SCRIPT");
			return default;
		}

		public static void Set<T> (string key, T value, bool useNewtonsoft = true) {
			var str = useNewtonsoft ? JsonConvert.SerializeObject(value) : JsonUtility.ToJson(value);
	#if USE_ENCRYPTION
			str = str.EncryptInMemory().Encrypt();
			key = key.Encrypt();
	#endif

	#if USE_DEFAULT_PLAYERPREFS
			PlayerPrefs.SetString(key, str);
	#endif

	#if USE_DATA_PATH_INSTEAD_OF_REGEDIT
			var e = $"{PlayerPrefsPath}{key}{GameConstants.TEXT_FILE_EXTENSION}".WriteToFile (str, true);
			if (e != null)
				Debug.LogError ($"{GameConstants.ERROR}{GameConstants.SPACE}{WritingContentsToFile}{GameConstants.SPACE}{PlayerPrefsPath}{key}{GameConstants.TEXT_FILE_EXTENSION}{GameConstants.SPACE}{With}{GameConstants.SPACE}{GameConstants.ERROR_LOWER}{GameConstants.COLON}{GameConstants.LINE_BREAK}{e.Message}");
	#endif

	#if USE_REGEDIT
			try {
				var regkey = Registry.CurrentUser.OpenSubKey(RegistrySubkey, true);
				if (regkey == null) {
					Registry.CurrentUser.CreateSubKey(RegistrySubkey);
					regkey = Registry.CurrentUser.OpenSubKey(RegistrySubkey, true);
				}
				regkey.SetValue (key, str);
			}
			catch (Exception e) {
				Debug.Log(string.Format($"{CouldntSetKeys}{GameConstants.SPACE}{key}.{GameConstants.SPACE}{GameConstants.ERROR}{GameConstants.SPACE}{Arrow}{GameConstants.SPACE}{e.Message}"));
			}
	#endif
		}
	}
}