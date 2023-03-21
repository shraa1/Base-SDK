//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		/// <summary>
		/// Add multiple items to a collection together.
		/// </summary>
		/// <typeparam name="T">Type of item to add to the collection.</typeparam>
		/// <param name="collection">Collection to add items to.</param>
		/// <param name="values">Items to add to the collection.</param>
		/// <returns>Returns the modified collection for chain operations.</returns>
		public static IEnumerable<T> AddRange<T>(this ICollection<T> collection, params T[] values) {
			values.ForEach(x => collection.Add(x));
			return collection;
		}

		public static IEnumerable<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> value) {
			value.ForEach(x => collection.Add(x));
			return collection;
		}

		public static IEnumerable<T> AddRanges<T>(this ICollection<T> collection, params ICollection<T>[] ranges) {
			ranges.ForEach(x => collection.AddRange(x));
			return collection;
		}

		public static void ForEach<T>(this ICollection<T> collection, Action<T> action) {
			foreach (var arrayObj in collection)
				action(arrayObj);
		}

		/// <summary>
		/// Shorthand for calling foreach on a JArray inline.
		/// </summary>
		/// <param name="arr">JArray being iterated on.</param>
		/// <param name="action">Action to perform on the item in the JArray.</param>
		public static void ForEach(this JArray arr, Action<JToken> action) {
			foreach (var iterator in arr)
				action(iterator);
		}

		/// <summary>
		/// Add items to the list only if they do not already have it.
		/// </summary>
		/// <typeparam name="T">Type of the list.</typeparam>
		/// <param name="list">List to add items into.</param>
		/// <param name="items">Collection of items to add to the list.</param>
		/// <returns>Returns the modified list, for chain operations.</returns>
		public static IEnumerable<T> AddSafely<T>(this ICollection<T> list, IEnumerable<T> items) {
			items.ForEach(x => {
				if (!list.Contains(x))
					list.Add(x);
			});
			return list;
		}
	}
}