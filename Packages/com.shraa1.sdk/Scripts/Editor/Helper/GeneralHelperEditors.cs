#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BaseSDK {
	public class GeneralHelperEditors : MonoBehaviour {
		[MenuItem("Tools/Open Persistent Data Path Folder")]
		public static void OpenPDP() {
			var procInfo = new ProcessStartInfo { UseShellExecute = true, FileName = Application.persistentDataPath };
			var pp = new Process() { StartInfo = procInfo };
			try { pp.Start(); }
			catch { }
		}

		[MenuItem("Tools/Clear Console &#%c")]
		public static void ClearConsole () {
			//TODO FIXME need to find the exact version where this was changed
#if UNITY_2017_1_OR_NEWER
			Assembly.GetAssembly(typeof(ActiveEditorTracker)).GetType("UnityEditor.LogEntries").GetMethod("Clear").Invoke(null, null);
#else
			Assembly.GetAssembly(typeof(ActiveEditorTracker)).GetType("UnityEditorInternal.LogEntries").GetMethod("Clear").Invoke(null, null);
#endif
		}
	}
}
#endif