using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions.Samples
{
	/// <summary>
	/// Attribute used to mark vector fields whose magnitude should be no larger than a specified maximum.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ValidateMagnitudeMaxAttribute : ValidationAttribute
	{
		private readonly float maxMagnitude;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateMagnitudeMaxAttribute"/> class.
		/// </summary>
		/// <param name="maxMagnitude">The maximum magnitude of the vector field.</param>
		public ValidateMagnitudeMaxAttribute(float maxMagnitude)
		{
			Message = $"Vector magnitude must be no larger than {maxMagnitude}.";
			this.maxMagnitude = maxMagnitude;
		}

#if UNITY_EDITOR 
		/*	You need to shield these methods with UNITY_EDITOR to avoid compilation errors in a build.
			UnityEditor.SerializedProperty is not available at runtime, so these methods in the base 
			class are similarly shielded. 
		*/
		
		/// <summary>
		/// Checks if the property magnitude is below the specified maximum.
		/// </summary>
		/// <param name="property">The property to check.</param>
		/// <returns><see langword="true"/> if the property's magnitude is no larger than the specified maximum; otherwise,
		/// <see langword="false"/>.</returns>
		[EditorOnly]
		public override bool IsValid(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					return Mathf.Abs(property.intValue) <= maxMagnitude;
				case UnityEditor.SerializedPropertyType.Float:
					return Mathf.Abs(property.floatValue) <= maxMagnitude;
				case UnityEditor.SerializedPropertyType.Vector2:
					return property.vector2Value.magnitude <= maxMagnitude;
				case UnityEditor.SerializedPropertyType.Vector3:
					return property.vector3Value.magnitude <= maxMagnitude;
				case UnityEditor.SerializedPropertyType.Vector4:
					return property.vector4Value.magnitude <= maxMagnitude;
				default:
					return true;
			}
		}
		
		/// <summary>
		/// Clamps the property to the specified maximum.
		/// </summary>
		/// <inheritdoc />
		protected override void Constrain(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					property.intValue = Mathf.Clamp(property.intValue, 0, (int)maxMagnitude);
					break;
				case UnityEditor.SerializedPropertyType.Float:
					property.floatValue = Mathf.Clamp(property.floatValue, 0, maxMagnitude);
					break;
				case UnityEditor.SerializedPropertyType.Vector2:
					property.vector2Value = Vector2.ClampMagnitude(property.vector2Value, maxMagnitude);
					break;
				case UnityEditor.SerializedPropertyType.Vector3:
					property.vector3Value = Vector3.ClampMagnitude(property.vector3Value, maxMagnitude);
					break;
				case UnityEditor.SerializedPropertyType.Vector4:
					property.vector4Value = property.vector4Value.ClampMagnitude(maxMagnitude);
					break;
			}
		}
#endif
	}
}
