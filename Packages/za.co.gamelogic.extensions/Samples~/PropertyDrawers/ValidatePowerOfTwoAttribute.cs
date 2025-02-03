using Gamelogic.Extensions.Internal;
using Gamelogic.Extensions.Support;

namespace Gamelogic.Extensions.Samples
{
	/// <summary>
	/// Marks an integer field to indicate it should be a power of two.
	/// </summary>
	public class ValidatePowerOfTwoAttribute : ValidationAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidatePowerOfTwoAttribute"/> class.
		/// </summary>
		public ValidatePowerOfTwoAttribute()
		{
			Message = "Value must be a power of two.";
			Color = Branding.Aqua;
		}

#if UNITY_EDITOR
		/*	You need to shield these methods with UNITY_EDITOR to avoid compilation errors in a build.
			UnityEditor.SerializedProperty is not available at runtime, so these methods in the base
			class are similarly shielded.
		*/
		/// <summary>
		/// Returns whether the value is a power of two.
		/// </summary>
		/// <inheritdoc/>
		[EditorOnly]
		public override bool IsValid(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					return property.intValue > 0 && (property.intValue & (property.intValue - 1)) == 0;
				default:
					return true;
			}
		}
		
		/// <summary>
		/// Returns the greatest power of two smaller or equal to `value`.
		/// </summary>
		/// <returns>The greatest power of two smaller or equal to `value`.</returns>
		/// <inheritdoc/>
		protected override void Constrain(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.Integer:
					property.intValue = Constrain(property.intValue);
					break;
			}
		}
		
		private int Constrain(int intValue)
		{
			if (intValue <= 1)
			{
				return 1;
			}
				
			int constrainedValue = 1;
			while (constrainedValue <= intValue / 2)
			{
				constrainedValue *= 2;
			}
				
			return constrainedValue;
		}
#endif
	}
}
