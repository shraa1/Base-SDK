using UnityEditor;

using UnityEngine;

using static UnityEditor.AssetDatabase;
using static UnityEditor.EditorBuildSettings;
using static UnityEditor.SceneManagement.EditorSceneManager;

namespace IdlingGame {
	[InitializeOnLoad]
	public class PlayFromScene0 {
		[MenuItem("Tools/Play from Scene 0")]
		public static void Play () {
			PlayerPrefs.SetString("PlayFromScene0", string.Empty);
			playModeStartScene = LoadAssetAtPath<SceneAsset>(scenes[0].path);
		}

		[MenuItem("Tools/Stop Playing from Scene 0")]
		public static void StopPlaying () {
			PlayerPrefs.DeleteKey("PlayFromScene0");
			playModeStartScene = null;
		}

		static PlayFromScene0 () {
			if (PlayerPrefs.HasKey("PlayFromScene0"))
				playModeStartScene = LoadAssetAtPath<SceneAsset>(scenes[0].path);
		}
	}
}