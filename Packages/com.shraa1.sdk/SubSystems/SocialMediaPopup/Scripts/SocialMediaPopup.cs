//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using BaseSDK.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace BaseSDK.SocialMediaPlug {
	public class SocialMediaPopup : MonoBehaviour {
		[SerializeField] private Image imgComponentCached;
		[SerializeField] private Button btnComponentCached;
		[SerializeField] private RectTransform rtComponentCached;
		private bool isAnimating = false, isStopped = false;
		private float initialPosition;
		private float time = 0f;
		private AnimationCurve curve;

		private void Awake() => initialPosition = rtComponentCached.anchoredPosition.y;

		public void Set(SocialMediaItem item) {
			btnComponentCached.onClick.AddListener(() => {
				Application.OpenURL(item.link);
				StopAnimation();
			});
			imgComponentCached.sprite = item.sprite;
		}

		public void StartAnimating(AnimationCurve curve) {
			isAnimating = true;
			this.curve = curve;
		}

		private void StopAnimation() => isStopped = true;

		private void Update() {
			if (!isAnimating)
				return;

			if (isStopped && rtComponentCached.anchoredPosition.y == initialPosition) {
				isAnimating = false;
				time = 0;
				isStopped = false;
				return;
			}

			rtComponentCached.anchoredPosition = new Vector2(rtComponentCached.anchoredPosition.x, curve.Evaluate(time));

			time += Time.deltaTime;
		}
	}
}