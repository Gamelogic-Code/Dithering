using System;

namespace Gamelogic.Extensions.DocumentationCode
{
	/// <summary>
	/// A pool with objects that can release themselves.
	/// </summary>
	public class PoolOfSelfReleasingObjects<T>
	{
		/// <summary>
		/// Represents an object that is managed by a pool.
		/// </summary>
		/// <remarks>
		/// Why an inner class? This prevents clashes, since this namespace defines more than one pooled object. 
		/// </remarks>
		public interface IPoolObject
		{
			/// <summary>
			/// The value of the object.
			/// </summary>
			T Value { get; }
		
			/// <summary>
			/// Releases the object back to the pool.
			/// </summary>
			void Release();
		}
		
		private class PoolObject : IPoolObject
		{
			private readonly IPool<PoolObject> owner;
			
			/// <inheritdoc />
			public T Value { get; }
		
			/// <summary>
			/// Initializes a new instance of <see cref="PoolObject"/>.
			/// </summary>
			/// <param name="value">The value to wrap.</param>
			/// <param name="owner">The pool controlling this object.</param>
			public PoolObject(T value, IPool<PoolObject> owner)
			{
				Value = value;
				this.owner = owner;
			}

			/// <summary>
			/// Releases this object from the pool it is in. 
			/// </summary>
			public void Release() => owner.Release(this);
		}

		private readonly IPool<PoolObject> pool;
		
		/// <summary>
		/// Initializes a new instance of <see cref="PoolOfSelfReleasingObjects{T}"/>.
		/// </summary>
		/// <param name="initialCapacity">The initial capacity of the pool.</param>
		/// <param name="create">The function that creates new objects.</param>
		/// <param name="destroy">The action that destroys objects.</param>
		/// <param name="activate">The action that activates objects.</param>
		/// <param name="deactivate">The action that deactivates objects.</param>
		/// <remarks>See <see cref="IPool{T}"/> for more detail on the parameters.</remarks>
		public PoolOfSelfReleasingObjects(
			int initialCapacity,
			Func<T> create,
			Action<T> destroy,
			Action<T> activate,
			Action<T> deactivate)
		{
			pool = new Pool<PoolObject>(
				initialCapacity,
				() => new PoolObject(create(), pool),
				po => destroy(po.Value),
				po => activate(po.Value),
				po => deactivate(po.Value));
		}
	}
}
