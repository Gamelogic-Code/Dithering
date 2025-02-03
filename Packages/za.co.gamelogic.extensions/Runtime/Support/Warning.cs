// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Support
{
	/// <summary>
	/// This class contains strings used to justify suppressions of warnings. 
	/// </summary>
	[Version(3, 0, 0)]
	[Obsolete("Use " + nameof(SuppressReason) + " instead.")]
	public static class Warning
	{
		/// <inheritdoc cref="SuppressReason.NameHasMath"/>
		public const string NameHasMath = SuppressReason.NameHasMath;
		
		/// <inheritdoc cref="SuppressReason.IntentionalFloatingPointComparison"/>
		public const string IntentionalFloatingPointComparison = SuppressReason.IntentionalFloatingPointComparison;
		
		/// <inheritdoc cref="SuppressReason.NeedUnitTests"/>
		public const string NeedUnitTests = SuppressReason.NeedUnitTests;
		
		/// <inheritdoc cref="SuppressReason.EditorInternal"/>
		public const string EditorInternal = SuppressReason.EditorInternal;
	}
}
