using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Attribute used to mark fields that should have a specific range.
	/// </summary>
	/// <remarks>
	/// Works fields of type <see cref="int"/> and <see cref="float"/>.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field)]
	[Version(4, 3, 0)]
	public class ValidateRangeAttribute : ValidationAttribute
	{
		private readonly float min;
		private readonly float max;

		public ValidateRangeAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
		
#if UNITY_EDITOR
		/// <summary>
		/// Checks if the property is within the specified range.
		/// </summary>
		/// <inheritdoc/>
		[EditorOnly]
		public override bool IsValid(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					return IsInRange(property.intValue);
				case UnityEditor.SerializedPropertyType.Float:
					return IsInRange(property.floatValue);
				default:
					return true;
			}
		}

		/// <summary>
		/// Constrains the property to be within the specified range.
		/// </summary>
		/// <remarks>If the property is outside the range, it is clamped to the nearest
		/// bound.</remarks>
		/// <inheritdoc/>
		[EditorOnly]
		protected override void Constrain(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					property.intValue = Clamp(property.intValue);
					break;
				case UnityEditor.SerializedPropertyType.Float:
					property.floatValue = Clamp(property.floatValue);
					break;
			}
		}
#endif
		
		private bool IsInRange(float value) => value >= min && value <= max;
		
		private float Clamp(float value) => Mathf.Clamp(value, min, max);
		
		private int Clamp(int value) => Mathf.Clamp(value, Mathf.CeilToInt(min), Mathf.FloorToInt(max));
	}
}
