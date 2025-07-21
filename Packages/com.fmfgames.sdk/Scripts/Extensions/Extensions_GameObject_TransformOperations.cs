using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseSDK.Extension {
	public static partial class Extensions {

		public static T GetOrAddComponent<T> (this GameObject gameObject) where T : Component {
			var a = gameObject.GetComponent<T>();
			a ??= gameObject.AddComponent<T>();
			return a;
		}

		public static List<Transform> GetChildren (this Transform t, bool findInactiveChildren = true) {
			if (t == null)
				throw new NullReferenceException("Can't Get Children of Null Transform");
			var l = t.GetComponentsInChildren<Transform>(findInactiveChildren).ToList();
			l.RemoveSafely(t);
			return l;
		}

		public static List<GameObject> GetChildren (this GameObject go, bool findInactiveChildren = true) => go.transform.GetChildren(findInactiveChildren)?.Select(x => x.gameObject)?.ToList();

		public static IEnumerable<Transform> GetSiblings (this Transform t, bool findInactiveSiblings = true) => t.parent?.GetComponentsInChildren<Transform>(findInactiveSiblings)?.ToList()?.RemoveSafely(t, t.parent);

		public static IEnumerable<GameObject> GetSiblings (this GameObject go, bool findInactiveSiblings = true) => go.transform.GetSiblings(findInactiveSiblings)?.Select(x => x.gameObject);

		public static List<GameObject> FindGameObjectsOfName (string name) => UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().FindAll(x => x.name == name);

		/// <summary>
		/// Is this transform a child of a transform that has a component of this type
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="transform">Child Transform</param>
		/// <returns>Is this transform a child of any transform with this type as a component</returns>
		public static bool IsNestedChildOfType<T> (this Transform transform) where T : Component {
			if (transform.parent == null)
				return false;
			if (transform.parent.GetComponent<T>() == null)
				return transform.parent.IsNestedChildOfType<T>();
			return transform.parent.GetComponent<T>() != null;
		}

		/// <summary>
		/// Shorthand for destroying any object
		/// </summary>
		/// <param name="go">GameObject to destroy</param>
		public static void Destroy (this UnityEngine.Object go) => UnityEngine.Object.Destroy(go);


		/// <summary>
		/// Shorthand for destroying any object immediately
		/// </summary>
		/// <param name="go">GameObject to destroy</param>
		public static void DestroyImmediate (this UnityEngine.Object go) => UnityEngine.Object.DestroyImmediate(go);
	}
}