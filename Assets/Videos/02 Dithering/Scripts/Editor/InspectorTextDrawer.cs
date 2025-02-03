using UnityEditor;
using UnityEngine;

namespace Gamelogic.Experimental.Editor
{
	/// <summary>
	/// The custom property drawer for the <see cref="InspectorTextAttribute"/> class.
	/// </summary>
	[CustomPropertyDrawer(typeof(InspectorTextAttribute))]
	public class InspectorTextDrawer : PropertyDrawer
	{
		private bool isEditing = false;

		private static readonly GUIStyle TextAreaStyle = new(EditorStyles.textArea)
		{
			wordWrap = true,
			active = new GUIStyleState()
			{
				textColor = Color.white
			}
		};
		
		private static readonly GUIStyle LabelStyle = new(EditorStyles.label)
		{
			wordWrap = true,
			alignment = TextAnchor.UpperLeft,
			richText = true,
			normal = new GUIStyleState
			{
				textColor = Color.white
			}
		};

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				EditorGUI.LabelField(position, label.text, "Use InspectorText with string.");
				return;
			}

			int lineCount = GetLineCount(property.stringValue);
			float textAreaHeight = (EditorGUIUtility.singleLineHeight * (lineCount));
			var textRect = new Rect(position.x, position.y, position.width, textAreaHeight);
			var buttonRect = new Rect(position.x, position.y + textAreaHeight, position.width,
				EditorGUIUtility.singleLineHeight);

			if (isEditing)
			{
				property.stringValue = EditorGUI.TextArea(textRect, property.stringValue, TextAreaStyle);
			}
			else
			{
				EditorGUI.LabelField(textRect, property.stringValue, LabelStyle);
			}

			if (GUI.Button(buttonRect, isEditing ? "Save" : "Edit"))
			{
				isEditing = !isEditing;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (isEditing)
			{
				int lineCount = GetLineCount(property.stringValue);
				float textAreaHeight =
					(EditorGUIUtility.singleLineHeight * (lineCount)); // Add 2 extra lines for padding
				return textAreaHeight + EditorGUIUtility.singleLineHeight; // Add space for the button
			}
			else
			{
				int lineCount = GetLineCount(property.stringValue);
				float textAreaHeight =
					(EditorGUIUtility.singleLineHeight * (lineCount)); // Add 2 extra lines for padding
				return textAreaHeight + EditorGUIUtility.singleLineHeight; // Add space for the button
			}
		}

		private int GetLineCount(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return 1; // At least one line for empty text
			}

			var style = isEditing ? TextAreaStyle : LabelStyle;
			
			float textWidth =
				EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 30; // Approximate width for text
			float textHeight = style.CalcHeight(new GUIContent(text), textWidth);

			int wrappedLineCount = Mathf.CeilToInt(textHeight / EditorGUIUtility.singleLineHeight);

			return wrappedLineCount;
		}
	}
}
