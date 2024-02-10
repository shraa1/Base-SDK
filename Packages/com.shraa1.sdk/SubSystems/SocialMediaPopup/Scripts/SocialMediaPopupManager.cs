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