using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BaseSDK.Models {
	[Serializable]
	public class GameState {
		public int GameStateVersion = 0;
		public long LastLogout = 0;
		public List<GameSaveState> GameSaveStates;
		public GameSaveStateBase LifetimeGameStates;

		private static readonly JsonSerializerSettings m_JsonSerializerSettings = new() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, TypeNameHandling = TypeNameHandling.Auto };

		public void Save () => LastLogout = DateTime.Now.Ticks;
		public override string ToString () => JsonConvert.SerializeObject(this, m_JsonSerializerSettings);
	}

	[Serializable]
	public class GameSaveStateBase {}

	[Serializable]
	public class GameSaveState : GameSaveStateBase {}
}