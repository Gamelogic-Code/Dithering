// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using System;
using System.Collections.Generic;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// An unsafe pool class that is used for benchmarking.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <remarks>
	/// For the most part, this class works similar to <see cref="Pool{T}"/>, except that it does not keep references to
	/// active objects, so when they are released no checks as to their validity is performed. Also,
	/// <see cref="ReleaseAll"/> is not supported, and <see cref="DecreaseCapacity"/> cannot drecrease the capacity below
	/// the number of currently inactive objects.
	///
	/// In general, you should use a <see cref="HashPool{T}"/>; this unsafe pool os only very marginally faster.
	/// </remarks>
	public class UnsafePool<T> : IPool<T>
	{
		private readonly Stack<T> inactiveObjects;
		private readonly Func<T> create;
		private readonly Action<T> destroy;
		private readonly Action<T> activate;
		private readonly Action<T> deactivate;

		public UnsafePool(
			int initialCapacity, 
			Func<T> create, 
			Action<T> destroy = null, 
			Action<T> activate = null, 
			Action<T> deactivate = null)
		{
			initialCapacity.ThrowIfNegative(nameof(initialCapacity));
			create.ThrowIfNull(nameof(create));
		
			this.create = create;
			this.destroy = destroy;
			this.activate = activate;
			this.deactivate = deactivate;
		
			Capacity = initialCapacity;
		
			inactiveObjects = new Stack<T>(initialCapacity);
		
			Create(initialCapacity);
		}
	
		private void Create(int count)
		{
			for (var i = 0; i < count; i++)
			{
				var obj = create();
				deactivate?.Invoke(obj);
				inactiveObjects.Push(obj);
			}
		}

		/// <inheritdoc />
		public int Capacity { get; private set; }
	
		/// <inheritdoc />
		public int ActiveCount => Capacity - inactiveObjects.Count;
	
		/// <inheritdoc />
		public bool HasAvailableObject => inactiveObjects.Count > 0;
	
		/// <inheritdoc />
		public T Get()
		{
			if (!HasAvailableObject)
			{
				throw new InvalidOperationException(ErrorMessages.NoInactiveObjects);
			}
		
			var obj = inactiveObjects.Pop();
			activate?.Invoke(obj);
		
			return obj;
		}

		/// <summary>Returns the object to the pool.</summary>
		/// <param name="obj">The object to return to the pool.</param>
		/// <remarks>
		/// This method does not check if the object is already inactive, and will not deactivate it if it is. This method
		/// will also not check if this object does not belong in the pool at all (that is, was never acquired from this),
		/// and will not throw an exception if this is the case.
		/// </remarks>
		/// <exception cref="InvalidOperationException">The pool is full.</exception>
		public void Release(T obj)
		{
			// No checks to guarantee this object is not already pushed

			if (Capacity == inactiveObjects.Count)
			{
				throw new InvalidOperationException(ErrorMessages.TooManyObjectsReleased);
			}
		
			deactivate?.Invoke(obj);
			inactiveObjects.Push(obj);
		}

		/// <inheritdoc />
		public void IncreaseCapacity(int increment)
		{
			increment.ThrowIfNegative(nameof(increment));
			Create(increment);
			Capacity += increment;
		}

		/// <summary>
		/// Decreases the capacity of the pool.
		/// </summary>
		/// <param name="decrement">The number of pool objects to destroy.</param>
		/// <param name="deactivateFirst">Whether to deactivate objects before destroying them. Since this pool does not
		/// keep track of active objects, no active objects will be destroyed, so this parameter is not relevant.</param>
		/// <returns>The number of objects that were destroyed.</returns>
		/// <remarks>
		/// This method will not decrease the capacity below the number of currently inactive objects, in other words
		/// active objects will not be destroyed. Therefor, <paramref name="deactivateFirst"/> is not relevant.
		/// </remarks>
		public int DecreaseCapacity(int decrement, bool deactivateFirst = false)
		{
			decrement.ThrowIfNegative(nameof(decrement));
			
			int count = Math.Min(inactiveObjects.Count, decrement);
		
			for (int i = 0; i < count; i++)
			{
				var obj = inactiveObjects.Pop();
				destroy?.Invoke(obj);
			}
		
			Capacity -= count;

			return count;
		}
		
		/// <exception cref="NotSupportedException"></exception>
		public void ReleaseAll() => throw new NotSupportedException();
	}
}
