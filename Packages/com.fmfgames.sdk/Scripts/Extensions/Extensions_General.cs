using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace BaseSDK.Extension {
	public static partial class Extensions {
		#region Private consts
		private const string ssFormat = @"ss";
		private const string mmssFormat = @"mm\:ss";
		private const string hhmmssFormat = @"hh\:mm\:ss";
		#endregion

		#region WaitForEndOfFrame
		/// <summary>
		/// Shorthand way to call WaitForEndOfFrame inline.
		/// </summary>
		/// <param name="monoBehaviour">Script to run the coroutine on.</param>
		/// <param name="action">Action to perform after the end of frame.</param>
		public static void WaitForEndOfFrame (this MonoBehaviour monoBehaviour, Action action) => monoBehaviour.StartCoroutine(WaitForEOF(action));

		/// <summary>
		/// Helper IEnumerator for waiting the appropriate time and execute the required code
		/// </summary>
		/// <param name="action"></param>
		/// <returns></returns>
		private static IEnumerator WaitForEOF (Action action) {
			yield return GameConstants.EOF;
			action?.Invoke();
		}
		#endregion

		/// <summary>
		/// Converts seconds to a string worth displaying in the UI properly formatted. Supports till hh:mm:ss and not days, months, etc.
		/// </summary>
		/// <param name="secs">Time in secs to format</param>
		/// <param name="showHours">If showHours is true, show time as hh:mm:ss, elsee mm:ss or ss depending on showMinutes value. If showHours is true, it takes precedence over showMinutes</param>
		/// <param name="showMinutes">If true, shows time as mm:ss</param>
		public static string ConvertSecondsToDisplayString (this long secs, bool showHours = false, bool showMinutes = true) => TimeSpan.FromSeconds(secs).ToString(showHours ? hhmmssFormat : showMinutes ? mmssFormat : ssFormat);

		/// <summary>
		/// Converts seconds to a string worth displaying in the UI properly formatted. Supports till hh:mm:ss and not days, months, etc.
		/// </summary>
		/// <param name="secs">Time in secs to format</param>
		/// <param name="showHours">If showHours is true, show time as hh:mm:ss, elsee mm:ss or ss depending on showMinutes value. If showHours is true, it takes precedence over showMinutes</param>
		/// <param name="showMinutes">If true, shows time as mm:ss</param>
		public static string ConvertSecondsToDisplayString (this double secs, bool showHours = false, bool showMinutes = true) => TimeSpan.FromSeconds(secs).ToString(showHours ? hhmmssFormat : showMinutes ? mmssFormat : ssFormat);

		/// <summary>
		/// Converts seconds to a string worth displaying in the UI properly formatted. Supports till hh:mm:ss and not days, months, etc.
		/// </summary>
		/// <param name="secs">Time in secs to format</param>
		/// <param name="showHours">If showHours is true, show time as hh:mm:ss, elsee mm:ss or ss depending on showMinutes value. If showHours is true, it takes precedence over showMinutes</param>
		/// <param name="showMinutes">If true, shows time as mm:ss</param>
		public static string ConvertSecondsToDisplayString (this float secs, bool showHours = false, bool showMinutes = true) => TimeSpan.FromSeconds(secs).ToString(showHours ? hhmmssFormat : showMinutes ? mmssFormat : ssFormat);

		/// <summary>
		/// Convert To Unix Time
		/// </summary>
		/// <param name="dateTime">Time to convert to unix time</param>
		/// <returns>Returns unix time</returns>
		public static long ConvertToUnixTime (this DateTime dateTime) => (long)(dateTime.ToUniversalTime() - GameConstants.k_EPOCH_DATETIME_UTC).TotalSeconds;

		/// <summary>
		/// Convert a color to its hexadecimal code.
		/// </summary>
		/// <param name="c">Colour to get hex value of.</param>
		/// <returns>Hexadecimal code of the color.</returns>
		public static string ToHex (this Color c) => string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));

		/// <summary>
		/// Wrap text in colour tags
		/// </summary>
		/// <param name="str">String to colour.</param>
		/// <param name="color">Color the text to this colour</param>
		/// <returns>Returns coloured string text</returns>
		public static string ToColor (this string str, Color color) => $"<color={color}>{str}</color>";

		/// <summary>
		/// Get the byte value of a float.
		/// </summary>
		/// <param name="c">Float to get byte value of.</param>
		/// <returns>Byte Value.</returns>
		private static byte ToByte (this float f) => (byte)(Mathf.Clamp01(f) * 255);

		/// <summary>
		/// Shorthand to convert an IConvertible to another IConvertible.
		/// </summary>
		/// <typeparam name="T">Type to convert to.</typeparam>
		/// <param name="source">Item to convert the type of.</param>
		/// <returns>Returns a converted item with the defined type.</returns>
		public static T To<T> (this IConvertible source) where T : IConvertible => (T)Convert.ChangeType(source, typeof(T));

		/// <summary>
		/// Is the given item Even or Odd?
		/// </summary>
		/// <typeparam name="T">Type of item.</typeparam>
		/// <param name="num">The item to be checked if it is even or not.</param>
		/// <returns>Returns true if the item is Even.</returns>
		public static bool IsEven<T> (this T num) where T : struct, IConvertible => num.To<long>() % 2 == 0;

		/// <summary>
		/// Is the item a palindrome?
		/// </summary>
		/// <typeparam name="T">Type of item.</typeparam>
		/// <param name="source">Item to check if it is a palindrome.</param>
		/// <returns>Returns if the item is a palindrome or not.</returns>
		public static bool IsPalindrome<T> (this T source) where T : IComparable, IConvertible, IComparable<T>, IEquatable<T> => source.ToString() == source.ToString().Reverse();

		/// <summary>
		/// Reverse an item.
		/// </summary>
		/// <typeparam name="T">Type of item to reverse.</typeparam>
		/// <param name="source">Item to reverse.</param>
		/// <returns>Returns the reversed item.</returns>
		public static T Reverse<T> (this T source) where T : IComparable, IConvertible, IComparable<T>, IEquatable<T> {
			var chrArr = source.ToString().ToCharArray();
			Array.Reverse(chrArr);
			return new string(chrArr).To<T>();
		}

		/// <summary>
		/// Is the given item inside the given range.
		/// </summary>
		/// <typeparam name="T">Type of item to check.</typeparam>
		/// <param name="actual">Value of the item being checked.</param>
		/// <param name="minValue">Low-end value of the range to check in.</param>
		/// <param name="maxValue">High-end value of the range to check in.</param>
		/// <returns>Returns if the given item is inside the given range.</returns>
		public static bool Between<T> (this T actual, T minValue, T maxValue) where T : IComparable<T> => actual.CompareTo(minValue) >= 0 && actual.CompareTo(maxValue) < 0;

		/// <summary>
		/// Shorthand way to convert a sprite to a byte array
		/// </summary>
		/// <param name="spr"></param>
		/// <returns></returns>
		public static IEnumerable<byte> ToBytes (this Sprite spr) => spr.texture.EncodeToPNG();

		/// <summary>
		/// Shorthand way to convert Vector3 to Vector2
		/// </summary>
		public static Vector2 Vector2 (this Vector3 vector3) => vector3;

		/// <summary>
		/// Shorthand way to convert Vector2 to Vector3
		/// </summary>
		public static Vector3 Vector3 (this Vector2 vector2) => vector2;

		/// <summary>
		/// Rounds up the float. Removes garbage small values like 1.500001 or 1.49999 to 1.5
		/// </summary>
		/// <param name="f1"></param>
		/// <param name="precision">How many characters to get after the decimal</param>
		/// <returns>Returns the rounded up float value.</returns>
		public static float Round (this float f1, int precision = 1) => float.Parse(f1.ToString($"n{precision}"));

		/// <summary>
		/// Shorthand for calculating MegaBytes from Bytes. 1024 Bytes = 1 MB
		/// </summary>
		public static float BytesToMB (this long bytes) => bytes * GameConstants.BYTES_TO_MEGABYTES_MULTIPLIER;

		/// <summary>
		/// Checks if the number is a power of 2
		/// </summary>
		/// <param name="x">Number to check</param>
		/// <returns>Returns true if the number is a square number</returns>
		public static bool IsPowerOfTwo (ulong x) => (x != 0) && ((x & (x - 1)) == 0);

		/// <summary>
		/// Deserialize a string into an object of type <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">Type to deserialize the string to</typeparam>
		/// <param name="str">The string being deserialized</param>
		/// <returns>Returns the deserialized object</returns>
		public static T Deserialize<T>(this string str, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<T>(str, settings);

		/// <summary>
		/// Serialize an object
		/// </summary>
		/// <param name="obj">Object being serialized to a string</param>
		/// <returns>Returns the serialized version of the object <paramref name="obj"/></returns>
		public static string Serialize(this object obj, JsonSerializerSettings settings = null) => JsonConvert.SerializeObject(obj, settings);
	}
}