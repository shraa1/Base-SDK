//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using BaseSDK.Extension;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaseSDK.Leaderboard {
	#region LeaderboardItemData
	[System.Serializable]
	public class LeaderboardItemData {
		#region Fields & their properties
		private string userName;
		public string UserName {
			get => userName;
			set => userName = value;
		}

		private string dateTime;
		public string DateTime {
			get => dateTime;
			set => dateTime = value;
		}

		private ulong score;
		public ulong Score {
			get => score;
			set => score = value;
		}

		private long seconds;
		public long SecondsCompletedIn {
			get => seconds;
			set => seconds = value;
		}

		private string text;
		public string Text {
			get => text;
			set => text = value;
		}

		private int rank;
		public int Rank {
			get => rank;
			set => rank = value;
		}
		#endregion

		#region Constructor
		public LeaderboardItemData() {}

		public LeaderboardItemData(JToken item) {
			UserName = (string)item["name"];
			Score = (ulong)item["score"];
			SecondsCompletedIn = (long)item["seconds"];
			Text = (string)item["text"];
			DateTime = (string)item["date"];
		}
		#endregion

		public override string ToString() => Newtonsoft.Json.JsonConvert.SerializeObject(this);
	}
	#endregion

	public class LeaderboardItem : MonoBehaviour {
		[SerializeField] private Image rankImage;
		[SerializeField] private TextMeshProUGUI userNameTMPUGUI;
		[SerializeField] private TextMeshProUGUI scoreTMPUGUI;
		[SerializeField] private TextMeshProUGUI secondsTMPUGUI;
		[SerializeField] private TextMeshProUGUI dateTMPUGUI;
		[SerializeField] private TextMeshProUGUI textTMPUGUI;

		public void Set(LeaderboardItemData data) {
			rankImage.sprite = LeaderboardsController.Instance.GetRankSprite(data.Rank);
			userNameTMPUGUI.text = data.UserName;
			scoreTMPUGUI.text = data.Score.ToString();
#if !UNITY_WEBGL
			secondsTMPUGUI.text = data.SecondsCompletedIn.ConvertSecondsToDisplayString();
			dateTMPUGUI.text = data.DateTime;
			textTMPUGUI.text = data.Text;

			//Turning off the gameobject will force a resize from the Grid/Horizontal/Vertical Layout component
			if (data.Text.IsNullOrEmpty())
				textTMPUGUI.gameObject.SetActive(false);
#else
			secondsTMPUGUI.transform.parent.gameObject.SetActive(false);
			dateTMPUGUI.gameObject.SetActive(false);
#endif
		}
	}
}