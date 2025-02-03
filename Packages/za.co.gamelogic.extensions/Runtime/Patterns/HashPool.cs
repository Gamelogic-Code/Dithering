using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Internal;
using JetBrains.Annotations;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// A light-weight pool class for objects that can be hashed.
	/// </summary>
	/// <inheritdoc cref="Pool{T}"/>
	/// <seealso cref="Pool{T}"/>
	[Version(3, 2, 0)]
	public class HashPool<T> : IPool<T>
	{
		private readonly HashSet<T> activeObjects;
		private readonly Stack<T> inactiveObjects;
		private readonly Func<T> create;
		private readonly Action<T> destroy;
		private readonly Action<T> deactivate;
		private readonly Action<T> activate;

		/// <inheritdoc />
		public int Capacity => activeObjects.Count + inactiveObjects.Count;

		/// <inheritdoc />
		public int ActiveCount => activeObjects.Count;

		/// <inheritdoc />
		public bool HasAvailableObject => inactiveObjects.Count > 0;
		
		public HashPool(
			int initialCapacity,
			[NotNull] Func<T> create,
			[CanBeNull] Action<T> destroy,
			[CanBeNull] Action<T> activate,
			[CanBeNull] Action<T> deactivate,
			[CanBeNull] IEqualityComparer<T> comparer = null)
		{
			initialCapacity.ThrowIfNegative(nameof(initialCapacity));
			create.ThrowIfNull(nameof(create));
			
			this.create = create;
			this.destroy = destroy;
			this.deactivate = deactivate;
			this.activate = activate;

			activeObjects = new HashSet<T>(comparer);
			inactiveObjects = new Stack<T>();

			Create(initialCapacity);
		}

		/// <inheritdoc />
		public T Get()
		{
			if (!HasAvailableObject)
			{
				throw new InvalidOperationException(ErrorMessages.NoInactiveObjects);
			}

			var obj = inactiveObjects.Pop();
			activate?.Invoke(obj);
			activeObjects.Add(obj);

			return obj;
		}

		/// <inheritdoc />
		public void Release(T obj)
		{
			if (!activeObjects.Contains(obj))
			{
				if (!inactiveObjects.Contains(obj))
				{
					throw new ArgumentException(nameof(obj), ErrorMessages.ObjectNotInPool);
				}
				
				throw new InvalidOperationException(ErrorMessages.ObjectAlreadyInactive);
			}

			activeObjects.Remove(obj);
			deactivate?.Invoke(obj);
			inactiveObjects.Push(obj);
		}

		/// <inheritdoc />
		public void IncreaseCapacity(int increment)
		{
			increment.ThrowIfNegative(nameof(increment));
			Create(increment);
		}

		/// <inheritdoc />
		public int DecreaseCapacity(int decrement, bool deactivateFirst = false)
		{
			decrement.ThrowIfNegative(nameof(decrement));

			int numberToDestroy = Mathf.Min(decrement, Capacity);
			int numberToRelease = Mathf.Max(0, numberToDestroy - inactiveObjects.Count);
			int numberOfInactiveObjectsToDestroy = numberToDestroy - numberToRelease;

			if (numberToRelease > 0)
			{
				var objectsToRelease = activeObjects.Take(numberToRelease).ToArray();

				foreach (var obj in objectsToRelease)
				{
					activeObjects.Remove(obj);
					if (deactivateFirst)
					{
						deactivate?.Invoke(obj);
					}

					destroy?.Invoke(obj);
				}
			}

			for (int i = 0; i < numberOfInactiveObjectsToDestroy; i++)
			{
				var obj = inactiveObjects.Pop();
				destroy?.Invoke(obj);
			}

			return numberToDestroy;
		}

		/// <inheritdoc />
		public void ReleaseAll()
		{
			// Using an explicit enumerator to avoid allocations by using foreach
			var enumerator = activeObjects.GetEnumerator();
			
			try
			{
				while (enumerator.MoveNext())
				{
					var obj = enumerator.Current;
					deactivate(obj);
					inactiveObjects.Push(obj);
				}
			}
			finally
			{
				enumerator.Dispose();
			}

			activeObjects.Clear();
		}

		private void Create(int newObjectCount)
		{
			for (int i = 0; i < newObjectCount; i++)
			{
				var obj = create();
				deactivate?.Invoke(obj);
				inactiveObjects.Push(obj);
			}
		}
	}
}
