using System;
using BaseSDK.Models;

namespace BaseSDK.Services {
	public interface IAchievementService : IService {
		void Unlock<T> (T id) where T : Enum;
		bool IsUnlocked<T> (T id) where T : Enum;
		void UpdateAchievementProgress<T> (T id, double amount = 1, bool forceSteamUpdate = false, bool shouldSteamStatsUpdate = true) where T : Enum;
		AchievementState GetAchievementState (string achievementName);
	}
}