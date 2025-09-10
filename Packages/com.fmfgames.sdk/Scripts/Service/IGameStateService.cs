using BaseSDK.Models;

namespace BaseSDK.Services {
	public interface IGameStateService<T> : IService where T : GameState, new() {
		T GameState { get; }
	}
}