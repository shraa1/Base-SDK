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
using UnityEngine;

namespace BaseSDK.SocialMediaPlug {
	public class SocialMediaPopupManager : Singleton<SocialMediaPopupManager> {

		[SerializeField] private List<SocialMediaItem> socialMediaItems = new List<SocialMediaItem>();
		[SerializeField] private RectTransform plugParent;
		[SerializeField] private SocialMediaPopup plugItemPrefab;
		[SerializeField] private AnimationCurve bounceCurve;
		[SerializeField] private float gapBetweenTwoPopupsAnimation = 2;
		[SerializeField] private float initialAnimationStartDelay = 1;

		protected virtual void Awake() {
			for(var i = 0; i < socialMediaItems.Count; i++) {
				var instantiated = Instantiate(plugItemPrefab, plugParent);
				instantiated.gameObject.SetActive(true);
				instantiated.Set(socialMediaItems[i]);
				Utils.Utilities.WaitBeforeExecuting(initialAnimationStartDelay + i * gapBetweenTwoPopupsAnimation, () => instantiated.StartAnimating(bounceCurve));
			}
		}
	}
}