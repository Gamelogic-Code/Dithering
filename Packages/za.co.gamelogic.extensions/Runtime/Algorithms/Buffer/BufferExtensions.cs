using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Provides extension methods for buffers.
	/// </summary>
	[Version(4, 1, 0)]
	public static class BufferExtensions
	{
		/// <summary>
		/// Sets the capacity of the buffer to the specified value.
		/// </summary>
		/// <param name="buffer">The buffer to set the capacity of.</param>
		/// <param name="newCapacity">The new capacity of the buffer.</param>
		/// <typeparam name="T">The type of elements in the buffer.</typeparam>
		/// <exception cref="ArgumentOutOfRangeException">The new capacity is negative.</exception>
		public static void SetCapacity<T>(this IResizeableBuffer<T> buffer, int newCapacity)
		{
			newCapacity.ThrowIfNegative(nameof(newCapacity));
		
			if (newCapacity < buffer.Capacity)
			{
				buffer.DecreaseCapacity(buffer.Capacity - newCapacity);
			}
			else
			{
				buffer.IncreaseCapacity(newCapacity - buffer.Capacity);
			}
		}
	}
}
