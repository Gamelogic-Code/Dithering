using System;

namespace Gamelogic.Extensions.DocumentationCode
{
	/// <summary>
	/// A pool that lazily creates objects when they are requested.
	/// </summary>
	/// <typeparam name="T">The type of the objects to pool.</typeparam>
	/// <remarks>
	/// This pool adjust its capacity as objects are requested. If there are no available objects,
	/// the pool will increase its capacity by one.
	///
	/// This class does not implement <see cref="IPool{T}"/> because it does not have a fixed capacity, and so
	/// the semantics of methods like <see cref="IPool{T}.HasAvailableObject"/> are not well-defined.
	/// </remarks>
	public class LazyPool<T>
		where T : class
	{
		private readonly IPool<T> pool;

		/// <summary>
		/// The total number of objects in the pool (active and inactive).
		/// </summary>
		public int Count => pool.ActiveCount;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LazyPool{T}"/> class.
		/// </summary>
		/// <param name="create">The function to create new objects.</param>
		/// <param name="kill">The action to destroy objects.</param>
		/// <param name="activate">The action to activate objects.</param>
		/// <param name="deactovate">The action to deactivate objects.</param>
		public LazyPool(Func<T> create, Action<T> kill, Action<T> activate, Action<T> deactovate) 
			=> pool = new Pool<T>(0, create, kill, activate, deactovate);

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyPool{T}"/> using the given pool as the underlying pool.
		/// </summary>
		public LazyPool(IPool<T> pool) => this.pool = pool;

		public T Get()
		{
			if (!pool.HasAvailableObject)
			{
				pool.IncreaseCapacity(1);
			}

			return pool.Get();
		}

		/// <inheritdoc cref="IPool{T}.Release"/>
		public void Release(T obj) => pool.Release(obj);

		/// <inheritdoc cref="IPool{T}.ReleaseAll"/>
		public void ReleaseAll() => pool.ReleaseAll();

		/// <inheritdoc cref="PoolExtensions.Clear{T}" />
		public void Clear() => pool.Clear();
	}
}
