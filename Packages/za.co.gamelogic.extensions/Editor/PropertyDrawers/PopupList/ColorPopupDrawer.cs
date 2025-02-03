using System.Collections.Generic;
using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A property drawer for color popup lists, used to display a dropdown with color options in the Unity Editor.
	/// </summary>
	[CustomPropertyDrawer(typeof(ColorPopupAttribute))]
	[Version(4, 3, 0)]
	internal class ColorPopupPropertyDrawer : PopupListPropertyDrawer<Color>
	{
		private static Dictionary<Color, Texture> colorBlocks = new Dictionary<Color, Texture>();
		
		/// <summary>
		/// Converts a color value into a <see cref="GUIContent"/> object with a colored square for display in the popup list.
		/// </summary>
		/// <param name="value">The color value to convert.</param>
		/// <returns>A <see cref="GUIContent"/> object representing the color value.</returns>
		protected override GUIContent GetContent(Color value) => CreateColoredSquare(value);

		
		
		/// <summary>
		/// Sets the color value of the serialized property based on the selected option in the popup list.
		/// </summary>
		/// <param name="property">The serialized property to set the value for.</param>
		/// <param name="value">The color value to set.</param>
		protected override void SetPropertyValue(SerializedProperty property, Color value) 
			=> property.colorValue = value;
		
		/// <summary>
		/// Gets the current color value of the serialized property.
		/// </summary>
		/// <param name="property">The serialized property to get the value from.</param>
		/// <returns>The current color value of the serialized property.</returns>
		protected override Color GetValue(SerializedProperty property) => property.colorValue;

		/// <summary>
		/// Creates a <see cref="Texture2D"/> filled with the specified color.
		/// </summary>
		/// <param name="color">The color to fill the texture with.</param>
		/// <returns>A <see cref="Texture2D"/> object filled with the specified color.</returns>
		public static Texture2D CreateColorTexture(Color color)
		{
			int size = 12;
			var texture = new Texture2D(size, size);
			var pixels = new Color[size * size];

			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = color;
			}

			texture.SetPixels(pixels);
			texture.Apply();
			
			return texture;
		}

		/// <summary>
		/// Creates a <see cref="GUIContent"/> object with a colored square based on the specified color.
		/// </summary>
		/// <param name="color">The color to create the square with.</param>
		/// <returns>A <see cref="GUIContent"/> object with a colored square.</returns>
		public static GUIContent CreateColoredSquare(Color color)
		{
			if(!colorBlocks.ContainsKey(color))
			{
				var texture = CreateColorTexture(color);
				colorBlocks[color] = texture;
			}
			
			return new GUIContent(color.ToString(), colorBlocks[color]);
		}
	}
}
