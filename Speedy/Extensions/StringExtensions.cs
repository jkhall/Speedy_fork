﻿#region References

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

#endregion

namespace Speedy.Extensions
{
	/// <summary>
	/// Extensions for the string type.
	/// </summary>
	public static class StringExtensions
	{
		#region Methods

		/// <summary>
		/// Convert string to a base 64 string.
		/// </summary>
		/// <param name="data"> The data to be converted. </param>
		/// <returns> The base 64 encoded string. </returns>
		public static string ToBase64(this string data)
		{
			return data == null ? string.Empty : Encoding.UTF8.GetBytes(data).ToBase64();
		}

		/// <summary>
		/// Convert string from a base 64 string.
		/// </summary>
		/// <param name="data"> The data to be converted. </param>
		/// <returns> The unencoded string. </returns>
		public static string FromBase64(this string data)
		{
			var bytes = Convert.FromBase64String(data);
			return Encoding.UTF8.GetString(bytes);
		}

		/// <summary>
		/// Convert string from a base 64 string.
		/// </summary>
		/// <param name="data"> The data to be converted. </param>
		/// <returns> The unencoded byte array. </returns>
		public static byte[] FromBase64ToByteArray(this string data)
		{
			var key = ";base64,";
			var index = data.IndexOf(key);
			if (index >= 0)
			{
				data = data.Substring(index + key.Length);
			}

			return Convert.FromBase64String(data);
		}

		/// <summary>
		/// Convert a hex string to a byte array.
		/// </summary>
		/// <param name="hexString"> A string with hex data (2 bytes per character). </param>
		/// <returns> The byte array value of the hex string. </returns>
		public static byte[] ConvertHexStringToByteArray(this string hexString)
		{
			if ((hexString.Length % 2) != 0)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
			}

			var data = new byte[hexString.Length / 2];
			for (var index = 0; index < data.Length; index++)
			{
				var byteValue = hexString.Substring(index * 2, 2);
				data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}

			return data;
		}

		/// <summary>
		/// To literal version of the string.
		/// </summary>
		/// <param name="input"> The string input. </param>
		/// <returns> The literal version of the string. </returns>
		public static string Escape(this string input)
		{
			if (input == null)
			{
				return "null";
			}

			var literal = new StringBuilder(input.Length);

			foreach (var c in input)
			{
				switch (c)
				{
					case '\'':
						literal.Append(@"\'");
						continue;
					case '\"':
						literal.Append("\\\"");
						continue;
					case '\\':
						literal.Append(@"\\");
						continue;
					case '\0':
						literal.Append(@"\0");
						continue;
					case '\a':
						literal.Append(@"\a");
						continue;
					case '\b':
						literal.Append(@"\b");
						continue;
					case '\f':
						literal.Append(@"\f");
						continue;
					case '\n':
						literal.Append(@"\n");
						continue;
					case '\r':
						literal.Append(@"\r");
						continue;
					case '\t':
						literal.Append(@"\t");
						continue;
					case '\v':
						literal.Append(@"\v");
						continue;
					default:
						// ASCII printable character
						if ((c >= 0x20) && (c <= 0x7e))
						{
							literal.Append(c);
							// As UTF16 escaped character
						}
						else
						{
							literal.Append(@"\u");
							literal.Append(((int) c).ToString("x4"));
						}
						continue;
				}
			}

			return literal.ToString();
		}

		/// <summary>
		/// Turn a byte array into a readable, escaped string
		/// </summary>
		/// <param name="bytes"> bytes </param>
		/// <returns> a string </returns>
		public static string Escape(this byte[] bytes)
		{
			var data = Encoding.UTF8.GetString(bytes);
			return Escape(data);
		}

		/// <summary>
		/// Convert a HEX string to a regular text string.
		/// </summary>
		/// <param name="value"> A string with hex data (2 bytes per character). </param>
		/// <returns> The string value. </returns>
		public static string FromHexString(this string value)
		{
			var bytes = ConvertHexStringToByteArray(value);
			var response = Encoding.UTF8.GetString(bytes);
			return response;
		}

		/// <summary>
		/// Convert the hex string back to byte array.
		/// </summary>
		/// <param name="value"> The hex string to be converter. </param>
		/// <returns> The byte array. </returns>
		public static byte[] FromHexStringToArray(this string value)
		{
			var bytes = new byte[value.Length / 2];

			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
			}

