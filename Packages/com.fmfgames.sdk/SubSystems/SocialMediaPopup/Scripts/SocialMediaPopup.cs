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