using UnityEngine;
using System;

namespace BaseSDK {
	public class GameConstants {
		#region Properties
		/// <summary>
		/// Desktop folder's path. Should work fine in MacOS as well.
		/// </summary>
		public static string DESKTOP_PATH => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		#endregion Properties

		#region Encryption Things
		/// <summary>
		/// AES Encryption key -> Update this key for each project
		/// </summary>
		public const string AES_KEY = "PSVJQRk9QTEpNVU1DWUZCRVFGV1VVT0ZOV1RRU1NaWQ=";

		/// <summary>
		/// AES Encryption IV -> Update this for each project
		/// </summary>
		public const string AES_IV = "ILUVYOU2k48pNVU1DWUZCRVFGV1VVT0ZOV1RRU1NaWQ=";

		/// <summary>
		/// RSA Encryption key -> Update this for each project if used
		/// </summary>
		public const string RSA_KEY = "1234";

		/// <summary>
		/// Padding char for custom encryption
		/// </summary>
		public const char ENCRYPTION_IN_MEMORY_PADDING_CHAR = '|';
		#endregion Encryption Things

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
		public static readonly Color DEFAULT_HIGHLIGHT_COLOUR_BUTTON2D = new Color(0.2f, 0.2f, 0.2f, 1f);
		public static readonly DateTime EpochDateTime = new DateTime(1970, 1, 1);
		public static readonly WaitForEndOfFrame EOF = new WaitForEndOfFrame();
		#endregion Other Static Readonlys
	}
}