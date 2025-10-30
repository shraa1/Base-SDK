using System;
using System.Collections.Generic;
using BaseSDK.Extension;
using Newtonsoft.Json;

namespace BaseSDK.Models {
	/// <summary>
	/// Base game state interface. Extend this interface in your client project to define your game's save structure.
	/// The SDK doesn't dictate whether you use single-slot, multi-slot, or any other save strategy.
	/// </summary>
	public interface IGameState {
		/// <summary>
		/// Version of the game state schema for migration/upgrade purposes
		/// </summary>
		int GameStateVersion { get; set; }

		/// <summary>
		/// Last logout timestamp in ticks
		/// </summary>
		long LastLogout { get; set; }

		/// <summary>
		/// Regardless of the save slots, this is common data, like lifetime coins, which will be a sum of coins in all save files, and (maybe) including deleted saved files.
		/// </summary>
		IGameSaveStateBase LifetimeGameStates { get; set; }

		IGameSaveState GetCurrentSlot();

		/// <summary>
		/// Called before serialization to update any necessary fields (e.g., LastLogout)
		/// </summary>
		void Save();
	}

	public interface IGameSaveStateBase { }
	public interface IGameSaveState : IGameSaveStateBase { }

	/// <summary>
	/// Default implementation of IGameState
	/// </summary>
	[Serializable]
	public abstract class GameStateBase : IGameState {
		public int GameStateVersion { get; set; } = 0;
		public long LastLogout { get; set; } = 0;
		public IGameSaveStateBase LifetimeGameStates { get; set; }

		public abstract IGameSaveState GetCurrentSlot();

		public void Save () => LastLogout = DateTime.Now.Ticks;
		public override string ToString () => this.Serialize(GameConstants.JsonSerializerSettings);
	}

	/// <summary>
	/// Extended game state with multi-slot save support. Use this if your game needs multiple save slots.
	/// </summary>
	[Serializable]
	public class MultiSaveGameStateBase : GameStateBase {
		public virtual List<IGameSaveState> GameSaveStates { get; set; } = new(GameConstants.MaxSaveSlots());
		[JsonIgnore] public int CurrentSlotIndex { get; protected set; } = -1;

		public override IGameSaveState GetCurrentSlot() =>
			CurrentSlotIndex.Between(0, GameSaveStates.Count) ? GameSaveStates[CurrentSlotIndex] : null;

		public void SetCurrentSlot(int val) {
			if (val.Between(0, GameSaveStates.Count))
				CurrentSlotIndex = val;
		}
	}

	/// <summary>
	/// Single-slot game state for games that only need New Game / Continue functionality
	/// </summary>
	[Serializable]
	public class SingleSaveGameState : GameStateBase {
		protected IGameSaveState CurrentSave { get; set; }
		public bool HasSaveData => CurrentSave != null;

		public void SetCurrentSlot(IGameSaveState newState) => CurrentSave = newState;
		public override IGameSaveState GetCurrentSlot() => CurrentSave;
	}
}