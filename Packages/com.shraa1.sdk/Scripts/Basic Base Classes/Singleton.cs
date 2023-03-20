//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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