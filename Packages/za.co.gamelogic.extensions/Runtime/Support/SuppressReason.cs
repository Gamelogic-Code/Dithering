using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Support
{
	[Version(4, 3, 0)]
	public class SuppressReason
	{
		/// <summary>
		/// Used to justify names such as `Matrix4x4`.
		/// </summary>
		public const string NameHasMath = "Name has math parts.";
		
		/// <summary>
		/// Used to justify comparing floats with `==`.
		/// </summary>
		public const string IntentionalFloatingPointComparison = "Intnetional floating point comparison.";
		
		/// <summary>
		/// Used to (temporarily) justify very complex methods. 
		/// </summary>
		public const string NeedUnitTests = "Need unit tests before we can refactor.";
		
		/// <summary>
		/// This is for things that are exposed for the editor, and have non-conforming names such as `__message` to
		/// discourage use.
		/// </summary>
		public const string EditorInternal = "Editor internal, do not use.";

		/// <summary>
		/// This is for marking methods that contains giant switch statements that cannot be reasonably be simplified.
		/// </summary>
		/// <remarks>
		/// In some cases giant switch statements can be simplified by the use of dictionaries, in which case that
		/// refactoring should be done and this justification cannot be used. However, if this would result in
		/// the dictionary containing a bunch of lambda expressions, it is better to leave the switch statement, and
		/// use this justification to suppress the warning if necessary.
		/// </remarks>
		public const string SwitchCannotBeSimplified = "Switch cannot be simplified.";
	}
}
