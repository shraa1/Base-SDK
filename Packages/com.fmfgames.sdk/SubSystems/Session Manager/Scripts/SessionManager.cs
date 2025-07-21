#pragma warning disable IDE0044 // Add readonly modifier
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using BaseSDK.Helper;
using BaseSDK.Extension;

namespace BaseSDK {
	public class SessionManager : Singleton<SessionManager> {
		/// <summary>
		/// Latest time when the game was recorded as played. If current time is below this, it would mean date abuse has occurred.
		/// </summary>
		private static long _latestTimestampPlayed;
		private static long LatestTimestampPlayed { get => PlayerPrefsManager.Get("_latestTimestampPlayed", DateTime.UtcNow.Ticks); set => PlayerPrefsManager.Set("_latestTimestampPlayed", value); }

		private bool HasCheated {
			get => PlayerPrefsManager.Get("dateAbused", false);
			set => PlayerPrefsManager.Set("dateAbused", value);
		}

		private void Awake () {
			Instance = this;
			_latestTimestampPlayed = LatestTimestampPlayed;
		}

		protected virtual void OnApplicationFocus (bool hasFocus) {
			if (!HasCheated)
				if (DateTime.UtcNow.Ticks > _latestTimestampPlayed)
					LatestTimestampPlayed = _latestTimestampPlayed = DateTime.UtcNow.Ticks;
			if (DateTime.UtcNow.Ticks < _latestTimestampPlayed)
				HasCheated = true;
		}

		protected override void OnApplicationQuit () {
			base.OnApplicationQuit();
			LatestTimestampPlayed = DateTime.UtcNow.Ticks;
		}
	}
}