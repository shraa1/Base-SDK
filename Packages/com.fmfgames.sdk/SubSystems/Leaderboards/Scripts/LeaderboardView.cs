using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaseSDK.Leaderboard {
	public class LeaderboardView : Singleton<LeaderboardView> {

		[SerializeField] private LeaderboardItem templateToDuplicate;
		[SerializeField] private ScrollRect scrollView;

		public void Setup(List<LeaderboardItemData> leaderboardItems) {
			leaderboardItems.ForEach(item => {
				var instantiated = Instantiate(templateToDuplicate, scrollView.content);
				instantiated.gameObject.SetActive(true);
				instantiated.Set(item);
			});
		}
	}
}