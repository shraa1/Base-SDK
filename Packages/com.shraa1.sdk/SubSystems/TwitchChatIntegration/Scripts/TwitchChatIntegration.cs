using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Events;
using BaseSDK.Extension;
using TMPro;

namespace BaseSDK.Utils {
	public class TwitchChatIntegration : Singleton<TwitchChatIntegration> {
		#region Variables
		/// <summary>
		/// Connection to the twitch chat API
		/// </summary>
		private TcpClient twitchClient;

		/// <summary>
		/// Reader for reading the twitchClient's messages
		/// </summary>
		private StreamReader streamReader;

		/// <summary>
		/// Writer for writing to the twitchClient's messages
		/// </summary>
		private StreamWriter streamWriter;


		/// <summary>
		/// URL to connect twitchClient to. Remains constant, provided by Twitch.
		/// </summary>
		private const string URL = "irc.chat.twitch.tv";
		/// <summary>
		/// Port to connect to twitchClient chat.
		/// </summary>
		private const int PORT = 6667;

		/// <summary>
		/// Connect to twitch on awake with the serialized values
		/// </summary>
		[SerializeField] private bool connectAtAwake = false;

		/// <summary>
		/// Username to connect to twitch
		/// Read more at https://dev.twitch.tv/docs/irc/guide#connecting-to-twitch-irc
		/// </summary>
		[SerializeField] private string userName;
		/// <summary>
		/// Username to connect to twitch
		/// Read more at https://dev.twitch.tv/docs/irc/guide#connecting-to-twitch-irc
		/// </summary>
		public string UserName { get => userName; set => userName = value; }

		/// <summary>
		/// OAuth key can be gotten from https://twitchapps.com/tmi
		/// The auth key should start with oauth:
		/// For example, if the password is abcd, it should be oauth:abcd
		/// Read more at https://dev.twitch.tv/docs/irc/guide#connecting-to-twitch-irc
		/// </summary>
		[SerializeField, HideInInspector] private string OauthPassword;
		/// <summary>
		/// OAuth key can be gotten from https://twitchapps.com/tmi
		/// The auth key should start with oauth:
		/// For example, if the password is abcd, it should be oauth:abcd
		/// Read more at https://dev.twitch.tv/docs/irc/guide#connecting-to-twitch-irc
		/// </summary>
		public string OAuthPassword { get => OauthPassword; set => OauthPassword = value; }

		/// <summary>
		/// Channel name to join
		/// Read more at https://dev.twitch.tv/docs/irc/guide#connecting-to-twitch-irc
		/// </summary>
		[SerializeField] private string channelName;
		/// <summary>
		/// Channel name to join
		/// Read more at https://dev.twitch.tv/docs/irc/guide#connecting-to-twitch-irc
		/// </summary>
		public string ChannelName { get => channelName; set => channelName = value; }

		/// <summary>
		/// Action to perform when a message is received from twitch chat.
		/// 1st parameter is the userName.
		/// 2nd parameter is the actual message;
		/// </summary>
		[SerializeField] private UnityEvent<string, string> onChatMessageReceived;
		/// <summary>
		/// Action to perform when a message is received from twitch chat.
		/// 1st parameter is the userName.
		/// 2nd parameter is the actual message;
		public UnityEvent<string, string> OnChatMessageReceived { get => onChatMessageReceived; set => onChatMessageReceived = value; }
		#endregion

		#region Constants
		private const string PRIVMSG = "PRIVMSG";
		private const string PART = "PART";
		private const string JOIN = "JOIN";
		private const string NICK = "NICK";
		private const string PASS = "PASS";
		private const string PING = "PING: tmi.twitch.tv";
		#endregion

		#region Unity Methods
		protected virtual void Awake () {
			if (connectAtAwake)
				ConnectToTwitch(UserName, OAuthPassword, ChannelName);
		}

