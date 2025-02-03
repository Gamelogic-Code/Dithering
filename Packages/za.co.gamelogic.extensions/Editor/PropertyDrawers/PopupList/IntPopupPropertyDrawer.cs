using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A property drawer for integer popup lists, used to display a dropdown with integer options in the Unity Editor.
	/// </summary>
	[CustomPropertyDrawer(typeof(IntPopupAttribute))]
	[Version(4, 3, 0)]
	public class IntPopupPropertyDrawer : PopupListPropertyDrawer<int>
	{
		/// <summary>
		/// Converts an integer value into a <see cref="GUIContent"/> object for display in the popup list.
		/// </summary>
		/// <param name="value">The integer value to convert.</param>
		/// <returns>A <see cref="GUIContent"/> object representing the integer value.</returns>
		protected override GUIContent GetContent(int value) => new GUIContent(value.ToString());

		/// <summary>
		/// Sets the integer value of the serialized property based on the selected option in the popup list.
		/// </summary>
		/// <param name="property">The serialized property to set the value for.</param>
		/// <param name="value">The integer value to set.</param>
		protected override void SetPropertyValue(SerializedProperty property, int value) => property.intValue = value;
		
		/// <summary>
		/// Gets the current integer value of the serialized property.
		/// </summary>
		/// <param name="property">The serialized property to get the value from.</param>
		/// <returns>The current integer value of the serialized property.</returns>
		protected override int GetValue(SerializedProperty property) => property.intValue;
	}
}