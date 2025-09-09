using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using BaseSDK.SirenixHelper;
#endif

namespace BaseSDK.Models {
	public class LeaderboardSaveData {
		[ShowInInspector] public Guid GUID { get; set; }
		[ShowInInspector] public uint Rank { get; set; }
		[ShowInInspector] public ulong Score { get; set; }
		[ShowInInspector] public string UserName { get; set; }
		[ShowInInspector] public long DateTime { get; set; }
		[ShowInInspector] public ulong EnemiesKilled { get; set; }
		public IGameLeaderboardData GameSpecificLeaderboardData { get; set; }
	}
}