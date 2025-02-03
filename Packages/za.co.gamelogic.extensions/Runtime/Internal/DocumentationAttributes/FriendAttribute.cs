// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;

namespace Gamelogic.Extensions.Internal
{
	/// <summary>
	/// Use to mark targets that are only exposed because communication 
	/// between classes is necessary to implement certain Unity features.
	/// Typically, when editor classes need private access to the classes
	/// they edit.
	/// </summary>
	[Version(2, 0, 0)]
	[AttributeUsage(AttributeTargets.All, Inherited = false)]
	public sealed class FriendAttribute : Attribute
	{
	}
}
