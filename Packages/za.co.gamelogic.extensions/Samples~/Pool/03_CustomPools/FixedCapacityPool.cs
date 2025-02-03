using System;

namespace Gamelogic.Extensions.DocumentationCode
{
	
	/// <summary>
	/// A pool with a fixed capacity.
	/// </summary>
	public class FixedCapacityPool<T>
		where T : class
	{
		private readonly IPool<T> pool;
		
		/// <inheritdoc cref="IPool{T}.Capacity"/>
		public int Capacity => pool.Capacity;
		
		/// <inheritdoc cref="IPool{T}.ActiveCount"/>
		public int ActiveCount => pool.ActiveCount;
		
		/// <inheritdoc cref="IPool{T}.HasAvailableObject"/>
		public bool HasAvailableObject => pool.HasAvailableObject;
		
		public FixedCapacityPool(
			int capacity,
			Func<T> create,
			Action<T> destroy,
			Action<T> activate,
			Action<T> deactivate) 
			=> pool = new Pool<T>(capacity, create, destroy, activate, deactivate);

		/// <inheritdoc cref="IPool{T}.Get"/>
		public T Get() => pool.Get();

		/// <inheritdoc cref="IPool{T}.Release"/>
		public void Release(T obj) => pool.Release(obj);
		
		/// <inheritdoc cref="IPool{T}.ReleaseAll"/>
		public void ReleaseAll() => pool.ReleaseAll();
	}
}
