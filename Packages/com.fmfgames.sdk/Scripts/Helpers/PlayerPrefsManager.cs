
using UnityEngine;
using BaseSDK.Extension;
using Newtonsoft.Json;

namespace BaseSDK.Helper {
	public class PlayerPrefsManager : MonoBehaviour {
		public static void DeleteKey(string key) {
	#if USE_DEFAULT_PLAYERPREFS
			PlayerPrefs.DeleteKey(key);
	#endif
		}

		public static void DeleteAll() {
	#if USE_DEFAULT_PLAYERPREFS
			PlayerPrefs.DeleteAll();
	#endif
		}

		public static T Get<T>(string key, T defaultValue = default, bool useNewtonsoft = true) {
#if USE_DEFAULT_PLAYERPREFS
			var str = PlayerPrefs.GetString(key, string.Empty);
			if (str.IsNullOrEmpty())
				return defaultValue;

			return useNewtonsoft ? JsonConvert.DeserializeObject<T>(str) : JsonUtility.FromJson<T>(str);
#endif
		}

		public static void Set<T> (string key, T value, bool useNewtonsoft = true) {
			var str = useNewtonsoft ? JsonConvert.SerializeObject(value) : JsonUtility.ToJson(value);

	#if USE_DEFAULT_PLAYERPREFS
			PlayerPrefs.SetString(key, str);
	#endif
		}
	}
}