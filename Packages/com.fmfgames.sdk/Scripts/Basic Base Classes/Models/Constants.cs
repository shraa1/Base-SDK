using System.IO;
using UnityEngine;

namespace BaseSDK.Models {
    public static class Constants {
		public static System.Func<string> GameName;

		#region File Paths Related
		public const string FIRST_LAUNCH_SAVED_FILE_NAME = "FirstLaunch.playedAlready";
		public const string SAVED_GAME_FILES_FOLDERNAME = "Saved Game Files";

		public static readonly string SAVED_FILE_NAME = $"{GameName}.sav";

		public static readonly string SAVE_FOLDER_PATH = Path.Combine(Application.dataPath, SAVED_GAME_FILES_FOLDERNAME);

		public static readonly string FIRST_LAUNCH_SAVED_FILE_PATH = Path.Combine(SAVE_FOLDER_PATH, FIRST_LAUNCH_SAVED_FILE_NAME);
		public static readonly string SAVE_FILE_PATH = Path.Combine(SAVE_FOLDER_PATH, SAVED_FILE_NAME);
		public static readonly string SAVE_TEMP_FILE_PATH = Path.Combine(SAVE_FOLDER_PATH, SAVED_FILE_NAME + ".tmp");
		#endregion File Paths Related
	}
}