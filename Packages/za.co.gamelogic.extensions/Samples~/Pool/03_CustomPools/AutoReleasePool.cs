using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamelogic.Extensions.DocumentationCode
{
	/// <summary>
	/// A pool that automatically release an object if no objects are available when a new object is required.
	/// </summary>
	/// <typeparam name="T">The type of the objects to pool.</typeparam>
	public class AutoReleasePool<T>
		where T : class
	{
		private readonly IPool<T> pool;

		/* Maintaining our own set of active objects is very inelegant. We need to, since we need to be able to get hold
			of an active object that we can release when needed.
			
			We need a hash set, so we can also remove objects efficiently when asked to release them. 
			
			This example releases any old object, but it is easy to come up with scenarios where we may want to release an 
			object based on some measure (lifetime, or visual importance).    
		*/
		private readonly HashSet<T> activeObjects;

		public AutoReleasePool(
			int initialCapacity,
			Func<T> create,
			Action<T> destroy,
			Action<T> activate,
			Action<T> deactivate)
		{
			pool = new Pool<T>(initialCapacity, create, destroy, activate, deactivate);
			activeObjects = new HashSet<T>();
		}

		public T Get()
		{
			if (!pool.HasAvailableObject)
			{
				var obj = activeObjects.First();
				Release(obj);
			}

			var newObj = pool.Get();
			activeObjects.Add(newObj);

			return newObj;
		}

		public void Release(T obj)
		{
			activeObjects.Remove(obj);
			pool.Release(obj);
		}
	}
}
