// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Globalization;
using Gamelogic.Extensions.Internal;
using UnityEngine;
using UnityEditor;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// Property drawer for the label field attribute.
	/// </summary>
	/// <seealso cref="UnityEditor.PropertyDrawer" />
	[Version(2, 5, 0)]
	[CustomPropertyDrawer(typeof (LabelFieldAttribute))]
	public class LabelFieldPropertyDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var nameFieldAttribute = (LabelFieldAttribute) attribute;
			var nameProperty = property.FindPropertyRelative(nameFieldAttribute.LabelField);

			if (nameProperty != null)
			{
				SetName(label, nameProperty);
			}

			EditorGUI.PropertyField(position, property, label, true);
		}

		// ReSharper disable once CognitiveComplexity
		// There is no real way to simplify this switch statement
		private static void SetName(GUIContent label, SerializedProperty nameProperty)
		{
			string name = label.text;
			
			switch (nameProperty.propertyType)
			{
				case SerializedPropertyType.Integer:
					name = nameProperty.intValue.ToString();
					break;
				case SerializedPropertyType.Boolean:
					name = nameProperty.boolValue.ToString();
					break;
				case SerializedPropertyType.Float:
					name = nameProperty.floatValue.ToString(CultureInfo.InvariantCulture);
					break;
				case SerializedPropertyType.String:
					name = nameProperty.stringValue;
					break;
				case SerializedPropertyType.Color:
					name = nameProperty.colorValue.ToString();
					break;
				case SerializedPropertyType.ObjectReference:
					name = nameProperty.objectReferenceValue.name;
					break;
				case SerializedPropertyType.Enum:
					name = nameProperty.enumDisplayNames[nameProperty.enumValueIndex];
					break;
				case SerializedPropertyType.Vector2:
					name = nameProperty.vector2Value.ToString();
					break;
				case SerializedPropertyType.Vector3:
					name = nameProperty.vector3Value.ToString();
					break;
				case SerializedPropertyType.Vector4:
					name = nameProperty.vector4Value.ToString();
					break;
				case SerializedPropertyType.Rect:
					name = nameProperty.rectValue.ToString();
					break;
				case SerializedPropertyType.Bounds:
					name = nameProperty.boundsValue.ToString();
					break;
				case SerializedPropertyType.Quaternion:
					name = nameProperty.quaternionValue.ToString();
					break;
				case SerializedPropertyType.Gradient:
				case SerializedPropertyType.ArraySize:
				case SerializedPropertyType.Character:
				case SerializedPropertyType.AnimationCurve:
				case SerializedPropertyType.LayerMask:
				case SerializedPropertyType.Generic:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			label.text = name;
		}
	}
}
