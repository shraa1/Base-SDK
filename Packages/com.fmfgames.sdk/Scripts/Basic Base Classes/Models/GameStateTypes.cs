using System;
using System.Collections.Generic;
using BaseSDK.Extension;

namespace BaseSDK.Models {
	[Serializable]
	public class GameState<GAEMSAVESTATE> where GAEMSAVESTATE : GameSaveStateBase {
		public int GameStateVersion = 0;
		public long LastLogout = 0;
		public List<GAEMSAVESTATE> GameSaveStates;
		public GAEMSAVESTATE LifetimeGameStates;

		public void Save () => LastLogout = DateTime.Now.Ticks;
		public override string ToString () => this.Serialize(GameConstants.JsonSerializerSettings);
	}

	[Serializable]
	public class GameSaveStateBase {}

	[Serializable]
	public class GameSaveState : GameSaveStateBase {}
}