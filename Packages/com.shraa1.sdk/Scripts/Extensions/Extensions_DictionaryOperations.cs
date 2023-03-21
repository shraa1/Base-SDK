//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BaseSDK.Utils;
using UnityEngine;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		/// <summary>
		/// Returns a string version of the dictionary's key value pairs
		/// </summary>
		/// <typeparam name="T">Type 1</typeparam>
		/// <typeparam name="U">Type 2</typeparam>
		/// <param name="dict">Dictionary to print</param>
		/// <returns>Returns a string version of the dictionary's key value pairs</returns>
		public static string Print<T, U>(this Dictionary<T, U> dict, bool useNewtonsoft = true) {
			if (useNewtonsoft)
				return JsonConvert.SerializeObject(dict, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

			var sb = new StringBuilder();
			foreach (var iterator in dict)
				sb.Append(iterator.ToString()).Append(GameConstants.COMMA).Append(GameConstants.SPACE);

			var str = sb.ToString();
			return str.Length > 0 ? str.Substring(0, str.Length - 2) : str;
		}

		/// <summary>
		/// Inline foreach call for a dictionary
		/// </summary>
		/// <typeparam name="T">Type 1</typeparam>
		/// <typeparam name="U">Type 2</typeparam>
		/// <param name="dict">Dictionary to operate on</param>
		/// <param name="action">Action to perform on each iterator</param>
		public static void ForEach<T, U>(this Dictionary<T, U> dict, Action<T, U> action) {
			foreach (var iterator in dict)
				action(iterator.Key, iterator.Value);
		}

		/// <summary>
		/// Find an element in the dictionary
		/// </summary>
		/// <typeparam name="T">Type 1</typeparam>
		/// <typeparam name="U">Type 2</typeparam>
		/// <param name="dict">Dictionary to find the element in</param>
		/// <param name="predicate">Predicate to find the matching element</param>
		/// <returns>Returns a KeyValuePair with the matching predicate</returns>
		public static KeyValuePair<T, U> Find<T, U> (this Dictionary<T, U> dict, Predicate<KeyValuePair<T, U>> predicate) {
			foreach(var iterator in dict)
				if(predicate(iterator))
					return iterator;
			return default;
		}

		/// <summary>
		/// Remove an element from a dictionary if the key exists
		/// </summary>
		/// <typeparam name="T">Type 1</typeparam>
		/// <typeparam name="U">Type 2</typeparam>
		/// <param name="dict">Dictionary to modify</param>
		/// <param name="key">Remove element with this key from the dictionary if available</param>
		/// <returns>Returns the modified dictionary for chain actions</returns>
		public static Dictionary<T, U> RemoveSafely<T, U>(this Dictionary<T, U> dict, T key) {
			if (dict.ContainsKey(key))
				dict.Remove(key);
			return dict;
		}

		/// <summary>
		/// If an element with a key does not exist in the dictionary, then add it, else update the value of the key.
		/// </summary>
		/// <typeparam name="T">Type 1</typeparam>
		/// <typeparam name="U">Type 2</typeparam>
		/// <param name="dict">Dictionary to modify</param>
		/// <param name="key">Add or update existing value for this key in the dictionary</param>
		/// <param name="value">Add or update the value in the dictionary for the defined key</param>
		/// <returns>Returns the modified dictionary for chain actions</returns>
		public static Dictionary<T, U> AddOrUpdate<T, U>(this Dictionary<T, U> dict, T key, U value) {
			if (dict.ContainsKey(key))
				dict[key] = value;
			else
				dict.Add(key, value);
			return dict;
		}
	}
}