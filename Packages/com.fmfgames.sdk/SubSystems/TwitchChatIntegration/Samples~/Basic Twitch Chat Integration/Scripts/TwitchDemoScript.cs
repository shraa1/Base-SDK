using System;
using System.Collections;
using System.Collections.Generic;
using BaseSDK.Extension;
using BaseSDK.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace BaseSDK.Demo {
	public class TwitchDemoScript : MonoBehaviour {

		#region Custom DataTypes
		[Serializable] private class TwitchChatMessageAutoReplies {
			public List<string> inMessages = new List<string>();
			public List<string> outMessages = new List<string>();
		}
		#endregion

		[SerializeField] private List<TwitchChatMessageAutoReplies> twitchChatMessageAutoReplies = new List<TwitchChatMessageAutoReplies>();

		private void Start () {
			TwitchChatIntegration.Instance.ConnectToTwitch();

			TwitchChatIntegration.Instance.OnChatMessageReceived.AddListener((name, msg) => {
				Debug.Log($"Message From {name}: {msg}");

				var find = twitchChatMessageAutoReplies.Find(x => x.inMessages.Contains(msg));
				if(find != null)
					TwitchChatIntegration.Instance.SendMessageToTwitch(find.outMessages.Random());
			});
		}
	}
}