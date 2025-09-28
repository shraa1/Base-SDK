using BaseSDK.Services;
using BaseSDK.Models;

namespace BaseSDK {
	public interface ISettingsService : IService {
		ISettingsState SettingsState { get; set; }
		void SetBrightnessValue (float newValue);
		ISettingsState GetNewSettingsState();
		ISettingsState GetSettingsState(string serialized);
	}
}