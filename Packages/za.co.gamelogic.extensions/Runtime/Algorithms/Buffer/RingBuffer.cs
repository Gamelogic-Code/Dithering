using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Represents buffer with a fixed capacity.
	/// </summary>
	/// <typeparam name="T">The type of elements this buffer can hold.</typeparam>
	/// <remarks>
	/// See <see cref="IBuffer{T}"/> and <see cref="IResizeableBuffer{T}"/>.
	/// </remarks>
	[Version(4,1, 0)]
	public sealed class RingBuffer<T> : IResizeableBuffer<T>
	{
		private readonly RandomAccessQueue<T> queue = new RandomAccessQueue<T>();

		/// <inheritdoc />
		public int Capacity { get; private set; }

		/// <inheritdoc />
		public int Count => queue.Count;

		/// <inheritdoc />
		public T First => queue.First();

		public bool IsFull => Capacity == Count;

		/// <inheritdoc />
		public T Last => queue.Peek();
		
		public T this[int index]
		{
			get
			{
				ValidateIndex(index);
				return queue[index];
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RingBuffer{T}"/> class.
		/// </summary>
		/// <param name="capacity">The capacity of this the new instance.</param>
		public RingBuffer(int capacity) => Capacity = capacity;
	
		/// <inheritdoc />s
		public void Clear() => queue.Clear();

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		
		/// <inheritdoc />
		public void Insert(T item)
		{
			if (queue.Count == Capacity)
			{
				queue.Dequeue();
			}
			
			queue.Enqueue(item);
		}

		/// <inheritdoc />
		public void IncreaseCapacity(int increment)
		{
			increment.ThrowIfNegative(nameof(increment));
			
			Capacity += increment;
		}

		/// <inheritdoc />
		public int DecreaseCapacity(int decrement)
		{
			decrement.ThrowIfNegative(nameof(decrement));

			if (decrement > Capacity)
			{
				decrement = Capacity;
			}
			
			Capacity -= decrement;
			
			while (queue.Count > Capacity)
			{
				queue.Dequeue();
			}
			
			return decrement;
		}
		
		private void ValidateIndex(int index) => index.ThrowIfOutOfRange(0, Count, nameof(index));
	}
}
