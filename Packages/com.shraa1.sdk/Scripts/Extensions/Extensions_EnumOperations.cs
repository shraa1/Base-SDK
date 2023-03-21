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

namespace BaseSDK.Extension {
	public static partial class Extensions {

		public static IEnumerable<Enum> GetFlags(this Enum value) => GetFlags(value, Enum.GetValues(value.GetType()).Cast<Enum>().ToArray());

		public static IEnumerable<Enum> GetIndividualFlags(this Enum value) => GetFlags(value, GetFlagValues(value.GetType()).ToArray());

		private static IEnumerable<Enum> GetFlags(Enum value, Enum[] values) {
			var bits = Convert.ToUInt64(value);
			var results = new List<Enum>();
			for (int i = values.Length - 1; i >= 0; i--) {
				var mask = Convert.ToUInt64(values[i]);
				if (i == 0 && mask == 0L)
					break;
				if ((bits & mask) == mask) {
					results.Add(values[i]);
					bits -= mask;
				}
			}
			if (bits != 0L)
				return Enumerable.Empty<Enum>();
			if (Convert.ToUInt64(value) != 0L)
				return results.Reverse<Enum>();
			if (bits == Convert.ToUInt64(value) && values.Length > 0 && Convert.ToUInt64(values[0]) == 0L)
				return values.Take(1);
			return Enumerable.Empty<Enum>();
		}

		private static List<Enum> GetFlagValues(Type enumType) {
			List<Enum> flags = new List<Enum>();
			var ee = Enum.GetValues(enumType).Cast<Enum>().ToList();

			ee.ForEach(x => {
				if (IsPowerOfTwo(Convert.ToUInt64(x)))
					flags.Add(x);
			});
			return flags;
		}

		public static T EnumParse<T>(this string str) where T : struct {
			try { return (T)Enum.Parse(typeof(T), str); }
			catch (Exception ex) { throw ex; }
		}
	}
}