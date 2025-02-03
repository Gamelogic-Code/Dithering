// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Globalization;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Provides some utility functions for Colors.
	/// </summary>
	[Version(1, 0, 0)]
	public static class ColorExtensions
	{
		#region Constants

		private const float LightOffset = 0.0625f;
		public const string HexStringBlack = "000";

		#endregion

		#region Static Methods

		/// <summary>
		/// Returns a color lighter than the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Lighter(this Color color)
		{
			return new Color(
				color.r + LightOffset,
				color.g + LightOffset,
				color.b + LightOffset,
				color.a);
		}

		/// <summary>
		/// Returns a color darker than the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Darker(this Color color)
		{
			return new Color(
				color.r - LightOffset,
				color.g - LightOffset,
				color.b - LightOffset,
				color.a);
		}

		/// <summary>
		/// Returns the brightness of the color, 
		/// defined as the average off the three color channels.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static float Brightness(this Color color)
		{
			return (color.r + color.g + color.b)/3;
		}

		/// <summary>
		/// Returns a new color with the RGB values scaled so that the color has the given
		/// brightness.
		/// </summary>
		/// <remarks>
		/// If the color is too dark, a gray is returned with the right brightness. The alpha
		/// is left unchanged.
		/// </remarks>
		/// <param name="color"></param>
		/// <param name="brightness"></param>
		public static Color WithBrightness(this Color color, float brightness)
		{
			if (color.IsApproximatelyBlack())
			{
				return new Color(brightness, brightness, brightness, color.a);
			}
			
			float factor = brightness/color.Brightness();

			float r = color.r*factor;
			float g = color.g*factor;
			float b = color.b*factor;

			float a = color.a;

			return new Color(r, g, b, a);
		}

		/// <summary>
		/// Returns whether the color is black or almost black.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static bool IsApproximatelyBlack(this Color color)
		{
			return color.r + color.g + color.b <= Mathf.Epsilon;
		}

		/// <summary>
		/// Returns whether the color is white or almost white.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static bool IsApproximatelyWhite(this Color color)
		{
			return color.r + color.g + color.b >= 1 - Mathf.Epsilon;
		}

		/// <summary>
		/// Returns an opaque version of the given color.
		/// </summary>
		/// <param name="color"></param>
		/// <returns></returns>
		public static Color Opaque(this Color color)
		{
			return new Color(color.r, color.g, color.b);
		}

		/// <summary>
		/// Returns a new color that is this color inverted.
		/// </summary>
		/// <param name="color">The color to invert.</param>
		/// <returns></returns>
		public static Color Invert(this Color color)
		{
			return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
		}

		/// <summary>
		/// Returns the same color, but with the specified alpha.
		/// </summary>
		/// <param name="color">The color.</param>
		/// <param name="alpha">The alpha.</param>
		/// <returns>Color.</returns>
		public static Color WithAlpha(this Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}
		
		/// <summary>
		/// Parses a <see cref="Color"/> from a hexadecimal string.
		/// </summary>
		/// <param name="hex">The hexadecimal string to parse.</param>
		/// <returns>The color that the string represents.</returns>
		/// <exception cref="FormatException">the string is not in a valid format.</exception>
		/// <remarks>See <see cref="TryParseHex"/> for details on what formats are supported.</remarks>
		public static Color ParseHex(string hex)
		{
			if (TryParseHex(hex, out var color))
			{
				return color;
			}

			throw new FormatException($"The color string '{hex}' is not in a valid format.");
		}
		
		/// <summary>
		/// Tries to parse a <see cref="Color"/> from a hexadecimal string.
		/// </summary>
		/// <param name="hex">The hexadecimal string to parse.</param>
		/// <param name="color">The color if the parsing was successful, <see cref="Color.black"/> otherwise.</param>
		/// <returns>Whether the parsing was successful.</returns>
		/// <remarks>
		/// The string may start with an optional `#` character.
		/// 
		/// If the string is 3 or 4 characters long, the characters are each repeated to form a 6 or 8 character string.
		///
		/// If the string has 6 characters, the first two characters are the red component, the next two are the green
		/// component, and the last two are the blue component.
		///
		/// If the string has 8 characters, the last two characters are the alpha component.
		/// </remarks>
		[Version(4, 3, 0)]
		public static bool TryParseHex(string hex, out Color color)
		{
			if(string.IsNullOrEmpty(hex))
			{
				return Failed(out color);
			}
			
			if (hex[0] == '#')
			{
				hex = hex.Substring(1);
			}

			// Implementation note: is there not a better way to do this?
			if (hex.Length == 3)
			{
				hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}";
			}

			if (hex.Length == 4)
			{
				hex = $"{hex[0]}{hex[0]}{hex[1]}{hex[1]}{hex[2]}{hex[2]}{hex[3]}{hex[3]}";
			}
			
			if(hex.Length != 6 && hex.Length != 8)
			{
				return Failed(out color);
			}

			byte a = byte.MaxValue;
			byte g = 0, b = 0;
			
			bool succeeded =
				byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, null, out byte r) && 
				byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, null, out g) && 
				byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, null, out b) &&
				(hex.Length == 6 || byte.TryParse(hex.Substring(6, 2), NumberStyles.HexNumber, null, out a));

			return succeeded ? FromRgba(r, g, b, a, out color) : Failed(out color);
			
			bool Failed(out Color c)
			{
				c = Color.black;
				return false;
			}
			
			bool FromRgba(byte newR, byte newG, byte newB, byte newA, out Color c)
			{
				c = new Color32(newR, newG, newB, newA);
				return true;
			}
		}

		/// <summary>
		/// Converts a color to a six-character string of the RGB values of the color in hexadecimal.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>The color as a hexadecimal string.</returns>
		[Version(4, 3, 0)]
		public static string ToRGBHex(Color color)
		{
			string r = Mathf.RoundToInt(color.r * 255f).ToString("X2");
			string g = Mathf.RoundToInt(color.g * 255f).ToString("X2");
			string b = Mathf.RoundToInt(color.b * 255f).ToString("X2");
			
			return $"{r}{g}{b}";
		}

		/// <summary>
		/// Converts a color to an eight-character string of the RGBA values of the color in hexadecimal.
		/// </summary>
		/// <param name="color">The color to convert.</param>
		/// <returns>The color as a hexadecimal string.</returns>
		[Version(4, 3, 0)]
		public static string ToRGBAHex(Color color)
		{
			string rgb = ToRGBHex(color);
			string a = Mathf.RoundToInt(color.a * 255f).ToString("X2");

			return $"{rgb}{a}";
		}

		#endregion
	}
}
