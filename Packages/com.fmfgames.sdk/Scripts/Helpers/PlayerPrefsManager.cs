
using UnityEngine;
using BaseSDK.Extension;
using Newtonsoft.Json;

namespace BaseSDK.Helper {
	public class PlayerPrefsManager : MonoBehaviour {
		private class PlayerPrefsSaveState {
			public bool Encrypted = false;
			public string Data = default;
		}

		public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);

		public static void DeleteAll() => PlayerPrefs.DeleteAll();

		public static T Get<T>(string key, T defaultValue = default, bool useNewtonsoft = true) {
			var str = PlayerPrefs.GetString(key, string.Empty);
			if (str.IsNullOrEmpty())
				return defaultValue;

			var obj = str.Deserialize<PlayerPrefsSaveState>();
			var deserialized = obj.Encrypted ? obj.Data.Decrypt<string>().Deserialize<T>() : obj.Data.Deserialize<T>();

			return useNewtonsoft ? deserialized : JsonUtility.FromJson<T>(str);
		}

		public static void Set<T> (string key, T value, bool useNewtonsoft = true, bool encrypted = true) {
			var state = new PlayerPrefsSaveState() { Encrypted = encrypted, Data = encrypted ? value.Serialize().Encrypt() : value.Serialize() };
			var str = useNewtonsoft ? JsonConvert.SerializeObject(state) : JsonUtility.ToJson(value);
			PlayerPrefs.SetString(key, str);
		}
	}
}