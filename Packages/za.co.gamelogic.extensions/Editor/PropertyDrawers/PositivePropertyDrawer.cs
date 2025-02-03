using System;
using UnityEditor;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// Property drawer for the PositivePropertyDrawer attribute.
	/// </summary>
	/// <seealso cref="ValidationPropertyDrawer"/>
	[Obsolete("Use " + nameof(ValidationPropertyDrawer) + " instead")]
	[CustomPropertyDrawer(typeof(PositiveAttribute), true)]
	public class PositivePropertyDrawer : ValidationPropertyDrawer
	{
	}
}
