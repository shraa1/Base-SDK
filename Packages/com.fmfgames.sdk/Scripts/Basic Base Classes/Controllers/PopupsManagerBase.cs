using System;
using System.Collections.Generic;
using BaseSDK.Models;
using BaseSDK.Services;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using BaseSDK.SirenixHelper;
#endif
using BaseSDK.Popups.Views;
using UnityEngine;
using UnityEngine.UI;
using BaseSDK.Controllers;

namespace BaseSDK.Popups.Controllers {
	public abstract class PopupsManagerBase<POPUPTYPE> : Configurable, IPopupService<POPUPTYPE> where POPUPTYPE : Enum {
		#region Inspector Variables
		[FoldoutGroup("Popups"), SerializeField] protected List<NamedPopupBtnReference> m_AllPopups;
		[FoldoutGroup("Special class types"), SerializeField] protected PopupBase<POPUPTYPE> m_SettingsPopupType;
		#endregion Inspector Variables

		#region Variables
		protected IPopup<POPUPTYPE> m_CurrentPopup;
		protected POPUPTYPE m_PendingPopupType;
		protected bool m_IsTransitioning = false;
		#endregion Variables

		#region Interface Implementation
		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(IPopupService<POPUPTYPE>));

		public virtual void Show (POPUPTYPE popupType) => TryOpenPopup(popupType);

		public virtual void Close () => TryOpenPopup(default);
		#endregion Interface Implementation

		#region Unity Methods
		protected virtual void Awake () => m_AllPopups.ForEach(x => x.Btn.onClick.AddListener(() => TryOpenPopup(x.PopupBase.PopupType)));
		#endregion Unity Methods

		#region Private/Protected Methods
		protected virtual void TryOpenPopup (POPUPTYPE popupType) {
			if ((m_CurrentPopup != null && EnumEquals(m_CurrentPopup.PopupType, popupType)) || EnumEquals(popupType, default)) {
				m_IsTransitioning = true;
				CloseCurrentPopup();
				return;
			}

			if (m_CurrentPopup != null) {
				m_PendingPopupType = popupType;
				m_IsTransitioning = true;

				if (EnumEquals(popupType, m_SettingsPopupType.PopupType))
					CloseCurrentPopup();
				else
					CloseCurrentPopup(OnPopupClosed);
				return;
			}

			OpenPopup(popupType);
		}

		protected virtual void OnPopupClosed () {
			m_IsTransitioning = false;
			if (EnumEquals(m_PendingPopupType, default)) return;

			var typeToOpen = m_PendingPopupType;
			m_PendingPopupType = default;
			OpenPopup(typeToOpen);
		}

		protected virtual void OpenPopup (POPUPTYPE popupType) {
			var popup = m_AllPopups.Find(p => EnumEquals(p.PopupBase.PopupType, popupType));
			if (!m_IsTransitioning) {
				m_IsTransitioning = true;
				_ = popup?.PopupBase.OpenPopup(() => {
					m_IsTransitioning = false;
					m_CurrentPopup = popup.PopupBase;
				});
			}
		}

		protected virtual void CloseCurrentPopup (Action onComplete) {
			if (m_CurrentPopup == null) {
				m_IsTransitioning = false;
				onComplete?.Invoke();
				return;
			}

			_ = m_CurrentPopup.ClosePopup(() => {
				m_CurrentPopup = null;
				m_IsTransitioning = false;
				onComplete?.Invoke();
			});
		}
		#endregion Private/Protected Methods

		#region Public Methods
		public virtual void ShowUIPopupCloseOthers (POPUPTYPE popupType) => TryOpenPopup(popupType);

		public virtual void CloseCurrentPopup () => CloseCurrentPopup(null);
		#endregion Public Methods

		#region Custom DataTypes
		[Serializable]
		public class NamedPopupBtnReference {
			public Button Btn;
			public PopupBase<POPUPTYPE> PopupBase;
		}
		#endregion Custom DataTypes

		protected static bool EnumEquals(POPUPTYPE a, POPUPTYPE b) => EqualityComparer<POPUPTYPE>.Default.Equals(a, b);
	}
}