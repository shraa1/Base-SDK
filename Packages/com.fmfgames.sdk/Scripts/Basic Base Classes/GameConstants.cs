using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace BaseSDK {
	public sealed class GameConstants {
		#region Private Constructor
		private GameConstants () { }
		#endregion Private Constructor

		#region Properties
		/// <summary>
		/// Desktop folder's path. Should work fine in MacOS as well.
		/// </summary>
		public static string DESKTOP_PATH => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		#endregion Properties

		#region AssetBundle Things
		/// <summary>
		/// File extension for asset bundles depending on platform
		/// </summary>
#if UNITY_ANDROID
		public const string ASSETBUNDLE_PLATFORM = "droid";
#elif UNITY_IOS
		public const string ASSETBUNDLE_PLATFORM = "ios";
#elif UNITY_STANDALONE_WIN
		public const string ASSETBUNDLE_PLATFORM = "win";
#elif UNITY_STANDALONE_OSX
		public const string ASSETBUNDLE_PLATFORM = "mac";
#endif
		#endregion AssetBundle Things

		#region File Extensions
		public const string ALL_FILETYPE_FILTER = "*.*";
		public const string SPRITEATLAS_FILE_EXTENSION = ".spriteatlas";
		public const string TEXT_FILE_EXTENSION = ".txt";
		#endregion File Extensions

		#region Special characters
		public const char DOUBLEQUOTE = '\"';
		public const char LINE_BREAK = '\n';
		public const char LINE_BREAK2 = '\u000d';
		public const char SLASH = '/';
		public const char PERIOD = '.';
		public const char SPACE = ' ';
		public const char COMMA = ',';
		#endregion Special characters

		#region Commonly Used Numbers
		public const float TWO_FIFTY_FIVE_FLOAT = 255f;
		public const int SECONDS_IN_AN_HOUR = 3600;
		#endregion Commonly Used Numbers

		#region Currency Things
		/// <summary>
		/// Culture names can be found on http://azuliadesigns.com/list-net-culture-country-codes/
		/// </summary>
		public static string DEFAULT_CULTURE_NAME = "en-US";

		/// <summary>
		/// Used to format currency for display in the game.
		/// </summary>
		public const string CURRENCY_FORMAT = "{0:C}";
		#endregion Currency Things

		#region Other Constants
		public const float OFF_SCREEN_POS_WORLD_SPACE = 100000f;
		public const string ANDROID_FILE_PREFIX = "file://";
		public const float BYTES_TO_MEGABYTES_MULTIPLIER = 1f / (1024f * 1024f);
		#endregion Other Constants

		#region Other Static Readonlys
		public static readonly Color DEFAULT_HIGHLIGHT_COLOUR_BUTTON2D = new(0.2f, 0.2f, 0.2f, 1f);
		public static readonly DateTime k_EPOCH_DATETIME_UTC = DateTime.UnixEpoch;
		public static readonly WaitForEndOfFrame EOF = new();

		public static readonly JsonSerializerSettings JsonSerializerSettings = new() {
			ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			TypeNameHandling = TypeNameHandling.Auto,
			MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead,
			Formatting = Formatting.Indented
		};
		#endregion Other Static Readonlys

		#region File Paths Related
		public static Func<string> GameName;
		public const string SAVED_GAME_FILES_FOLDERNAME = "Saved Game Files";
		public const string FIRST_LAUNCH_SAVED_FILE_NAME = "FirstLaunch.playedAlready";

		public static readonly string SAVE_FOLDER_PATH = Path.Combine(Application.dataPath, SAVED_GAME_FILES_FOLDERNAME);
		public static readonly string FIRST_LAUNCH_SAVED_FILE_PATH = Path.Combine(SAVE_FOLDER_PATH, FIRST_LAUNCH_SAVED_FILE_NAME);
		#endregion File Paths Related
	}
}