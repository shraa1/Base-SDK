using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BaseSDK.Extension;
using UnityEngine;

namespace BaseSDK.CSV {
	public class CSVParser : MonoBehaviour {
		#region Inspector Variables
		/// <summary>
		/// If we have a csv file, set it in the inspector. If it is dynamically loaded, it can be set using the property
		/// </summary>
		[SerializeField] private TextAsset csvAsset;
		#endregion Inspector Variables

		#region Properties
		/// <summary>
		/// CSV asset to parse if CSV file parameter is not passed in the GetDictWithKVPs method
		/// </summary>
		public TextAsset CSVAsset { get => csvAsset; set => csvAsset = value; }
		#endregion Properties

		#region Consts/Static Readonlys
		/// <summary>
		/// Column serparator characters
		/// </summary>
		private static readonly char[] fieldSeparator = { ',' };
		#endregion Consts/Static Readonlys

		/// <summary>
		/// Return a dictionary with KVPs
		/// </summary>
		/// <param name="columnHeader">Column header like "en", etc.</param>
		/// <param name="csvFile">Csv file to parse. If null is passed, the class instance's csvAsset will be used</param>
		/// <param name="throwExceptions">Pretty self explanatory</param>
		/// <returns>Return a dictionary with all keys and the values of the column header</returns>
		public Dictionary<string, string> GetDictWithKVPs(string columnHeader, TextAsset csvFile = null, bool throwExceptions = false) {
			var dict = new Dictionary<string, string>();

			if (CSVAsset == null && csvFile == null) {
				if (throwExceptions)
					Debug.Log("CSV File is null");
				return dict;
			}

			var csv = CSVAsset;
			if (csvFile != null) {
				csv = csvFile;
				if (throwExceptions && csvFile.text.IsNullOrEmpty())
					throw new Exception("CSV Parsing Error");
			}

			var lines = csv.text.Split(GameConstants.LINE_BREAK, GameConstants.LINE_BREAK2);
			if (lines.Length == 0) {
				if (throwExceptions)
					Debug.LogError("0 lines in the localization csv file");
				return dict;
			}

			var ind = -1;
			var headers = lines[0].Split(fieldSeparator, StringSplitOptions.None);

			for (var i = 1; i < headers.Length; i++) {
				Debug.LogError(headers[i]);
				if (headers[i] == columnHeader) {
					ind = i;
					break;
				}
			}

			if (ind == -1) {
				Debug.LogError($"Locale {columnHeader} was not found in the CSV");
				return dict;
			}

			Regex CSVParserRegex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

			for (var i = 1; i < lines.Length; i++) {
				var fields = CSVParserRegex.Split(lines[i]);

				for (var j = 0; j < fields.Length; j++) {
					fields[j] = fields[j].TrimStart(GameConstants.SPACE, GameConstants.DOUBLEQUOTE);
					fields[j] = fields[j].TrimEnd(GameConstants.DOUBLEQUOTE);
				}

				if (fields.Length > ind) {
					var key = fields[0];
					if (dict.ContainsKey(key))
						continue;
					dict.Add(key, fields[ind]);
				}
			}

			return dict;
		}
	}
}