// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	[Version(4, 3, 0)]
	public class SeparatorAttribute : PropertyAttribute
	{
		public int Height { get; }
		
		public Color Color { get; }
		
		public SeparatorAttribute()
		{
			Height = PropertyDrawerData.SeparatorHeight;
			Color = PropertyDrawerData.SeparatorColor;
		}
		
		public SeparatorAttribute(string color)
		{
			Height = PropertyDrawerData.SeparatorHeight;
			Color = 
				color == null || !ColorExtensions.TryParseHex(color, out var rgbColor) 
					? PropertyDrawerData.SeparatorColor 
					: rgbColor;
		}
		
		public SeparatorAttribute(int height)
		{
			Height = height;
			Color = PropertyDrawerData.SeparatorColor;
		}
		
		public SeparatorAttribute(string color, int height)
		{
			Height = height;
			Color = 
				color == null || !ColorExtensions.TryParseHex(color, out var c) 
					? Color.black 
					: c;
		}
	}
}
