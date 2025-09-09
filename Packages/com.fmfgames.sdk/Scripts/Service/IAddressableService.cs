using System;
using BaseSDK.Utils.Helpers;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BaseSDK.Services {
	public interface IAddressableService : IService {
		void LoadAsset<T>(AssetReference assetReference, Action<float> onProgress = null, Action<T> onComplete = null);
		void LoadScene(AssetReferenceScene assetReferenceScene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single, Action<float> onProgress = null, Action onComplete = null);
		void UnloadScene(AssetReferenceScene assetReferenceScene);
		void ReleaseAsset(AsyncOperationHandle handle);
	}
}