using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BaseSDK.Utils;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		/// <summary>
		/// Find the number of occurrences of a certain substring within the given string.
		/// </summary>
		/// <param name="str">Given string to search in.</param>
		/// <param name="occurrenceString">Is this occurrenceString present as a substring in the given string.</param>
		/// <returns>Number of occurrences of the occurrenceString in the given string.</returns>
		public static int Occurrences(this string str, string occurrenceString) => (int)str?.Split(new string[] { occurrenceString }, StringSplitOptions.None)?.Length - 1;

		/// <summary>
		/// Find the number of occurrences of a certain character within the given string.
		/// </summary>
		/// <param name="str">Given string to search in.</param>
		/// <param name="occurrenceChar">Is this occurrenceChar present as a substring in the given string.</param>
		/// <returns>Number of occurrences of the occurrenceChar in the given string.</returns>
		public static int Occurrences(this string str, char occurrenceChar) => (int)str?.Split(occurrenceChar).Length - 1;

		/// <summary>
		/// Shorthand way of doing string replace to remove a substring. Returns the modified string to chain together.
		/// </summary>
		/// <param name="str">String to remove text from.</param>
		/// <param name="stringToRemove">Substring to remove from the given string.</param>
		/// <returns>Returns the modified string to chain together.</returns>
		public static string Remove(this string str, string stringToRemove) => str.Replace(stringToRemove, string.Empty);

		/// <summary>
		/// Remove all substrings from a given string.
		/// </summary>
		/// <param name="str">String to remove text from.</param>
		/// <param name="stringsToRemove">Substrings to remove from the given string.</param>
		/// <returns>Returns the modified string to chain together.</returns>
		public static string RemoveAll(this string str, params string[] stringsToRemove) {
			for(var i = 0; i < stringsToRemove.Length; i++)
				str = str.Remove(stringsToRemove[i]);
			return str;
		}

		/// <summary>
		/// Does the string end with either of the substrings? Useful for checking if a FileName is ending with a certain File Extension, etc.
		/// </summary>
		/// <param name="str">String to look at the end of.</param>
		/// <param name="endStrings">Substrings to check if the given string ends with either one of them.</param>
		/// <returns>Returns of the given string ends with either one of the substrings.</returns>
		public static bool EndsWithEither(this string str, params string[] endStrings) {
			for (int i = 0; i < endStrings.Length; i++)
				if (str.EndsWith(endStrings[i]))
					return true;
			return false;
		}

		/// <summary>
		/// Joins a list of strings, using the separator.
		/// </summary>
		/// <typeparam name="T">Type of separator</typeparam>
		/// <param name="list">List of strings to join.</param>
		/// <param name="separator">Separator char/string to join the strings.</param>
		/// <returns>Returns a string with the concatenated string using the separator.</returns>
		public static string Join<T>(this IEnumerable<string> list, T separator) {
			var sb = new StringBuilder();
			foreach (var iterator in list)
				sb.Append(iterator).Append(GameConstants.SPACE).Append(separator);
			return sb.ToString();
		}

		/// <summary>
		/// Shorthand way of checking if string is Null or Empty.
		/// </summary>
		/// <param name="str">String to check if it is Null or Empty.</param>
		/// <returns>Is the string is Null or Empty.</returns>
		public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

		/// <summary>
		/// Converts the number into a valid currency based on game's current Culture Settings
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cultureName">Culture names cant be found on http://azuliadesigns.com/list-net-culture-country-codes/ </param>
		/// <returns></returns>
		public static string ToCurrency<T>(this T num, string currentCultureName) where T : struct, IConvertible => string.Format(new CultureInfo(currentCultureName), GameConstants.CURRENCY_FORMAT, num);

		/// <summary>
		/// Shorthand way to do string.Format.
		/// </summary>
		/// <param name="str">Format string.</param>
		/// <param name="args">Parameter list.</param>
		/// <returns>Returns the formatted string.</returns>
		public static string Format(this string str, params object[] args) => string.Format(str, args);

		/// <summary>
		/// Returns whether the string contains all the given substrings.
		/// </summary>
		/// <param name="str">String to check in.</param>
		/// <param name="strings">Substrings to check in the given string for occurrence.</param>
		/// <returns>Returns true if all the substrings are present in the given string.</returns>
		public static bool ContainsAll(this string str, params string[] strings) {
			foreach (var element in strings)
				if (!str.Contains(element))
					return false;
			return true;
		}

		/// <summary>
		/// Returns whether the string contains atleast one of the strings in the array, as a substring
		/// </summary>
		/// <param name="str">String to check in.</param>
		/// <param name="strings">Strings to check in the given string for occurrence.</param>
		/// <returns>Returns true if at least 1 of the substrings is present in the given string.</returns>
		public static bool ContainsAtLeastOne(this string str, params string[] strings) {
			foreach (var element in strings)
				if (str.Contains(element))
					return true;
			return false;
		}
	}
}