using System;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	public interface IPoolObject<out T>
	{
		T Value { get; }
		
		/* Design note: since we are wrapping the objects we want to pool anyways, we can track whether they are active
			and add some additional checks to make them safer. 
		*/
		bool IsActive { get; }
	}
	
	public static class PoolExtensions
	{
		/// <summary>
		/// A wrapper for objects in a pool. 
		/// </summary>
		/// <seealso cref="PoolExtensions.GetHashPool{T}"/>
		/// <remarks>
		/// This class provides a stable hash function, so it is safe to use these objects in hash sets and dictionaries.
		/// </remarks>
		[Version(3, 2, 0)]
		private class PoolObject<T> : IPoolObject<T>
		{
			// ReSharper disable once StaticMemberInGenericType
			private static int idCounter = 0;
		
			private readonly int id;
		
			/// <summary>
			/// Gets the value of the object.
			/// </summary>
			public T Value { get; }
		
			/// <summary>
			/// Gets whether the object is active.
			/// </summary>
			/* The pool is the only object suppose to set this. We can enforce this because the class is internal to the
				pool. 
			*/
			public bool IsActive { get; set; }
		
			/// <summary>
			/// Creates a new pool object.
			/// </summary>
			/// <param name="value">The value of the object.</param>
			public PoolObject(T value)
			{
				unchecked
				{
					id = idCounter++;
				}
			
				Value = value;
				IsActive = true;
			}
		
			/// <inheritdoc />
			public override int GetHashCode() => id;
		}
		
		/// <summary>
		/// Sets the capacity of the pool. If the new capacity is less than the current capacity, the pool will be
		/// reduced to the new capacity by destroying objects. If the new capacity is greater than the current capacity,
		/// new objects will be created to fill the pool.
		/// </summary>
		/// <param name="pool">The pool to set the capacity of.</param>
		/// <param name="newCapacity">The new capacity.</param>
		/// <param name="deactivateFirst">Whether to deactivate the object before destroying them.</param>
		/// <typeparam name="T">The type of the objects in the pool.</typeparam>
		/// <throws><see cref="ArgumentOutOfRangeException"/><paramref name="newCapacity"/> is negative.</throws>
		public static void SetCapacity<T>(this IPool<T> pool, int newCapacity, bool deactivateFirst = false)
		{
			newCapacity.ThrowIfNegative(nameof(newCapacity));

			if (pool.Capacity == newCapacity)
			{
				return;
			}

			if (pool.Capacity > newCapacity)
			{
				pool.DecreaseCapacity(pool.Capacity - newCapacity, deactivateFirst);
			}
			else
			{
				pool.IncreaseCapacity(newCapacity - pool.Capacity);
			}
		}

		/// <summary>
		/// Destroys all objects in the pool, reducing the capacity to zero.
		/// </summary>
		///<param name="pool">The pool to clear.</param>
		/// <param name="deactivateFirst">Whether deactivate the object before destroying them.</param>
		public static void Clear<T>(this IPool<T> pool, bool deactivateFirst = false)
			=> pool.SetCapacity(0, deactivateFirst);
		
		/// <summary>
		/// Gets a number of objects from a pool, and adds them to the given collection.
		/// </summary>
		/// <param name="collection">The collection to add objects to.</param>
		/// <param name="pool">The pool to get objects from.</param>
		/// <param name="count">The number of objects to get.</param>
		/// <typeparam name="T">The type of the objects in the pool.</typeparam>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
		/// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
		/// <exception cref="InvalidOperationException">The pool does not have enough objects to satisfy the request.</exception>
		/// <remarks> The idea with this method is that you may often want a bunch of object from a pool, but
		/// we do not want to create new objects (which may defy the point of pooling in the first place). Therefore,
		/// this method adds it directly to your collection.
		/// </remarks>
		/// <seealso cref="TryAddFromPool{T}"/>
		public static void AddFromPool<T>(this ICollection<T> collection, IPool<T> pool, int count)
		{
			collection.ThrowIfNull(nameof(collection));
			pool.ThrowIfNull(nameof(pool));
			count.ThrowIfNegative(nameof(count));
			
			for (int i = 0; i < count; i++)
			{
				collection.Add(pool.Get());
			}
		}
		
		/// <summary>
		/// Gets a number of objects from a pool, and adds them to the given collection.
		/// </summary>
		/// <param name="collection">The collection to add objects to.</param>
		/// <param name="pool">The pool to get objects from.</param>
		/// <param name="count">The number of objects to get.</param>
		/// <typeparam name="T">The type of the objects in the pool.</typeparam>
		/// <returns>The number of objects that were added to the collection.</returns>
		public static int TryAddFromPool<T>(
			this ICollection<T> collection,
			IPool<T> pool, 
			int count)
		{
			collection.ThrowIfNull(nameof(collection));
			pool.ThrowIfNull(nameof(pool));
			count.ThrowIfNegative(nameof(count));
			
			for (int i = 0; i < count; i++)
			{
				if (pool.HasAvailableObject)
				{
					collection.Add(pool.Get());
				}
				else
				{
					return i;
				}
			}

			return count;
		}

		/// <summary>
		/// Gets a hash pool for any type - hashable or not. 
		/// </summary>
		/// <param name="initialCapacity">The initial capacity of the pool.</param>
		/// <param name="create">The function that creates a new object of type T. This should create objects in the
		/// active state; they will be deactivated by the pool.</param>
		/// <param name="destroy">The function that destroys an object of type T. If it is an ordinary C# object, you do
		/// not need to provide a destroy method - the object will be removed from the pool, and (provided you do not have
		/// references to it), it will be released naturally like any C# object. If T is a Unity object, or
		/// a component whose game object you want to be destroyed, this action should do that. Also consider using the
		/// <see cref="MonoBehaviourPool{T}"/> class instead.</param>
		/// <param name="activate">A function called when an object is activated, when it is acquired.</param>
		/// <param name="deactivate">A function called when an object is deactivated, either when releasing it, or
		/// when it is created for the first time.</param>
		/// <typeparam name="T">The type of the objects to pool.</typeparam>
		/// <returns>A new hash pool.</returns>
		/// <remarks>Objects acquired from this pool will be wrapped in a <see cref="PoolObject{T}"/> object. Access the
		/// original object using <see cref="PoolObject{T}.Value"/>.
		/// </remarks>
		public static IPool<IPoolObject<T>> GetHashPool<T>(
			int initialCapacity,
			Func<T> create,
			Action<T> destroy = null,
			Action<T> activate = null,
			Action<T> deactivate = null)
		{
		
			return new HashPool<IPoolObject<T>>(
				initialCapacity,
				Create,
				Destroy,
				Activate,
				Deactivate);
			
			PoolObject<T> Create() => new PoolObject<T>(create());
			void Destroy(IPoolObject<T> obj) => destroy?.Invoke(obj.Value);
			
			void Activate(IPoolObject<T> obj)
			{
				if(obj.IsActive)
				{
					throw new InvalidOperationException(ErrorMessages.ObjectAlreadyActive);
				}
				
				activate?.Invoke(obj.Value);
				SetActive(obj, true);
			}

			void Deactivate(IPoolObject<T> obj)
			{
				if(!obj.IsActive)
				{
					throw new InvalidOperationException(ErrorMessages.ObjectAlreadyInactive);
				}
				
				deactivate?.Invoke(obj.Value);
				SetActive(obj, false);
			}
			
			void SetActive(IPoolObject<T> obj, bool active) => ((PoolObject<T>)obj).IsActive = active;
		}
		
		/// <summary>
		/// Gets an item from the pool if one is available.
		/// </summary>
		/// <param name="pool">The pool to get an object from.</param>
		/// <param name="obj">The object that was acquired, or the default value of T if no object was available.</param>
		/// <typeparam name="T">The type of the objects in the pool.</typeparam>
		/// <returns><see langword="true"/> if an object was acquired, <see langword="false"/> otherwise.</returns>
		public static bool TryGet<T>(this IPool<T> pool, out T obj)
		{
			if (pool.HasAvailableObject)
			{
				obj = pool.Get();
				return true;
			}

			obj = default;
			return false;
		}

		/// <summary>
		/// Sets the capacity of the pool to the current active count.
		/// </summary>
		/// <param name="pool">The pool to trim.</param>
		/// <typeparam name="T">The type of the objects in the pool.</typeparam>
		public static void Trim<T>(this IPool<T> pool)
		{
			pool.SetCapacity(pool.ActiveCount);
		}
		
		/// <summary>
		/// Adds all available objects from the pool to the collection, activating each of them. 
		/// </summary>
		/// <param name="collection">The collection to add objects to.</param>
		/// <param name="pool">The pool to get objects from.</param>
		/// <typeparam name="T">The type of the objects in the pool.</typeparam>
		/// <remarks> Similar to <see cref="AddFromPool{T}"/>, but adds all available objects from the pool.
		/// </remarks>
		public static void AddAllAvailableFromPool<T>(this ICollection<T> collection, IPool<T> pool)
		{
			collection.ThrowIfNull(nameof(collection));
			pool.ThrowIfNull(nameof(pool));
			
			int inactiveCount = pool.Capacity - pool.ActiveCount;
			collection.AddFromPool(pool, inactiveCount);
		}
	}
}
