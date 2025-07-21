using System.Collections.Generic;

namespace BaseSDK.Services {
	public static class GlobalServices {
		/// <summary>
		/// Use a game client side enum to register services for context based service providers
		/// </summary>
		public static Dictionary<int, IServiceProvider> ServiceProviders = new();

		/// <summary>
		/// Initialize a service provider based on the context, like lobby, game, etc.
		/// </summary>
		/// <param name="contextIndex">What context is the service registered for?</param>
		public static void Init(int contextIndex) {
			ServiceProviders[contextIndex] ??= new ServiceProvider();
		}
	}
}