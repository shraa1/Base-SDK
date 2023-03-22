//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#pragma warning disable IDE0065 // Misplaced using directive
using System;
using System.Linq;
using System.Text;
using System.Threading;
using Discord;
using BaseSDK.Extension;
using UnityEngine;
using Disc = Discord;

namespace BaseSDK {
	public class DiscordController : Singleton<DiscordController> {
		#region Variables
		private Disc.Discord discord;
#pragma warning disable IDE0044 // Add readonly modifier
		[SerializeField] private bool requireDiscordForGame;
		[SerializeField] private long clientID;
		[SerializeField] private string discordStateText;
		[SerializeField] private string discordDetailsText;
		[SerializeField] private string smallImageName;
		[SerializeField] private string largeImageName;
		[SerializeField] private string largeText;
		[SerializeField] private string smallText;

        [SerializeField] private long myUserId;
#pragma warning restore IDE0044 // Add readonly modifier
		#endregion

		#region Properties
		private Disc.Discord Discord => discord ??= new Disc.Discord(clientID, (ulong)(requireDiscordForGame ? Disc.CreateFlags.Default : Disc.CreateFlags.NoRequireDiscord));

		private ActivityManager ActivityManager => Discord.GetActivityManager();

		private UserManager UserManager => Discord.GetUserManager();

		private AchievementManager AchievementManager => Discord.GetAchievementManager();

		private LobbyManager LobbyManager => Discord.GetLobbyManager();

        private ApplicationManager ApplicationManager => Discord.GetApplicationManager();

        private ImageManager ImageManager => Discord.GetImageManager();

        private StoreManager StoreManager => Discord.GetStoreManager();
        
        private StorageManager StorageManager => Discord.GetStorageManager();

        private RelationshipManager RelationshipManager => Discord.GetRelationshipManager();

        private OverlayManager OverlayManager => discord.GetOverlayManager();
        #endregion

        #region Unity Methods
        protected void Start() {
            // Get the current locale. This can be used to determine what text or audio the user wants.
            Debug.Log($"Current Locale: {ApplicationManager.GetCurrentLocale()}");
            // Get the current branch. For example alpha or beta.
            Debug.Log($"Current Branch: {ApplicationManager.GetCurrentBranch()}");

            // If you want to verify information from your game's server then you can grab the access token and send it to your server.
            // This automatically looks for an environment variable passed by the Discord client, if it does not exist the Discord client will focus itself for manual authorization.
            // By-default the SDK grants the identify and rpc scopes. Read more at https://discordapp.com/developers/docs/topics/oauth2
            ApplicationManager.GetOAuth2Token((Result result, ref OAuth2Token oauth2Token) => Debug.Log($"Access Token {oauth2Token.AccessToken}"));

            // Received when someone accepts a request to spectate
            ActivityManager.OnActivitySpectate += secret => Debug.LogFormat("OnSpectate {0}", secret);
            // A join request has been received. Render the request on the UI.
            ActivityManager.OnActivityJoinRequest += (ref User user) => Debug.Log($"OnJoinRequest {user.Id} {user.Username}");

            var activity = new Activity() {
				Details = discordDetailsText,
				State = discordStateText,
				Timestamps = new ActivityTimestamps() { Start = DateTime.UtcNow.ConvertToUnixTime() },
				Assets = new ActivityAssets() { SmallImage = smallImageName, LargeImage = largeImageName, LargeText = largeText, SmallText = smallText },
				//Secrets = new ActivitySecrets() { Spectate = LobbyManager.GetLobbyActivitySecret }
			};
			ActivityManager.UpdateActivity(activity, response => {
				if (response == Result.Ok)
					Debug.Log("Discord status set");
				else
					Debug.Log("Discord status failed");
			});

            //Debug.Log(AchievementManager.CountUserAchievements());
            //Debug.Log(UserManager);
            //Debug.Log(UserManager.GetCurrentUser());

            //FetchAvatar(ImageManager(), myClientId, sprite => FindObjectOfType<SpriteRenderer>().sprite = sprite);
        }

        protected void Update() {
			Discord.RunCallbacks();
		}
		#endregion

		#region Helpers
		public void Quit() {
			Shutdown(() =>
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false
#else
				Application.Quit()
#endif
			);
		}

