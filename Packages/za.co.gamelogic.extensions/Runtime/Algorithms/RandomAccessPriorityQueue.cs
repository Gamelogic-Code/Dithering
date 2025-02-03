using System;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Represents a generic index-based priority queue where elements are ordered based on their priority determined by an
	/// <see cref="IComparer{T}"/>.
	/// </summary>
	/// <remarks>
	/// This implementation of a priority queue uses an array-based binary heap. It associates each value with an index,
	/// allowing for efficient updates of the queue based on index. The index should be between 0 (inclusive) and the
	/// given capacity (exclusive). 
	/// </remarks>
	/// <typeparam name="TElement">The type of elements in the priority queue.</typeparam>
	/// <typeparam name="TPriority">The type of the priority of elements in the priority queue.</typeparam>
	/*	This data structure is designed for cases where the index is within a fixed range, and therefore
		the capacity is not expected to change. 
		
		See: Algorithms (4th Ed.) by Robert Sedgewick and Kevin Wayne, p. 320.
		
		This class is the correctly named version of IndexPriorityQueue.
	*/
	[Version(4, 2, 0)]
	public sealed class RandomAccessPriorityQueue<TElement, TPriority> 
	{
		private const int NotInQueue = -1;

		private readonly TElement[] elements;
		private readonly TPriority[] priorities;
		private readonly int[] priorityQueue; // Contains indexes of values
		private readonly int[] queuePosition;
		private readonly int capacity;
		private readonly IComparer<TPriority> comparer;

		/// <summary>
		/// Gets the number of elements currently in the priority queue.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexPriorityQueue{TElement,TPriority}"/> class with a specified capacity and
		/// comparer.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the priority queue can hold.</param>
		/// <param name="comparer">The IComparer to determine the priority of elements.</param>
		public RandomAccessPriorityQueue(int capacity, IComparer<TPriority> comparer)
		{
			comparer.ThrowIfNull(nameof(comparer));
			capacity.ThrowIfNegative(nameof(capacity));
		
			this.capacity = capacity;
			this.comparer = comparer;
			elements = new TElement[capacity];
			priorities = new TPriority[capacity];
			priorityQueue = new int[capacity + 1];
			queuePosition = new int[capacity];
			queuePosition.Fill(NotInQueue);
			
			Count = 0;
		}
		
		/// <summary>
		/// Clears all elements from the priority queue.
		/// </summary>
		public void Clear()
		{
			elements.FillWithDefault();
			priorities.FillWithDefault();
			priorityQueue.Fill(0);
			queuePosition.Fill(NotInQueue);
			Count = 0;
		}

		/// <summary>
		/// Determines whether the priority queue contains the specified index.
		/// </summary>
		/// <param name="index">The index to check for presence in the queue.</param>
		/// <returns><see langword="true"/> if the index is in the queue; otherwise, <see langword="false"/>.</returns>
		public bool Contains(int index)
		{
			ValidateIndex(index);
		
			return queuePosition[index] != NotInQueue;
		}
	
		/// <summary>
		/// Gets a value indicating whether the priority queue is empty.
		/// </summary>
		/// <returns><see langword="true"/> if the queue is empty; otherwise, <see langword="false"/>.</returns>
		public bool IsEmpty => Count == 0;
	
		/// <summary>
		/// Inserts a value with an associated index into the priority queue.
		/// </summary>
		/// <param name="index">The index associated with the value.</param>
		/// <param name="value">The value to insert.</param>
		/// <param name="priority">The priority of the value.</param>
		public void Enqueue(int index, TElement value, TPriority priority)
		{
			ValidateIndex(index);
			ValidateIndexNotPresent(index);
		
			elements[index] = value;
			priorities[index] = priority;
			Count++;
			priorityQueue[Count] = index;
			queuePosition[index] = Count;
			Swim(Count);
		}
	
		/// <summary>
		/// Returns the minimum element of the priority queue without removing it.
		/// </summary>
		/// <returns>A tuple containing the index and value of the minimum element.</returns>
		/*	What to peek and pop exactly is an interesting question, and depends on the application really.
			I decided to peek and pop both, even though I'd have preferred to peek and pop the index only, because
			it seems in some applications the value may be needed, and the user would not be able to get it without
			maintaining their own dictionary of indexes to values.
		*/
		public (int index, TElement value) Peek()
		{
			ValidateNotEmpty();
		
			return (priorityQueue[1], elements[priorityQueue[1]]);
		}
	
		/// <summary>
		/// Removes and returns the minimum element from the priority queue.
		/// </summary>
		/// <returns>A tuple containing the index and value of the minimum element that was removed.</returns>
		public (int index, TElement value) Dequeue()
		{
			ValidateNotEmpty();
		
			var minimumValue = elements[priorityQueue[1]];
			int index = priorityQueue[1];
			elements[priorityQueue[1]] = default;
			queuePosition[priorityQueue[1]] = NotInQueue;
			Swap(1, Count);
			Count--;
			Sink(1);
		
			return (index, minimumValue);
		}
		
		/// <summary>
		/// Peeks at the minimum element of the priority queue if it is not empty.
		/// </summary>
		/// <param name="result">A tuple containing the index and value of the minimum element.</param>
		/// <returns><see langword="true"/> if the queue was not empty; otherwise, <see langword="false"/>.</returns>
		public bool TryPeek(out (int index, TElement value) result)
		{
			if (IsEmpty)
			{
				result = default;
				return false;
			}
		
			result = Peek();
			return true;
		}
		
		/// <summary>
		/// Dequeues the minimum element from the priority queue if it is not empty.
		/// </summary>
		/// <param name="result">A tuple containing the index and value of the minimum element that was removed.</param>
		/// <returns><see langword="true"/> if the queue was not empty and an element was removed; <see langword="false"/>
		/// otherwise.</returns>
		public bool TryDequeue(out (int index, TElement value) result)
		{
			if (IsEmpty)
			{
				result = default;
				return false;
			}
		
			result = Dequeue();
			return true;
		}
		
		/// <summary>
		/// Removes the element at the specified index from the priority queue.
		/// </summary>
		/// <param name="index">The index of the element to remove.</param>
		public void Remove(int index)
		{
			ValidateIndex(index);
			ValidateIndexPresent(index);

			int position = queuePosition[index];
			Swap(position, Count);
			Count--;
			Sink(position);
			Swim(position);

			elements[index] = default;
			priorities[index] = default;
			queuePosition[index] = NotInQueue;
		}
	
		/// <summary>
		/// Updates the value of the element at the specified index.
		/// </summary>
		/// <param name="index">The index of the element to update.</param>
		/// <param name="element">The new value to replace the current value.</param>
		/// <param name="priority">The new priority of the element.</param>
		public void UpdateValue(int index, TElement element, TPriority priority)
		{
			ValidateIndex(index);
			ValidateIndexPresent(index);

			int comparisonResult = comparer.Compare(priority, priorities[index]);
			elements[index] = element;
			priorities[index] = priority;

			if (comparisonResult < 0)
			{
				Swim(queuePosition[index]);
			}
			else if (comparisonResult > 0)
			{
				Sink(queuePosition[index]);
			}
			
			// If comparisonResult == 0, the key remains unchanged, so no action is needed
		}
	
		private void Swim(int index)
		{
			while (index > 1 && IsLess(index, index / 2))
			{
				Swap(index, index / 2);
				index /= 2;
			}
		}
	
		private void Sink(int index)
		{
			while (2 * index <= Count)
			{
				int j = 2 * index;
				if (j < Count && IsLess(j + 1, j))
				{
					j++;
				}
				if (!IsLess(j, index))
				{
					break;
				}
				Swap(index, j);
				index = j;
			}
		}

		private bool IsLess(int i, int j)
		{
			return comparer.Compare(priorities[priorityQueue[i]], priorities[priorityQueue[j]]) < 0;
		}

		private void Swap(int i, int j)
		{
			priorityQueue.SwapAt(i, j);
			queuePosition.SwapAt(priorityQueue[i], priorityQueue[j]);
		}

		private void ValidateIndex(int index) => index.ThrowIfOutOfRange(0, capacity, nameof(index));

		private void ValidateIndexNotPresent(int index)
		{
			if (Contains(index))
			{
				throw new ArgumentException(ErrorMessages.IndexAlreadyAssigned , nameof(index));
			}
		}
	
		private void ValidateIndexPresent(int index)
		{
			if (!Contains(index))
			{
				throw new ArgumentException(ErrorMessages.NoObjectAtIndex, nameof(index));
			}
		}

		private void ValidateNotEmpty()
		{
			if (IsEmpty)
			{
				throw new InvalidOperationException(ErrorMessages.CollectionIsEmpty);
			}
		}
	}
}
