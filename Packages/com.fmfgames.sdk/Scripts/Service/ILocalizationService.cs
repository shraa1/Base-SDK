using System.Collections.Generic;
using System.Globalization;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace BaseSDK.Services {
	public interface ILocalizationService : IService {
		List<LocalizedStringTable> LocalizationTables { get; }
		List<CultureInfo> SupportedLanguages { get; }

		void SetLanguage (int index);
		StringTable GetStringTable(string tableName);
		TableReference GetTableReference(string tableName);
	}
}