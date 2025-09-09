using System;
using System.IO;
using BaseSDK.Extension;
using BaseSDK.Helper;
using BaseSDK.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace BaseSDK.Controllers {
	public class SettingsManager<T> : Configurable, IManagerBehaviour, ISettingsService<T> where T : SettingsState, new () {
		#region Variables and Consts
		private const string PLAYERPREF_KEY = "m_SettingsState";
		#endregion Variables and Consts

		#region Properties
		public T SettingsState {
			get {
				if (m_SettingsState == null)
					Load();
				return m_SettingsState;
			}
			set => m_SettingsState = value;
		}
		private T m_SettingsState;
		#endregion Properties

		#region Interface Implementation
		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(ISettingsService<T>));

		public virtual void Save () {
			var SAVED_FILE_NAME = $"{GameConstants.GameName()}_Settings.json";
			var serialized = JsonConvert.SerializeObject(SettingsState, Formatting.Indented);
			PlayerPrefsManager.Set(PLAYERPREF_KEY, serialized);

			var path = Path.Combine(Application.dataPath, GameConstants.SAVED_GAME_FILES_FOLDERNAME);
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			using var sw = new StreamWriter(Path.Combine(path, SAVED_FILE_NAME));
			sw.Write(serialized);
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public virtual void Load () {
			var SAVED_FILE_NAME = $"{GameConstants.GameName()}_Settings.json";
			var savFilePath = Path.Combine(Application.dataPath, GameConstants.SAVED_GAME_FILES_FOLDERNAME, SAVED_FILE_NAME);

			//Fresh launch
			if (!File.Exists(savFilePath)) {
				SettingsState = new T();
				Save();
				return;
			}

			using var sr = new StreamReader(savFilePath);
			var savSettingsState = sr.ReadToEnd();
			var ppGameState = PlayerPrefsManager.Get(PLAYERPREF_KEY, string.Empty);
			T ppSS = null;
			try {
				ppSS = ppGameState.Deserialize<T>();
			}
			catch {
				SettingsState = savSettingsState.Deserialize<T>();
				return;
			}
			SettingsState = ppSS;
		}

		public void CheckForUpgrade () { }
		public void Upgrade (int oldVersion, int newVersion) { }

		public void SetBrightnessValue (float newValue) => SettingsState.Brightness = newValue;
		#endregion Interface Implementation

		#region Unity Methods
		protected virtual void Awake() => SetResolutionTo16IsTo9();

		protected virtual void OnApplicationQuit () => Save();
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