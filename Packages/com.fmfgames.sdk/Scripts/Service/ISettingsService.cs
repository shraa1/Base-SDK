using BaseSDK.Services;
using BaseSDK.Models;

namespace BaseSDK {
	public interface ISettingsService<T> : IService where T : SettingsState, new() {
		T SettingsState { get; set; }
		void SetBrightnessValue (float newValue);
	}
}