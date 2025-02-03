using UnityEditor;
using UnityEngine;

namespace Gamelogic.Experimental.Editor
{
	/// <summary>
	/// The custom property drawer for the <see cref="MatrixArray"/> class.
	/// </summary>
	[CustomPropertyDrawer(typeof(MatrixArray))]
	public class MatrixArrayDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			using var dimensionsProp = property.FindPropertyRelative("dimensions");
			using var valuesProp = property.FindPropertyRelative("values");

			// Draw dimensions field
			Rect dimensionsRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.PropertyField(dimensionsRect, dimensionsProp, GUIContent.none);

			// Ensure values array size matches dimensions
			Vector2Int dimensions = dimensionsProp.vector2IntValue;
			int requiredSize = dimensions.x * dimensions.y;
			if (valuesProp.arraySize != requiredSize)
			{
				valuesProp.arraySize = requiredSize;
			}

			// Draw the grid of float values
			float cellWidth = position.width / Mathf.Max(1, dimensions.x);
			Rect gridRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 5, position.width, EditorGUIUtility.singleLineHeight);

			for (int row = 0; row < dimensions.y; row++)
			{
				for (int col = 0; col < dimensions.x; col++)
				{
					int index = row * dimensions.x + col;
					Rect cellRect = new Rect(position.x + col * cellWidth, gridRect.y + row * EditorGUIUtility.singleLineHeight, cellWidth - 2, EditorGUIUtility.singleLineHeight);
					SerializedProperty elementProp = valuesProp.GetArrayElementAtIndex(index);
					EditorGUI.PropertyField(cellRect, elementProp, GUIContent.none);
				}
			}

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			SerializedProperty dimensionsProp = property.FindPropertyRelative("dimensions");
			Vector2Int dimensions = dimensionsProp.vector2IntValue;
			
			// Calculate the height: dimensions label + grid rows
			float gridHeight = dimensions.y * EditorGUIUtility.singleLineHeight;
			return EditorGUIUtility.singleLineHeight + gridHeight + 5;
		}
	}
}
