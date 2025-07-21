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