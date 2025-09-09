using System;
using System.Collections.Generic;
using BaseSDK.Extension;

namespace BaseSDK.Services {
	public static class GlobalServices {
		/// <summary>
		/// Use a game client side enum to register services for context based service providers
		/// </summary>
		private static readonly Dictionary<int, IServiceProvider> s_ServiceProviders = new();

		/// <summary>
		/// Initialize a service provider based on the context, like lobby, game, etc.
		/// </summary>
		/// <param name="contextIndex">What context is the service registered for?</param>
		public static void Initialize<T>(T contextIndex) where T : Enum =>
			s_ServiceProviders.AddOrUpdate(contextIndex.To<int>(), new ServiceProvider());

		/// <summary>
		/// Uninitialize a service provider when the scope of the service ended
		/// </summary>
		/// <param name="contextIndex"></param>
		public static void UnInitialize<T>(T contextIndex) where T : Enum =>
			s_ServiceProviders.RemoveSafely(contextIndex.To<int>());

		/// <summary>
		/// Get the Service Provider for the type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">IService being fetched</typeparam>
		/// <param name="serviceScope">Scope where the service service was registered</param>
		/// <returns>Returns the relevant service if found, in the current scope</returns>
		public static IServiceProvider GetServiceProvider<T>(T serviceScope) where T : Enum =>
			s_ServiceProviders.GetSafely(serviceScope.To<int>());
	}
}