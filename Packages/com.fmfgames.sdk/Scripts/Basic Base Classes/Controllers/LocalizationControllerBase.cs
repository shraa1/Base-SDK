using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using BaseSDK.Models;
using BaseSDK.Services;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace BaseSDK.Controllers {
	public abstract class LocalizationControllerBase : Configurable, ILocalizationService {
		#region String Tables
		[SerializeField] private List<LocalizedStringTable> m_LocalizationTables = new();
		#endregion String Tables

		#region Properties
		public List<CultureInfo> SupportedLanguages { get; } = new();
		public List<LocalizedStringTable> LocalizationTables => m_LocalizationTables;
		#endregion Properties

		#region Interface Implementation
		public virtual (int scope, Type interfaceType) RegisteringTypes => ((int)ServicesScope.GLOBAL, typeof(ILocalizationService));

		/// <summary>
		/// Wait for Localization to finish initializing
		/// </summary>
		/// <returns></returns>
		public override IEnumerator Setup () {
			yield return LocalizationSettings.InitializationOperation;
			LocalizationSettings.AvailableLocales.Locales.ForEach(x => SupportedLanguages.Add(new(x.Formatter.ToString())));
			Initialized = true;
		}

		/// <summary>
		/// Sets the new language
		/// </summary>
		/// <param name="index">index of the language from the list of languages</param>
		public void SetLanguage (int index) => LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
		#endregion Interface Implementation

		#region Public Helper Methods
		/// <summary>
		/// Get the LocalizedStringTable using a name
		/// </summary>
		/// <param name="tableName">Name as serialized for a particular table</param>
		/// <returns>Returns the LocalizedStringTable in NamedLocalizedStringTable</returns>
		public LocalizedStringTable GetLocalizedStringTable (string tableName) =>
			LocalizationTables.Find(x => x.TableReference.TableCollectionName == tableName);

		/// <summary>
		/// Get StringTable using a NamedLocalizedStringTable
		/// </summary>
		/// <param name="tableName">Name as serialized for a particular table</param>
		/// <returns>Returns the StringTable from the LocalizedStringTable in NamedLocalizedStringTable</returns>
		public StringTable GetStringTable (string tableName) => GetLocalizedStringTable(tableName)?.GetTable();

		/// <summary>
		/// Get Table Reference using a NamedLocalizedStringTable
		/// </summary>
		/// <param name="tableName">Name as serialized for a particular table</param>
		/// <returns>Returns the LocalizedStringTable in NamedLocalizedStringTable</returns>
		public TableReference GetTableReference (string tableName) {
			var table = GetLocalizedStringTable(tableName);
			return table == null ? default : table.TableReference;
		}
		#endregion Public Helper Methods
	}
}