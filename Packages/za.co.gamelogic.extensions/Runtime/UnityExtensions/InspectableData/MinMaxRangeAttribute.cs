// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Use this attribute to specify the range for a <see cref="MinMaxFloat"/> field, property, parameter or return value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	[Version(4, 2, 0)]
	public class MinMaxRangeAttribute : Attribute
	{
		private float min, max;
		
		public MinMaxRangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
		
		public MinMaxRangeAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}
		
		public (float min, float max) GetRange()
		{
			return (min, max);
		}
	}
}
