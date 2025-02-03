using System;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Represents a generic buffer interface that only supports reading capabilities.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IReadOnlyBuffer<out T> : IEnumerable<T>
	{
		/// <summary>
		/// Gets the maximum number of elements the buffer can hold.
		/// </summary>
		int Capacity { get; }

		/// <summary>
		/// Gets the current number of elements in the buffer.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets the first element in the buffer.
		/// </summary>
		T First { get; }

		/// <summary>
		/// Gets a value indicating whether the buffer is full.
		/// </summary>
		bool IsFull { get; }

		/// <summary>
		/// Gets the last element in the buffer.
		/// </summary>
		T Last { get; }
		
		/// <summary>
		/// Gets the element at the specified index.
		/// </summary>
		T this[int index] { get; }
	}
	
	/// <summary>
	/// Represents a generic buffer interface.
	/// </summary>
	/// <typeparam name="T">The type of elements in the buffer.</typeparam>
	[Version(4,1, 0)]
	public interface IBuffer<T> : IReadOnlyBuffer<T>
	{
		/// <summary>
		/// Clears all items from the buffer.
		/// </summary>
		void Clear();

		/// <summary>
		/// Inserts an item into the buffer.
		/// </summary>
		/// <param name="item">The item to insert into the buffer.</param>
		void Insert(T item);
	}
	
	/// <summary>
	/// A buffer that can change its capacity.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IResizeableBuffer<T> : IBuffer<T>
	{
		/// <summary>
		/// Increases the capacity of this <see cref="IResizeableBuffer{T}"/> by the specified increment.
		/// </summary>
		/// <param name="increment">How much to increase the capacity by.</param>
		/// <exception cref="ArgumentOutOfRangeException">The increment is negative.</exception>
		void IncreaseCapacity(int increment);
		
		/// <summary>
		/// Decreases the capacity of this <see cref="IResizeableBuffer{T}"/> by the specified decrement (up to the current capacity).
		/// </summary>
		/// <param name="decrement">How much to decrease the capacity by.</param>
		/// <returns>The actual amount by which the capacity was decreased.</returns>
		/// <exception cref="ArgumentOutOfRangeException">The decrement is negative.</exception>
		/// <remarks>If the capacity is lower than the current <see cref="IReadOnlyBuffer{T}.Count"/>, elements are removed from
		/// the buffer until the capacity is reached.
		/// </remarks>
		int DecreaseCapacity(int decrement);
	}
}
