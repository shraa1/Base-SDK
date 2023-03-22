//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using UnityEngine;

namespace BaseSDK.Utils.Helpers {
	/// <summary>
	/// Instead of having to write meaningless pieces of code like Application.OpenURL in a useless class just so that we can call it from UI on a button click is just messy. Instead, apply this script
	/// on any gameobject, and use the helper methods instead, no need to mess up the main logic based scripts
	/// </summary>
	public class GenericHelperMonobehaviour : MonoBehaviour {
		[SerializeField] private List<GameObject> dontDestroyGameObjects = new List<GameObject>();

		public void OpenURL(string urlToOpen) => Application.OpenURL(urlToOpen);

		private void Awake() => dontDestroyGameObjects.ForEach(x => {
			if (x.transform.parent != null)
				x.transform.SetParent(null, true);
			DontDestroyOnLoad(x);
		});
	}
}