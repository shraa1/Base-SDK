//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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