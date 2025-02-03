using System;
using Gamelogic.Extensions.Internal;
using JetBrains.Annotations;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// A pool suitable for MonoBehaviour objects that can be instantiated from a given prefab. 
	/// </summary>
	/// <typeparam name="T">The MonoBehaviour type of the objects to pool.</typeparam>
	public class MonoBehaviourPool<T> : IPool<T>
		where T : Component
	{
		private readonly IPool<T> pool;

		/// <inheritdoc cref="Pool{T}.HasAvailableObject"/>
		public bool IsObjectAvailable => pool.HasAvailableObject;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoBehaviourPool{T}"/> class.
		/// </summary>
		/// <param name="prefab">The prefab used to instantiate objects from.</param>
		/// <param name="parent">The parent object to which the pool objects will be attached. If null the
		/// object will be spawned without any parent.</param>
		/// <param name="initialCount">The initial count of objects to create.</param>
		/// <param name="setToSleep">A function called on objects when they are put to sleep.</param>
		/// <param name="wakeUp">A function called on an object when it is woken up.</param>
		/// <remarks>
		/// The pool instantiates instances of the prefab to create new objects for the pool, for the first time in this
		/// constructor, and later when the pool is grown with <see cref="IncreaseCapacity"/> or <see cref="PoolExtensions.SetCapacity{T}"/>.
		///
		/// The game objects of the instances are destroyed when the size of the pool is reduced with
		/// <see cref="DecreaseCapacity"/>, <see cref="PoolExtensions.SetCapacity{T}"/>, or <see cref="PoolExtensions.Clear{T}"/>.
		/// </remarks>
		/// <throws><see cref="ArgumentNullException"/><paramref name="prefab"/> is <see langref="null"/>.</throws>
		/// <throws><see cref="ArgumentOutOfRangeException"/><paramref name="initialCount"/> is negative.</throws>
		public MonoBehaviourPool(
			[NotNull]T prefab,
			[CanBeNull] GameObject parent, 
			int initialCount,  
			[CanBeNull] Action<T> wakeUp, 
			[CanBeNull] Action<T> setToSleep)
		{
			prefab.ThrowIfNull(nameof(prefab));
			initialCount.ThrowIfNegative(nameof(initialCount));
			
			pool = new HashPool<T>(
				initialCount, 
				() => GLMonoBehaviour.Instantiate(prefab, parent), 
				obj => UnityEngine.Object.Destroy(obj.gameObject),
				wakeUp,
				setToSleep);
		}
		
		/// <inheritdoc cref="Pool{T}.Get"/>
		[Obsolete("Use Get instead.")]
		public T GetNewObject() => Get();

		/// <inheritdoc cref="Pool{T}.Release"/>\
		[Obsolete("Use Release instead.")]
		public void ReleaseObject(T obj) => Release(obj);

		/// <inheritdoc cref="Pool{T}.IncreaseCapacity"/>
		[Obsolete("Use IncreaseCapacity instead.")]
		public void IncCapacity(int increment) => pool.IncreaseCapacity(increment);
		
		/// <inheritdoc cref="Pool{T}.DecreaseCapacity"/>
		[Obsolete("Use DecreaseCapacity instead.")]
		public void DecCapacity(int decrement) => pool.DecreaseCapacity(decrement);

		/// <inheritdoc />
		public int Capacity => pool.Capacity;
		
		/// <inheritdoc />
		[Version(3, 2, 0)]
		public int ActiveCount => pool.ActiveCount;
		
		/// <inheritdoc />
		public bool HasAvailableObject => pool.HasAvailableObject;
		
		/// <inheritdoc />
		[Version(3, 2, 0)]
		public T Get() => pool.Get();

		/// <inheritdoc />
		[Version(3, 2, 0)]
		public void Release(T obj) => pool.Release(obj);

		/// <inheritdoc />
		[Version(3, 2, 0)]
		public void IncreaseCapacity(int increment) => pool.IncreaseCapacity(increment);

		/// <inheritdoc />
		[Version(3, 2, 0)]
		public int DecreaseCapacity(int decrement, bool deactivateFirst = false) => pool.DecreaseCapacity(decrement, deactivateFirst);

		/// <inheritdoc />
		[Version(3, 2, 0)]
		public void ReleaseAll() => pool.ReleaseAll();
	}
}
