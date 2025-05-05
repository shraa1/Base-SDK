using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BaseSDK.Extension {
	public static class EncryptionConfig {
		public static Func<string> GetAESEncryptionKey;
		public static Func<string> GetAESEncryptionIV;
		public static Func<int> GetAESEncryptionBlockSize;
		public static Func<int> GetAESEncryptionKeySize;
		public static Func<string> GetRSAEncryptionKey;
	}

	public static partial class Extensions {
		/// <summary>
		/// Encrypt any string using either AES or RSA encryption. Encryption size is 256
		/// </summary>
		/// <param name="stringToEncrypt">String to encrypt</param>
		/// <param name="encryptionType">Type of encryption</param>
		/// <returns></returns>
		public static string Encrypt (this string stringToEncrypt, EncryptionType encryptionType = EncryptionType.AES) {
			if (encryptionType == EncryptionType.AES) {
				var encrypt = AESmanaged.CreateEncryptor();

				var bytes = Encoding.UTF8.GetBytes(stringToEncrypt);
				using var ms = new MemoryStream();
				using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write)) {
					cs.Write(bytes, 0, bytes.Length);
					cs.FlushFinalBlock();
				}
				var encryptedBytes = ms.ToArray();
				return Convert.ToBase64String(encryptedBytes);
			}
			else if (encryptionType == EncryptionType.RSA) {
				if (stringToEncrypt.IsNullOrEmpty())
					throw new ArgumentException($"Cannot Encrypt {(stringToEncrypt == null ? "null" : "empty")} string");

				var cspp = new CspParameters { KeyContainerName = EncryptionConfig.GetRSAEncryptionKey() };
				var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true };

				var bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(stringToEncrypt), true);
				return BitConverter.ToString(bytes);
			}
			return stringToEncrypt;
		}

		/// <summary>
		/// Decrypt a string to a specific type using either RSA or AES. Encryption size is 256.
		/// </summary>
		/// <typeparam name="T">Convert to this type before returning the value</typeparam>
		/// <param name="stringToDecrypt">The encrypted string meant to be decrypted</param>
		/// <param name="encryptionType">Type of encryption</param>
		/// <returns></returns>
		public static T Decrypt<T> (this string stringToDecrypt, EncryptionType encryptionType = EncryptionType.AES) where T : IConvertible {
			if (encryptionType == EncryptionType.AES) {
				if (string.IsNullOrEmpty(stringToDecrypt))
					throw new ArgumentException("Attempted to decrypt an empty string.");
				if (!IsBase64String(stringToDecrypt))
					throw new FormatException("Encrypted string is not valid base64.");

				var decrypt = AESmanaged.CreateDecryptor();

				var bytes = Convert.FromBase64String(stringToDecrypt);
				using var ms = new MemoryStream();
				using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write)) {
					cs.Write(bytes, 0, bytes.Length);
					cs.FlushFinalBlock();
				}

				var decryptedBytes = ms.ToArray();
				return Encoding.UTF8.GetString(decryptedBytes).To<T>();
			}
			else if (encryptionType == EncryptionType.RSA) {
				if (stringToDecrypt.IsNullOrEmpty())
					throw new ArgumentException($"Cannot Decrypt {(stringToDecrypt == null ? "null" : "empty")} string");
				var result = string.Empty;
				try {
					var cspp = new CspParameters { KeyContainerName = EncryptionConfig.GetRSAEncryptionKey() };
					var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true };
					var splits = stringToDecrypt.Split('-');
					var bytes = Array.ConvertAll(splits, x => Convert.ToByte(byte.Parse(x, NumberStyles.HexNumber)));
					result = Encoding.UTF8.GetString(rsa.Decrypt(bytes, true));
				}
				catch { /* Not doing anything */ }
				return result.To<T>();
			}
			return stringToDecrypt.To<T>();
		}

		public static bool IsBase64String (string s) {
			var buffer = new Span<byte>(new byte[s.Length]);
			return Convert.TryFromBase64String(s, buffer, out _);
		}

		private static RijndaelManaged _AESmanaged;
		private static RijndaelManaged AESmanaged =>
			//simplify for c# 8.0 and further, with unity upgrade
			_AESmanaged ??= new RijndaelManaged {
				KeySize = EncryptionConfig.GetAESEncryptionKeySize(),
				BlockSize = EncryptionConfig.GetAESEncryptionBlockSize(),
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7,
				Key = Convert.FromBase64String(EncryptionConfig.GetAESEncryptionKey()),
				IV = Convert.FromBase64String(EncryptionConfig.GetAESEncryptionIV()),
			};
	}

	public enum EncryptionType { AES, RSA }
}