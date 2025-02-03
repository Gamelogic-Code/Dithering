using System;
using UnityEditor;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// Property drawer for the NonNegativePropertyDrawer attribute.
	/// </summary>
	/// <seealso cref="ValidationPropertyDrawer"/>
	[Obsolete("Use " + nameof(ValidationPropertyDrawer) + " instead")]
	[CustomPropertyDrawer(typeof(NonNegativeAttribute), true)]
	public class NonNegativePropertyDrawer : ValidationPropertyDrawer
	{
	}
}
