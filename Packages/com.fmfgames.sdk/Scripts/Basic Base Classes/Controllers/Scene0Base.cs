using System.Collections;
using System.Collections.Generic;
using BaseSDK.Models;
using BaseSDK.Services;
using UnityEngine;

namespace BaseSDK {
	/// <summary>
	/// Scene0 Manager's base class setup
	/// </summary>
	public abstract class Scene0Base : MonoBehaviour {
		#region Inspector Variables
		[SerializeField] protected List<MonoBehaviour> m_IConfigurables = new();
		#endregion Inspector Variables

		#region Unity Methods
		protected virtual void Awake() {
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;

			//Since Global scope should be initialized before anything else, 
			GlobalServices.Initialize(ServicesScope.GLOBAL);
		}
		#endregion Unity Methods

		#region Private/Protected Helper Methods
		/// <summary>
		/// Fire Setups for IConfigurables
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator LoadAllData() {
			foreach (var configurable in m_IConfigurables)
				yield return StartCoroutine((configurable as IConfigurable).Setup());
		}
		#endregion Private/Protected Helper Methods
	}
}