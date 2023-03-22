//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BaseSDK.Utils;
using BaseSDK.Extension;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BaseSDK.Leaderboard {
	public class LeaderboardsController : Singleton<LeaderboardsController> {
		#region Consts
#if UNITY_WEBGL
		[SerializeField] private string AddScore_Link = "https://www.infamy.dev/highscore/add?id={PrivateKey}&name={UserName}&score={Score}";
		[SerializeField] private string GetJSON_Link = "https://infamy.dev/highscore/json?id={PublicKey}";
#else
		[SerializeField] private string AddScore_Link = "http://dreamlo.com/lb/{PrivateKey}/add/{UserName}/{Score}/{CompletedTime}/{Data}";
		[SerializeField] private string GetJSON_Link = "http://dreamlo.com/lb/{PublicKey}/json";
#endif
		#endregion

		#region Inspector Variables
		[SerializeField] private string privateKey = string.Empty;
		[SerializeField] private string publicKey = string.Empty;
		[SerializeField] private List<Sprite> rankSprites = new List<Sprite>();
		#endregion

		#region Unity Methods
		protected virtual void Awake() {
#if UNITY_WEBGL
			AddScore_Link = AddScore_Link.Replace("{PrivateKey}", privateKey);
#else
			AddScore_Link = AddScore_Link.Replace("{PrivateKey}", privateKey);
#endif
			GetJSON_Link = GetJSON_Link.Replace("{PublicKey}", publicKey);
		}

		protected virtual void Start() {
			GetLeaderboardScores(list => LeaderboardView.Instance.Setup(list));
		}
		#endregion

		#region Public Methods
		public void AddNewLeaderboardScore(LeaderboardItemData item, Action<List<LeaderboardItemData>> onScoreFetched = null) {
			if (item != null)
#if UNITY_WEBGL
				AddNewLeaderboardScore(item.UserName, item.Score, onScoreFetched);
#else
				AddNewLeaderboardScore(item.UserName, item.Score, item.SecondsCompletedIn, item.Text, onScoreFetched);
#endif
		}

		/// <summary>
		/// Add a new score to the leaderboard
		/// </summary>
		/// <param name="userName">UserName of this entry</param>
		/// <param name="score">Score for this round/session</param>
		/// <param name="onScoreFetched">After adding the new score, a GetScore operation will be automatically performed. Use this action to do operations on the resulting list</param>
#if !UNITY_WEBGL
		/// <param name="secondsCompletedIn">Time in seconds from start of this round/session to the end</param>
		/// <param name="text">Any additional text</param>
		public void AddNewLeaderboardScore(string userName, ulong score, long secondsCompletedIn = 0, string text = "", Action<List<LeaderboardItemData>> onScoreFetched = null) {
#else
		public void AddNewLeaderboardScore(string userName, ulong score, Action<List<LeaderboardItemData>> onScoreFetched = null) {
#endif
			if (score == 0)
				return;

#if UNITY_WEBGL
			StartCoroutine(SendScore(AddScore_Link.Replace("{UserName}", userName).Replace("{Score}", score.ToString()), onScoreFetched));
#else
			StartCoroutine(SendScore(AddScore_Link.Replace("{UserName}", userName).Replace("{Score}", score.ToString()).Replace("{CompletedTime}", secondsCompletedIn.ToString()).Replace("{Data}", text), onScoreFetched));
#endif
		}

		public void GetLeaderboardScores (Action<List<LeaderboardItemData>> onScoreFetched) => StartCoroutine(GetScoreIEnumerator(onScoreFetched));

		public Sprite GetRankSprite(int rank) => rank >= rankSprites.Count ? null : rankSprites[rank];
		#endregion

		#region Private IEnumerators
		/// <summary>
		/// Add Score and Get the updated list of onScoreFetched is not null
		/// </summary>
		/// <param name="link">link to send score to</param>
		/// <param name="onScoreFetched">if this is not null, get score list after adding the score and then execute this Action</param>
		/// <returns></returns>
		protected virtual IEnumerator SendScore(string link, Action<List<LeaderboardItemData>> onScoreFetched = null) {
			using (var request = UnityWebRequest.Get(link)) {
				yield return request.SendWebRequest();

				switch (request.result) {
					case UnityWebRequest.Result.Success:
						if(onScoreFetched != null)
							GetLeaderboardScores(onScoreFetched);
						break;

					case UnityWebRequest.Result.ConnectionError:
					case UnityWebRequest.Result.DataProcessingError:
					case UnityWebRequest.Result.ProtocolError:
						MessageBox.Show("Error", "Got an error when saving score to leaderboard", i => {
							if (i == 0)
								StartCoroutine(SendScore(link, onScoreFetched));
						}, "Retry", "Okay");
						break;

					case UnityWebRequest.Result.InProgress:
						break;
				}
			}
		}

		protected virtual IEnumerator GetScoreIEnumerator(Action<List<LeaderboardItemData>> onScoreFetched) {
			using (var request = UnityWebRequest.Get(GetJSON_Link)) {
				yield return request.SendWebRequest();

				switch (request.result) {
					case UnityWebRequest.Result.Success:
						var list = new List<LeaderboardItemData>();

						if (GetJSON_Link.Contains("infamy")) {
							//If the list is empty, it returns an object rather than an array
							try {
								var infamyList = JsonConvert.DeserializeObject<JArray>(request.downloadHandler.text);
								foreach (var kvp in infamyList)
									//TODO currently infamy.dev returns the rank not on index based. If it changes, make change here
									list.Add(new LeaderboardItemData { Rank = (int)kvp["rank"] - 1, UserName = (string)kvp["name"], Score = (ulong)kvp["score"] });
							}
							catch (InvalidCastException) { }

							onScoreFetched?.Invoke(list);
						}
					else {
							var dreamloDict = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(request.downloadHandler.text);
							if (dreamloDict["dreamlo"]["leaderboard"].ToString().IsNullOrEmpty()) {
								onScoreFetched?.Invoke(list);
								break;
							}

							var leaderboardArr = dreamloDict["dreamlo"]["leaderboard"]["entry"];

							if (leaderboardArr as JArray != null) {
								var i = 0;
								foreach (var kvp in leaderboardArr as JArray) {
									list.Add(new LeaderboardItemData(kvp) { Rank = i });
									i++;
								}
							}
							else if (leaderboardArr != null)
								list.Add(new LeaderboardItemData(leaderboardArr));

							onScoreFetched?.Invoke(list);
						}
						break;

					case UnityWebRequest.Result.ConnectionError:
					case UnityWebRequest.Result.DataProcessingError:
					case UnityWebRequest.Result.ProtocolError:
						MessageBox.Show("Error", "Got an error when getting the latest scores from the leaderboard", i => {
							if (i == 0)
								StartCoroutine(GetScoreIEnumerator(onScoreFetched));
						}, "Retry", "Okay");
						break;

					case UnityWebRequest.Result.InProgress:
						break;
				}
			}
			yield return null;
		}
		#endregion
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(LeaderboardsController))]
	public class LeaderboardsControllerEditor : Editor {

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Open Website to get Public & Private Keys"))
#if UNITY_WEBGL
				Application.OpenURL("https://www.infamy.dev");
#else
				Application.OpenURL("http://dreamlo.com");
#endif
		}
	}
#endif
}