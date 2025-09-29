using System;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using BaseSDK.SirenixHelper;
#endif
using BaseSDK.Popups.Views;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using BaseSDK.Settings.Views.Rebind;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using BaseSDK.Helper;
using BaseSDK.Extension;
using DG.Tweening;
using BaseSDK.Services;
using BaseSDK.Models;
using System.Linq;

namespace BaseSDK.Settings.Views {
	public abstract partial class SettingsPopupBase<POPUPTYPE> : PopupBase<POPUPTYPE> where POPUPTYPE : Enum {
		#region Inspector Variables
		[FoldoutGroup("Overall"), SerializeField] private InputActionAsset m_InputActionAsset;
		[FoldoutGroup("Generic Settings"), SerializeField] private Button m_RestoreDeaultsBtn;
		[FoldoutGroup("Generic Settings"), SerializeField] private Slider m_VibrationSlider;
		[FoldoutGroup("Generic Settings"), SerializeField] private TMP_Dropdown m_LanguageDropdown;

		[FoldoutGroup("Audio"), SerializeField] private Slider m_MasterVolumeSlider;
		[FoldoutGroup("Audio"), SerializeField] private Slider m_MusicVolumeSlider;
		[FoldoutGroup("Audio"), SerializeField] private Slider m_SFXVolumeSlider;

		[FoldoutGroup("Display"), SerializeField] private Slider m_BrightnessSlider;
		[FoldoutGroup("Display"), SerializeField] private Toggle[] m_FullScreenToggles;
		[FoldoutGroup("Display"), SerializeField] private Toggle[] m_BorderlessToggles;
		[FoldoutGroup("Display"), SerializeField] private Toggle[] m_VSyncToggles;
		[FoldoutGroup("Display")] public Volume GlobalVolume;

		[FoldoutGroup("Controls"), SerializeField] private RebindActionUIBase[] m_RebindActionUIs;
		#endregion Inspector Variables

		#region Variables
		protected IAudioService AudioService => GlobalServices.GetServiceProvider(ServicesScope.GLOBAL).Get<IAudioService>();
		protected ISettingsService SettingsService => GlobalServices.GetServiceProvider(ServicesScope.GLOBAL).Get<ISettingsService>();
		protected ColorAdjustments m_ColorAdjustments;
		#endregion Variables

		#region Unity Methods
		protected override void Awake() {
			base.Awake();
			_ = GlobalVolume.profile.TryGet(out m_ColorAdjustments);

			#region Listeners
			m_VibrationSlider.onValueChanged.AddListener(OnVibrationValueChanged);
			m_LanguageDropdown.onValueChanged.AddListener(OnLanguageDropdownValueChanged);
			m_MasterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
			m_MusicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
			m_SFXVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
			m_BrightnessSlider.onValueChanged.AddListener(OnBrightnessValueChanged);
			m_RestoreDeaultsBtn.onClick.AddListener(RestoreDefaults);
			#endregion Listeners

			var rebinds = PlayerPrefsManager.Get("rebinds", string.Empty);
			if (!rebinds.IsNullOrEmpty()) {
				m_InputActionAsset.LoadBindingOverridesFromJson(rebinds);
				m_RebindActionUIs.ForEach(x => x.UpdateBindingDisplay());
			}
		}

		protected virtual void OnDestroy() {
			PlayerPrefsManager.Set("rebinds", m_InputActionAsset.SaveBindingOverridesAsJson());
			m_MasterVolumeSlider.onValueChanged.RemoveAllListeners();
			m_MusicVolumeSlider.onValueChanged.RemoveAllListeners();
			m_SFXVolumeSlider.onValueChanged.RemoveAllListeners();
		}
		#endregion Unity Methods

		#region Overrides
		public override Tweener OpenPopup(Action onComplete = null) {
			InitGeneric();
			InitAudio();
			InitDisplay();

			return base.OpenPopup(onComplete);
		}
		#endregion Overrides

		#region Helper Methods
		protected virtual void RestoreDefaults() {
			SettingsService.SettingsState.Reset();

			InitGeneric();
			InitAudio();
			InitDisplay();
		}

