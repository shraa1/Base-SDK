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
using System.Linq;
using System.Text;
using BaseSDK.Utils;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		/// <summary>
		/// Get a random item from the list.
		/// </summary>
		/// <typeparam name="T">Type of the list.</typeparam>
		/// <param name="ienumerable">IEnumerable of items to check in.</param>
		/// <returns>Returns an item at a random index in the list.</returns>
		public static T Random<T>(this IEnumerable<T> ienumerable) => ienumerable.ElementAt(UnityEngine.Random.Range(0, ienumerable.Count()));

		/// <summary>
		/// Inline way to call foreach on any list/array/ienum
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="action"></param>
		public static void ForEach<T>(this IEnumerable<T> array, Action<T> action) {
			foreach (var arrayObj in array)
				action(arrayObj);
		}

		/// <summary>
		/// Useful for printing out lists when debugging.
		/// </summary>
		/// <typeparam name="T">Preferrably a type that can be easily stringified. Some non-Serializable classes might not work
		/// according to the requirement, so try using Newtonsoft to serialize instead</typeparam>
		/// <param name="enumerable">Operate on this list</param>
		/// <param name="useNewtonsoftSerializer">If you use Newtonsoft in your project, you should add USING_NEWTONSOFT in the scripting defines. Useful for serializing some complex classes which may be marked as sealed or internal</param>
		/// <returns></returns>
		public static string Print<T>(this IEnumerable<T> enumerable, bool useNewtonsoft = true) {
			if (useNewtonsoft)
				return JsonConvert.SerializeObject(enumerable, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

			var sb = new StringBuilder();
			foreach (var iterator in enumerable)
				sb.Append(iterator.ToString()).Append(GameConstants.COMMA).Append(GameConstants.SPACE);

			var str = sb.ToString();
			return str.Length > 0 ? str.Substring(0, str.Length - 2) : str;
		}

		/// <summary>
		/// Inline way to call for loop on any list/array/ienum
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="action"></param>
		public static void For<T>(this IEnumerable<T> enumerable, Action<int> action) {
			for (var i = 0; i < enumerable.Count(); i++)
				action?.Invoke(i);
		}

		/// <summary>
		/// Remove one or more item from the collection if it is present in the list, else ignore.
		/// Warning: If called on an array, a new copy with the removed items will be made.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection to modify</param>
		/// <param name="args">Items to be removed from the collection</param>
		/// <returns>Returns the list with removed items so that operations can be chained to make inline operations</returns>
		public static ICollection<T> RemoveSafely<T>(this ICollection<T> collection, params T[] args) {
			if (!collection.GetType().IsArray) {
				foreach (var iterator in args)
					if (collection.Contains(iterator))
						collection.Remove(iterator);
			}
			//Create a copy for an array
			else {
				var list = collection.ToList();
				foreach (var iterator in args)
					if (list.Contains(iterator))
						list.Remove(iterator);
				collection = list.ToArray();
			}
			return collection;
		}

		/// <summary>
		/// Remove one item from the collection if it is present in the list, else ignore.
		/// Warning: If called on an array, a new copy with the removed items will be made.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="collection">The collection to modify</param>
		/// <param name="item">Item to be removed from the collection</param>
		/// <returns>Returns the list with removed items so that operations can be chained to make inline operations</returns>
		public static ICollection<T> RemoveSafely<T>(this ICollection<T> collection, T item) {
			if (!collection.GetType().IsArray) {
				if (collection.Contains(item))
					collection.Remove(item);
			}
			//Create a copy for an array
			else {
				var list = collection.ToList();
				list.Remove(item);
				collection = list.ToArray();
			}
			return collection;
		}

		/// <summary>
		/// Is the specific item a part of a collection?
		/// </summary>
		/// <typeparam name="T">Type of the item.</typeparam>
		/// <param name="source">The item being looked for, inside the collection.</param>
		/// <param name="args">The collection of items which may contain the given item.</param>
		/// <returns>Returns true if the source is found in the collection, else returns false.</returns>
		public static bool In<T>(this T source, params T[] args) => (bool)args?.Contains(source);

		/// <summary>
		/// Merges/Collapses a 2D collection into a single list. Can be filtered so that duplicates are not added.
		/// </summary>
		/// <typeparam name="T">Type of the 2D collection.</typeparam>
		/// <param name="lists">2D collection to merge.</param>
		/// <param name="onlyDistinct">Allow duplicate entries? If yes, then pass false.</param>
		/// <returns></returns>
		public static IEnumerable<T> Merge<T>(this IEnumerable<IEnumerable<T>> lists, bool onlyDistinct = false) {
			var list = new List<T>();
			lists.ForEach(innerList => list.AddRange(innerList));
			if (onlyDistinct)
				list = list.Distinct().ToList();
			return list;
		}

		/// <summary>
		/// Add an item to the list only if it does not already have it.
		/// </summary>
		/// <typeparam name="T">Type of list.</typeparam>
		/// <param name="list">List to add item into.</param>
		/// <param name="item">Item to add to the list.</param>
		/// <returns>Returns the modified list, for chain operations.</returns>
		public static IEnumerable<T> AddSafely<T>(this ICollection<T> list, T item) {
			if (!list.Contains(item))
				list.Add(item);
			return list;
		}
	}
}