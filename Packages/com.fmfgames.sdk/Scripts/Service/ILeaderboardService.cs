using System.Collections;
using System.Collections.Generic;
using BaseSDK.Models;

namespace BaseSDK.Services {
	public interface ILeaderboardService : IService {
		List<LeaderboardSaveData> LeaderboardSaveDatas { get; }
		long LastFetchedTimeStamp { get; }

		IEnumerator FetchLeaderboardScores();
	}
}