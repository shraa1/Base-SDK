using System;
using System.Collections.Generic;
using System.Linq;
using BaseSDK.Utils;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		/// <summary>
		/// Shuffles the list items.
		/// </summary>
		/// <typeparam name="T">Type of the list.</typeparam>
		/// <param name="listToShuffle">List that is being shuffled.</param>
		/// <returns>Returns the shuffled list for chain operations.</returns>
		public static List<T> Shuffle<T>(this List<T> listToShuffle) {
			for (int i = 0; i < listToShuffle.Count; i++) {
				var k = UnityEngine.Random.Range(0, listToShuffle.Count);
				var value = listToShuffle[k];
				listToShuffle[k] = listToShuffle[i];
				listToShuffle[i] = value;
			}
			return listToShuffle;
		}

		/// <summary>
		/// Initialize a list with a certain default value and list size.
		/// </summary>
		/// <typeparam name="T">Type of the list.</typeparam>
		/// <param name="list">List to initialize.</param>
		/// <param name="size">Initialization size of the list.</param>
		/// <param name="defaultVal">Default value when initializing the list.</param>
		/// <returns>Returns the initialized list with the default values for chain operations.</returns>
		public static List<T> Init<T>(this List<T> list, int size, T defaultVal = default) {
			for (var i = 0; i < size; i++)
				list.Add(defaultVal);
			return list;
		}

		/// <summary>
		/// Get a random item from the list and return it
		/// </summary>
		/// <typeparam name="T">Type of item in the list</typeparam>
		/// <param name="list">List to get the random item from</param>
		/// <returns>Returns a randomed item from the list</returns>
		public static T Random<T> (this List<T> list) => list[UnityEngine.Random.Range(0, list.Count)];
	}
}