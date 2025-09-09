using System.Collections;
using System.Collections.Generic;
using BaseSDK.Models;
using BaseSDK.Services;
using UnityEngine;

namespace BaseSDK {
	public abstract class SceneServiceHandler : MonoBehaviour {
		#region Inspector Variables
		[SerializeField] protected List<MonoBehaviour> m_IConfigurables = new();
		#endregion Inspector Variables

		/// <summary>
		/// Initialize things like Lobby scoped services provider, encryption key, game name, etc.
		/// </summary>
		protected abstract void Init();

		/// <summary>
		/// Fire Setups for IConfigurables
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator LoadAllData() {
			foreach (var configurable in m_IConfigurables)
				yield return StartCoroutine((configurable as IConfigurable).Setup());
		}

		protected virtual void RegisterServices() {
			m_IConfigurables.ForEach(x => {
				if (x is IService) {
					var (scope, interfaceType) = (x as IService).RegisteringTypes;
					GlobalServices.GetServiceProvider((ServicesScope)scope).Register(interfaceType, x as IService);
				}
			});
		}
	}
}