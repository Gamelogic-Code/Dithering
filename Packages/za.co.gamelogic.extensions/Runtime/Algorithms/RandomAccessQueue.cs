using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Represents a queue that allows random access to its elements.
	/// </summary>
	/// <typeparam name="T">The type of elements in the queue.</typeparam>
	/// <remarks>
	/// This <see cref="RandomAccessQueue{T}"/> follows the design of <see cref="Queue{T}"/> very closely,
	/// and can be used as a drop-in replacement for it.
	/// </remarks>
	[Version(4, 2, 0)]
	public class RandomAccessQueue<T> : IReadOnlyCollection<T>, ICollection
	{
		private const int DefaultCapacity = 8;
		
		private int version;
	
		private T[] items;
		private int head;
		private int tail;

		/// <inheritdoc cref="ICollection.Count"/>
		public int Count { get; private set; }
	
		/// <inheritdoc cref="ICollection.IsSynchronized"/>
		public bool IsSynchronized => false;

		/// <inheritdoc cref="ICollection.SyncRoot"/>
		public object SyncRoot => items.SyncRoot;

		/// <summary>
		/// Gets a value indicating whether the queue is empty.
		/// </summary>
		public bool IsEmpty => Count == 0;
	
		/// <summary>
		/// Gets a value indicating whether the queue is full, that is, <see cref="Count"/> is equal to the capapcity.
		/// </summary>
		public bool IsFull => Count == Capacity;

		/// <summary>
		/// Returns the element at the specified index, where the head of the queue is at index 0.
		/// </summary>
		/// <param name="index">The index of the element to retrieve.</param>
		/// <returns>The element at the specified index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">The index is out of range.</exception>
		public T this[int index]
		{
			get
			{
				ValidateIndex(index);
				return ElementAtUnchecked(index);
			}
		}
		
		private int Capacity => items.Length;
	
		/// <summary>
		/// Initializes a new instance of the <see cref="RandomAccessQueue{T}"/> class that is empty and has the specified
		/// capacity.
		/// </summary>
		/// <param name="capacity">The initial capacity of the queue. If omitted, the default capacity will be used.</param>
		public RandomAccessQueue(int capacity = DefaultCapacity)
		{
			items = new T[capacity];
			head = 0;
			tail = 0;
			Count = 0;
			version = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RandomAccessQueue{T}"/> class that contains elements copied from
		/// the specified collection.
		/// </summary>
		/// <param name="collection"></param>
		public RandomAccessQueue(IEnumerable<T> collection)
		{
			items = collection.ToArray();
			head = 0;
			tail = Count;
			Count = items.Length;
			version = 0;
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public void Clear()
		{
			for (int i = 0; i < Count; i++)
			{
				items[(head + i) % Capacity] = default; // This is to let go of references.
			}
		
			head = 0;
			tail = 0;
			Count = 0;
			version++;
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public bool Contains(T element)
		{
			for (int i = 0; i < Count; i++)
			{
				if (ElementAtUnchecked(i).Equals(element))
				{
					return true;
				}
			}
		
			return false;
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public void CopyTo(Array array, int index)
		{
			array.ThrowIfNull(nameof(array));
			index.ThrowIfNegative(nameof(index));
		
			if(array.Length - index < Count)
			{
				throw new ArgumentException("The number of elements in the source collection is greater than the available space from the index to the end of the destination array.");
			}
		
			for (int i = 0; i < Count; i++)
			{
				array.SetValue(ElementAtUnchecked(i), index + i);
			}
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public T Dequeue()
		{
			ValidateNotEmpty();
		
			var item = items[head];
			items[head] = default;
			head = (head + 1) % Capacity;
			Count--;
		
			if (Count > 0 && Count == Capacity / 4)
			{
				Resize(Capacity / 2);
			}
		
			version++;
			return item;
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public void Enqueue(T item)
		{
			if (IsFull)
			{
				Resize(Capacity * 2);
			}
		
			items[tail] = item;
			tail = (tail + 1) % Capacity;
			version++;
			Count++;
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public void EnsureCapacity(int capacity)
		{
			if (capacity < Capacity)
			{
				return;
			}
			
			version++;
			Resize(capacity);
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public T Peek()
		{
			ValidateNotEmpty();
			return ElementAtUnchecked(0);
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public void TrimExcess()
		{
			if (!(Count < 0.9f * Capacity))
			{
				return;
			}
			
			version++;
			Resize(Count);
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public T[] ToArray()
		{
			var array = new T[Count];
		
			for (int i = 0; i < Count; i++)
			{
				array[i] = ElementAtUnchecked(i);
			}
		
			return array;
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public bool TryDequeue(out T item)
		{
			if (IsEmpty)
			{
				item = default;
				return false;
			}
		
			item = Dequeue();
			return true;
		}
	
		/// <inheritdoc cref="Queue{T}"/>
		public bool TryPeek(out T item)
		{
			if (IsEmpty)
			{
				item = default;
				return false;
			}
		
			item = Peek();
			return true;
		}

		private void Resize(int capacity)
		{
			var resized = new T[capacity];
		
			for (int i = 0; i < Count; i++)
			{
				resized[i] = items[(head + i) % Capacity];
			}
		
			items = resized;
			head = 0;
			tail = Count;
		}
	
		private void ValidateNotEmpty()
		{
			if (IsEmpty)
			{
				ThrowHelper.ThrowContainerEmpty();
			}
		}

		private void ValidateIndex(int index) => index.ThrowIfOutOfRange(0, Count, nameof(index));

		public IEnumerator<T> GetEnumerator()
		{
			int originalVersion = version;
			
			for (int i = 0; i < Count; i++)
			{
				ValidateVersionsMatch(originalVersion);
				yield return items[(head + i) % Capacity];
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private T ElementAtUnchecked(int index) => items[(head + index) % Capacity];
		
		private void ValidateVersionsMatch(int originalVersion)
		{
			if (version != originalVersion)
			{
				throw new InvalidOperationException("The collection was modified after the enumerator was created.");
			}
		}
	}
}
