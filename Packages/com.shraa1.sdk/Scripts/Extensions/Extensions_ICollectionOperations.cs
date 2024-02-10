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