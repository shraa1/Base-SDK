using System;
using System.Collections;
using BaseSDK.Models;
using BaseSDK.Services;
using BaseSDK.Utils.Helpers;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BaseSDK.Controllers {
	/// <summary>
	/// Manage Addressables through this script.
	/// </summary>
	public class AddressablesManager : Configurable, IAddressableService {
		#region Interface Implementation
		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(IAddressableService));
		#endregion Interface Implementation

		#region Public Helper Methods
		public void LoadAsset<T> (AssetReference assetReference, Action<float> onProgress = null, Action<T> onComplete = null) {
			var operation = assetReference.LoadAssetAsync<T>();
			_ = StartCoroutine(OperationProgress(operation, onProgress, onComplete));
		}

		public void LoadScene (AssetReferenceScene assetReferenceScene, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode = UnityEngine.SceneManagement.LoadSceneMode.Single, Action<float> onProgress = null, Action onComplete = null) {
			var operation = assetReferenceScene.LoadSceneAsync(loadSceneMode);
			_ = StartCoroutine(OperationProgress(operation, onProgress, onComplete));
		}

		public void UnloadScene (AssetReferenceScene assetReferenceScene) => assetReferenceScene.UnLoadScene();

		public void ReleaseAsset (AsyncOperationHandle handle) {
			if (handle.IsValid())
				Addressables.Release(handle);
		}
		#endregion Public Helper Methods

		#region Private Helper Methods
		private IEnumerator OperationProgress<T>(AsyncOperationHandle<T> operation, Action<float> onProgress, Action<T> onComplete) {
			while (!operation.IsDone) {
				onProgress?.Invoke(operation.PercentComplete);
				yield return null;
			}
			if (operation.Status == AsyncOperationStatus.Succeeded)
				onComplete?.Invoke(operation.Result);
			else
				throw operation.OperationException;
		}

		private IEnumerator OperationProgress<T>(AsyncOperationHandle<T> operation, Action<float> onProgress, Action onComplete) {
			while (!operation.IsDone) {
				onProgress?.Invoke(operation.PercentComplete);
				yield return null;
			}
			if (operation.Status == AsyncOperationStatus.Succeeded)
				onComplete?.Invoke();
			else
				throw operation.OperationException;
		}
		#endregion Private Helper Methods
	}
}