using System;

namespace Gamelogic.Extensions.DocumentationCode
{
	public class FakePool<T> : IPool<T>
	{
		readonly Func<T> create;
		readonly Action<T> destroy;
		readonly Action<T> activate;
		readonly Action<T> deactivate;
		
		public FakePool(int capacity, Func<T> create, Action<T> destroy, Action<T> activate, Action<T> deactivate)
		{
			this.create = create;
			this.destroy = destroy;
			this.activate = activate;
			this.deactivate = deactivate;
			Capacity = capacity;
		}

		public int Capacity { get; private set;}
		
		public int ActiveCount { get; private set;}
		
		public bool HasAvailableObject => ActiveCount < Capacity;
		
		public T Get()
		{
			if (!HasAvailableObject)
			{
				throw new InvalidOperationException("No available objects");
			}
			
			ActiveCount++;
			var obj = create();
			activate(obj);
			return obj;
		}

		public void Release(T obj)
		{
			ActiveCount--;
			deactivate(obj);
			destroy(obj);
		}

		public void IncreaseCapacity(int increment)
		{
			Capacity += increment;
		}

		public int DecreaseCapacity(int decrement, bool deactivateFirst = false)
		{
			Capacity -= decrement;

			return decrement;
		}

		public void ReleaseAll()
		{
			// Only possible if we had a container of all objects released, which this implementation does not. 
			throw new NotImplementedException();
		}
	}
}
