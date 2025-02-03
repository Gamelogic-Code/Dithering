#if UNITY_2021_1_OR_NEWER
using System;
using UnityEngine.Pool;

namespace Gamelogic.Extensions.Samples
{
	/// <summary>
	/// Class that wraps a Unity ObjectPool to implement the IPool interface so we can use it in the benchmarks.
	/// </summary>
	/// <typeparam name="T">The type of the objects to pool.</typeparam>
	public class UnityObjectPoolWrapper<T> : IPool<T>
		where T : class
	{
		private readonly ObjectPool<T> pool;
		
		public UnityObjectPoolWrapper(
			int capacity,
			Func<T> create,
			Action<T> destroy,
			Action<T> activate,
			Action<T> deactivate)
		{
			Capacity = capacity;
			pool = new ObjectPool<T>(create, activate, deactivate, destroy, false, capacity, capacity);
		}

		/// <inheritdoc cref="IPool{T}.Capacity"/>
		/*	Keep track of the capacity ourselves since Unity pool may have fewer objects created than the capacity, as it
			is a lazy pool.
		*/   
		public int Capacity { get; }
		
		/// <inheritdoc cref="IPool{T}.ActiveCount"/>
		public int ActiveCount => pool.CountActive;
		
		/// <inheritdoc cref="IPool{T}.HasAvailableObject"/>
		public bool HasAvailableObject => pool.CountActive < Capacity;
		
		/// <inheritdoc cref="IPool{T}.Get"/>
		public T Get() => pool.Get();

		/// <inheritdoc cref="IPool{T}.Release"/>
		public void Release(T obj) => pool.Release(obj);

		/// <inheritdoc cref="IPool{T}.IncreaseCapacity"/>
		public void IncreaseCapacity(int increment) => throw new NotImplementedException();

		/// <inheritdoc cref="IPool{T}.DecreaseCapacity"/>
		public int DecreaseCapacity(int decrement, bool deactivateFirst = false) => throw new NotImplementedException();

		/// <inheritdoc cref="IPool{T}.ReleaseAll"/>
		public void ReleaseAll() => throw new NotImplementedException();
	}
}
#endif
