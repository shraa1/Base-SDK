using BaseSDK.Extension;
using BaseSDK.Utils;
using UnityEditor;
using UnityEngine;

namespace BaseSDK.EditorScripts {
	[CustomEditor(typeof(TwitchChatIntegration))]
	public class TwitchChatIntegrationEditor : Editor {
		private UnityEditor.PackageManager.Requests.AddRequest addPackage_Result;

		private void OnEnable () {
			EditorApplication.update += Update;
		}

		private void Update () {
			if (addPackage_Result != null && addPackage_Result.IsCompleted) {
				addPackage_Result = null;
				PackageDependenciesHelper.AddTextMeshProEssentials();
			}
		}

		public override void OnInspectorGUI () {
			var connectAtAwake = serializedObject.FindProperty("connectAtAwake");
			connectAtAwake.boolValue = EditorGUILayout.Toggle("Connect At Awake", connectAtAwake.boolValue);

			var userName = serializedObject.FindProperty("userName");
			userName.stringValue = EditorGUILayout.TextField("Username", userName.stringValue);

			EditorGUILayout.HelpBox("OAuth Password is meant to be secret, do not share it with anyone. Usually, do not serialize this, preferably get this value from an InputField or a text file or something similar", MessageType.Warning);
			var password = serializedObject.FindProperty("OauthPassword");
			password.stringValue = EditorGUILayout.TextField("OAuth Password", password.stringValue);

			if (password.stringValue.IsNullOrEmpty() && GUILayout.Button("Get OAuth Password"))
				Application.OpenURL("https://twitchapps.com/tmi");

			GUILayout.Space(40);

			EditorGUILayout.HelpBox("If you want to Connect to twitch and not join a channel immediately, leave the channel name empty", MessageType.Warning);
			var channelName = serializedObject.FindProperty("channelName");
			channelName.stringValue = EditorGUILayout.TextField("Channel To Join", channelName.stringValue);

			var onChatMessageReceived = serializedObject.FindProperty("onChatMessageReceived");
			EditorGUILayout.PropertyField(onChatMessageReceived);

			serializedObject.ApplyModifiedProperties();
		}
	}
}