using BaseSDK.Models;
using BaseSDK.Services;

namespace BaseSDK {
	public interface IGameStateService<T> : IService where T : GameState, new() {
		T GameState { get; }
	}
}