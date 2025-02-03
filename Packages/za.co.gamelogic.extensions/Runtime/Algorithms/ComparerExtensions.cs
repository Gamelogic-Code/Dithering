using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	[Version(3, 0, 0)]
	public static class ComparerExtensions
	{
		public static bool Less<T>(this IComparer<T> comparer, T a, T b) => comparer.Compare(a, b) < 0;
	}
}
