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