		public void Shutdown(Action onCompleteAction) {
			try {
				ActivityManager.ClearActivity(callback => {
					Discord.Dispose();
					Discord.RunCallbacks();
					onCompleteAction?.Invoke();
				});
				Discord.RunCallbacks();
			}
			catch { }
		}
        #endregion

        #region Helper methods from the example script in Discord's SDK

        void Main() {
            // Received when someone accepts a request to join or invite.
            // Use secrets to receive back the information needed to add the user to the group/party/match
            ActivityManager.OnActivityJoin += secret => {
                Debug.Log($"OnJoin {secret}");
                LobbyManager.ConnectLobbyWithActivitySecret(secret, (Result result, ref Lobby lobby) => {
                    Debug.Log($"Connected to lobby: {lobby.Id}");
                    LobbyManager.ConnectNetwork(lobby.Id);
                    LobbyManager.OpenNetworkChannel(lobby.Id, 0, true);
                    LobbyManager.GetMemberUsers(lobby.Id).ForEach(x => LobbyManager.SendNetworkMessage(x.Id, x.Id, 0, Encoding.UTF8.GetBytes($"Hello, {x.Username}!")));
                    UpdateActivity(Discord, lobby);
                });
            };

            // An invite has been received. Consider rendering the user / activity on the UI.
            ActivityManager.OnActivityInvite += (ActivityActionType Type, ref User user, ref Activity activity2) => {
                Debug.LogFormat("OnInvite {0} {1} {2}", Type, user.Username, activity2.Name);
                // activityManager.AcceptInvite(user.Id, result =>
                // {
                //     Debug.LogFormat("AcceptInvite {0}", result);
                // });
            };
            // This is used to register the game in the registry such that Discord can find it.
            // This is only needed by games acquired from other platforms, like Steam.
            ActivityManager.RegisterCommand();

            // The auth manager fires events as information about the current user changes.
            // This event will fire once on init.
            // GetCurrentUser will error until this fires once.
            UserManager.OnCurrentUserUpdate += () => {
                var currentUser = UserManager.GetCurrentUser();
                Debug.Log(currentUser.Username + " =>" + currentUser.Id.ToString());
            };

            // If you store Discord user ids in a central place like a leaderboard and want to render them.
            // The users manager can be used to fetch arbitrary Discord users. This only provides basic
            // information and does not automatically update like relationships.
            UserManager.GetUser(450795363658366976, (Result result, ref User user) => {
                if (result == Result.Ok) {
                    Debug.LogFormat("user fetched: {0}", user.Username);

                    // Request users's avatar data.
                    // This can only be done after a user is successfully fetched.
                    //FetchAvatar(imageManager, user.Id);
                }
                else
                    Debug.LogFormat("user fetch error: {0}", result);
            });

            // It is important to assign this handle right away to get the initial relationships refresh.
            // This callback will only be fired when the whole list is initially loaded or was reset
            RelationshipManager.OnRefresh += () => {
                // Filter a user's relationship list to be just friends
                RelationshipManager.Filter((ref Relationship relationship) => relationship.Type == RelationshipType.Friend);
                // Loop over all friends a user has.
                Debug.LogFormat("relationships updated: {0}", RelationshipManager.Count());
                for (var i = 0; i < Math.Min(RelationshipManager.Count(), 10); i++) {
                    // Get an individual relationship from the list
                    var r = RelationshipManager.GetAt((uint)i);
                    Debug.LogFormat("relationships: {0} {1} {2} {3}", r.Type, r.User.Username, r.Presence.Status, r.Presence.Activity.Name);

                    // Request relationship's avatar data.
                    FetchAvatar(ImageManager, r.User.Id);
                }
            };
            // All following relationship updates are delivered individually.
            // These are fired when a user gets a new friend, removes a friend, or a relationship's presence changes.
            RelationshipManager.OnRelationshipUpdate += (ref Relationship r) => Debug.LogFormat("relationship updated: {0} {1} {2} {3}", r.Type, r.User.Username, r.Presence.Status, r.Presence.Activity.Name);
            LobbyManager.OnLobbyMessage += (lobbyID, userID, data) => Debug.LogFormat("lobby message: {0} {1}", lobbyID, Encoding.UTF8.GetString(data));
            LobbyManager.OnNetworkMessage += (lobbyId, userId, channelId, data) => Debug.LogFormat("network message: {0} {1} {2} {3}", lobbyId, userId, channelId, Encoding.UTF8.GetString(data));
            LobbyManager.OnSpeaking += (lobbyID, userID, speaking) => Debug.LogFormat("lobby speaking: {0} {1} {2}", lobbyID, userID, speaking);
            // Create a lobby.
            var transaction = LobbyManager.GetLobbyCreateTransaction();
            transaction.SetCapacity(6);
            transaction.SetType(LobbyType.Public);
            transaction.SetMetadata("a", "123");
            transaction.SetMetadata("a", "456");
            transaction.SetMetadata("b", "111");
            transaction.SetMetadata("c", "222");

            LobbyManager.CreateLobby(transaction, (Result result, ref Lobby lobby) => {
                if (result != Result.Ok)
                    return;

                // Check the lobby's configuration.
                Debug.LogFormat("lobby {0} with capacity {1} and secret {2}", lobby.Id, lobby.Capacity, lobby.Secret);

                // Check lobby metadata.
                foreach (var key in new string[] { "a", "b", "c" })
                    Debug.LogFormat("{0} = {1}", key, LobbyManager.GetLobbyMetadataValue(lobby.Id, key));

                // Print all the members of the lobby.
                foreach (var user in LobbyManager.GetMemberUsers(lobby.Id))
                    Debug.LogFormat("lobby member: {0}", user.Username);

                // Send everyone a message.
                LobbyManager.SendLobbyMessage(lobby.Id, "Hello from C#!", _ => Debug.Log("sent message"));

                // Update lobby.
                var lobbyTransaction = LobbyManager.GetLobbyUpdateTransaction(lobby.Id);
                lobbyTransaction.SetMetadata("d", "e");
                lobbyTransaction.SetCapacity(16);
                LobbyManager.UpdateLobby(lobby.Id, lobbyTransaction, _ => Debug.Log("lobby has been updated"));

                // Update a member.
                var lobbyID = lobby.Id;
                var userID = lobby.OwnerId;
                var memberTransaction = LobbyManager.GetMemberUpdateTransaction(lobbyID, userID);
                memberTransaction.SetMetadata("hello", "there");
                LobbyManager.UpdateMember(lobbyID, userID, memberTransaction, _ => Debug.LogFormat("lobby member has been updated: {0}", LobbyManager.GetMemberMetadataValue(lobbyID, userID, "hello")));

                // Search lobbies.
                var query = LobbyManager.GetSearchQuery();
                // Filter by a metadata value.
                query.Filter("metadata.a", LobbySearchComparison.GreaterThan, LobbySearchCast.Number, "455");
                query.Sort("metadata.a", LobbySearchCast.Number, "0");
                // Only return 1 result max.
                query.Limit(1);
                LobbyManager.Search(query, _ => {
                    Debug.LogFormat("search returned {0} lobbies", LobbyManager.LobbyCount());
                    if (LobbyManager.LobbyCount() == 1)
                        Debug.LogFormat("first lobby secret: {0}", LobbyManager.GetLobby(LobbyManager.GetLobbyId(0)).Secret);
                });

                // Connect to voice chat.
                LobbyManager.ConnectVoice(lobby.Id, _ => Debug.Log("Connected to voice chat!"));

                // Setup networking.
                LobbyManager.ConnectNetwork(lobby.Id);
                LobbyManager.OpenNetworkChannel(lobby.Id, 0, true);

                // Update activity.
                UpdateActivity(Discord, lobby);
            });

            /*
            overlayManager.OnOverlayLocked += locked => Debug.Log($"Overlay Locked: {locked}");
            overlayManager.SetLocked(false);
            */

            var contents = new byte[20000];
            var random = new System.Random();
            random.NextBytes(contents);
            Debug.LogFormat("storage path: {0}", StorageManager.GetPath());
            StorageManager.WriteAsync("foo", contents, res => {
                var files = StorageManager.Files();
                foreach (var file in files)
                    Debug.LogFormat("file: {0} size: {1} last_modified: {2}", file.Filename, file.Size, file.LastModified);
                StorageManager.ReadAsyncPartial("foo", 400, 50, (result, data) => Debug.LogFormat("partial contents of foo match {0}", Enumerable.SequenceEqual(data, new ArraySegment<byte>(contents, 400, 50))));
                StorageManager.ReadAsync("foo", (result, data) => {
                    Debug.LogFormat("length of contents {0} data {1}", contents.Length, data.Length);
                    Debug.LogFormat("contents of foo match {0}", Enumerable.SequenceEqual(data, contents));
                    Debug.LogFormat("foo exists? {0}", StorageManager.Exists("foo"));
                    StorageManager.Delete("foo");
                    Debug.LogFormat("post-delete foo exists? {0}", StorageManager.Exists("foo"));
                });
            });

            StoreManager.OnEntitlementCreate += (ref Entitlement entitlement) => Debug.LogFormat("Entitlement Create1: {0}", entitlement.Id);

            // Start a purchase flow.
            // storeManager.StartPurchase(487507201519255552, result =>
            // {
            //     if (result == Discord.Result.Ok)
            //     {
            //         Debug.LogFormat("Purchase Complete");
            //     }
            //     else
            //     {
            //         Debug.LogFormat("Purchase Canceled");
            //     }
            // });

            // Get all entitlements.
            StoreManager.FetchEntitlements(result => {
                if (result == Result.Ok) {
                    foreach (var entitlement in StoreManager.GetEntitlements())
                        Debug.LogFormat("entitlement: {0} - {1} {2}", entitlement.Id, entitlement.Type, entitlement.SkuId);
                }
            });

            // Get all SKUs.
            StoreManager.FetchSkus(result => {
                if (result == Result.Ok) {
                    foreach (var sku in StoreManager.GetSkus())
                        Debug.LogFormat("sku: {0} - {1} {2}", sku.Name, sku.Price.Amount, sku.Price.Currency);
                }
            });

            // Pump the event look to ensure all callbacks continue to get fired.
            try {
                while (true) {
                    Discord.RunCallbacks();
                    LobbyManager.FlushNetwork();
                    Thread.Sleep(1000 / 60);
                }
            }
            finally {
                Discord.Dispose();
            }
        }










