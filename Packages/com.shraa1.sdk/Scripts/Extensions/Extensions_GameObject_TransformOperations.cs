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
using System.Linq;
using UnityEngine;

namespace BaseSDK.Extension {
	public static partial class Extensions {

		public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
			var a = gameObject.GetComponent<T>();
			a ??= gameObject.AddComponent<T>();
			return a;
		}

		public static List<Transform> GetChildren(this Transform t, bool findInactiveChildren = true) {
			if (t == null)
				throw new NullReferenceException("Can't Get Children of Null Transform");
			var l = t.GetComponentsInChildren<Transform>(findInactiveChildren).ToList();
			l.RemoveSafely(t);
			return l;
		}

		public static List<GameObject> GetChildren(this GameObject go, bool findInactiveChildren = true) => go.transform.GetChildren(findInactiveChildren)?.Select(x => x.gameObject)?.ToList();

		public static IEnumerable<Transform> GetSiblings(this Transform t, bool findInactiveSiblings = true) => t.parent?.GetComponentsInChildren<Transform>(findInactiveSiblings)?.ToList()?.RemoveSafely(t, t.parent);

		public static IEnumerable<GameObject> GetSiblings(this GameObject go, bool findInactiveSiblings = true) => go.transform.GetSiblings(findInactiveSiblings)?.Select(x => x.gameObject);

		public static List<GameObject> FindGameObjectsOfName(string name) => UnityEngine.Object.FindObjectsOfType<GameObject>().ToList().FindAll(x => x.name == name);

		/// <summary>
		/// Is this transform a child of a transform that has a component of this type
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="transform">Child Transform</param>
		/// <returns>Is this transform a child of any transform with this type as a component</returns>
		public static bool IsNestedChildOfType<T>(this Transform transform) where T : Component {
			if (transform.parent == null)
				return false;
			if (transform.parent.GetComponent<T>() == null)
				return transform.parent.IsNestedChildOfType<T>();
			if (transform.parent.GetComponent<T>() != null)
				return true;
			return false;
		}

		/// <summary>
		/// Shorthand for destroying any object
		/// </summary>
		/// <param name="go">GameObject to destroy</param>
		public static void Destroy(this UnityEngine.Object go) => UnityEngine.Object.Destroy(go);


		/// <summary>
		/// Shorthand for destroying any object immediately
		/// </summary>
		/// <param name="go">GameObject to destroy</param>
		public static void DestroyImmediate(this UnityEngine.Object go) => UnityEngine.Object.DestroyImmediate(go);
	}
}