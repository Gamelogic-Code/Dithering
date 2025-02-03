using System.IO;
using Gamelogic.Extensions.Editor;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Samples
{
	/*	This is an example of a custom property drawer that uses the PopupListDrawer to create a popup list of
		scenes in the build settings.
		
		We use the BuildScenePopupAttribute to mark the field that should be drawn as a popup list of scenes.
	*/
	public class BuildScenePropertyDrawer
	{
		[CustomPropertyDrawer(typeof(BuildScenePopupAttribute))]
		public class BuildScenePopupPropertyDrawer : PopupListPropertyDrawer<string>
		{
			/// <summary>
			/// Converts a string value into a <see cref="GUIContent"/> object for display in the popup list.
			/// </summary>
			/// <param name="value">The string value to convert.</param>
			/// <returns>A <see cref="GUIContent"/> object representing the string value.</returns>
			protected override GUIContent GetContent(string value)
				=> new GUIContent( Path.GetFileNameWithoutExtension(value));

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
}
