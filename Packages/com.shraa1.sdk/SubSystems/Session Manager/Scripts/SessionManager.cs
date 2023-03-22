//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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