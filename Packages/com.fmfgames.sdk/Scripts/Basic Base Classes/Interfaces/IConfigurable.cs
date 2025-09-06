using System.Collections;

namespace BaseSDK {
	public interface IConfigurable {
		bool Initialized { get; set; }

		IEnumerator Setup ();
	}
}