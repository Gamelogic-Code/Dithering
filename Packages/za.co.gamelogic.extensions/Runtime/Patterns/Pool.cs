using System;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gamelogic.Extensions	
{
	/// <summary>
	/// A light-weight pool class for objects that cannot be hashed.
	/// </summary>
	/// <typeparam name="T">The type of the objects to pool. Since the methods defined by the interface would
	/// copy instances of pool objects if <typeparamref name="T"/> was a struct, <typeparamref name="T"/> must be a
	/// <see langword="class" />.</typeparam>
	/// <remarks>
	/// When the pool is created, it is filled up to capacity with objects that are newly created. A pool object can be
	/// active or inactive. When an object is active, it can be used by the client. When it is inactive, it should not be
	/// used, but the system has no checks for this. Objects are set to inactive when they are created for the first
	/// time. 
	///
	/// To get an object that you can use, call <see cref="Get"/>. This will activate one of the pool objects up for
	/// your use.
	///
	/// When you are done with it, call <see cref="Release"/>. This will deactivate the object, and allow it to
	/// be reused.
	///
	/// Acquiring an object or releasing it does not destroy or create any objects, and pool objects always remain in
	/// the pool unless the capacity is reduced. 
	///
	/// We recommend implementing a bool isActive field in the object you want to pool, and maintain it in the activate
	/// and deactivate actions you provide in the constructor. You can then perform asserts in your code on this field to
	/// ensure you do not accidentally use objects that are inactive. 
	/// </remarks>
	/// <seealso cref="HashPool{T}"/>
	/* Design note: we _could_ enforce T to implement a certain interface, and provide and maintain the isActive field
		described automatically. However, we did not want to impose any constraints on pool objects.
		
		A better name for this class would be `DefaultPool` so we could use Pool for static methods (including factory 
		methods that could give you different pools more conveniently). Since this is a very old class though, I did not
		want to rename it. :/
	*/
	public class Pool<T> : IPool<T>
		where T : class
	{
		private readonly Func<T> create;
		private readonly Action<T> destroy;
		private readonly Action<T> deactivate;
		private readonly Action<T> activate;
		private readonly List<T> poolObjects;

		/// <inheritdoc />
		public int Capacity => poolObjects.Count;

		/// <inheritdoc />
		[Version(3, 2, 0)]
		public int ActiveCount { get; private set; }

		/// <inheritdoc />
		public bool HasAvailableObject => ActiveCount < Capacity;

		/// <summary>
		/// Initializes a new instance of the <see cref="Pool{T}"/> class.
		/// </summary>
		/// <param name="initialCapacity">The initial capacity of the pool, that is, how many object to create.</param>
		/// <param name="create">A function that creates a new object of type <typeparamref name="T"/>. This should create
		/// objects in the active state; they will be deactivated by the pool. This function should (ideally) also
		/// instantiate everything the class will need, and do any tasks that do not be repeated each time the
		/// object is made active.</param>
		/// <param name="destroy">The action that destroys an object of type <typeparamref name="T"/>. If it is an
		/// ordinary C# object, you do not need to provide a destroy method — references to the object from this object
		/// will be removed, and (provided you do not have references to it somewhere too), it will be released naturally
		/// like any C# object. If <typeparamref name="T"/> is a Unity <see cref="UnityEngine.Object"/>, or a <see cref="Component"/> whose
		/// <see cref="Component.gameObject"/> you want to be destroyed, this action should do that. Also consider using
		/// the <see cref="MonoBehaviourPool{T}"/> class instead. This action is called by methods that allow you to
		/// specify whether to deactivate objects first, so this action need not do any tasks that are already done
		/// by <paramref name="deactivate"/> action.</param>
		/// <param name="deactivate">A function called when an object is activated, either when releasing it, or
		/// when it is created for the first time. If your object accumulate references to other objects while
		/// it is active, and they cannot be re-used, you should remove them in this action, so that these objects do not
		/// hog memory while they are inactive.</param>
		/// <param name="activate">A function called when an object is activated, when it is acquired.</param>
		/// <throws><see cref="ArgumentOutOfRangeException"/><paramref name="initialCapacity"/> is negative.</throws>
		/// <throws><see cref="ArgumentNullException"/><paramref name="create"/> is <see langword="null"/>.</throws>
		public Pool(
			int initialCapacity,
			[NotNull] Func<T> create,
			[CanBeNull] Action<T> destroy,
			[CanBeNull] Action<T> activate,
			[CanBeNull] Action<T> deactivate)
		{
			initialCapacity.ThrowIfNegative(nameof(initialCapacity));
			create.ThrowIfNull(nameof(create));

			this.create = create;
			this.destroy = destroy;
			this.deactivate = deactivate;
			this.activate = activate;

			poolObjects = new List<T>(initialCapacity);
			ActiveCount = 0; //all are inactive

			Create(initialCapacity);
		}
		
		/// <inheritdoc />
		[Version(3, 2, 0)]
		public T Get()
		{
			if (!HasAvailableObject)
			{
				throw new InvalidOperationException(ErrorMessages.NoInactiveObjects);
			}

			var obj = poolObjects[ActiveCount];
			ActiveCount++;
			activate?.Invoke(obj);

			return obj;
		}
		
		/// <inheritdoc cref="Get"/>
		[Obsolete("Use Get instead.")]
		public T GetNewObject() => Get();
		
		/// <inheritdoc />
		public void Release([CanBeNull] T obj) => DeactivateAndReorder(obj);

		/// <inheritdoc />
		[Version(3, 2, 0)] 
		public void IncreaseCapacity(int increment)
		{
			increment.ThrowIfNegative(nameof(increment));
			Create(increment);
		}
		
		/// <inheritdoc cref="IncreaseCapacity"/>
		[Obsolete("Use IncreaseCapacity instead.")]
		public void IncCapacity(int increment) => IncreaseCapacity(increment);

		/// <inheritdoc />
		[Version(3, 2, 0)]
		public int DecreaseCapacity(int decrement, bool deactivateFirst = false)
		{
			decrement.ThrowIfNegative(nameof(decrement));

			int initialCapacity = Capacity;
			int remainingObjectsCount = Mathf.Max(0, Capacity - decrement);
			int destroyCount = Capacity - remainingObjectsCount;

			// Do it in reverse so indexes are valid as we reduce the list. 
			for (int i = remainingObjectsCount; i < initialCapacity; i++)
			{
				var obj = poolObjects[i];
				
				if(i < ActiveCount && deactivateFirst && deactivate != null)
				{
					deactivate(obj);
				}
				
				destroy(obj);
			}
			
			poolObjects.RemoveRange(remainingObjectsCount, destroyCount);

			Assert.AreEqual(initialCapacity, Capacity + destroyCount);

			return destroyCount;
		}
		
		/// <inheritdoc cref="DecreaseCapacity"/>
		[Obsolete("Use DecreaseCapacity instead.")]
		public int DecCapacity(int decrement, bool deactivateFirst = false) => DecreaseCapacity(decrement, deactivateFirst);

		/// <inheritdoc />
		[Version(3, 2, 0)]
		public void ReleaseAll()
		{
			for (int i = 0; i < ActiveCount; i++)
			{
				deactivate(poolObjects[i]);
			}
			
			ActiveCount = 0;
		}

		private void Create(int newObjectCount)
		{
			for (int i = 0; i < newObjectCount; i++)
			{
				var obj = create();
				deactivate?.Invoke(obj);
				poolObjects.Add(obj);
			}
		}

		/* Implementation note: This method does a linear search in the list for the object, and is therefore not
			as fast as one would like. Alternative designs:
				1. Limit T to safely hash tables and use a hash set for these objects. (This is now implemented in HashPool). 
				2. Give the user an ID or token when giving new objects that they can supply to this method 
					to release the object more efficiently. 
		*/
		private void DeactivateAndReorder(T obj)
		{
			int index = poolObjects.IndexOf(obj);

			if(index < 0)
			{
				throw new ArgumentException(nameof(obj), ErrorMessages.ObjectNotInPool);
			}
			
			if (index >= ActiveCount)
			{
				throw new InvalidOperationException(ErrorMessages.ObjectAlreadyInactive);
			}

			DeactivateAndReorderObjectAt(index);
		}
		
		private void DeactivateAndReorderObjectAt(int index)
		{
			Assert.IsTrue(index < ActiveCount);
			Assert.IsTrue(index >= 0);
			var obj = poolObjects[index];
			deactivate(obj);

			int lastActiveIndex = ActiveCount - 1;
			SwapObjects(lastActiveIndex, index);
			ActiveCount--;
		}

		private void SwapObjects(int index1, int index2)
			=> (poolObjects[index1], poolObjects[index2]) = (poolObjects[index2], poolObjects[index1]);
	}
}
