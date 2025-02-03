using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions.Support
{
	/// <summary>
	/// Provides a set of colors that can be used for branding, mostly to be used in examples. 
	/// </summary>
	[Version(4, 2, 0)]
	public static class Branding
	{
		/// <summary>
		/// Provides the branding color's Hex values. 
		/// </summary>
		/// <remarks>
		/// Among other things, this is useful for specifying colors in attributes. 
		/// </remarks>
		[Version(4, 3, 0)]
		[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
		public static class Hex
		{
			public const string Coral = "#FF716F";
			public const string Carrot = "#F9A332";
			public const string Lemon = "#F9EE34";
			public const string Apple = "#76C83D";
			public const string Aqua = "#7BD9E1";
			public const string Azure = "#0297FE";
			public const string Indigo = "#30287A";
			
			public const string CoralLight = "#FFA5A5";
			public const string CarrotLight = "#FABC68";
			public const string LemonLight = "#FCF79A";
			public const string AppleLight = "#A3DB7D";
			public const string AquaLight = "#B3E9EF";
			public const string AzureLight = "#52B9FE";
			public const string IndigoLight = "#5F52C8";
			
			public const string CoralDark = "#FF3C36";
			public const string CarrotDark = "#F88318";
			public const string LemonDark = "#F0D506";
			public const string AppleDark = "#1A902D";
			public const string AquaDark = "#37BBD1";
			public const string AzureDark = "#0154C7";
			public const string IndigoDark = "#1B123A";
			
			public const string White = "#FFF";
			public const string Black = "#000";
			
			[Version(4, 4, 0)]
			public const string Gray3 = "#333";
			
			[Version(4, 4, 0)]
			public const string Gray6 = "#666";
			
			[Version(4, 4, 0)]
			public const string Gray9 = "#999";
			
			[Version(4, 4, 0)]
			public const string GrayA = "#AAA";
			
			[Version(4, 4, 0)]
			public const string GrayC = "#CCC";
		}

		public static readonly Color Coral = ColorExtensions.ParseHex(Hex.Coral);
		public static readonly Color Carrot = ColorExtensions.ParseHex(Hex.Carrot);
		public static readonly Color Lemon = ColorExtensions.ParseHex(Hex.Lemon);
		public static readonly Color Apple = ColorExtensions.ParseHex(Hex.Apple);
		public static readonly Color Aqua = ColorExtensions.ParseHex(Hex.Aqua);
		public static readonly Color Azure = ColorExtensions.ParseHex(Hex.Azure);
		public static readonly Color Indigo = ColorExtensions.ParseHex(Hex.Indigo);

		public static readonly Color CoralLight = ColorExtensions.ParseHex(Hex.CoralLight);
		public static readonly Color CarrotLight = ColorExtensions.ParseHex(Hex.CarrotLight);
		public static readonly Color LemonLight = ColorExtensions.ParseHex(Hex.LemonLight);
		public static readonly Color AppleLight = ColorExtensions.ParseHex(Hex.AppleLight);
		public static readonly Color AquaLight = ColorExtensions.ParseHex(Hex.AquaLight);
		public static readonly Color AzureLight = ColorExtensions.ParseHex(Hex.AzureLight);
		public static readonly Color IndigoLight = ColorExtensions.ParseHex(Hex.IndigoLight);

		public static readonly Color CoralDark = ColorExtensions.ParseHex(Hex.CoralDark);
		public static readonly Color CarrotDark = ColorExtensions.ParseHex(Hex.CarrotDark);
		public static readonly Color LemonDark = ColorExtensions.ParseHex(Hex.LemonDark);
		public static readonly Color AppleDark = ColorExtensions.ParseHex(Hex.AppleDark);
		public static readonly Color AquaDark = ColorExtensions.ParseHex(Hex.AquaDark);
		public static readonly Color AzureDark = ColorExtensions.ParseHex(Hex.AzureDark);
		public static readonly Color IndigoDark = ColorExtensions.ParseHex(Hex.IndigoDark);
		
		[Version(4, 3, 0)]
		public static readonly Color White = Color.white;
		
		[Version(4, 3, 0)]
		public static readonly Color Black = Color.black;

		[Version(4, 4, 4)]
		public static readonly Color Gray3 = ColorExtensions.ParseHex(Hex.Gray3);
		
		[Version(4, 4, 4)]
		public static readonly Color Gray6 = ColorExtensions.ParseHex(Hex.Gray6);
		
		[Version(4, 4, 4)]
		public static readonly Color Gray9 = ColorExtensions.ParseHex(Hex.Gray9);
		
		[Version(4, 4, 4)]
		public static readonly Color GrayA = ColorExtensions.ParseHex(Hex.GrayA);
		
		[Version(4, 4, 4)]
		public static readonly Color GrayC = ColorExtensions.ParseHex(Hex.GrayC);
		
		private static readonly Color[] ColorList = 
		{
			Coral, Carrot, Lemon, Apple, Aqua, Azure, Indigo,
			CoralLight, CarrotLight, LemonLight, AppleLight, AquaLight, AzureLight, IndigoLight,
			CoralDark, CarrotDark, LemonDark, AppleDark, AquaDark, AzureDark, IndigoDark
		};
		
		/// <summary>
		/// The default colors used in the Gamelogic examples.
		/// </summary>
		public static IReadOnlyList<Color> Colors => ColorList;
	}
}
