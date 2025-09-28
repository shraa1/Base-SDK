using System;
using System.Collections.Generic;
using BaseSDK.Extension;

namespace BaseSDK.Models {
	public interface IGameState {
		int GameStateVersion { get; set; }
		long LastLogout { get; set; }
		List<IGameSaveState> GameSaveStates { get; set; }
		IGameSaveState LifetimeGameStates { get; set; }

		void Save();
	}

	public interface IGameSaveStateBase { }
	public interface IGameSaveState : IGameSaveStateBase { }

	[Serializable]
	public class GameState : IGameState {
		public int GameStateVersion { get; set; } = 0;
		public long LastLogout { get; set; } = 0;
		public List<IGameSaveState> GameSaveStates { get; set; }
		public IGameSaveState LifetimeGameStates { get; set; }

		public void Save () => LastLogout = DateTime.Now.Ticks;
		public override string ToString () => this.Serialize(GameConstants.JsonSerializerSettings);
	}
}