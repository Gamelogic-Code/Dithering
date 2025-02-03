// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A property drawer for the <see cref="MinMaxFloat"/> class.
	/// </summary>
	[CustomPropertyDrawer(typeof(MinMaxFloat))]
	public class MinMaxFloatPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var minProp = property.FindPropertyRelative("min");
			var maxProp = property.FindPropertyRelative("max");

			float minValue = minProp.floatValue;
			float maxValue = maxProp.floatValue;

			float rangeMin = 0f;
			float rangeMax = 1f;
				
			// Extract the range from the attribute
			var rangeAttribute = fieldInfo.GetCustomAttribute<MinMaxRangeAttribute>();

			if (rangeAttribute != null)
			{
				(rangeMin, rangeMax) = rangeAttribute.GetRange();
			}
			
			// Use the extracted range for the MinMaxSlider
			EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);

			if (GUI.changed)
			{
				minProp.floatValue = minValue;
				maxProp.floatValue = maxValue;
			}

			EditorGUI.EndProperty();
		}
	}
}
