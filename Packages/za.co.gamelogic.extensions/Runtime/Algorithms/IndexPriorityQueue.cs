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
	/* This class was misnamed, hence it was made Obsolete. This is anticipating several data structures that will follow the same naming
		pattern, so better to fix it now and not have all the classes be named poorly. .*/
	[Version(4, 0, 0)]
	[Obsolete("Use RandomAccessPriorityQueue instead.")]
	public class IndexPriorityQueue<TElement, TPriority>
	{
		private readonly RandomAccessPriorityQueue<TElement, TPriority> queue;
		
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.Count"/>
		public int Count => queue.Count;

		/// <summary>
		/// Initializes a new instance of the <see cref="IndexPriorityQueue{TElement,TPriority}"/> class with a specified capacity and
		/// comparer.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the priority queue can hold.</param>
		/// <param name="comparer">The IComparer to determine the priority of elements.</param>
		public IndexPriorityQueue(int capacity, IComparer<TPriority> comparer) => queue = new RandomAccessPriorityQueue<TElement, TPriority>(capacity, comparer);

		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.Clear"/>
		public void Clear() => queue.Clear();

		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.Contains"/>
		public bool Contains(int index) => queue.Contains(index);
	
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.IsEmpty"/>
		public bool IsEmpty => queue.IsEmpty;
	
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.Enqueue"/>
		public void Enqueue(int index, TElement value, TPriority priority) => queue.Enqueue(index, value, priority);
	
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.Peek"/>
		public (int index, TElement value) Peek() =>	queue.Peek();
	
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.Dequeue"/>
		public (int index, TElement value) Dequeue() => queue.Dequeue();
		
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.TryPeek"/>
		public bool TryPeek(out (int index, TElement value) result) => queue.TryPeek(out result);
		
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.TryDequeue"/>
		public bool TryDequeue(out (int index, TElement value) result) => queue.TryDequeue(out result);
		
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.Remove"/>
		public void Remove(int index) => queue.Remove(index);
	
		/// <inheritdoc cref="RandomAccessPriorityQueue{TElement,TPriority}.UpdateValue"/>
		public void UpdateValue(int index, TElement element, TPriority priority) => queue.UpdateValue(index, element, priority);
	}
}
