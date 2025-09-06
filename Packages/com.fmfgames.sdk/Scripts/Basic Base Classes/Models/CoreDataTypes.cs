using System;

namespace BaseSDK.Models {
	/// <summary>
	/// What is the scope of this service?
	/// Since the enum can't be extended in the client project, use 0 to 999 for BaseSDK additions,
	/// and add game-specific Scopes in the client project from 1000+
	/// </summary>
	public enum ServicesScope {
		GLOBAL = 0,
		LOBBY = 1,
		GAME = 2,
	}

	/// <summary>
	/// All sources of currencies in the game
	/// </summary>
	public enum CurrencySource {
		COINS = 0,
	}

	[Serializable]
	public struct RewardItem {
		public CurrencySource Currency;
		public double Amount;
	}
}