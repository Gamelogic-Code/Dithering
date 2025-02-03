using System;

namespace Gamelogic.Extensions.Internal
{
#if UNITY_EDITOR
	/* Design note: We make this class also editor only, to ensure it is not used for non-editor code.
	*/

	/// <summary>
	/// Used to mark symbols defined in non-editor projects that are nevertheless only used in editor code.
	/// </summary>
	/// <remarks>
	/// It is sometimes necessary to define editor code on non-editor projects, for example
	/// for run-time attributes that have editor-only validation defined, such as <see cref="ValidationAttribute"/>
	/// and its subclasses. This attribute is used to mark this situation for documentation purposes. 
	/// </remarks>
	[Version(4, 3, 0)]
	[AttributeUsage(AttributeTargets.All)]
	[EditorOnly]
	public sealed class EditorOnly : Attribute
	{
	}
#endif
}
