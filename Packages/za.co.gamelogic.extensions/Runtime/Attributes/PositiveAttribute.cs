// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Mark fields that should always be positive with this attribute.
	/// </summary>
	[Obsolete("Use ValidatePositiveAttribute instead.")]
	[Version(1, 2, 0)]
	public class PositiveAttribute : ValidatePositiveAttribute
	{
	}
}
