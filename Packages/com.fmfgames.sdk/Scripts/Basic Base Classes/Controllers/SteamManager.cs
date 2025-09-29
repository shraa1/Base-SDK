#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX) || !STEAMWORKS_NET
#define DISABLESTEAMWORKS
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using BaseSDK.Services;
using System.Collections;
using BaseSDK.Controllers;
using BaseSDK.Models;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

namespace BaseSDK.Steam.Controllers {
	// The SteamManager is designed to work with Steamworks.NET
	// This file is released into the public domain.
	// Where that dedication is not recognized you are granted a perpetual,
	// irrevocable license to copy and modify this file as you see fit.
	// Version: 1.0.12

	//
	// The SteamManager provides a base implementation of Steamworks.NET on which you can build upon.
	// It handles the basics of starting up and shutting down the SteamAPI for use.
	//
	[DisallowMultipleComponent]
	public abstract class SteamManagerBase : Configurable, ISteamService {
#if !DISABLESTEAMWORKS
		#region Variables/Consts
		protected SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

		private static readonly Dictionary<string, double> m_StatCache = new();
		private float m_StatLastStoreTime = 0f;
		private const float m_StoreCooldown = 30f;
		#endregion Variables/Consts

		[AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
		protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText) => Debug.LogWarning(pchDebugText);

		#region Interface Implementations
		public (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(ISteamService));
		public bool IsSteamInitialized { get; private set; } = false;
		public bool IsSteamRunning => SteamAPI.IsSteamRunning();

		public override IEnumerator Setup() {
			if (!Packsize.Test())
				Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);

			if (!DllCheck.Test())
				Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);

			yield return null;

			// Initializes the Steamworks API.
			// If this returns false then this indicates one of the following conditions:
			// [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
			// [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
			// [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
			// [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
			// [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
			// Valve's documentation for this is located here:
			// https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
			IsSteamInitialized = SteamAPI.Init();

			if (IsSteamInitialized) {
				// Set up our callback to receive warning messages from Steam.
				// You must launch with "-debug_steamapi" in the launch args to receive warnings.
				m_SteamAPIWarningMessageHook = new(SteamAPIDebugTextHook);
				SteamClient.SetWarningMessageHook(m_SteamAPIWarningMessageHook);
			}

			Initialized = true;
		}
		#endregion Interface Implementations

		#region Unity Methods
		// OnApplicationQuit gets called too early to shutdown the SteamAPI.
		// Because the SteamManager should be persistent and never disabled or destroyed we can shutdown the SteamAPI here.
		// Thus it is not recommended to perform any Steamworks work in other OnDestroy functions as the order of execution can not be garenteed upon Shutdown. Prefer OnDisable().
		protected virtual void OnDestroy() {
			if (IsSteamInitialized)
				SteamAPI.Shutdown();
		}

		protected virtual void Update() {
			if (IsSteamInitialized)
				// Run Steam client callbacks
				SteamAPI.RunCallbacks();
		}
		#endregion Unity Methods

		#region Helper Methods
		public void UpdateStats(string statName, double value, bool forceUpdate = false) {
			if (!IsSteamInitialized || value < 0)
				return;

			//TODO FIXME this entire logic needs some thorough testing

			// Store stats based on cooldown or forceUpdate
			if (forceUpdate || Time.realtimeSinceStartup - m_StatLastStoreTime >= m_StoreCooldown) {
				// Check if value has changed
				if (!m_StatCache.ContainsKey(statName) || !Mathf.Approximately((float)m_StatCache[statName], (float)value)) {
					var existingValue = 0d;
					if (SteamUserStats.GetStat(statName, out int intValue))
						existingValue = intValue;
					else if (SteamUserStats.GetStat(statName, out float floatValue))
						existingValue = floatValue;

					if (existingValue > value)
						value = existingValue;

					m_StatCache[statName] = value;

					// Use int if value fits within int range and is an integer
					_ = value <= int.MaxValue && Math.Abs(value % 1) < double.Epsilon
						? SteamUserStats.SetStat(statName, (int)value)
						: SteamUserStats.SetStat(statName, (float)value);
				}

				m_StatLastStoreTime = Time.realtimeSinceStartup;
			}
		}

		public void StoreStats() {
			if (!IsSteamInitialized)
				return;

			_ = SteamUserStats.StoreStats();
		}
		#endregion Helper Methods
#else
		public (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(ISteamService));
		public bool IsSteamInitialized => false;
#endif // !DISABLESTEAMWORKS
	}
}