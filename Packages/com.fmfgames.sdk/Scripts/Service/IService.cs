using System;

namespace BaseSDK.Services {
	public interface IService : IConfigurable {
		(int scope, Type interfaceType) RegisteringTypes { get; }
	}

	public interface IServiceProvider {
		T Get<T>() where T : class, IService;
		void Register(Type type, IService service);
		void Unregister<T>() where T : class, IService;
	}
}