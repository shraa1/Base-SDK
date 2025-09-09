using System.Collections;
using UnityEngine;

namespace BaseSDK.Controllers {
	public class Configurable : MonoBehaviour, IConfigurable {
		public virtual bool Initialized { get; set; } = false;

		public virtual IEnumerator Setup() {
			yield return null;
			Initialized = true;
		}
	}
}