		private void RadioButtonHelper(Toggle[] toggles, bool val, Action<bool> onValueChanged) {
			toggles.ForEach(x => x.onValueChanged.RemoveAllListeners());

			//TODO HACK FIXME improve this
			toggles[0].isOn = !val;
			toggles[1].isOn = val;
			toggles[0].onValueChanged.AddListener(newVal => RadioButtonHelper(toggles, false, onValueChanged));
			toggles[1].onValueChanged.AddListener(newVal => RadioButtonHelper(toggles, true, onValueChanged));

			onValueChanged?.Invoke(toggles[1].isOn);
		}
		#endregion Helper Methods
	}

	/// <summary>
	/// Settings popup partial class for General settings
	/// </summary>
	public abstract partial class SettingsPopupBase<POPUPTYPE> {
		#region Callback Methods
		protected virtual void OnLanguageDropdownValueChanged(int newLangIndex) {
			var localizationService = GlobalServices.GetServiceProvider(ServicesScope.GLOBAL).Get<ILocalizationService>();
			localizationService.SetLanguage(newLangIndex);
			SettingsService.SettingsState.Locale = localizationService.SupportedLanguages[newLangIndex].Name;
		}

		protected virtual void OnVibrationValueChanged(float newVol) {
			//TODO yet to implement
		}
		#endregion Callback Methods

		#region Init
		public virtual void InitGeneric() {
			var localizationService = GlobalServices.GetServiceProvider(ServicesScope.GLOBAL).Get<ILocalizationService>();
			var settings = SettingsService.SettingsState;
			m_VibrationSlider.value = settings.VibrationValue;

			var languages = localizationService.SupportedLanguages;
			var ind = languages.IndexOf(languages.Find(x => x.Name == settings.Locale));
			m_LanguageDropdown.ClearOptions();
			m_LanguageDropdown.AddOptions(languages.Select(x => x.DisplayName).ToList());
			m_LanguageDropdown.value = ind;
		}
		#endregion Init
	}

	/// <summary>
	/// Settings popup partial class for Audio
	/// </summary>
	public abstract partial class SettingsPopupBase<POPUPTYPE> {
		#region Callback Methods
		protected virtual void OnMasterVolumeChanged(float newVol) {
			SettingsService.SettingsState.MasterVolume = newVol;
			AudioService.SetVolume(VOLUMETYPE.MASTER, newVol);
		}

		protected virtual void OnMusicVolumeChanged(float newVol) {
			SettingsService.SettingsState.MusicVolume = newVol;
			AudioService.SetVolume(VOLUMETYPE.MUSIC, newVol);
		}

		protected virtual void OnSFXVolumeChanged(float newVol) {
			SettingsService.SettingsState.SFXVolume = newVol;
			AudioService.SetVolume(VOLUMETYPE.SFX, newVol);
		}
		#endregion Callback Methods

		#region Init
		public virtual void InitAudio() {
			m_MasterVolumeSlider.value = SettingsService.SettingsState.MasterVolume;
			m_MusicVolumeSlider.value = SettingsService.SettingsState.MusicVolume;
			m_SFXVolumeSlider.value = SettingsService.SettingsState.SFXVolume;
		}
		#endregion Init
	}

	/// <summary>
	/// Settings popup partial class for Display
	/// </summary>
	public abstract partial class SettingsPopupBase<POPUPTYPE> {
		#region Callback Methods
		protected virtual void OnBrightnessValueChanged(float newBrightnessVal) {
			SettingsService.SettingsState.Brightness = newBrightnessVal;
			if (m_ColorAdjustments != null)
				m_ColorAdjustments.postExposure.value = Mathf.Log(newBrightnessVal) / Mathf.Log(2f);
		}
		#endregion Callback Methods

		#region Init
		public virtual void InitDisplay() {
			m_BrightnessSlider.value = SettingsService.SettingsState.Brightness;

			RadioButtonHelper(m_FullScreenToggles, SettingsService.SettingsState.FullScreen, val => {
				SettingsService.SettingsState.FullScreen = val;
				Screen.fullScreen = val;
			});
			RadioButtonHelper(m_BorderlessToggles, SettingsService.SettingsState.Borderless, val => SettingsService.SettingsState.Borderless = val);
			RadioButtonHelper(m_VSyncToggles, SettingsService.SettingsState.VSync, val => SettingsService.SettingsState.VSync = val);
		}
		#endregion Init
	}
}