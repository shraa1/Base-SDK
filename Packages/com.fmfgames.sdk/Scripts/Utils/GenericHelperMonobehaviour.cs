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