using Newtonsoft.Json;
using System;

namespace BaseSDK.Models {
	public enum QuestType { }

	public enum QuestDuration { DAILY = 0, WEEKLY = 1, }

	[Serializable]
	public class QuestState {
		public QuestType Type;
		public QuestStateProgress Progress = new();
		public long TimeCompleted = 0L;
		public long ExpiresAt = 0L;
		public QuestProgressState Status;

		public override string ToString() => JsonConvert.SerializeObject(this);
	}

	[Serializable]
	public class QuestStateProgress {
		public string InternalName = string.Empty;
		public double TargetValue;
		public double CurrentValue;
		public int NumberOfTimesAllowed;
		public int NumberOfTimesFinished;
		public QuestDuration Type;

		public override string ToString() => JsonConvert.SerializeObject(this);
	}

	public enum QuestProgressState { LOCKED = 0, PROGRESS = 1, COMPLETED = 2, CLAIMED = 3, COMPLETELYLOCKED = 4, }
}