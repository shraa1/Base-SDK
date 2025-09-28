#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX) || !STEAMWORKS_NET
#define DISABLESTEAMWORKS
#endif

namespace BaseSDK.Services {
	public interface ISteamService : IService {
#if !DISABLESTEAMWORKS
		bool IsSteamInitialized { get; }
		bool IsSteamRunning { get; }

		void UpdateStats(string statName, double value, bool forceUpdate = false);
		void StoreStats();
#endif
	}
}