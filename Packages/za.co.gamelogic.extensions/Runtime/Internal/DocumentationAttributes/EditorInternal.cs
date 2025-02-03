using System;

namespace Gamelogic.Extensions.Internal
{
	/// <summary>
	/// Use to mark targets that are only supposed to be used by internal editor code.
	/// </summary>
	[Version(2, 4, 0)]
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class EditorInternal : Attribute
	{
	}
}
