//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using System.IO;
using System.Linq;
using BaseSDK.Extension;

namespace BaseSDK.EditorScripts {
	public static class PackageDependenciesHelper {
#region Helpers for doing specific things after importing/removing/etc. packages
		/// <summary>
		/// After Importing the InputSystem package from code for package manager, change the setting in the dropdown in the PlayerSettings
		/// </summary>
		public static void ChangeInputSystemPlayerSettings() {
			Client.Add("com.unity.inputsystem");
			var allPlayerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>();
			if (allPlayerSettings.Length > 0) {
				var playerSettings = Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault();
				if (playerSettings != null) {
					var playerSettingsObject = new SerializedObject(playerSettings);

#if UNITY_2020_1_OR_NEWER
					var str = "activeInputHandler";
#else
					var str = "enableNativePlatformBackendsForNewInputSystem";
#endif
					var enableNativePlatformBackendsForNewInputSystem = playerSettingsObject.FindProperty(str);

					enableNativePlatformBackendsForNewInputSystem.intValue = 1;

					playerSettingsObject.ApplyModifiedProperties();
					AssetDatabase.Refresh();
				}

				AssetDatabase.Refresh();
#if UNITY_2020_2_OR_NEWER
				AssetDatabase.RefreshSettings();
#endif
			}
		}

		/// <summary>
		/// After adding the TMP package from code for the manager, we need to import the "Essential Resources"
		/// </summary>
		public static void AddTextMeshProEssentials() {
			AssetDatabase.importPackageCompleted += ImportCallback;
			AssetDatabase.ImportPackage($"{Directory.GetDirectories($"{Application.dataPath.Remove("Assets")}Library\\PackageCache\\").ToList().Find(x => x.Contains("com.unity.textmeshpro"))}\\Package Resources\\TMP Essential Resources.unitypackage", false);
		}
#endregion

#region Methods from Unity Internal/Private classes/methods. Easier than using System.Reflection
		/// <summary>
		/// Got the method from the TMP_PackageResourceImporter class from packman TMP script
		/// </summary>
		private static void ImportCallback(string packageName) {
#if TMP_PRESENT
			TMPro_EventManager.ON_RESOURCES_LOADED();
#if UNITY_2018_3_OR_NEWER
			SettingsService.NotifySettingsProviderChanged();
#endif
#endif
			AssetDatabase.importPackageCompleted -= ImportCallback;
		}
#endregion
	}
}
#endif