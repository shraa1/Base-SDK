using System;
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using UnityEngine;

namespace BaseSDK.Utils {
	public static class Utilities {
		public static T EitherOf<T> (params T[] args) => args[UnityEngine.Random.Range(0, args.Length)];

		public static Tweener WaitBeforeExecuting (float waitTime, Action action) {
			if (Mathf.Approximately(waitTime, 0f)) {
				action?.Invoke();
				return null;
			}
			var a = 0f;
			return DOTween.To(() => a, x => a = x, waitTime, waitTime)
				.OnComplete(() => action?.Invoke());
		}

		public static char GetRandomCharacter () {
			var start = UnityEngine.Random.Range(0, 2) == 0 ? 'a' : 'A';
			return (char)(start + UnityEngine.Random.Range(0, 26));
		}

		public static List<GameObject> FindGameObjectsOfName (IEnumerable<string> names) {
			var list = new List<GameObject>();
			names.ToList().ForEach(x =>
				list.AddRange(UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None).ToList().FindAll(y => y.name == x)));
			return list;
		}

		public static void Quit () =>
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
	}
}