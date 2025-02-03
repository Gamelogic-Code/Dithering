// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Class for representing a bounded range.
	/// </summary>
	[Version(1, 2, 0)]
	[Serializable]
	public class MinMaxFloat
	{
		#region Public Fields

		public float min = 0.0f;
		public float max = 1.0f;

		#endregion

		public MinMaxFloat()
		{
			min = 0.0f;
			max = 1.0f;
		}

		public MinMaxFloat(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Calculates the linear interpolation between min and max.
		/// </summary>
		/// <param name="t">The interpolation parameter, typically between 0 and 1.</param>
		/// <returns>The interpolated value.</returns>
		[Version(4, 2, 0)]
		public float Lerp(float t) => Mathf.Lerp(min, max, t);
		
		/// <summary>
		/// Calculates the inverse linear interpolation between min and max, that is the parameter t such that
		/// Lerp(t) = value.
		/// </summary>
		/// <param name="value">The value to calculate the inverse lerp for.</param>
		/// <returns>The inverse lerp value.</returns>
		[Version(4, 2, 0)]
		public float InverseLerp(float value) => Mathf.InverseLerp(min, max, value);
		
		/// <summary>
		/// Clamps the value to the range [min, max].
		/// </summary>
		/// <param name="value">The value to clamp.</param>
		/// <returns>The clamped value.</returns>
		[Version(4, 2, 0)]
		public float Clamp(float value) => Mathf.Clamp(value, min, max);
		
		/// <summary>
		/// Calculates the linear interpolation between min and max without clamping the t parameter.
		/// </summary>
		/// <param name="t">The interpolation parameter.</param>
		/// <returns>The interpolated value.</returns>
		[Version(4, 2, 0)]
		public float LerpUnclamped(float t) => Mathf.LerpUnclamped(min, max, t);
		
		
		
	}
}