			return bytes;
		}

		/// <summary>
		/// Gets a stable hash code for a string value.
		/// </summary>
		/// <param name="value"> The string value. </param>
		/// <returns> The hash code for the value. </returns>
		public static int GetStableHashCode(this string value)
		{
			unchecked
			{
				var hash1 = 5381;
				var hash2 = hash1;

				for (var i = 0; (i < value.Length) && (value[i] != '\0'); i += 2)
				{
					hash1 = ((hash1 << 5) + hash1) ^ value[i];
					if ((i == (value.Length - 1)) || (value[i + 1] == '\0'))
					{
						break;
					}
					hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
				}

				return hash1 + (hash2 * 1566083941);
			}
		}

		/// <summary>
		/// Trims string to a maximum length.
		/// </summary>
		/// <param name="value"> The value to process. </param>
		/// <param name="max"> The maximum length of the string. </param>
		/// <param name="addEllipses"> The option to add ellipses to shorted strings. Defaults to false. </param>
		/// <returns> The value limited to the maximum length. </returns>
		public static string MaxLength(this string value, int max, bool addEllipses = false)
		{
			if (string.IsNullOrWhiteSpace(value) || (max <= 0))
			{
				return string.Empty;
			}

			var shouldAddEllipses = addEllipses && (value.Length > max) && (max >= 4);

			return value.Length > max ? value.Substring(0, shouldAddEllipses ? max - 3 : max) + (shouldAddEllipses ? "..." : string.Empty) : value;
		}

		/// <summary>
		/// Reverse the string order. Ex. "ABC" -> "CBA"
		/// </summary>
		/// <param name="value"> The value to reverse. </param>
		/// <returns> The string in a reversed order. </returns>
		public static string ReverseString(this string value)
		{
			var myArr = value.ToCharArray();
			Array.Reverse(myArr);
			return new string(myArr);
		}

		/// <summary>
		/// Converts a string to hex string value. Ex. "A" -> "41"
		/// </summary>
		/// <param name="value"> The string value to convert. </param>
		/// <param name="delimiter"> An optional delimited to put between bytes of the data. </param>
		/// <param name="prefix"> An optional prefix to put before each byte of the data. </param>
		/// <returns> The string in a hex string format. </returns>
		public static string ToHexString(this string value, string delimiter = null, string prefix = null)
		{
			var bytes = Encoding.Default.GetBytes(value);
			var hexString = bytes.ToHexString(null, null, delimiter, prefix);
			return hexString;
		}

		/// <summary>
		/// Converts a byte array to a hex string format. Ex. [41],[42] = "4142"
		/// </summary>
		/// <param name="data"> The byte array to convert. </param>
		/// <returns> The byte array in a hex string format. </returns>
		public static string ToHexString(this Guid data)
		{
			var hexString = BitConverter.ToString(data.ToByteArray());
			hexString = hexString.Replace("-", "");
			return hexString;
		}

		/// <summary>
		/// Converts a byte array to a hex string format. Ex. [41],[42] = "4142"
		/// </summary>
		/// <param name="data"> The byte array to convert. </param>
		/// <param name="startIndex"> The starting position within value. </param>
		/// <param name="length"> The number of array elements in value to convert. </param>
		/// <param name="delimiter"> An optional delimited to put between bytes of the data. </param>
		/// <param name="prefix"> An optional prefix to put before each byte of the data. </param>
		/// <returns> The byte array in a hex string format. </returns>
		public static string ToHexString(this byte[] data, int? startIndex = null, int? length = null, string delimiter = null, string prefix = null)
		{
			var hexString = BitConverter.ToString(data, startIndex ?? 0, length ?? data.Length);
			hexString = (prefix ?? "") + hexString.Replace("-", (delimiter ?? "") + (prefix ?? ""));
			return hexString;
		}

		/// <summary>
		/// Convert a string into a secure string.
		/// </summary>
		/// <param name="input"> The string. </param>
		/// <returns> The secure string. </returns>
		public static SecureString ToSecureString(this string input)
		{
			var secure = new SecureString();
			foreach (var c in input)
			{
				secure.AppendChar(c);
			}

			secure.MakeReadOnly();
			return secure;
		}

		/// <summary>
		/// Converts a SecureString to an unsecure string.
		/// </summary>
		/// <param name="value"> The value to be converted. </param>
		/// <returns> The unsecure string. </returns>
		public static string ToUnsecureString(this SecureString value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			var unmanagedString = IntPtr.Zero;

			try
			{
				unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
				return Marshal.PtrToStringUni(unmanagedString);
			}
			finally
			{
				Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
			}
		}

		/// <summary>
		/// Turn a readable string into a byte array
		/// </summary>
		/// <param name="value"> a string, optionally with escape sequences in it </param>
		/// <returns> The unescape version of the provided value. </returns>
		public static string Unescape(this string value)
		{
			var count = 0;
			var isEscaped = false;
			var parseHexNext = false;
			var parseHexCount = 0;
			var parseHexCounts = new List<int>();

			// first we count the number of chars we will be returning
			for (var i = 0; i < value.Length; i++)
			{
				var c = value[i];

				if (parseHexNext)
				{
					if (Uri.IsHexDigit(c))
					{
						parseHexCount++;
					}
					else
					{
						parseHexCounts.Add(parseHexCount);
						count += parseHexCount;
						parseHexNext = false;
						parseHexCount = 0;
					}
				}

				// if we are not in  an escape sequence and the char is a escape char
				else if ((isEscaped == false) && (c == '\\'))
				{
					// escape
					isEscaped = true;

					// increment count
					count++;
				}

				// else if we are escaped
				else if (isEscaped)
				{
					// reset escape state
					isEscaped = false;

					// check the char against the set of known escape chars
					switch (char.ToLower(c))
					{
						case '0':
						case 'a':
						case 'b':
						case 'f':
						case 'n':
						case 'r':
						case 't':
						case 'v':
						case '\'':
						case '\\':
						case '"':
							// do not increment count
							break;

						case 'u':
							// Skip the 4 value because they are unicode values
							i += 4;
							break;

						case 'x':
							// do not increment count
							parseHexNext = true;
							parseHexCount = 0;
							break;

						default:
							// this is not a valid escape sequence
							throw new Exception($"Invalid escape sequence at char of [{c}] at offset {i - 1}.");
					}
				}
				else
				{
					// normal char increment count
					count++;
				}
			}

			if (parseHexNext)
			{
				throw new Exception($"Invalid escape sequence at char '{value.Length - 1}' missing hex value.");
			}

			if (isEscaped)
			{
				throw new Exception($"Invalid escape sequence at char '{value.Length - 1}'.");
			}

			// create a character array for the result
			var chars = new char[count];
			var hexCountIndex = 0;
			var j = 0;

			// actually populate the array
			for (var i = 0; i < value.Length; i++)
			{
				var c = value[i];

				// if we are not in  an escape sequence and the char is a escape char
				if ((isEscaped == false) && (c == '\\'))
				{
					// escape
					isEscaped = true;
				}

				// else if we are escaped
				else if (isEscaped)
				{
					// reset escape state
					isEscaped = false;

					// check the char against the set of known escape chars
					switch (char.ToLower(value[i]))
					{
						case '0':
							chars[j++] = '\0';
							break;

						case 'a':
							chars[j++] = '\a';
							break;

						case 'b':
							chars[j++] = '\b';
							break;

						case 'f':
							chars[j++] = '\f';
							break;

						case 'n':
							chars[j++] = '\n';
							break;

						case 'r':
							chars[j++] = '\r';
							break;

						case 't':
							chars[j++] = '\t';
							break;

						case 'v':
							chars[j++] = '\v';
							break;

						case '\\':
							chars[j++] = '\\';
							break;

						case '\'':
							chars[j++] = '\'';
							break;

						case '"':
							chars[j++] = '"';
							break;

						case 'x':
							chars[j++] = '\\';
							chars[j++] = c;
							var hexCount = parseHexCounts[hexCountIndex++];
							for (var h = 0; h < hexCount; h++)
							{
								chars[j++] = value[++i];
							}
							break;

						case 'u':
							chars[j++] = (char) ((Uri.FromHex(value[++i]) << 12) | (Uri.FromHex(value[++i]) << 8) | (Uri.FromHex(value[++i]) << 4) | Uri.FromHex(value[++i]));
							break;
					}
				}
				else
				{
					// normal char
					chars[j++] = c;
				}
			}

			return new string(chars);
		}

		internal static int AlignedStringLength(this string val)
		{
			var len = val.Length + (4 - (val.Length % 4));
			if (len <= val.Length)
			{
				len += 4;
			}

			return len;
		}

		#endregion
	}
}