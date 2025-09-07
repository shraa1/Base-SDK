using System.IO;
using UnityEngine;

namespace BaseSDK.Models {
    public static class Constants {
		public static System.Func<string> GameName;

		#region Saved Files Related
		public const string SAVED_FILE_NAME = "FirstLaunch.playedAlready";
		public const string SAVED_FOLDER_NAME = "Saved Game Files";
		public static readonly string SAVED_FILE_PATH = Path.Combine(Application.dataPath, SAVED_FOLDER_NAME, SAVED_FILE_NAME);
		#endregion Saved Files Related
	}
}