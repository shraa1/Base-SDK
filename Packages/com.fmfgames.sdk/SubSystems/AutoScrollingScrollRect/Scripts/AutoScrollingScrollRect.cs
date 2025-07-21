#pragma warning restore CS0649     // Unused variable will always be null
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using BaseSDK.Controllers;
using System;

namespace BaseSDK.Views {
	/// <summary>
	/// Used for auto scrolling, that can not be manipulated by the player. Example use case is "Credits" section scrolling in any game or movie.
	/// </summary>
#if UNITY_EDITOR
	[RequireComponent(typeof(ScrollRect)), CanEditMultipleObjects]
#else
	[RequireComponent(typeof(ScrollRect))]
#endif
	public class AutoScrollingScrollRect : MonoBehaviour {
		#region Custom DataTypes
		public enum ScrollDirection {
			/// <summary>
			/// Scrolls vertically
			/// </summary>
			Vertical,
			/// <summary>
			/// Scrolls horizontally
			/// </summary>
			Horizontal
		}
		#endregion

		#region Variables & Constants
		/// <summary>
		/// Needed to know if the curve should be used for the x or y axis. Using 2 separate curved for Horizontal & Vertical curves will make this redundant,
		/// but there seems no practical need to use for a diagonal-ish scroll, so not adding unnecessary clutter in the editor
		/// </summary>
		public ScrollDirection scrollDirection = ScrollDirection.Vertical;

		/// <summary>
		/// Used to evaluate the point to move the scrollrect to. Helps create EaseIn, EaseOut, etc. like functionalities
		/// </summary>
		[SerializeField] private AnimationCurve curve;

		/// <summary>
		/// Sadly, we can't get the curve's proper length, so we have to explicitly say how long the scrolling is taking place for.
		/// </summary>
		[SerializeField, Tooltip("Time in seconds to scroll the content")] private float scrollTime = 1f;

		/// <summary>
		/// Is the curve using actual values from the scene? Is the content supposed to move from position 0 to 1200? Then you're
		/// not using a normalized curve. If you want to use normalized curve, mark the curve between 0 and 1, then the content's
		/// size is used for multiplication. Better to use normalized curves if you end up changing the content sizes far too often
		/// or the content is filled dynamically. If you want exact precise values, use non-normalized curves instead.
		/// </summary>
		[SerializeField, Tooltip("Is the curve using actual values from the scene? Is the content supposed to move from position 0 to 1200? Then you're not using a normalized curve. If you want to use normalized curve, mark the curve between 0 and 1, then the content's size is used for multiplication. Better to use normalized curves if you end up changing the content sizes far too often or the content is filled dynamically. If you want exact precise values, use non-normalized curves instead.")]
		private bool isCurveNormalized = true;

		/// <summary>
		/// Immediately start scrolling the scrollrect.
		/// </summary>
		[SerializeField] private bool startScrollingOnLoad = true;

		private ScrollRect scrollRect;
		private RectTransform rt;
		private float currentTime = 0f;

		private event Action OnUpdate;
		#endregion

		#region Unity Methods
		/// <summary>
		/// Awake. Setup up everything that is needed for scrolling. Start scrolling if startScrollingOnLoad was true
		/// </summary>
		private void Awake() {
			AllSetupMethods();
		}

		private void Update () {
			OnUpdate?.Invoke();
		}

		private void OnEnable () {
			if (startScrollingOnLoad)
				StartScroll();
		}

		/// <summary>
		/// OnDisable. Unhook the ScrollingEvaluation method on the TimerController's OnUpdate
		/// </summary>
		private void OnDisable() => OnUpdate -= ScrollingEvaluation;
		#endregion

		#region Helper Methods
		/// <summary>
		/// Cache elements that are needed for use later
		/// </summary>
		private void CacheElements() {
			scrollRect = GetComponent<ScrollRect>();
			rt = GetComponent<RectTransform>();
		}

		/// <summary>
		/// Disable Manual scrolling, horizontal or vertical
		/// </summary>
		private void DisableManualScrolling() {
			scrollRect.horizontal = false;
			scrollRect.vertical = false;
		}

		/// <summary>
		/// Do any functionality to set up the auto scrolling
		/// </summary>
		[ContextMenu("AllSetupMethods")]
		private void AllSetupMethods() {
			CacheElements();
			DisableManualScrolling();
		}

		/// <summary>
		/// Public method to be called to start scrolling
		/// </summary>
		public void StartScroll() {
			currentTime = 0f;
			StartCoroutine(StartScrolling());
			OnUpdate += ScrollingEvaluation;
		}

		/// <summary>
		/// Evaluate and apply the position to the content in the OnUpdate event
		/// </summary>
		private void ScrollingEvaluation() {
			var val = curve.Evaluate(currentTime) * (!isCurveNormalized ? 1f : GetContentSizeBasedOnDirection());
//TODO Check if horizontal is working correctly, might not be working correctly
			scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, val);
			currentTime += Time.deltaTime;
		}

		private float GetContentSizeBasedOnDirection() => scrollDirection == ScrollDirection.Vertical ? scrollRect.content.rect.height - scrollRect.viewport.rect.height : scrollRect.content.rect.width - scrollRect.viewport.rect.width;

		/// <summary>
		/// Wait for scrollTime seconds, and then unsubscribe the ScrollingEvaluation method from OnUpdate event
		/// </summary>
		private IEnumerator StartScrolling() {
			yield return new WaitForSeconds(scrollTime);
			OnUpdate -= ScrollingEvaluation;
		}
		#endregion
	}

	namespace EditorScripts {
#if UNITY_EDITOR
		[CustomEditor(typeof(AutoScrollingScrollRect))]
		public class AutoScrollingScrollRectEditor : Editor {
			public override void OnInspectorGUI() {
				var aSR = (target as AutoScrollingScrollRect);
				var sR = aSR.GetComponent<ScrollRect>();

				if ((aSR.scrollDirection == AutoScrollingScrollRect.ScrollDirection.Horizontal && sR.content.rect.width < sR.viewport.rect.width) ||
					(aSR.scrollDirection == AutoScrollingScrollRect.ScrollDirection.Vertical && sR.content.rect.height < sR.viewport.rect.height))
					EditorGUILayout.HelpBox("Make sure that the content size (horizontal/vertical, depending on your scrollDirection) is bigger than that of the viewport that encompases the content", MessageType.Warning, true);

				base.OnInspectorGUI();
			}
		}
#endif
	}
}