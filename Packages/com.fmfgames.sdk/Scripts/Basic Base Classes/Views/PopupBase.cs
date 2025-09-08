using System;
using System.Collections.Generic;
using DG.Tweening;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using BaseSDK.SirenixHelper;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace BaseSDK.Views {
	public abstract class PopupBase<POPUPTYPE> : MonoBehaviour, IPopup<POPUPTYPE> where POPUPTYPE : Enum {
		#region Inspector Variables
		[FoldoutGroup("Popup Base Fields"), SerializeField] protected Button m_BlockerImage;
		[FoldoutGroup("Popup Base Fields"), SerializeField] protected RectTransform m_AnimationRoot;
		[FoldoutGroup("Popup Base Fields"), SerializeField] protected float m_OpenClosePopupSpeed = .3f;
		[FoldoutGroup("Popup Base Fields"), SerializeField] protected Ease m_OpenPopupEase = Ease.OutBounce;
		[FoldoutGroup("Popup Base Fields"), SerializeField] protected Ease m_ClosePopupEase = Ease.InBounce;
		[FoldoutGroup("Popup Base Fields"), SerializeField] private POPUPTYPE m_PopupType;
		#endregion Inspector Variables

		#region Properties
		public POPUPTYPE PopupType { get => m_PopupType; set => m_PopupType = value; }
		#endregion Properties

		#region Variables
		protected List<Tweener> m_AllTweener = new();
		protected Vector3 m_AlmostZero = new(0f, .001f, 1f);
		#endregion Variables

		#region Interface Implementation
		public bool IsOpen { get; set; }

		public virtual Tweener OpenPopup (Action onComplete = null) {
			m_BlockerImage.gameObject.SetActive(true);
			return PlayAnimation(true, Vector3.one, m_OpenPopupEase, onComplete);
		}

		public virtual Tweener ClosePopup (Action onComplete = null) => PlayAnimation(false, m_AlmostZero, m_ClosePopupEase, onComplete);
		#endregion Interface Implementation

		#region Unity Methods
		protected virtual void Awake () {
			_ = ClosePopup();
			m_BlockerImage.gameObject.SetActive(false);
		}
		#endregion Unity Methods

		#region Private/Protected Helpers
		protected virtual Tweener PlayAnimation (bool open, Vector3 finalVector, Ease ease, Action onComplete = null) {
			IsOpen = m_BlockerImage.interactable = false;
			m_AllTweener.ForEach(x => x?.Kill());
			m_AllTweener.Clear();
			var tweener = m_AnimationRoot.DOScale(finalVector, m_OpenClosePopupSpeed)
				.SetEase(ease)
				.OnComplete(() => {
					IsOpen = m_BlockerImage.interactable = open;
					m_BlockerImage.gameObject.SetActive(open);
					onComplete?.Invoke();
				});
			m_AllTweener.Add(tweener);
			return tweener;
		}
		#endregion Private/Protected Helpers

	}
}