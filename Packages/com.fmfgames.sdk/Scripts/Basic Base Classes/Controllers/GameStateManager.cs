using System;
using System.Collections;
using System.IO;
using BaseSDK.Extension;
using BaseSDK.Helper;
using BaseSDK.Models;
using BaseSDK.Services;
using UnityEngine;
using UnityEngine.Profiling;

namespace BaseSDK.Controllers {
	public abstract class GameStateManager : Configurable, IGameStateService, IManagerBehaviour {
		#region Properties
		private IGameState m_GameState;
		public IGameState GameState {
			get {
				if (m_GameState == null)
					Load();
				return m_GameState;
			}
			protected set => m_GameState = value;
		}
		#endregion Properties

		#region Variables
		private const string k_PLAYERPREF_KEY = "m_GameState";
		#endregion Variables

		#region Unity Methods
		protected virtual void OnApplicationQuit() => Save();
		#endregion Unity Methods

		#region Interface Implementation
		public abstract IGameState GetNewGameState();

		public abstract IGameState GetGameState(string serialized);

		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(IGameStateService));

		public override IEnumerator Setup() {
			Load();
			yield return null;
			Initialized = true;
		}

		public virtual void Load() {
			var SAVED_FILE_NAME = $"{GameConstants.GameName()}.sav";
			var SAVE_FILE_PATH = Path.Combine(GameConstants.SAVE_FOLDER_PATH, SAVED_FILE_NAME);
			var SAVE_TEMP_FILE_PATH = Path.Combine(GameConstants.SAVE_FOLDER_PATH, SAVED_FILE_NAME + ".tmp");

			if (File.Exists(SAVE_TEMP_FILE_PATH)) {
				if (File.Exists(SAVE_FILE_PATH))
					File.Delete(SAVE_FILE_PATH);
				File.Move(SAVE_TEMP_FILE_PATH, SAVE_FILE_PATH);
			}

			//Fresh launch
			if (!File.Exists(SAVE_FILE_PATH)) {
				GameState = GetNewGameState();
				Save();
				return;
			}

			using var sr = new StreamReader(SAVE_FILE_PATH);
			var savGameState = sr.ReadToEnd();
			var ppGameState = PlayerPrefsManager.Get(k_PLAYERPREF_KEY, string.Empty);

			var ppGS = GetGameState(ppGameState);
			var savGS = GetGameState(savGameState);
			ppGS ??= GetNewGameState();
			savGS ??= GetNewGameState();

			ppGS = ppGS.LastLogout > savGS.LastLogout ? ppGS : savGS;
			GameState = ppGS;
		}

		public virtual void Save() {
			var SAVED_FILE_NAME = $"{GameConstants.GameName()}.sav";
			var SAVE_FILE_PATH = Path.Combine(GameConstants.SAVE_FOLDER_PATH, SAVED_FILE_NAME);
			var SAVE_TEMP_FILE_PATH = Path.Combine(GameConstants.SAVE_FOLDER_PATH, SAVED_FILE_NAME + ".tmp");

			//Update Last Logout value
			GameState.Save();
#if UNITY_EDITOR
			var encrypted = GameState.Serialize(GameConstants.JsonSerializerSettings);
#else
			var encrypted = GameState.Serialize(GameConstants.JsonSerializerSettings).Encrypt();
#endif

			PlayerPrefsManager.Set(k_PLAYERPREF_KEY, encrypted);

			if (!Directory.Exists(GameConstants.SAVE_FOLDER_PATH))
				Directory.CreateDirectory(GameConstants.SAVE_FOLDER_PATH);

			File.WriteAllText(SAVE_TEMP_FILE_PATH, encrypted);
			if (File.Exists(SAVE_FILE_PATH))
				File.Delete(SAVE_FILE_PATH);
			File.Move(SAVE_TEMP_FILE_PATH, SAVE_FILE_PATH);
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public virtual void CheckForUpgrade() { }

		public virtual void Upgrade(int oldVersion, int newVersion) { }
		#endregion Interface Implementation
	}
}