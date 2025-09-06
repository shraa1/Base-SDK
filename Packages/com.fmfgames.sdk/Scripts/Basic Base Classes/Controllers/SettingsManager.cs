using System;
using System.Collections;
using System.IO;
using BaseSDK;
using BaseSDK.Helper;
using BaseSDK.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace BaseSDK.Controllers {
	public class SettingsManager : Singleton<SettingsManager>, IConfigurable, IManagerBehaviour, ISettingsService {
		#region Variables
		private static string k_SAVED_FILE_NAME = $"{Application.productName}_Settings.json";
		private const string k_SAVED_FOLDER_NAME = "Saved Game Files";
		private const string k_PLAYERPREF_KEY = "m_SettingsState";
		#endregion Variables

		#region Properties
		public SettingsState SettingsState {
			get {
				if (m_SettingsState == null)
					Instance.Load();
				return m_SettingsState;
			}
			set => m_SettingsState = value;
		}
		private SettingsState m_SettingsState;
		#endregion Properties

		#region Interface Implementation
		public (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(ISettingsService));

		public void Save () {
			var serialized = JsonConvert.SerializeObject(SettingsState, Formatting.Indented);
			PlayerPrefsManager.Set(k_PLAYERPREF_KEY, serialized);

			var path = Path.Combine(Application.dataPath, k_SAVED_FOLDER_NAME);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			using var sw = new StreamWriter(Path.Combine(path, k_SAVED_FILE_NAME));
			sw.Write(serialized);
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public void Load () {
			var savFilePath = Path.Combine(Application.dataPath, k_SAVED_FOLDER_NAME, k_SAVED_FILE_NAME);
			//Fresh launch
			if (!File.Exists(savFilePath)) {
				SettingsState = new();
				Save();
				return;
			}

			using var sr = new StreamReader(savFilePath);
			var savSettingsState = sr.ReadToEnd();
			var ppGameState = PlayerPrefsManager.Get(k_PLAYERPREF_KEY, string.Empty);
			SettingsState ppSS = null;
			try {
				ppSS = JsonConvert.DeserializeObject<SettingsState>(ppGameState);
			}
			catch {
				SettingsState = JsonConvert.DeserializeObject<SettingsState>(savSettingsState);
				return;
			}
			SettingsState = ppSS;
		}

		public void CheckForUpgrade () { }
		public void Upgrade (int oldVersion, int newVersion) { }

		public bool Initialized { get; set; } = false;

		public IEnumerator Setup () {
			yield return null;
			Initialized = true;
		}

		public void SetBrightnessValue (float newValue) => SettingsState.Brightness = newValue;
		#endregion Interface Implementation

		#region Unity Methods
		protected override void OnApplicationQuit () {
			Save();
			base.OnApplicationQuit();
		}

		protected virtual void Awake() => SetResolutionTo16IsTo9();
		#endregion Unity Methods

		#region Helper Methods
		protected virtual void SetResolutionTo16IsTo9 () {
			var screen = Screen.currentResolution;
			var targetAspect = 16f / 9f;
			var width = screen.width;
			var height = Mathf.RoundToInt(width / targetAspect);

			if (height > screen.height) {
				height = screen.height;
				width = Mathf.RoundToInt(height * targetAspect);
			}
			Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
		}
		#endregion Helper Methods
	}
}