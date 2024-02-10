using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSDK {
	/// <summary>
	/// Used to check if game game's screen was updated/resized
	/// </summary>
	public class ScreenResolutionUpdater : Singleton<ScreenResolutionUpdater> {
		/// <summary>
		/// Current screen size saved
		/// </summary>
		private Vector2 ScreenSize = Vector2.zero;

		/// <summary>
		/// Fired when Screen Resolution has changed
		/// </summary>
		public static event Action<Vector2> OnScreenResolutionChanged;

		/// <summary>
		/// Initialize
		/// </summary>
		private void Awake () {
			ScreenSize = new Vector2(Screen.width, Screen.height);
			DontDestroy = true;
		}

		/// <summary>
		/// Update screen size if needed, and fire the event
		/// </summary>
		private void Update () {
			var vec = new Vector2(Screen.width, Screen.height);
			if (vec != ScreenSize) {
				OnScreenResolutionChanged?.Invoke(vec);
				ScreenSize = vec;
			}
		}
	}
}