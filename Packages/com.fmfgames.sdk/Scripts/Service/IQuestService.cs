using System;
using BaseSDK.Models;

namespace BaseSDK.Services {
	public interface IQuestService : IService {
		void RegisterQuestsForProgressTracking<T> (T questType, IQuestItem questItem) where T : Enum;
		void UpdateQuestProgress<T> (T questType, double amount = 1d) where T : Enum;
		bool IsCompleted<T> (T questType) where T : Enum;
		void OnQuestCompleted (QuestState questState);
	}

	public interface IQuestItem {
		void UpdateProgress (double amount);
	}
}