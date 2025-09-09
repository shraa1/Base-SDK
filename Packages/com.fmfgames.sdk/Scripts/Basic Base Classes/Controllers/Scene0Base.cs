using BaseSDK.Models;
using BaseSDK.Services;
using UnityEngine;

namespace BaseSDK {
	/// <summary>
	/// Scene0 Manager's base class setup
	/// </summary>
	public abstract class Scene0Base : SceneServiceHandler {
		#region Unity Methods
		protected virtual void Awake() {
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;

			//Since Global scope should be initialized before anything else
			GlobalServices.Initialize(ServicesScope.GLOBAL);
			//Register other things like
			Init();
			//Register the services present in this scene
			RegisterServices();

			StartCoroutine(LoadAllData());
		}
		#endregion Unity Methods

		#region Private/Protected Helper Methods
		/// <summary>
		/// Initialize things like Lobby scoped services provider, encryption key, game name, etc.
		/// </summary>
		protected override abstract void Init();
		#endregion Private/Protected Helper Methods
	}
}