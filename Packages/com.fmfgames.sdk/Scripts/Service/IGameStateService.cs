using BaseSDK.Models;

namespace BaseSDK.Services {
	public interface IGameStateService<GAMESTATE, GAMESAVESTATE> : IService
		where GAMESTATE : GameState<GAMESAVESTATE>, new()
		where GAMESAVESTATE : GameSaveStateBase, new() {
		GAMESTATE GameState { get; }
	}
}