using System;
using System.Collections;
using System.Collections.Generic;
using BaseSDK.Services;
using BaseSDK.Utils.Helpers;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using BaseSDK.SirenixHelper;
#endif
using UnityEngine;
using UnityEngine.Audio;
using BaseSDK.Models;

namespace BaseSDK.Controllers {
	/// <summary>
	/// Identify AudioSources based on the string name
	/// </summary>
	[Serializable]
	public class NamedAudioSourceAssetReference {
		public string AudioName;
		public AudioSource AudioSource;
		public AssetReferenceAudioClip AudioClipAssetReference;
	}

	public class AudioManager : Configurable, IAudioService {
		#region Inspector Variables
		/// <summary>
		/// AudioSources and respective Audioclips they play
		/// </summary>
		[FoldoutGroup("Audios"), DrawWithUnity, SerializeField] protected List<NamedAudioSourceAssetReference> m_AudiosInfo = new();

#if UNITY_EDITOR
		public List<NamedAudioSourceAssetReference> AudiosInfo => m_AudiosInfo;
		public AudioMixerGroup SFXAudioMixerGroup;
#endif

		[FoldoutGroup("Mixer & Settings"), SerializeField] protected AudioMixer m_AudioMixer;
		[FoldoutGroup("Music Settings"), SerializeField] protected AudioSource m_MusicSource;
		[FoldoutGroup("Music Settings"), SerializeField] protected float m_MusicFadeTime = 1f;
		#endregion Inspector Variables

		#region Variables
		/// <summary>
		/// Move on only if all audios have been downloaded
		/// </summary>
		protected bool m_DownloadedAllAudios = false;

		private static readonly Dictionary<VOLUMETYPE, string> m_VolumeType = new() {
			{ VOLUMETYPE.MASTER, "Master" },
			{ VOLUMETYPE.MUSIC, "Music" },
			{ VOLUMETYPE.SFX, "SFX" },
		};
		protected const float k_MIN_VOLUME = .0001f;
		protected const float k_AUDIO_MULTIPLIER = 20f;
		#endregion Variables

		#region Unity Methods
		/// <summary>
		/// Initializations
		/// </summary>
		protected virtual void Awake () {
			var i = 0;
			if (m_AudiosInfo.Count == 0)
				m_DownloadedAllAudios = true;
			else {
				var service = GlobalServices.GetServiceProvider(ServicesScope.GLOBAL).Get<IAddressableService>();
				foreach (var audioInfo in m_AudiosInfo)
					service.LoadAsset<AudioClip>(audioInfo.AudioClipAssetReference, null, obj => {
						i++;
						if (i == m_AudiosInfo.Count)
							m_DownloadedAllAudios = true;
						audioInfo.AudioSource.clip = obj;
					});
			}
		}
		#endregion Unity Methods

		#region Interface Implementation
		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(IAudioService));

		/// <summary>
		/// IConfigurable implementation
		/// </summary>
		public override IEnumerator Setup () {
			while (!m_DownloadedAllAudios)
				yield return null;
			Initialized = true;

			var settingsManager = GlobalServices.GetServiceProvider(ServicesScope.GLOBAL).Get<ISettingsService<SettingsState>>();
			SetVolume(VOLUMETYPE.MASTER, settingsManager.SettingsState.MasterVolume);
			SetVolume(VOLUMETYPE.MUSIC, settingsManager.SettingsState.MusicVolume);
			SetVolume(VOLUMETYPE.SFX, settingsManager.SettingsState.SFXVolume);
		}

		public virtual void SetVolume (VOLUMETYPE volumeType, float volume) {
			var parameter = m_VolumeType[volumeType];
			_ = m_AudioMixer.SetFloat(parameter, Mathf.Log10(Mathf.Clamp(volume, k_MIN_VOLUME, 1f)) * k_AUDIO_MULTIPLIER);
			PlayerPrefs.SetFloat(parameter, volume);
		}

		public virtual void PlayMusic (AudioClip clip, bool loop = true) {
			if (m_MusicSource.isPlaying && m_MusicSource.clip == clip) return;
			_ = StartCoroutine(FadeMusic(clip, loop));
		}

		public virtual void StopMusic () => m_MusicSource.Stop();
		#endregion Interface Implementation

		#region Music
		protected virtual IEnumerator FadeMusic (AudioClip newClip, bool loop) {
			for (var t = 0f; t < m_MusicFadeTime; t += Time.unscaledDeltaTime) {
				m_MusicSource.volume = 1 - (t / m_MusicFadeTime);
				yield return null;
			}

			m_MusicSource.clip = newClip;
			m_MusicSource.loop = loop;
			m_MusicSource.Play();

			for (var t = 0f; t < m_MusicFadeTime; t += Time.unscaledDeltaTime) {
				m_MusicSource.volume = t / m_MusicFadeTime;
				yield return null;
			}

			m_MusicSource.volume = 1f;
		}
		#endregion Music

		#region SFX
		/// <summary>
		/// Play an audio based on the clipname passed. Pass an OnComplete if you want
		/// </summary>
		/// <param name="clipName">Clip name cached in the inspector/list</param>
		/// <param name="onAudioFinishedPlaying">Callback for audio's finish event</param>
		public virtual void PlayClip (string clipName, Action onAudioFinishedPlaying = null) {
			var find = m_AudiosInfo.Find(x => x.AudioName == clipName);
			if (find == null) return;
			find.AudioSource.Play();

			_ = StartCoroutine(ReleaseAfterPlay(find.AudioSource, onAudioFinishedPlaying));
		}

		protected virtual IEnumerator ReleaseAfterPlay (AudioSource source, Action onAudioFinishedPlaying = null) {
			yield return new WaitForSecondsRealtime(source.clip.length);
			source.Stop();
			onAudioFinishedPlaying?.Invoke();
		}
		#endregion SFX
	}

#if UNITY_EDITOR
	[UnityEditor.CustomEditor(typeof(AudioManager))]
	public class AudioManagerEditor : UnityEditor.Editor {
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();

			var am = target as AudioManager;
			am.AudiosInfo.ForEach(audioInfo => {
				if (audioInfo.AudioSource == null) {
					audioInfo.AudioSource = am.gameObject.AddComponent<AudioSource>();
					audioInfo.AudioSource.outputAudioMixerGroup = am.SFXAudioMixerGroup;
					UnityEditor.EditorUtility.SetDirty(am);
				}
			});
		}
	}
#endif
}