#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		public static string GetAssetPath(this Object obj) => AssetDatabase.GetAssetPath(obj);

		public static string GetFullAssetPath(this Object obj) => GetFullAssetPath(GetAssetPath(obj));

		public static string GetFullAssetPath(this string localPath) => Application.dataPath.Replace("Assets", string.Empty) + localPath;
	}
}
#endif