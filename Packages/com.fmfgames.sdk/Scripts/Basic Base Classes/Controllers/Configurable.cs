using System.Collections;
using UnityEngine;

namespace BaseSDK.Controllers {
	public abstract class Configurable : MonoBehaviour, IConfigurable {
		public virtual bool Initialized { get; set; } = false;

		public virtual IEnumerator Setup() {
			yield return null;
			Initialized = true;
		}
	}
}