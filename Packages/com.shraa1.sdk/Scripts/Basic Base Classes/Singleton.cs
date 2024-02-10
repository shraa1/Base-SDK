using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

namespace BaseSDK {
	[DisallowMultipleComponent]
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour {
		/// <summary>
		/// A list of tweeners for this Singleton<T> type
		/// </summary>
		public static List<Tweener> allTweeners = new List<Tweener>();
		
		/// <summary>
		/// If true, then don't create new gameobject. Could happen that Instance getter is triggered during game end.
		/// </summary>
		protected static bool appQuitStarted = false;

		/// <summary>
		/// Lock Object so that singleton creation is thread safe
		/// </summary>
		private static readonly object lockObject = new object();

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

		private static T instance;

		/// <summary>
		/// Instance of singleton
		/// </summary>
		public static T Instance {
			get {
				lock(lockObject) {
					if (instance == null) {
						var allComponentsOfType = FindObjectsOfType<T>();
						if (allComponentsOfType.Length > 0) {
							instance = allComponentsOfType[0];
							if (allComponentsOfType.Length > 1)
								Debug.Log($"Singleton -> There's a problem. Multiple instances of type {typeof(T).Name} present in the scene");
							return instance;
						}

						if (appQuitStarted)
							return instance;

						var go = new GameObject(typeof(T).FullName);
						var component = go.AddComponent<T>();
						instance = component;
					}
				}
				return instance;
			}
			set {
				if (instance != null)
					Debug.Log($"Singleton -> Instance of {typeof(T).Name} has already been set and you tried setting it again.");
				else
					instance = value;
			}
		}

		protected virtual void OnApplicationQuit() => appQuitStarted = true;
	}
}