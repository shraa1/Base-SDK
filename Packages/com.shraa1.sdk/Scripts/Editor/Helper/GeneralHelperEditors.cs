//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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