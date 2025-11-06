using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseSDK {
	/// <summary>
	/// Used to check if game game's screen was updated/resized
	/// </summary>
	public class ScreenResolutionUpdater : Singleton<ScreenResolutionUpdater> {
		[Serializable]
		public struct ScreenResolutionChangeInfo {
			public Vector2 PreviousResolution;
			public Vector2 CurrentResolution;

			public ScreenOrientation PreviousOrientation;
			public ScreenOrientation CurrentOrientation;

			public ScreenOrientation PreviousResolutionBasedOrientation;
			public ScreenOrientation CurrentResolutionBasedOrientation;

			public float AspectRatioBefore;
			public float AspectRatioAfter;

			public readonly Vector2 ResolutionDelta => CurrentResolution - PreviousResolution;

			public readonly bool IsLandscapeBefore => AspectRatioBefore >= 1f;
			public readonly bool IsLandscapeAfter => AspectRatioAfter >= 1f;

			public readonly bool HasAnyChange => PreviousResolution != CurrentResolution || PreviousOrientation != CurrentOrientation;

			public override readonly string ToString() =>
				$"PrevRes: {PreviousResolution}, CurrRes: {CurrentResolution},  PrevOri: {PreviousOrientation}, CurrOri: {CurrentOrientation}, ResolutionChanged: {PreviousResolution != CurrentResolution}, OrientationChanged: {PreviousOrientation != CurrentOrientation}";
		}

		public static event Action<ScreenResolutionChangeInfo> OnAnyChange;
		public static event Action<ScreenResolutionChangeInfo> OnResolutionChange;
		public static event Action<ScreenResolutionChangeInfo> OnOrientationChange;
		public static event Action<ScreenResolutionChangeInfo> OnResolutionBasedOrientationChange;

		private Vector2 m_PreviousResolution;
		private ScreenOrientation m_PreviousOrientation;
		private ScreenOrientation m_PreviousLogicalLandscape;

		/// <summary>
		/// Initialize
		/// </summary>
		private void Awake() {
			DontDestroy = true;
			m_PreviousResolution = new Vector2(Screen.width, Screen.height);
			m_PreviousOrientation = Screen.orientation;
			m_PreviousLogicalLandscape = Screen.width >= Screen.height ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;
		}

		private void Update() {
			var currentResolution = new Vector2(Screen.width, Screen.height);
			var currentOrientation = Screen.orientation;
			var currentLogicalLandscape = Screen.width >= Screen.height ? ScreenOrientation.LandscapeLeft : ScreenOrientation.Portrait;

			var resolutionChanged = currentResolution != m_PreviousResolution;
			var orientationChanged = false;
			var logicalOrientationChanged = false;

			// Check for both system orientation change and logical (width/height) orientation change
			if (m_PreviousOrientation != currentOrientation)
				orientationChanged = true;
			if (m_PreviousLogicalLandscape != currentLogicalLandscape)
				logicalOrientationChanged = true;

			if (!resolutionChanged && !orientationChanged)
				return;

			var info = new ScreenResolutionChangeInfo {
				PreviousResolution = m_PreviousResolution,
				CurrentResolution = currentResolution,
				PreviousOrientation = m_PreviousOrientation,
				CurrentOrientation = currentOrientation,
				PreviousResolutionBasedOrientation = m_PreviousLogicalLandscape,
				CurrentResolutionBasedOrientation = currentLogicalLandscape,
				AspectRatioBefore = m_PreviousResolution.x / m_PreviousResolution.y,
				AspectRatioAfter = currentResolution.x / currentResolution.y,

			};

			if (resolutionChanged)			OnResolutionChange?.Invoke(info);
			if (orientationChanged)			OnOrientationChange?.Invoke(info);
			if (logicalOrientationChanged)	OnResolutionBasedOrientationChange?.Invoke(info);
			if (info.HasAnyChange)			OnAnyChange?.Invoke(info);

			// Update stored state
			m_PreviousResolution = currentResolution;
			m_PreviousOrientation = currentOrientation;
			m_PreviousLogicalLandscape = currentLogicalLandscape;
		}
	}
}