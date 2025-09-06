using Newtonsoft.Json;
using System;

namespace BaseSDK.Models {
	[Serializable]
	public class AchievementState {
		public string InternalName = string.Empty;
		public AchievementType Type;
		public Progress Progress = new();
		public long TimeCompleted = 0L;
		public AchievementProgressState Status;

		public override string ToString() => JsonConvert.SerializeObject(this);
	}

	[Serializable]
	public struct AchievementProgress {
		public string InternalName;
		public double TargetValue;
		public RewardItem Reward;
	}

	public enum AchievementProgressState { LOCKED = 0, PROGRESS = 1, COMPLETED = 2, CLAIMED = 3, }

	public enum AchievementType {
		FIRSTRUN = 0,
	}

	[Serializable]
	public struct Progress {
		public double CurrentValue;
		public AchievementProgress AchievementProgress;
	}
}