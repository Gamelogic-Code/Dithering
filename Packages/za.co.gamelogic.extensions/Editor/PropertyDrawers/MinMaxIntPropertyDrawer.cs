using System.Reflection;
using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A property drawer for the <see cref="MinMaxInt"/> class.
	/// </summary>
	[CustomPropertyDrawer(typeof(MinMaxInt))]
	[Version(4, 2, 0)]
	public class MinMaxIntPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);
			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var minProp = property.FindPropertyRelative("min");
			var maxProp = property.FindPropertyRelative("max");

			int minValue = minProp.intValue;
			int maxValue = maxProp.intValue;

			float rangeMin = 0;
			float rangeMax = int.MaxValue;
				
			// Extract the range from the attribute
			var rangeAttribute = fieldInfo.GetCustomAttribute<MinMaxRangeAttribute>();

			if (rangeAttribute != null)
			{
				(rangeMin, rangeMax) = rangeAttribute.GetRange();
			}

			float minFloatValue = minValue;
			float maxFloatValue = maxValue;
			
			// Use the extracted range for the MinMaxSlider
			EditorGUI.MinMaxSlider(position, ref minFloatValue, ref maxFloatValue, rangeMin, rangeMax);

			if (GUI.changed)
			{
				minProp.intValue = Mathf.RoundToInt(minFloatValue);
				maxProp.intValue = Mathf.RoundToInt(maxFloatValue);
			}

			EditorGUI.EndProperty();
		}
	}
}
