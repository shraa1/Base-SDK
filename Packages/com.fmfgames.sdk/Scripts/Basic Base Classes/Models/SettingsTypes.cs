using System;
using Newtonsoft.Json;

namespace BaseSDK.Models {
	public interface ISettingsState {
		#region Generic
		float VibrationValue { get; set; }
		string Locale { get; set; }
		#endregion Generic

		#region Audio
		public float MasterVolume { get; set; }
		public float MusicVolume { get; set; }
		public float SFXVolume { get; set; }
		#endregion Audio

		#region Display
		public float Brightness { get; set; }
		public bool FullScreen { get; set; }
		public bool Borderless { get; set; }
		public bool VSync { get; set; }
		#endregion Display

		#region Methods
		void Reset();
		#endregion Methods
	}

	[Serializable]
	public class SettingsState : ISettingsState {
		#region Generic
		public float VibrationValue { get; set; } = 1f;
		public string Locale { get; set; } = "en-US";
		#endregion Generic

		#region Audio
		public float MasterVolume { get; set; } = 1f;
		public float MusicVolume { get; set; } = 1f;
		public float SFXVolume { get; set; } = 1f;
		#endregion Audio

		#region Display
		public float Brightness { get; set; } = 1f;
		public bool FullScreen { get; set; }
		public bool Borderless { get; set; }
		public bool VSync { get; set; }
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