using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Represents a buffer with a fixed capacity of two items.
	/// </summary>
	/// <typeparam name="T">The type of items contained in the buffer.</typeparam>
	/// <remarks>
	/// This class has been optimized for the special case of buffer a value and its previous value. 
	/// </remarks>
	[Version(4,1, 0)]
	public sealed class Capacity2Buffer<T> : IBuffer<T>
	{
		private const int FullCapacity = 2;
		
		private readonly T[] items = new T[2];

		/// <inheritdoc />
		public IEnumerator<T> GetEnumerator()
		{
			if (Count == 1)
			{
				yield return items[1];
				yield break;
			}
		
			for (int i = 0; i < Count; i++)
			{
				yield return items[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		/// <inheritdoc />
		public int Capacity => FullCapacity;
	
		/// <inheritdoc />
		public bool IsFull => Count == FullCapacity;

		/// <inheritdoc />
		public int Count { get; private set; }

		/// <inheritdoc cref="IBuffer{T}" />
		public T First
		{
			get
			{
				switch (Count)
				{
					case 0:
						throw ThrowHelper.CollectionEmptyException;
					case 1:
						return items[1];
					case 2:
						return items[0];
					default:
						throw ThrowHelper.UnreachableCodeException;
				}
			}
		}
	
		/// <summary>
		/// Gets a value indicating whether this buffer has a value. 
		/// </summary>
		public bool HasValue => Count > 0;
	
		/// <summary>
		/// Gets a value indicating whether this buffer has a previous value.
		/// </summary>
		public bool HasPreviousValue => Count > 1;

		/// <inheritdoc cref="IBuffer{T}"/>
		public T Last
		{
			get
			{
				switch (Count)
				{
					case 0:
						throw ThrowHelper.CollectionEmptyException;
					case 1:
					case 2:
						return items[1];
					default:
						throw ThrowHelper.UnreachableCodeException;
				}
			}
		}
		
		/// <inheritdoc />
		public T this[int index]
		{
			get
			{
				ValidateIndex(index);
				return items[index];
			}
		}

		/// <inheritdoc />
		public void Clear()
		{
			Count = 0;
			items[0] = default;
			items[1] = default;
		}

		/// <inheritdoc />
		public void Insert(T item)
		{
			items[0] = items[1];
			items[1] = item;
		
			if (Count < 2)
			{
				Count++;
			}
		}

		private void ValidateIndex(int index) => index.ThrowIfOutOfRange(0, Count, nameof(index));
	}
}
