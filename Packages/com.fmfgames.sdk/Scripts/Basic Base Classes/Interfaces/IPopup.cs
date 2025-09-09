using System;
using DG.Tweening;

namespace BaseSDK {
	/// <summary>
	/// Interface for UI popups that can open and close with tween animation.
	/// </summary>
	public interface IPopup<POPUPTYPE> where POPUPTYPE : Enum {
		#region Variables
		POPUPTYPE PopupType { get; }

		bool IsOpen { get; }

		Tweener OpenPopup (Action onComplete = null);
		Tweener ClosePopup (Action onComplete = null);
		#endregion Variables
	}
}