using System;
using System.Collections;
using System.IO;
using BaseSDK.Extension;
using BaseSDK.Helper;
using BaseSDK.Models;
using Newtonsoft.Json;
using UnityEngine.Profiling;

namespace BaseSDK.Controllers {
	public class GameStateManager<T> : Configurable, IGameStateService<T>, IManagerBehaviour where T : GameState, new() {
		#region Properties
		private T m_GameState;
		public T GameState {
			get {
				if (m_GameState == null)
					Load();
				return m_GameState;
			}
			private set => m_GameState = value;
		}
		#endregion Properties

		#region Variables
		private const string k_PLAYERPREF_KEY = "m_GameState";
		#endregion Variables

		#region Unity Methods
		protected virtual void OnApplicationQuit () => Save();
		#endregion Unity Methods

		#region Interface Implementation
		public (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(IGameStateService<T>));

		public override IEnumerator Setup () {
			Load();
			yield return null;
			Initialized = true;
		}

		public void Load () {
			if (File.Exists(Constants.SAVE_TEMP_FILE_PATH)) {
				if (File.Exists(Constants.SAVE_FILE_PATH))
					File.Delete(Constants.SAVE_FILE_PATH);
				File.Move(Constants.SAVE_TEMP_FILE_PATH, Constants.SAVE_FILE_PATH);
			}

			//Fresh launch
			if (!File.Exists(Constants.SAVE_FILE_PATH)) {
				GameState = new T();
				Save();
				return;
			}

			Profiler.BeginSample("Open GameState File");
			var savGameState = string.Empty;
			using (var sr = new StreamReader(Constants.SAVE_FILE_PATH))
				savGameState = sr.ReadToEnd();
			var ppGameState = PlayerPrefsManager.Get(k_PLAYERPREF_KEY, string.Empty);
			Profiler.EndSample();

			Profiler.BeginSample("Deserialize JSONs");
			var ppGS = ppGameState.Decrypt<string>().Deserialize<T>();
			var savGS = savGameState.Decrypt<string>().Deserialize<T>();
			Profiler.EndSample();

			Profiler.BeginSample("Assign Final GameStates");
			//Take the latest data, ignore the older one. File writing might fail sometimes, probably more often than registry failures
			var useLatest = ppGS.LastLogout > savGS.LastLogout ? ppGS : savGS;
			GameState = ppGameState == savGameState ? ppGS : useLatest;
			Profiler.EndSample();
		}

		public void Save () {
			//Update Last Logout value
			GameState.Save();
			var encrypted = JsonConvert.SerializeObject(GameState, Formatting.None).Encrypt();
			PlayerPrefsManager.Set(k_PLAYERPREF_KEY, encrypted);

			if (!Directory.Exists(Constants.SAVE_FOLDER_PATH))
				Directory.CreateDirectory(Constants.SAVE_FOLDER_PATH);

			File.WriteAllText(Constants.SAVE_TEMP_FILE_PATH, encrypted);
			if (File.Exists(Constants.SAVE_FILE_PATH))
				File.Delete(Constants.SAVE_FILE_PATH);
			File.Move(Constants.SAVE_TEMP_FILE_PATH, Constants.SAVE_FILE_PATH);
#if UNITY_EDITOR
			UnityEditor.AssetDatabase.Refresh();
#endif
		}

		public void CheckForUpgrade () { }

		public void Upgrade (int oldVersion, int newVersion) { }
		#endregion Interface Implementation
	}
}