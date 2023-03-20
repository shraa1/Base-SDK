//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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