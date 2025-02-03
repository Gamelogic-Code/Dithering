using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// Property drawer for attributes that derive from <see cref="ValidationAttribute"/>.
	/// </summary>
	/// <remarks>
	/// This is one of the key classes to support validating and constraining values in the Unity editor.
	///
	/// See [Property Drawers](../content/PropertyDrawers.md) for more details.
	/// 
	/// </remarks>
	[CustomPropertyDrawer(typeof(ValidationAttribute), true)]
	[Version(4, 3, 0)]
	public class ValidationPropertyDrawer : PropertyDrawer
	{
		private ValidationAttribute Attribute => (ValidationAttribute)attribute;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (!ShouldDrawWarning(property))
			{
				return base.GetPropertyHeight(property, label);
			}

			var guiContent = new GUIContent(Attribute.Message);
			bool oldWordWrap = EditorStyles.miniLabel.wordWrap;

			EditorStyles.miniLabel.wordWrap = true;

			float height =
				EditorStyles.miniLabel.CalcHeight(guiContent, Screen.width - 19) +
				EditorGUI.GetPropertyHeight(property, label, true);
			EditorStyles.miniLabel.wordWrap = oldWordWrap;

			return height;

		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!ShouldDrawWarning(property))
			{
				DrawAndCheckField(position, property);
				
				return;
			}

			var guiContent = new GUIContent(Attribute.Message);
			bool oldWordWrap = EditorStyles.miniLabel.wordWrap;
			EditorStyles.miniLabel.wordWrap = true;

			var color = GUI.color;
			var contentColor = GUI.contentColor;
			var backgroundColor = GUI.backgroundColor;

			if (EditorGUIUtility.isProSkin)
			{			
				GUI.color = Attribute.Color;
			}
			else
			{
				EditorGUI.DrawRect(position, Attribute.Color);
				GUI.contentColor = Color.black;
				GUI.backgroundColor = Attribute.Color;
			}

			float graphHeight = EditorGUI.GetPropertyHeight(property, label, true);
			float labelHeight = EditorStyles.miniLabel.CalcHeight(guiContent, Screen.width - 19);
			position.height = labelHeight;
			EditorGUI.LabelField(position, Attribute.Message, EditorStyles.miniLabel);
					
			position.y += labelHeight;
			position.height = graphHeight;

			EditorGUI.PropertyField(position, property);
			EditorStyles.miniLabel.wordWrap = oldWordWrap;

			if (EditorGUIUtility.isProSkin)
			{
				GUI.color = color;
			}
			else
			{
				GUI.contentColor = contentColor;
				GUI.backgroundColor = backgroundColor;
			}
		}

		private bool ShouldWarnInConsole(SerializedProperty property) 
			=> Attribute.WarnInConsole && !Attribute.IsValid(property);
		
		private bool ShouldDrawWarning(SerializedProperty property) 
			=> !Attribute.ForceValue && Attribute.WarnInInspector && !Attribute.IsValid(property);

		private void DrawAndCheckField(Rect position, SerializedProperty property)
		{
			EditorGUI.PropertyField(position, property);
			
			if(ShouldForce(property))
			{
				Attribute.ConstrainAndVerify(property);
			}
			else if (ShouldWarnInConsole(property))
			{
				Debug.LogWarning(Attribute.Message);
			}
		}

		private bool ShouldForce(SerializedProperty property) 
			=> Attribute.ForceValue && !Attribute.IsValid(property);
	}
}