        // Request user's avatar data. Sizes can be powers of 2 between 16 and 2048
        private void FetchAvatar(ImageManager imageManager, long userID, Action<Sprite> action = null) {
			imageManager.Fetch(ImageHandle.User(userID), (result, handle) => {
				if (result == Result.Ok) {
					// You can also use GetTexture2D within Unity.
					// These return raw RGBA.
					var data = imageManager.GetData(handle);
					Debug.LogError($"image updated {handle.Id} {data.Length}");
#if UNITY_EDITOR || UNITY_STANDALONE
					var texture2d = imageManager.GetTexture(handle);
					action?.Invoke(texture2d.Flip(Extensions.TextureFlipDirection.Both).ToSprite());
#endif
				}
				else
					Debug.LogError($"Image error {handle.Id}");
			});
		}

		// Update user's activity for your game.
		// Party and secrets are vital.
		// Read https://discordapp.com/developers/docs/rich-presence/how-to for more details.
		private void UpdateActivity(Disc.Discord discord, Lobby lobby) {
			var activity = new Activity {
				State = "olleh",
				Details = "foo details",
				Timestamps = { Start = 5, End = 6 },
				Assets = {
					LargeImage = "foo largeImageKey",
					LargeText = "foo largeImageText",
					SmallImage = "foo smallImageKey",
					SmallText = "foo smallImageText",
				},
				Party = {
					Id = lobby.Id.ToString(),
					Size = {
						CurrentSize = LobbyManager.MemberCount(lobby.Id),
						MaxSize = (int)lobby.Capacity
					}
				},
				Secrets = { Join = LobbyManager.GetLobbyActivitySecret(lobby.Id) },
				Instance = true
			};

			ActivityManager.UpdateActivity(activity, result => {
				Debug.Log($"Update Activity {result}");

				// Send an invite to another user for this activity.
				// Receiver should see an invite in their DM.
				// Use a relationship user's ID for this.
				// activityManager.SendInvite(
				//       364843917537050624,
				//       Disc.ActivityActionType.Join,
				//       "",
				//       inviteResult => {
				//           Debug.Log("Invite {0}", inviteResult);
				//       }
				//   );
			});
		}
		#endregion
	}
}

#if UNITY_EDITOR
namespace BaseSDK.EditorScript {
	using UnityEditor;

	[CustomEditor(typeof(DiscordController))]
	public class DiscordControllerEditor : Editor {
		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if (GUILayout.Button("Open Discord Documentation"))
				Application.OpenURL("https://discord.com/developers/docs/intro");
		}
	}
}
#endif