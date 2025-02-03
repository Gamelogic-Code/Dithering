using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Represents a pool of objects, with the idea to reuse objects instead of creating and destroying them.
	/// </summary>
	/// <typeparam name="T">The type of the objects to pool. </typeparam>
	/// <remarks>
	/// Unity introduced its own pool objects in Unity 2020. The built-in pool is similar to the one provided here,
	/// but this interface gives a bit more control over managing objects.
	///
	/// Here are some differences:
	///	- Unity pools do not allow you to specify the creation or destruction of pool objects.
	///	- Unity pools do not allow you to specify activation and deactivation actions.
	///	- Unity pools do not offer ways to manage the capacity of the pool explicitly.
	///
	/// What pool should I use?
	///	- If your objects are safe to be hashed, use <see cref="HashPool{T}"/>.
	///	- Otherwise, if your pool capacity is low, use <see cref="Pool{T}"/>.
	///	- Otherwise, use the pool returned by <see cref="PoolExtensions.GetHashPool{T}" />.
	///
	/// When should I use a pool?
	///	- When allocations of objects are expensive, such as creating game objects in the scene.
	///	- When you do a lot of allocations and de-allocations of the same type of object, such as in custom a particle
	/// system
	///
	/// We have several examples of how you can use our pools to build more sophisticated pools:
	/// [Custom Pools](../content/custom_pools.md).
	/// </remarks>
	[Version(3, 2, 0)]
	public interface IPool<T>
	{
		/// <summary>
		/// The total number of objects in the pool (active and inactive), the maximum number of objects that can be
		/// returned by <see cref="Get"/>.
		/// </summary>
		/// <value>The capacity.</value>
		int Capacity { get; }
		
		/// <summary>
		/// The number of objects that are currently active.
		/// </summary>
		int ActiveCount { get; }
		
		/// <summary>
		/// Returns whether there is an inactive object available to get.
		/// </summary>
		/// <value><see langword="true" /> if an inactive object is available; otherwise, <see langword="false" />.</value>
		bool HasAvailableObject { get; }

		/// <summary>
		/// Gets a new object from the pool.
		/// </summary>
		/// <returns>A freshly activated object.</returns>
		/// <exception cref="InvalidOperationException">No inactive objects are available.</exception>
		T Get();
		
		/// <summary>
		/// Releases the specified object back to the pool.
		/// </summary>
		/// <param name="obj">The object to release. We do not restrict the pool from containing null objects,
		/// therefore <paramref name="obj"/> can be null if the create action passed into the constructor can
		/// produce null elements. This is an unusual use case though.</param>
		/// <exception cref="ArgumentException"><paramref name="obj"/> is not in the pool.</exception>
		/// <exception cref="InvalidOperationException"><paramref name="obj"/> is already inactive.</exception>
		void Release(T obj);
		
		/// <summary>
		/// Increases the capacity of the pool. 
		/// </summary>
		/// <param name="increment">The number of new pool objects to add.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="increment"/> is negative.</exception>
		/*  Why separate Inc and Dec methods?
			An alternative would be to implement these as extension methods of a single SetCapacity method. However,
			usually the implementation of the two cases would be different anyways, so doing it this way is more direct.
		*/
		void IncreaseCapacity(int increment);
		
		/// <summary>
		/// Decreases the capacity of the pool.
		/// </summary>
		/// <param name="decrement">The number of pool objects to destroy.</param>
		/// <param name="deactivateFirst">Whether to deactivate (active) objects before destroying them.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="decrement"/> is negative.</exception>
		/// <remarks>
		/// This method may destroy active objects.
		/// </remarks>
		int DecreaseCapacity(int decrement, bool deactivateFirst = false);
		
		/// <summary>
		/// Releases all objects in the pool.
		/// </summary>
		/*  Why not make an extension method that use Release(obj)?
			We do not provide a way to iterate over all objects in the pool, so it is not possible. We do not provide a
			way to iterate over pool objects because it could introduce a variety of issues, such as releasing objects
			while iterating, and so on.
		*/
		void ReleaseAll();
	}
}
