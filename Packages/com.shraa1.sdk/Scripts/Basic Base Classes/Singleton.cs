#pragma warning disable S2743 // Static fields should not be used in generic types
using System.Collections.Generic;

using DG.Tweening;

using UnityEngine;

namespace BaseSDK {
	[DisallowMultipleComponent]
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		/// <summary>
		/// A list of tweeners for this Singleton<T> type
		/// </summary>
		public static List<Tweener> AllTweeners { get; set; } = new();

		/// <summary>
		/// If true, then don't create new gameobject. Could happen that Instance getter is triggered during game end.
		/// </summary>
		protected static bool m_AppQuitStarted = false;

		/// <summary>
		/// Lock Object so that singleton creation is thread safe
		/// </summary>
		private static readonly object m_LockObject = new();

		/// <summary>
		/// Set's the gameobject to DontDestroyOnLoad if needed
		/// </summary>
		protected bool DontDestroy {
			set {
				//if condition can potentially be avoided, as not calling this setter will just mean that the gameObject gets destroyed, same as if we set false here
				if (value) {
					//Avoid warning for calling DontDestroyOnLoad for gameobjects which are children
					if (transform.parent != null)
						transform.SetParent(null);
					DontDestroyOnLoad(gameObject);
				}
			}
		}

		protected static T m_Instance;

		/// <summary>
		/// Instance of singleton
		/// </summary>
		public static T Instance {
			get {
				lock (m_LockObject) {
					if (m_Instance == null) {
						var allComponentsOfType = FindObjectsByType<T>(FindObjectsSortMode.None);
						if (allComponentsOfType.Length > 0) {
							m_Instance = allComponentsOfType[0];
							if (allComponentsOfType.Length > 1)
								Debug.Log($"Singleton -> There's a problem. Multiple instances of type {typeof(T).Name} present in the scene");
							return m_Instance;
						}

						if (m_AppQuitStarted)
							return m_Instance;

						var go = new GameObject(typeof(T).FullName);
						var component = go.AddComponent<T>();
						m_Instance = component;
					}
				}
				return m_Instance;
			}
			set {
				if (m_Instance != null)
					Debug.Log($"Singleton -> Instance of {typeof(T).Name} has already been set and you tried setting it again.");
				else
					m_Instance = value;
			}
		}

		protected virtual void OnApplicationQuit () => m_AppQuitStarted = true;
	}
}