using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Attribute used to mark fields that should not be negative.
	/// </summary>
	/// <remarks>Can be applied to <see cref="int"/> and <see cref="float"/> fields.</remarks>
	[AttributeUsage(AttributeTargets.Field)]
	[Version(4, 3, 0)]
	public class ValidateNotNegativeAttribute : ValidationAttribute
	{
#if UNITY_EDITOR
		/// <summary>
		/// Checks if the property is not negative.
		/// </summary>
		/// <inheritdoc cref="ValidationAttribute.IsValid"/>
		[EditorOnly]
		public override bool IsValid(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					return property.intValue >= 0;
				case UnityEditor.SerializedPropertyType.Float:
					return property.floatValue >= 0;
				default:
					return true;
			}
		}

		/// <summary>
		/// Constraints the value to be non-negative.
		/// </summary>
		/// <param name="property">The value to constrain.</param>
		/// <returns>The constrained value. Negative values are constrained to 0.</returns>
		[EditorOnly]
		protected override void Constrain(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					property.intValue = Mathf.Max(0, property.intValue);
					break;
				case UnityEditor.SerializedPropertyType.Float:
					property.floatValue = Mathf.Max(0, property.floatValue);
					break;
			}
		}
#endif
	}
}
