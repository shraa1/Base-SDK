//Copyright © 2023 Shraavan (shraa1)
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”),
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense of the Software,
//and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#pragma warning disable IDE0090 // Use 'new(...)'
using System;
using BaseSDK.Extension;

/// <summary>
/// This is an extended int. This does not store the int value in memory directly, instead, it encrypts the int, and then decrypts it when reading.
/// This is useful for protection against cheats like CheatEngine, etc. which will look up the memories of values in any process and modify them.
/// For example, in a resource collection game, to complete the level faster, if your current resource is 10, you can pause the game, modify the value in
/// memory, and then resume the game to be closer to the finish.
/// 
/// While this approach is useful for data protection, it is still Encrypting/Decrypting data, which can be very bad for performance. So do not use
/// it if you are modifying a value constantly.
/// 
/// Not yet sure how to make the type visible in the inspector, will look into it ;)
/// Also need to test all the Interface's implementations for conversion
/// Also need to add summaries
/// </summary>
public struct intX : IComparable, IComparable<int>, IConvertible, IEquatable<int>, IFormattable {

	private string encodedValue;

	public intX(int val) => encodedValue = val.ToString().Encrypt();

	public static implicit operator int(intX intXParam) => intXParam.encodedValue.IsNullOrEmpty() ? 0 : intXParam.encodedValue.Decrypt<int>();
	public static implicit operator intX(int intXVal) => new intX(intXVal);

	#region Interface Implementations
	public int CompareTo(object obj) => encodedValue.CompareTo(obj);

	public int CompareTo(int other) => encodedValue.CompareTo(other);

	public TypeCode GetTypeCode() => encodedValue.GetTypeCode();

	public bool ToBoolean(IFormatProvider provider) => Convert.ToBoolean(encodedValue, provider);

	public byte ToByte(IFormatProvider provider) => Convert.ToByte(encodedValue, provider);

	public char ToChar(IFormatProvider provider) => Convert.ToChar(encodedValue, provider);

	public DateTime ToDateTime(IFormatProvider provider) => Convert.ToDateTime(encodedValue, provider);

	public decimal ToDecimal(IFormatProvider provider) => Convert.ToDecimal(encodedValue, provider);

	public double ToDouble(IFormatProvider provider) => Convert.ToDouble(encodedValue, provider);

	public short ToInt16(IFormatProvider provider) => Convert.ToInt16(encodedValue, provider);

	public int ToInt32(IFormatProvider provider) => Convert.ToInt32(encodedValue, provider);

	public long ToInt64(IFormatProvider provider) => Convert.ToInt64(encodedValue, provider);

	public sbyte ToSByte(IFormatProvider provider) => Convert.ToSByte(encodedValue, provider);

	public float ToSingle(IFormatProvider provider) => Convert.ToSingle(encodedValue, provider);

	public string ToString(IFormatProvider provider) => Convert.ToString(encodedValue, provider);

	public object ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(encodedValue, conversionType, provider);

	public ushort ToUInt16(IFormatProvider provider) => Convert.ToUInt16(encodedValue, provider);

	public uint ToUInt32(IFormatProvider provider) => Convert.ToUInt32(encodedValue, provider);

	public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(encodedValue, provider);

	public bool Equals(int other) => encodedValue.Equals(other);

	public string ToString(string format, IFormatProvider formatProvider) => encodedValue.IsNullOrEmpty() ? 0.ToString(format, formatProvider) : encodedValue.Decrypt<int>().ToString(format, formatProvider);
	#endregion
}
#pragma warning restore IDE0090 // Use 'new(...)'