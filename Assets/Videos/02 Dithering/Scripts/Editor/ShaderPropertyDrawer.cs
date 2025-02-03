using UnityEngine;
using UnityEditor;

namespace Gamelogic.Experimental.Editor
{
	/// <summary>
	/// The custom property drawer for the <see cref="ShaderProperty"/> class.
	/// </summary>
	[CustomPropertyDrawer(typeof(ShaderProperty))]
	public class ShaderPropertyDrawer : PropertyDrawer
	{
		// ReSharper disable once StringLiteralTypo
		private static readonly Texture2D WarningIcon =
			EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Rect? tooltipRect = null;
			var material = GetMaterial(property);

			using var nameProp = property.FindPropertyRelative("name");
			using var typeProp = property.FindPropertyRelative("shaderPropertyType");
			string propertyName = nameProp.stringValue;
			var type = (ShaderPropertyType)typeProp.enumValueIndex;

			const float warningIconSize = 16f;
			const float maxTypeWidth = 100f;
			float remainingWidth = position.width - maxTypeWidth - warningIconSize - 5;
			float halfRemainingWidth = remainingWidth / 2f;

			var warningRect = new Rect(position.x, position.y + (position.height - warningIconSize) / 2, warningIconSize,
				warningIconSize);
			var nameRect = new Rect(position.x + warningIconSize + 5, position.y, halfRemainingWidth - warningIconSize,
				position.height);
			var typeRect = new Rect(nameRect.xMax, position.y, Mathf.Min(maxTypeWidth, position.width / 3f),
				position.height);
			var valueRect = new Rect(typeRect.xMax, position.y, halfRemainingWidth, position.height);

			bool isKeyword = type == ShaderPropertyType.Keyword || type == ShaderPropertyType.KeywordSet;
			
			// Arrays cannot be checked
			if (material != null && !isKeyword && !IsArrayType(type) && !material.HasProperty(propertyName))
			{
				GUI.DrawTexture(warningRect, WarningIcon);
				tooltipRect = warningRect;
			}

			EditorGUI.PropertyField(nameRect, nameProp, GUIContent.none);
			EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);

			DrawPropertyValueField(valueRect, property, type);

			if (tooltipRect.HasValue)
			{
				Tooltip(tooltipRect.Value, "This property is not used by the shader.");
			}
		}

		private void DrawPropertyValueField(Rect valueRect, SerializedProperty property, ShaderPropertyType type)
		{
			switch (type)
			{
				case ShaderPropertyType.Int:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("intValue"), GUIContent.none);
					break;
				case ShaderPropertyType.Float:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("floatValue"), GUIContent.none);
					break;
				case ShaderPropertyType.Texture:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("textureValue"), GUIContent.none);
					break;
				case ShaderPropertyType.Color:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("colorValue"), GUIContent.none);
					break;
				case ShaderPropertyType.Vector:
				case ShaderPropertyType.ScreenTextureSize:
				var vectorProp = property.FindPropertyRelative("vectorValue");
				DrawVector4(valueRect, vectorProp);
				break;
				case ShaderPropertyType.Keyword:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("keywordValue"), GUIContent.none);
					break;
				case ShaderPropertyType.KeywordSet:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("keywordSetValue.keywords"), true);
					break;
				case ShaderPropertyType.FloatArray:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("floatArrayValue"), true);
					break;
				case ShaderPropertyType.MatrixArray:
					EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("matrixArrayValue"), true);
					break;
			}
		}

		private static void DrawVector4(Rect valueRect, SerializedProperty vectorProp)
		{
			var newValue = EditorGUI.Vector4Field(valueRect, GUIContent.none, vectorProp.vector4Value);
			vectorProp.vector4Value = newValue;
		}
		
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			using var typeProperty = property.FindPropertyRelative("shaderPropertyType");
			var type = (ShaderPropertyType)typeProperty.enumValueIndex;

			return type switch
			{
				ShaderPropertyType.KeywordSet => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("keywordSetValue.keywords"), true),
				ShaderPropertyType.FloatArray => EditorGUI.GetPropertyHeight(
					property.FindPropertyRelative("floatArrayValue"), true),
				ShaderPropertyType.MatrixArray => EditorGUI.GetPropertyHeight(property.FindPropertyRelative("matrixArrayValue"), true),
				_ => EditorGUIUtility.singleLineHeight
			};
		}

		private static Material GetMaterial(SerializedProperty property)
		{
			var serializedObject = property.serializedObject;
			var postProcess = serializedObject.targetObject as PostProcess;
			return postProcess != null ? postProcess.ScreenMaterial : null;
		}
		
		private static void Tooltip(Rect rect, string message)
		{
			if (Event.current.type != EventType.Repaint || !rect.Contains(Event.current.mousePosition))
			{
				return;
			}

			bool isDarkTheme = EditorGUIUtility.isProSkin;
			var warningTextColor = isDarkTheme ? new Color(1f, .8f, 0.2f) : new Color(1f, 0.8f, 0f); 

			var style = new GUIStyle(EditorStyles.helpBox)
			{
				wordWrap = true,
				fontSize = 12,
				padding = new RectOffset(5, 5, 5, 5),
				normal = { textColor = warningTextColor } 
			};

			var textSize = style.CalcSize(new GUIContent(message));
			float tooltipWidth = Mathf.Clamp(textSize.x, 150, 400);
			float tooltipHeight = style.CalcHeight(new GUIContent(message), tooltipWidth);

			var mousePosition = Event.current.mousePosition;
			var tooltipRect = new Rect(mousePosition.x + 15, mousePosition.y + 10, tooltipWidth, tooltipHeight);

			GUI.Label(tooltipRect, message, style);
		}

		private static bool IsArrayType(ShaderPropertyType type) 
			=> type is ShaderPropertyType.FloatArray or ShaderPropertyType.MatrixArray;
	}
}
