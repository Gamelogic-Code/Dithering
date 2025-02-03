// Copyright Gamelogic (c) http://www.gamelogic.co.za

using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A property drawer for fields marked with the Highlight Attribute.
	/// </summary>
	[Version(1, 2, 0)]
	[CustomPropertyDrawer(typeof(HighlightAttribute))]
	public class HighlightPropertyDrawer : PropertyDrawer
	{
		private HighlightAttribute Attribute => (HighlightAttribute) attribute;
		
		public override void OnGUI(Rect position,
			SerializedProperty property,
			GUIContent label)
		{
			
			var newColor = Attribute.color;
			
			if (EditorGUIUtility.isProSkin)
			{			
				var oldColor = GUI.color;
				GUI.color = newColor;
				EditorGUI.PropertyField(position, property, label);
				GUI.color = oldColor;
			}
			else
			{
				var oldBackgroundColor = GUI.backgroundColor;
				var oldContentColor = GUI.contentColor;
				
				EditorGUI.DrawRect(position, newColor);
				GUI.contentColor = Color.black;
				GUI.backgroundColor = newColor;
				
				EditorGUI.PropertyField(position, property, label);
				GUI.backgroundColor = oldBackgroundColor;
				GUI.contentColor = oldContentColor;
			}
		}
	}
}
