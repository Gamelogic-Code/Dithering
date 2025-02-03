using System;
using UnityEditor;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// Property drawer for the WarningIfNull attribute.
	/// </summary>
	/// <seealso cref="ValidationPropertyDrawer"/>
	[Obsolete("Use " + nameof(ValidationPropertyDrawer) + " instead")]
	[CustomPropertyDrawer(typeof(WarningIfNullAttribute), true)]
	public class WarningIfNullPropertyDrawer : ValidationPropertyDrawer
	{
	}
}
