// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Gamelogic.Extensions.Internal;
using Gamelogic.Extensions.Support;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Methods for additional math functions.
	/// </summary>
	[Version(1, 4, 0)]
	public static class GLMathf 
	{
		#region Constants

		public static readonly float Sqrt3 = Mathf.Sqrt(3);

		#endregion

		#region Static Methods

		/// <summary>
		/// Linearly interpolates between two values between 0 and 1 if values wrap around from 1 back to 0.
		/// </summary>
		/// <remarks>This is useful, for example, in lerping between angles.</remarks>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// float angleInRad1 = 1;
		/// float angleInRad2 = 5;
		/// float revolution = Mathf.PI * 2;
		/// float interpolation = WLerp(angleInRad1 / revolution, angleInRad2 / revolution, 0.5f);
		/// 
		/// //interpolation == (5 + 1 + Mathf.PI * 2)/2 = 3 + Mathf.PI
		/// ]]>
		/// </code>
		/// </example>
		public static float Wlerp01(float v1, float v2, float t)
		{
			GLDebug.Assert(InRange(v1, 0, 1), "v1 is not in [0, 1)");
			GLDebug.Assert(InRange(v2, 0, 1), "v2 is not in [0, 1)");

			if (Mathf.Abs(v1 - v2) <= 0.5f)
			{
				return Mathf.Lerp(v1, v2, t);
			}
			else if (v1 <= v2)
			{
				return Frac(Mathf.Lerp(v1 + 1, v2, t));
			}
			else
			{
				return Frac(Mathf.Lerp(v1, v2 + 1, t));
			}
		}

		/// <summary>
		/// Tests whether the given value lies in the range [0, 1).
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <returns><c>true</c> if the given value is equal or greater than 0 and smaller than 1, <c>false</c> otherwise.</returns>
		public static bool InRange01(float value) => InRange(value, 0, 1);

		/// <summary>
		/// Tests whether the given value lies in the half-open interval specified by its endpoints, that is, whether the value
		/// lies in the interval <c>[closedLeft, openRight)</c>.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="closedLeft">The left end of the interval.</param>
		/// <param name="openRight">The right end of the interval.</param>
		/// <returns><c>true</c> if the given value is equal or greater than <c>closedLeft</c> and smaller than <c>openRight</c>, <c>false</c> otherwise.</returns>
		public static bool InRange(float value, float closedLeft, float openRight) 
			=> value >= closedLeft && value < openRight;

		/// <summary>
		/// Mod operator that also works for negative m.
		/// </summary>
		/// <param name="m">The m.</param>
		/// <param name="n">The n.</param>
		/// <returns>System.Int32.</returns>
		[Version(2, 2, 0)]
		public static int FloorMod(int m, int n)
		{
			int mod = m % n;
			// If m is negative and mod is not 0, we adjust it because C#'s % operator
			// does not produce a floor modulus result directly.
			if (m < 0 && mod != 0)
			{
				mod += n;
			}

			return mod;
		}

		/// <summary>
		/// Mod operator that also works for negative m.
		/// </summary>
		/// <param name="m">The m.</param>
		/// <param name="n">The n.</param>
		/// <returns>System.Int32.</returns>
		[Version(2, 2, 0)]
		public static float FloorMod(float m, float n)
		{
			float mod = m % n;
			// If m is negative and mod is not 0, we adjust it because C#'s % operator
			// does not produce a floor modulus result directly.
			if (m < 0 && mod != 0)
			{
				mod += n;
			}

			return mod;
		}

		/// <summary>
		/// Floor division that also work for negative m.
		/// </summary>
		/// <param name="m">The m.</param>
		/// <param name="n">The n.</param>
		/// <returns>System.Int32.</returns>
		[Version(2, 2, 0)]
		public static int FloorDiv(int m, int n)
		{
			if (m >= 0)
			{
				return m / n;
			}

			int t = m / n;

			if (t * n == m)
			{
				return t;
			}

			return t - 1;
		}

		/// <summary>
		/// Returns the fractional part of a floating point number.
		/// </summary>
		/// <param name="x">The number to get the fractional part of.</param>
		/// <returns>The fractional part of the given number.</returns>
		/// <remarks>The result is always the number minus the number's floor.</remarks>
		public static float Frac(float x)
		{
			return x - Mathf.Floor(x);
		}

		/// <summary>
		/// Returns the sign function evaluated at the given value.
		/// </summary>
		/// <returns>1 if the given value is positive, -1 if it is negative, and 0 if it is 0.</returns>
		public static int Sign(float x)
		{
			if (x > 0) return 1;
			if (x < 0) return -1;

			return 0;
		}

		/// <summary>
		/// Returns the sign function evaluated at the given value.
		/// </summary>
		/// <returns>1 if the given value is positive, -1 if it is negative, and 0 if it is 0.</returns>
		public static int Sign(int p) => p > 0 ? 1 : p < 0 ? -1 : 0;

		/// <summary>
		/// Checks if two floating point numbers are equal.
		/// </summary>
		/// <param name="a">The first number.</param>
		/// <param name="b">The second number.</param>
		/// <returns><see langword="true"/> if the two numbers are equal, <see langword="false"/> otherwise.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SuppressMessage(
			"ReSharper", 
			"CompareOfFloatsByEqualityOperator", 
			Justification = SuppressReason.IntentionalFloatingPointComparison)]
		public static bool Equal(float a, float b) => a == b;

		/// <summary>
		/// Performs bilinear interpolation between four values.
		/// </summary>
		/// <param name="a">The value at the top-left corner.</param>
		/// <param name="b">The value at the top-right corner.</param>
		/// <param name="c">The value at the bottom-left corner.</param>
		/// <param name="d">The value at the bottom-right corner.</param>
		/// <param name="u">The horizontal interpolation parameter (between 0 and 1).</param>
		/// <param name="v">The vertical interpolation parameter (between 0 and 1).</param>
		/// <returns>The interpolated value.</returns>
		[Version(4, 0, 0)]
		public static float Bilerp(float a, float b, float c, float d, float u, float v)
		{
			float s1 = Mathf.Lerp(a, b, u);
			float s2 = Mathf.Lerp(c, d, u);

			return Mathf.Lerp(s1, s2, v);
		}
		
		/// <summary>
		/// Performs trilinear interpolation between eight values.
		/// </summary>
		/// <param name="c000">The value at the corner (0, 0, 0).</param>
		/// <param name="c100">The value at the corner (1, 0, 0).</param>
		/// <param name="c010">The value at the corner (0, 1, 0).</param>
		/// <param name="c110">The value at the corner (1, 1, 0).</param>
		/// <param name="c001">The value at the corner (0, 0, 1).</param>
		/// <param name="c101">The value at the corner (1, 0, 1).</param>
		/// <param name="c011">The value at the corner (0, 1, 1).</param>
		/// <param name="c111">The value at the corner (1, 1, 1).</param>
		/// <param name="u">The interpolation parameter along the x-axis (between 0 and 1).</param>
		/// <param name="v">The interpolation parameter along the y-axis (between 0 and 1).</param>
		/// <param name="w">The interpolation parameter along the z-axis (between 0 and 1).</param>
		/// <returns>The interpolated value.</returns>
		[Version(4, 0, 0)]
		public static float Trilerp(
			float c000, float c100, float c010, float c110,
			float c001, float c101, float c011, float c111,
			float u, float v, float w)
		{
			float c00 = Mathf.Lerp(c000, c100, u);
			float c10 = Mathf.Lerp(c010, c110, u);
			float c01 = Mathf.Lerp(c001, c101, u);
			float c11 = Mathf.Lerp(c011, c111, u);

			float c0 = Mathf.Lerp(c00, c10, v);
			float c1 = Mathf.Lerp(c01, c11, v);

			return Mathf.Lerp(c0, c1, w);
		}

		#endregion

		#region Obsolete
		[Obsolete("Use FloorDiv instead")]
		public static int Div(int m, int n) => FloorDiv(m, n);

		[Obsolete("Use FloorMod instead")]
		public static int Mod(int m, int n) => FloorMod(m, n);

		[Obsolete("Use FloorMod instead")]
		public static float Mod(float m, float n) => FloorMod(m, n);

		/// <summary>
		/// Returns the highest integer equal to the given float.
		/// </summary>
		[Obsolete("Use Mathf.FloorToInt")]
		public static int FloorToInt(float x) => Mathf.FloorToInt(x);

		[Obsolete("Use Frac instead.")]
		public static float Wrap01(float value)
		{
			int n = Mathf.FloorToInt(value);
			float result = value - n;

			return result;
		}
		#endregion
	}
}
