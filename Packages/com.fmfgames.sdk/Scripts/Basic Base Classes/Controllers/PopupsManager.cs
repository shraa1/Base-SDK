using System;
using System.Collections.Generic;
using BaseSDK.Models;
using BaseSDK.Services;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using BaseSDK.SirenixHelper;
#endif
using BaseSDK.Views;
using UnityEngine;
using UnityEngine.UI;

namespace BaseSDK.Controllers {
	public class PopupsManager<POPUPTYPE> : Configurable, IPopupService<POPUPTYPE> where POPUPTYPE : Enum {
		#region Inspector Variables
		[FoldoutGroup("Popups"), SerializeField] private List<NamedInputActionReference> m_AllPopups;
		[FoldoutGroup("Special class types"), SerializeField] private PopupBase<POPUPTYPE> m_SettingsPopupType;
		#endregion Inspector Variables

		#region Private Variables
		private IPopup<POPUPTYPE> m_CurrentPopup;
		private POPUPTYPE m_PendingPopupType;
		private bool m_IsTransitioning = false;
		#endregion Private Variables

		#region Interface Implementation
		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(IPopupService<POPUPTYPE>));

		public virtual void Show (POPUPTYPE popupType) => TryOpenPopup(popupType);

		public virtual void Close () => TryOpenPopup(default);
		#endregion Interface Implementation

		#region Unity Methods
		private void Awake () => m_AllPopups.ForEach(x => x.Btn.onClick.AddListener(() => TryOpenPopup(x.PopupBase.PopupType)));
		#endregion Unity Methods

		#region Private Methods
		private void TryOpenPopup (POPUPTYPE popupType) {
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

		private void OnPopupClosed () {
			m_IsTransitioning = false;
			if (EnumEquals(m_PendingPopupType, default)) return;

			var typeToOpen = m_PendingPopupType;
			m_PendingPopupType = default;
			OpenPopup(typeToOpen);
		}

		private void OpenPopup (POPUPTYPE popupType) {
			var popup = m_AllPopups.Find(p => EnumEquals(p.PopupBase.PopupType, popupType));
			if (!m_IsTransitioning) {
				m_IsTransitioning = true;
				_ = popup?.PopupBase.OpenPopup(() => {
					m_IsTransitioning = false;
					m_CurrentPopup = popup.PopupBase;
				});
			}
		}

		private void CloseCurrentPopup (Action onComplete) {
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
		#endregion Private Methods

		#region Public Methods
		public virtual void ShowUIPopupCloseOthers (POPUPTYPE popupType) => TryOpenPopup(popupType);

		public virtual void CloseCurrentPopup () => CloseCurrentPopup(null);
		#endregion Public Methods

		#region Custom DataTypes
		[Serializable]
		public class NamedInputActionReference {
			public Button Btn;
			public PopupBase<POPUPTYPE> PopupBase;
		}
		#endregion Custom DataTypes

		private static bool EnumEquals(POPUPTYPE a, POPUPTYPE b) =>
			EqualityComparer<POPUPTYPE>.Default.Equals(a, b);
	}
}