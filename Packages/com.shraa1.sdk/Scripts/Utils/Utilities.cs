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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseSDK.Utils {
	public class Utilities {
		public static T EitherOf<T>(params T[] args) => args[UnityEngine.Random.Range(0, args.Length)];

		public static Tweener WaitBeforeExecuting(float waitTime, Action action) {
			if (waitTime == 0f) {
				action?.Invoke();
				return null;
			}
			float a = 0f;
			return DOTween.To(() => a, x => a = x, waitTime, waitTime)
				.OnComplete(() => action?.Invoke());
		}

		public static char GetRandomCharacter () {
			var start = UnityEngine.Random.Range(0, 2) == 0 ? 'a' : 'A';
			return (char)(start + UnityEngine.Random.Range(0, 26));
		}

		public static List<GameObject> FindGameObjectsOfName(IEnumerable<string> names) {
			var list = new List<GameObject>();
			names.ToList().ForEach(x =>
				list.AddRange(UnityEngine.Object.FindObjectsOfType<GameObject>().ToList().FindAll(y => y.name == x)));
			return list;
		}
	}
}