using System;

namespace BaseSDK.Services {
	public interface IPopupService : IService {
		void Show<T> (T type) where T : Enum;
		void Close<T> (T type) where T : Enum;
	}
}