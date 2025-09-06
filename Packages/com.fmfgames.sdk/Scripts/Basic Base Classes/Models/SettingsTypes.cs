using System;
using Newtonsoft.Json;

namespace BaseSDK.Models {
	[Serializable]
	public class SettingsState {
		#region Generic
		public float VibrationValue = 1f;
		public string Locale = "en-US";
		#endregion Generic

		#region Audio
		public float MasterVolume = 1f;
		public float MusicVolume = 1f;
		public float SFXVolume = 1f;
		#endregion Audio

		#region Display
		public float Brightness = 1f;
		public bool FullScreen;
		public bool Borderless;
		public bool VSync;
		#endregion Display

		#region Methods
		public override string ToString () => JsonConvert.SerializeObject(this);

		public void Reset() {
			VibrationValue = 1f;
			Locale = "en-US";
			MasterVolume = 1f;
			MusicVolume = 1f;
			SFXVolume = 1f;
			Brightness = 1f;
			FullScreen = true;
			Borderless = true;
			VSync = true;
		}
		#endregion Methods
	}
}