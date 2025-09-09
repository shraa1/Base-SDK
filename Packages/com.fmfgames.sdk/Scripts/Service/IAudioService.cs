using UnityEngine;

namespace BaseSDK.Services {
	public enum VOLUMETYPE { MASTER, MUSIC, SFX }

	public interface IAudioService : IService {
		void SetVolume (VOLUMETYPE volumeType, float volume);
		void PlayMusic (AudioClip clip, bool loop = true);
		void StopMusic ();
	}
}