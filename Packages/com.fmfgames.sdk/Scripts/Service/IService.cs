using System;
using BaseSDK.Models;

namespace BaseSDK.Services {
	public interface IService {
		(int scope, Type interfaceType) RegisteringTypes { get; }
	}

	public interface IServiceProvider {
		T Get<T>() where T : class, IService;
		void Register(Type type, IService service);
		void Unregister<T>() where T : class, IService;
	}
}