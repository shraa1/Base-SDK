using BaseSDK.Services;
using BaseSDK.Models;

namespace BaseSDK {
	public interface ISettingsService : IService {
		SettingsState SettingsState { get; set; }
		void SetBrightnessValue (float newValue);
	}
}