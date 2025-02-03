// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Mark numeric values that should always be non-negative.
	/// </summary>
	[Version(1, 2, 0)]
	[Obsolete("Use ValidateNonNegativeAttribute instead.")]
	public class NonNegativeAttribute : ValidateNotNegativeAttribute
	{
	}
}
