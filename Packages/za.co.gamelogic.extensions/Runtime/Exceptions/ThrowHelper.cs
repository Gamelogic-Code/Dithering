using System;
using Gamelogic.Extensions.Internal;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Support;
using JetBrains.Annotations;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Class that provides helper methods for throwing exceptions.
	/// </summary>
	[Version(4, 0, 0)]
	public static class ThrowHelper
	{
		internal static readonly Exception UnreachableCodeException =
			new InvalidOperationException("Unreachable code.");
		
		internal static readonly InvalidOperationException CollectionEmptyException 
			= new InvalidOperationException(ErrorMessages.ContainerEmpty);
		
		internal static readonly InvalidOperationException CollectionFullException 
			= new InvalidOperationException(ErrorMessages.ContainerFull);
		
		/// <summary>
		/// Throws a NullReferenceException if the given argument is null.
		/// </summary>
		/// <param name="argument">An argument to check.</param>
		/// <param name="argName">The name of the argument.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void ThrowIfNull(this object argument, string argName)
		{
			if (argument == null)
			{
				throw new ArgumentNullException(argName);
			}
		}

		/// <summary>
		/// Throws a ArgumentOutOfRange exception if the integer is negative.
		/// </summary>
		/// <param name="argument">The integer to check.</param>
		/// <param name="argName">The name of the argument.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static void ThrowIfNegative(this int argument, string argName)
		{
			if (argument < 0)
			{
				throw new ArgumentOutOfRangeException(argName, argument, ErrorMessages.ArgumentCannotBeNegative);
			}
		}

		/// <summary>
		/// Throws a ArgumentOutOfRange exception if the float is negative.
		/// </summary>
		/// <param name="argument">The float to check.</param>
		/// <param name="argName">The name of the argument.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static void ThrowIfNegative(this float argument, string argName)
		{
			if (argument < 0)
			{
				throw new ArgumentOutOfRangeException(argName, argument, ErrorMessages.ArgumentCannotBeNegative);
			}
		}
		
		[AssertionMethod]
		internal static int ThrowIfNotPositive(this int argument, string argName = null)
		{
			if (argument <= 0)
			{
				throw new ArgumentOutOfRangeException(argName);
			}

			return argument;
		}

		/// <summary>
		/// Throws a ArgumentOutOfRange exception if the integer is out of range.
		/// </summary>
		/// <param name="argument">The integer to check.</param>
		/// <param name="minInclusive">The minimum value of the range, included.</param>
		/// <param name="maxExclusive">The maximum value of the range, not included.</param>
		/// <param name="argName">The name of the argument.</param>
		public static void ThrowIfOutOfRange(this int argument, int minInclusive, int maxExclusive, string argName)
		{
			if (argument < minInclusive || argument >= maxExclusive)
			{
				throw new ArgumentOutOfRangeException(argName, argument, GetMessage());
			}

			return;

			string GetMessage() => string.Format(ErrorMessages.ArgumentMustBeInRange, minInclusive, maxExclusive);
		}
		
		[DoesNotReturn]
		internal static void ThrowContainerEmpty() 
			=> throw CollectionEmptyException;

		[DoesNotReturn]
		internal static void ThrowContainerFull() 
			=> throw CollectionFullException;

		internal static T[] ThrowIfEmpty<T>(this T[] list, string listArgName = null)
		{
			if (list.Length == 0)
			{
				ThrowContainerEmpty();
			}

			return list;
		}

		internal static IList<T> ThrowIfEmpty<T>(this IList<T> list, string listArgName = null)
		{
			if (!list.Any())
			{
				ThrowContainerEmpty();
			}

			return list;
		}

		internal static T ThrowIfNull<T>(
			[NotNull, NoEnumeration] this T obj, 
			string objArgName = null)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(objArgName);
			}

			return obj;
		}
	
		internal static string ThrowIfNullOrEmpty(
			[NotNull, NoEnumeration] this string obj, 
			string objArgName = null)
		{
			if (string.IsNullOrEmpty(obj))
			{
				throw new ArgumentNullException(objArgName);
			}

			return obj;
		}
	
		internal static string ThrowIfNullOrWhiteSpace(
			[NotNull, NoEnumeration] this string obj, 
			string objArgName = null)
		{
			if (string.IsNullOrWhiteSpace(obj))
			{
				throw new ArgumentNullException(objArgName);
			}

			return obj;
		}

		[Version(4, 2, 0)]
		internal static Type ThrowIfNotAssignableFrom<T>(this Type targetType)
		{
			if (!targetType.IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(string.Format(ErrorMessages.TypeNotAssignableFromType, targetType, typeof(T)));
			}

			return targetType;
		}
		
		[Version(4, 2, 0)]
		internal static Type ThrowIfNotAssignableFrom(this Type targetType, Type sourceType)
		{
			if (!targetType.IsAssignableFrom(sourceType))
			{
				throw new ArgumentException(string.Format(ErrorMessages.TypeNotAssignableFromType, targetType, sourceType));
			}

			return targetType;
		}
	}
}
