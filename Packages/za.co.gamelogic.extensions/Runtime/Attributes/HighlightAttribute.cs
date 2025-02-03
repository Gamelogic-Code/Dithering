using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Mark simple types to be highligted in the inspector.
	/// </summary>
	/// <seealso cref="UnityEngine.PropertyAttribute" />
	public class HighlightAttribute : PropertyAttribute
	{
		public Color color;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="HighlightAttribute"/> class, which uses the default color.
		/// </summary>
		public HighlightAttribute() => color = PropertyDrawerData.HighlightColor;

		/// <summary>
		/// Initializes a new instance of the <see cref="HighlightAttribute"/> class, which uses the specified color.
		/// </summary>
		/// <param name="hexString">The hex string.<see cref="ColorExtensions.TryParseHex"/> to see details of the format.</param>
		public HighlightAttribute(string hexString) => color = ColorExtensions.ParseHex(hexString);
	}
}
