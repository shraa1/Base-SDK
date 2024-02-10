using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		public static void PauseTweeners<T>(this Singleton<T> singleton) where T : MonoBehaviour =>
			Singleton<T>.allTweeners.ForEach((Tweener tweener) => tweener.Pause());
			
		public static void ResumeTweeners<T>(this Singleton<T> singleton) where T : MonoBehaviour =>
			Singleton<T>.allTweeners.ForEach((Tweener tweener) => tweener.Play());

		public static List<Tweener> Kill(this List<Tweener> tweeners) { tweeners?.ForEach(x => x.Kill()); return tweeners; }

		public static Tweener DoFade(this TextMesh textMesh, float endValue, float duration) {
			float a = textMesh.color.a;
			return DOTween.To(() => a, x => a = x, endValue, duration).OnUpdate(() => {
				Color c = textMesh.color;
				c.a = a;
				textMesh.color = c;
			});
		}

		public static Tweener DoFade(this TextMeshPro textMesh, float endValue, float duration) {
			float a = textMesh.color.a;
			return DOTween.To(() => a, x => a = x, endValue, duration).OnUpdate(() => {
				Color c = textMesh.color;
				c.a = a;
				textMesh.color = c;
			});
		}

		public static Tweener OnComplete<T>(this Tweener tweener, Action<T> onComplete, T val) => tweener.OnComplete(() => onComplete?.Invoke(val));
	}
}