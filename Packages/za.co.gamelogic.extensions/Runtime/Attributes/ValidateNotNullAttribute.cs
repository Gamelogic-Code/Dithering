using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Attribute used to mark fields that should not be null.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Version(4, 3, 0)]
	public class ValidateNotNullAttribute : ValidationAttribute
	{
		public ValidateNotNullAttribute()
		{
			ForceValue = false;
			Message = "Value cannot be null.";
		}

#if UNITY_EDITOR
		/// <summary>
		/// Checks if the property is not null.
		/// </summary>
		/// <inheritdoc/>
		[EditorOnly]
		public override bool IsValid(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.ObjectReference:
					return property.objectReferenceValue != null;
				case UnityEditor.SerializedPropertyType.String:
					return property.stringValue != null;
				case UnityEditor.SerializedPropertyType.ExposedReference:
					return property.exposedReferenceValue != null;
				default:
					return true;
			}
		}
#endif
	}
}
