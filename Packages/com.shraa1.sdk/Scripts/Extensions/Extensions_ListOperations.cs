//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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