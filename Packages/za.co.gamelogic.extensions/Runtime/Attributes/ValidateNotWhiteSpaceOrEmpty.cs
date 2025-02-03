using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Marks a string field that should not be empty or whitespace.
	/// </summary>
	[Version(4, 3, 0)]
	public class ValidateNotWhiteSpaceOrEmpty : ValidateMatchRegularExpressionAttribute
	{
		public ValidateNotWhiteSpaceOrEmpty() : base(@"\S")
		{
			Message = "Value cannot be empty or whitespace.";
		}
	}
}
