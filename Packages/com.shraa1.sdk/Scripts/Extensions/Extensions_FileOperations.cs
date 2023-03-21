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
using System.IO;
using UnityEngine;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		/// <summary>
		/// Call the function on a string which is the path where the file is located
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="content"></param>
		/// <param name="throwErrorIfFileNotFound"></param>
		/// <returns></returns>
		public static Exception WriteToFile(this string filePath, string content, bool throwErrorIfFileNotFound = false) {
			Exception e = null;
			if (!File.Exists(filePath) && throwErrorIfFileNotFound)
				e = new FileNotFoundException();
			else if (!File.Exists(filePath))
				return null;
			else {
				try {
					using (var sw = new StreamWriter(filePath))
						sw.Write(content);
				}
				catch (Exception ex) { e = ex; }
			}
			return e;
		}

		public static string ReadFromFile(this string filePath, out Exception e) {
			var str = string.Empty;
			e = null;

#if UNITY_ANDROID && !UNITY_EDITOR
		try {
			UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
			www.SendWebRequest();
			while (!www.isDone && www.error.IsNullOrEmpty()) { }
			if (www.error.IsNullOrEmpty())
				str = www.downloadHandler.text;
			else
				e = new Exception(www.error);
		}
		catch (Exception ex) {
			e = ex;
		}
#else

			if (!File.Exists(filePath))
				e = new FileNotFoundException();
			else {
				try {
					using (var sw = new StreamReader(filePath))
						str = sw.ReadToEnd();
				}
				catch (Exception ex) {
					e = ex;
				}
			}
#endif
			return str;
		}
	}
}