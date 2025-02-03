using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Attribute used to mark fields that should be positive.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Version(4, 3, 0)]
	public class ValidatePositiveAttribute : ValidationAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidatePositiveAttribute"/> class.
		/// </summary>
		public ValidatePositiveAttribute()
		{
			Message = "Value must be positive.";
		}

#if UNITY_EDITOR
		/// <summary>
		/// Checks if the property is positive.
		/// </summary>
		/// <inheritdoc/>
		[EditorOnly]
		public override bool IsValid(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					return property.intValue > 0;
				case UnityEditor.SerializedPropertyType.Float:
					return property.floatValue > 0;
				default:
					return true;
			}
		}

		/// <summary>
		/// Constraints the property to be positive.
		/// </summary>
		/// <remarks> If the property is less than or equal to 0, it is constrained to 1 for integers
		/// and 0.1 for floats.
		/// </remarks>
		[EditorOnly]
		protected override void Constrain(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					property.intValue = Mathf.Max(1, property.intValue);
					break;
				case UnityEditor.SerializedPropertyType.Float:
					property.floatValue = Mathf.Max(0.1f, property.floatValue);
					break;
			}
		}
#endif
	}
}
