using UnityEngine.Assertions;

namespace Gamelogic.Extensions.DocumentationCode
{
	/// <summary>
	/// A pool for managing enemies.
	/// </summary>
	/// <remarks>
	/// This is an example of how to encapsulate the creation of objects completely within a pool.
	/// </remarks>
	public class EnemyPool : IPool<EnemyPool.IEnemy>
	{
		/// <summary>
		/// Represents an enemy in the game. 
		/// </summary>
		public interface IEnemy { }
		
		// This class is hidden from users; they cannot construct instances.
		// The only way to get instances is through the pool. 
		private class Enemy : IEnemy
		{
			/// <inheritdoc cref="IEnemy"/>
			public bool IsActive { get; set; }

			/// <inheritdoc cref="IEnemy"/>
			public void Activate() => IsActive = true;

			/// <inheritdoc cref="IEnemy"/>
			public void Deactivate() => IsActive = false;
		}
		
		/// <inheritdoc />
		public int Capacity => pool.Capacity;

		/// <inheritdoc />
		public int ActiveCount => pool.ActiveCount;

		/// <inheritdoc />
		public bool HasAvailableObject => pool.HasAvailableObject;

		private IPool<IEnemy> pool;

		/// <summary>
		/// Initializes an instance of <see cref="EnemyPool"/> with the given initial capacity. 
		/// </summary>
		/// <param name="initialCapacity"></param>
		public EnemyPool(int initialCapacity)
		{
			pool = new Pool<IEnemy>(
				initialCapacity,
				CreateEnemy,
				DestroyEnemy,
				Activate,
				Deactivate);
		}
		
		/// <inheritdoc />
		public IEnemy Get() => pool.Get();

		/// <inheritdoc />
		public void Release(IEnemy enemy) => pool.Release(enemy);
		
		/// <inheritdoc />
		public void IncreaseCapacity(int increment) => pool.IncreaseCapacity(increment);

		/// <inheritdoc />
		public int DecreaseCapacity(int decrement, bool deactivateFirst = false) 
			=> pool.DecreaseCapacity(decrement, deactivateFirst);

		/// <inheritdoc />
		public void ReleaseAll() => pool.ReleaseAll();

		private static void Activate(IEnemy enemy)
		{
			var enemyImplementation = (Enemy)enemy;

			Assert.IsFalse(enemyImplementation.IsActive);
			enemyImplementation.Activate();
		}

		private static void Deactivate(IEnemy enemy)
		{
			var enemyImplementation = (Enemy)enemy;

			Assert.IsTrue(enemyImplementation.IsActive);
			enemyImplementation.Deactivate();
		}

		private static IEnemy CreateEnemy() => new Enemy();

		private void DestroyEnemy(IEnemy enemy) { /* do nothing */ }
	}
}
