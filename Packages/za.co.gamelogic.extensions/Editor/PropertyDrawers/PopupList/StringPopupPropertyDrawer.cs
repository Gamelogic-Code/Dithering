using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A property drawer for string popup lists, used to display a dropdown with string options in the Unity Editor.
	/// </summary>
	[CustomPropertyDrawer(typeof(StringPopupAttribute))]
	[CustomPropertyDrawer(typeof(TagPopupAttribute))]
	[CustomPropertyDrawer(typeof(LayerPopupAttribute))]
	[Version(4, 3, 0)]
	public class StringPopupPropertyDrawer : PopupListPropertyDrawer<string>
	{
		/// <summary>
		/// Converts a string value into a <see cref="GUIContent"/> object for display in the popup list.
		/// </summary>
		/// <param name="value">The string value to convert.</param>
		/// <returns>A <see cref="GUIContent"/> object representing the string value.</returns>
		protected override GUIContent GetContent(string value) => new GUIContent(value);

		/// <summary>
		/// Sets the string value of the serialized property based on the selected option in the popup list.
		/// </summary>
		/// <param name="property">The serialized property to set the value for.</param>
		/// <param name="value">The string value to set.</param>
		protected override void SetPropertyValue(SerializedProperty property, string value) 
			=> property.stringValue = value;
		
		/// <summary>
		/// Gets the current string value of the serialized property.
		/// </summary>
		/// <param name="property">The serialized property to get the value from.</param>
		/// <returns>The current string value of the serialized property.</returns>
		protected override string GetValue(SerializedProperty property) => property.stringValue;
	}
}
