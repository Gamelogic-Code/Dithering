// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Used to mark inspectable fields as read-only (that is, making them uneditable, even if they are visible).
	/// </summary>
	/// <seealso cref="UnityEngine.PropertyAttribute" />
	[Version(2, 5, 0)]
	[AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyAttribute : PropertyAttribute
	{
	}
}
