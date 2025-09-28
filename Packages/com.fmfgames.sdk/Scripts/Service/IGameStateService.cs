using BaseSDK.Models;

namespace BaseSDK.Services {
	public interface IGameStateService : IService {
		IGameState GameState { get; }
	}
}