		protected virtual void Update () {
			//twitchClient was null, must mean that it was not connected.
			if (twitchClient == null || !twitchClient.Connected)
				return;

			//Checks if the client has any messages
			if (twitchClient.Available > 0) {
				//Get the entire message
				var message = streamReader.ReadLine();

				//Debug.Log(message);
				//:twitchintegrationbot1!twitchintegrationbot1@twitchintegrationbot1.tmi.twitch.tv PRIVMSG #shraa1 :ggwp

				if (message.Contains(PRIVMSG)) {
					var name = message.Substring(1, message.IndexOf("!") - 1);
					var actualMessage = message.Substring(message.IndexOf(":", 1) + 1);

					//Debug.Log($"{name}: {actualMessage}");
					OnChatMessageReceived?.Invoke(name, actualMessage);
				}

				//Stay connected.
				//From the documentation: About once every five minutes, the server will send you a PING :tmi.twitch.tv. To ensure that your connection to the server is not prematurely terminated, reply with PONG :tmi.twitch.tv.
				if (message.Contains(PING) && twitchClient != null && twitchClient.Connected && streamWriter != null) {
					streamWriter.WriteLine("PONG: tmi.twitch.tv");
					streamWriter.Flush();
				}
			}
		}
		#endregion

		#region Connection related methods
		public void ConnectToTwitch () => ConnectToTwitch(UserName, OAuthPassword, ChannelName);

		/// <summary>
		/// Connect to Twitch API.
		/// Note: PING-PONG messages are considered and handled, so we will not disconnect with 5 minutes of inactivity. After a connection call, there should be a proper disconnect call
		/// </summary>
		/// <param name="userName">Username of the twitch account.</param>
		/// <param name="OauthPassword">Secret OAuth password.</param>
		/// <param name="channelName">Channel to join when connecting, so that we can join the channel when logging in</param>
		public void ConnectToTwitch (string userName, string OauthPassword, string channelName = null) {
			LeaveChannel();

			//Connect to twitch chat client
			twitchClient = new TcpClient(URL, PORT);

			//Get the reader and writer streams
			streamReader = new StreamReader(twitchClient.GetStream());
			streamWriter = new StreamWriter(twitchClient.GetStream());

			//Write the data needed to send to twitch api to get connected
			streamWriter.WriteLine($"{PASS} {OAuthPassword = OauthPassword}");
			streamWriter.WriteLine($"{NICK} {UserName = userName.ToLower()}");

			if (!channelName.IsNullOrEmpty())
				JoinChannel(channelName);

			//This sends the data to the twitch API, basically making a connection call
			streamWriter.Flush();
		}

		/// <summary>
		/// Joins a channel with the given Channel Name
		/// </summary>
		/// <param name="channelName">Joins the channel. Should not be null or empty, otherwise a System.Exception will be thrown</param>
		public void JoinChannel (string channelName) {
			if (channelName.IsNullOrEmpty())
				throw new System.Exception("channelName should not be " + (channelName == null ? "null" : "Empty"));

			if (twitchClient != null && twitchClient.Connected && streamWriter != null) {
				//Join this channel
				streamWriter.WriteLine($"{JOIN} #{ChannelName = channelName.ToLower()}");
				streamWriter.Flush();
			}
		}

		/// <summary>
		/// Disconnect from a joined channel, and then disconnect the TcpClient properly
		/// </summary>
		public void LeaveChannel () {
			if (twitchClient != null && twitchClient.Connected && streamWriter != null) {
				//Leave the channel before disconnecting the TcpClient
				streamWriter.WriteLine($"{PART} #{ChannelName}");
				streamWriter.Flush();

				//Debug.Log("There was already an active connection, so disconnecting that first");
				twitchClient.GetStream().Close();
				twitchClient.Close();
				twitchClient.Dispose();
			}
		}

		/// <summary>
		/// Sends a message from the app/unity editor to twitch. Use this after a twitch connection is active
		/// </summary>
		/// <param name="msg">The message to send to twitch.
		/// Note: This message is not received back in the streamreader, so no worries about filtering the messages. But if the same user sends the message from some other app/browser from twitch, it does get recognized.</param>
		public void SendMessageToTwitch (string msg) {
			if (twitchClient != null && twitchClient.Connected && streamWriter != null) {
				streamWriter.WriteLine($"{PRIVMSG} #{ChannelName} :{msg}");
				streamWriter.Flush();
			}
		}
		#endregion

		#region UI Helper method
		//#if TMP_PRESENT
		/// <summary>
		/// Sends the text from the InputField as a message from the app/unity editor to twitch. Use this after a twitch connection is active
		/// </summary>
		/// <param name="inputField"></param>
		public void SendMessageToTwitch (TMP_InputField inputField) => SendMessageToTwitch(inputField.text);
		//#endif
		#endregion
	}
}