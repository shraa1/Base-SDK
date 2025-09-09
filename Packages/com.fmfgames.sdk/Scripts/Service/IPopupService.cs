using System;

namespace BaseSDK.Services {
	public interface IPopupService<POPUPTYPE> : IService where POPUPTYPE : Enum {
		void Show (POPUPTYPE type);
		void Close ();
	}
}