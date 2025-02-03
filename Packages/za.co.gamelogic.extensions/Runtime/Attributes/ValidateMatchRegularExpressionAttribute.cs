using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Marks a string field to indicate it should match a regular expression.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	[Version(4, 3, 0)]
	public class ValidateMatchRegularExpressionAttribute : ValidationAttribute
	{
		private readonly string pattern;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidateMatchRegularExpressionAttribute"/> class.
		/// </summary>
		/// <param name="pattern">The regular expression pattern to match.</param>
		public ValidateMatchRegularExpressionAttribute(string pattern)
		{
			Message = $"Value must match the regular expression {pattern}.";
			this.pattern = pattern;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Checks if the property matches the regular expression pattern.
		/// </summary>
		/// <inheritdoc/>
		[EditorOnly]
		public override bool IsValid(UnityEditor.SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case UnityEditor.SerializedPropertyType.String:
					return property.stringValue != null && System.Text.RegularExpressions.Regex.IsMatch(property.stringValue, pattern);
				default:
					return true;
			}
		}
#endif
	}
}
