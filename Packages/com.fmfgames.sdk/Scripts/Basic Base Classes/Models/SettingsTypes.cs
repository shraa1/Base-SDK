using System;
using Newtonsoft.Json;

namespace BaseSDK.Models {
	public interface ISettingsState {
		#region Generic
		float VibrationValue { get; set; }
		string Locale { get; set; }
		#endregion Generic

		#region Audio
		float MasterVolume { get; set; }
		float MusicVolume { get; set; }
		float SFXVolume { get; set; }
		#endregion Audio

		#region Display
		float Brightness { get; set; }
		bool FullScreen { get; set; }
		bool Borderless { get; set; }
		bool VSync { get; set; }
		#endregion Display

		#region Methods
		void Reset();
		#endregion Methods
	}

	[Serializable]
	public class SettingsState : ISettingsState {
		#region Generic
		public virtual float VibrationValue { get; set; } = 1f;
		public virtual string Locale { get; set; } = "en-US";
		#endregion Generic

		#region Audio
		public virtual float MasterVolume { get; set; } = 1f;
		public virtual float MusicVolume { get; set; } = 1f;
		public virtual float SFXVolume { get; set; } = 1f;
		#endregion Audio

		#region Display
		public virtual float Brightness { get; set; } = 1f;
		public virtual bool FullScreen { get; set; }
		public virtual bool Borderless { get; set; }
		public virtual bool VSync { get; set; }
		#endregion Display

		#region Methods
		public override string ToString () => JsonConvert.SerializeObject(this);

		public virtual void Reset() {
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