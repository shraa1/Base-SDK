using System;
using System.IO;
using BaseSDK.Controllers;
using BaseSDK.Helper;
using BaseSDK.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace BaseSDK.Settings.Controllers {
	public abstract class SettingsManagerBase : Configurable, IManagerBehaviour, ISettingsService {
		#region Variables and Consts
		private const string PLAYERPREF_KEY = "m_SettingsState";
		#endregion Variables and Consts

		#region Properties
		public ISettingsState SettingsState {
			get {
				if (m_SettingsState == null)
					Load();
				return m_SettingsState;
			}
			set => m_SettingsState = value;
		}
		private ISettingsState m_SettingsState;
		#endregion Properties

		#region Interface Implementation
		public abstract ISettingsState GetNewSettingsState();

		public abstract ISettingsState GetSettingsState(string serialized);

		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(ISettingsService));

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
				SettingsState = GetNewSettingsState();
				Save();
				return;
			}

			using var sr = new StreamReader(savFilePath);
			var savSettingsState = sr.ReadToEnd();
			var ppSettingsState = PlayerPrefsManager.Get(PLAYERPREF_KEY, string.Empty);
			ISettingsState ppSS = null;
			ppSS = GetSettingsState(ppSettingsState);
			ppSS ??= GetSettingsState(savSettingsState);
			ppSS ??= GetNewSettingsState();

			SettingsState = ppSS;
		}

		public virtual void CheckForUpgrade () { }
		public virtual void Upgrade (int oldVersion, int newVersion) { }

		public virtual void SetBrightnessValue (float newValue) => SettingsState.Brightness = newValue;
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