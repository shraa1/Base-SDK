using System;
using System.Collections.Generic;

namespace BaseSDK.Services {
	public class ServiceProvider : IServiceProvider {
		#region Variables
		private readonly Dictionary<Type, IService> m_Services = new();
		#endregion Variables

		#region Methods
		public T Get<T>() where T : class, IService => m_Services.TryGetValue(typeof(T), out var service) ? (T)service : null;
		public void Register(Type type, IService service) => m_Services[type] = service;
		public void Unregister<T>() where T : class, IService => m_Services.Remove(typeof(T));
		#endregion Methods
	}
}