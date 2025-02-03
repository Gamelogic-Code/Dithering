// Copyright Gamelogic (c) http://www.gamelogic.co.za

using Gamelogic.Extensions.Internal;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// Wraps a SerializedProperty, and provides additional functions, such as
	/// tooltips and a more powerful Find method.
	/// </summary>
	[Version(1, 2, 0)]
	public class GLSerializedProperty
	{
		public SerializedProperty SerializedProperty { get; set; }

		public string CustomTooltip { get; set; }

		public SerializedPropertyType PropertyType => SerializedProperty.propertyType;

		public Object ObjectReferenceValue
		{
			get => SerializedProperty.objectReferenceValue;
			set => SerializedProperty.objectReferenceValue = value;
		}

		
		public int EnumValueIndex
		{
			get => SerializedProperty.enumValueIndex;
			set => SerializedProperty.enumValueIndex = value;
		}

		public string[] EnumNames => SerializedProperty.enumNames;

		public bool BoolValue
		{
			get => SerializedProperty.boolValue;
			set => SerializedProperty.boolValue = value;
		}

		public int IntValue
		{
			get => SerializedProperty.intValue;
			set => SerializedProperty.intValue = value;
		}

		public float FloatValue
		{
			get => SerializedProperty.floatValue;
			set => SerializedProperty.floatValue = value;
		}

		public string StringValue
		{
			get => SerializedProperty.stringValue;
			set => SerializedProperty.stringValue = value;
		}

		public GLSerializedProperty FindPropertyRelative(string name)
		{
			var property = SerializedProperty.FindPropertyRelative(name);

			return new GLSerializedProperty
			{
				SerializedProperty = property
			};
		}
	}
}
