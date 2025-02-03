using System;
using UnityEngine.Assertions;

namespace Gamelogic.Extensions.DocumentationCode
{
	/// <summary>
	/// A pool that does not allow null objects, and performs additional checks on the state of objects
	/// when performing operations on them to ensure integrity of the system.
	/// </summary>
	/// <remarks>
	/// To use this pool, the objects you want to pool should implement the <see cref="IPoolObject"/> interface.
	/// </remarks>
	public class RobustPool : IPool<RobustPool.IPoolObject>
	{
		/// <summary>
		/// An object managed by a pool.
		/// </summary>
		public interface IPoolObject
		{
			/// <summary>
			/// Gets and sets whether the object is awake.
			/// </summary>
			/// <remarks>The <c>set</c> method should only be called by the pool.</remarks>
			bool IsActive { get; set; }
		
			/// <summary>
			/// Activates the object.
			/// </summary>
			/// <remarks>
			/// Implementors: Any logic that you want to execute when the object is activated should be placed here.
			/// </remarks>
			void Activate();
		
			/// <summary>
			/// Deactivates the object.
			/// </summary>
			/// <remarks>
			/// Implementors: Any logic that you want to execute when the object is deactivated should be placed here.
			/// </remarks>
			void Deactivate();
		}
		
		private readonly IPool<IPoolObject> pool;
		private readonly Func<IPoolObject> create;
		private readonly Action<IPoolObject> destroy;
		
		/// <inheritdoc cref="Capacity"/>
		public int Capacity => pool.Capacity;

		/// <inheritdoc cref="ActiveCount"/>
		public int ActiveCount => pool.ActiveCount;

		/// <inheritdoc cref="HasAvailableObject"/>
		public bool HasAvailableObject => pool.HasAvailableObject;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="RobustPool"/> class.
		/// </summary>
		/// <param name="initialCount">The initial number of objects in the pool.</param>
		/// <param name="create">The function that creates new objects.</param>
		/// <param name="destroy">The action that destroys objects.</param>
		public RobustPool(int initialCount, Func<IPoolObject> create, Action<IPoolObject> destroy)
		{
			initialCount.ThrowIfNegative(nameof(initialCount));
			create.ThrowIfNull(nameof(create));
			destroy.ThrowIfNull(nameof(destroy));
			
			this.create = create;
			this.destroy = destroy;
			
			// Create calls this.create, so it needs to be assigned before the pool is created. 
			pool = new Pool<IPoolObject>(initialCount, Create, Destroy, Activate, Deactivate);
		}

		/// <inheritdoc cref="IPool{T}.Get"/>
		public IPoolObject Get()
		{
			var newObject = pool.Get();
			Assert.IsNotNull(newObject);
			return newObject;
		}
		
		/// <inheritdoc cref="IPool{T}.Release"/>
		public void Release(IPoolObject obj)
		{
			obj.ThrowIfNull(nameof(obj));
			pool.Release(obj);
		}

		/// <inheritdoc cref="IPool{T}.IncreaseCapacity"/>
		public void IncreaseCapacity(int amount) => pool.IncreaseCapacity(amount);
		
		/// <inheritdoc cref="IPool{T}.DecreaseCapacity"/>
		public int DecreaseCapacity(int amount, bool deactivateFirst = false) => pool.DecreaseCapacity(amount, deactivateFirst);

		/// <inheritdoc cref="IPool{T}.ReleaseAll"/>
		public void ReleaseAll() => pool.ReleaseAll();

		private static void Activate(IPoolObject obj)
		{
			Assert.IsFalse(obj.IsActive);
			obj.Activate();
			obj.IsActive = true;
		}
		
		private static void Deactivate(IPoolObject obj)
		{
			Assert.IsTrue(obj.IsActive);
			obj.Deactivate();
			obj.IsActive = false;
		}
		
		private IPoolObject Create()
		{
			var obj = create();
			
			if(obj == null)
			{
				throw new InvalidOperationException("The create Func provided in the constructor returned null");
			}
			
			obj.IsActive = false;
			return obj;
		}
		
		private void Destroy(IPoolObject obj)
		{
			Assert.IsNotNull(obj);
			destroy(obj);
		}
	}
}
