using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions.Algorithms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

namespace Gamelogic.Extensions.Samples
{
	/// <summary>
	/// This example is used to benchmark the different pool implementations. 
	/// </summary>
	public class PoolBenchmark : GLMonoBehaviour
	{
		/// <summary>
		/// The type of pool to test.
		/// </summary>
		public enum PoolType
		{
			None, // No pool is used - objects are created and destroyed as needed. 
			Pool, // The default pool implementation.
			HashPool, // A tool optimised for objects that can be hashed. 
			UnsafePool, // A pool that uses unsafe code to improve performance ever so slightly.
			UnityObjectPoolWrapper // A wrapper around Unity's ObjectPool to compare performance against their implementation.
		}
	
		/*	A wrapper class for pooled objects. Usually, this is not needed; here we use it so the tests are more uniform.  
		*/ 
		private class PoolObject<T>
		{
			// ReSharper disable once StaticMemberInGenericType
			private static int idCounter = 0;
		
			private readonly int id;
		
			/// <summary>
			/// The value of the object.
			/// </summary>
			public T Value { get; }
		
			/// <summary>
			/// Whether the object is active.
			/// </summary>
			public bool IsActive { get; set; }
		
			public PoolObject(T value)
			{
				unchecked
				{
					id = idCounter++;
				}
			
				Value = value;
				IsActive = true;
			}
		
			public override int GetHashCode() => id;
		}
	
		[ValidateNotNull]
		[SerializeField] private Transform prefab = null;
	
		[SerializeField] private PoolType poolType = PoolType.None;
	
		[ValidateNotNegative]
		[SerializeField] private int maxObjectCount = 2000;

		[ValidateNotNegative]
		[SerializeField] private int minActivityRate = 1000;
		
		[ValidateNotNegative]
		[SerializeField] private int maxActivityRate = 2000;
		
		[SerializeField] private List<Color> colors = null;
	
	
		private IPool<PoolObject<Transform>> pool;
	
		private List<PoolObject<Transform>> activeObjects = new List<PoolObject<Transform>>();
	
		private IGenerator<Color> colorGenerator;
		private IGenerator<float> sizeGenerator;
	
		public void Awake()
		{
			activeObjects = new List<PoolObject<Transform>>();

			switch (poolType)
			{
				case PoolType.None:
					break;
			
				case PoolType.Pool:
					pool = new Pool<PoolObject<Transform>>(maxObjectCount, Create, Destroy, Activate, Deactivate);
					break;
			
				case PoolType.HashPool:
					pool = new HashPool<PoolObject<Transform>>(maxObjectCount, Create, Destroy, Activate, Deactivate);
					break;
			
				case PoolType.UnsafePool:
					pool = new UnsafePool<PoolObject<Transform>>(maxObjectCount, Create, Destroy, Activate, Deactivate);
					break;
			
#if UNITY_2021_1_OR_NEWER
			case PoolType.UnityObjectPoolWrapper:
				pool = new UnityObjectPoolWrapper<PoolObject<Transform>>(maxObjectCount, Create, Destroy, Activate, Deactivate);
				break;
#else
				case PoolType.UnityObjectPoolWrapper:
					Debug.LogError("UnityObjectPoolWrapper is only available in Unity 2021.1 or newer.");
					break;
#endif
			}
		
			// Can be null if UnityObjectPoolWrapper is selected and the Unity version is not 2021.1 or newer.
			if(poolType == PoolType.UnityObjectPoolWrapper && pool == null)
			{
				return;
			}

			var alphaGenerator = Generator.UniformRandomFloat();
			var indexGenerator = Generator.UniformRandomInt(colors.Count);
			var baseColor = Generator.Choose(colors, indexGenerator);
			colorGenerator = Generator.Combine(baseColor, alphaGenerator, (color, alpha) => color.WithAlpha(alpha));
		
			sizeGenerator = Generator.UniformRandomFloat();
			Debug.Log("Start");
			StartCoroutine(Run());
			Invoke(Pause, 60);
		}

		private void Pause()
		{
#if UNITY_EDITOR
			EditorApplication.isPaused = true;
#endif
		}

		private PoolObject<Transform> Create()
		{
			var instance = Instantiate(prefab);
			var poolObject = new PoolObject<Transform>(instance);
			
			if(poolType == PoolType.UnityObjectPoolWrapper)
			{
				poolObject.IsActive = false;
			}
			
			return poolObject;
		}
	
		private void Destroy(PoolObject<Transform> obj)
		{
			Destroy(obj.Value.gameObject);
		}
	
		private void Activate(PoolObject<Transform> obj)
		{
			Assert.IsFalse(obj.IsActive);
			obj.Value.position = Random.insideUnitSphere * 10;
			obj.Value.gameObject.SetActive(true);
			obj.Value.GetComponent<SpriteRenderer>().color = colorGenerator.Next();
			obj.Value.localScale = prefab.localScale * sizeGenerator.Next();
			obj.IsActive = true;
		}
	
		private void Deactivate(PoolObject<Transform> obj)
		{
			Assert.IsTrue(obj.IsActive);
			obj.Value.gameObject.SetActive(false);
			obj.IsActive = false;
		}
	
		private IEnumerator Run()
		{
			while (Application.isPlaying)
			{
				for (int i = 0; i < Random.Range(minActivityRate, maxActivityRate); i++)
				{
					AddObject();
				}
			
				for (int i = 0; i < Random.Range(minActivityRate, maxActivityRate); i++)
				{
					RemoveObject();
				}
			
				yield return null;
			}
		}

		private void AddObject()
		{
			if(activeObjects.Count >= maxObjectCount)
			{
				return;
			}
		
			PoolObject<Transform> obj;
			switch (poolType)
			{
				case PoolType.Pool:
				case	PoolType.HashPool:
				case PoolType.UnsafePool:
				case PoolType.UnityObjectPoolWrapper:
					Assert.IsTrue(pool.HasAvailableObject);
					obj = pool.Get();
					activeObjects.Add(obj);
					break;
				case PoolType.None:
					obj = Create();
					Deactivate(obj);
					Activate(obj);
					activeObjects.Add(obj);
					break;
			}
		}
	
		private void RemoveObject()
		{
			if (activeObjects.Count == 0)
			{
				return;
			}
		
			int index = Random.Range(0, activeObjects.Count);
			var obj = activeObjects[index];

			switch (poolType)
			{
				case PoolType.Pool:
				case PoolType.HashPool:
				case PoolType.UnsafePool: 
				case PoolType.UnityObjectPoolWrapper:
					pool.Release(obj);
					break;
				case PoolType.None:
					Destroy(obj);
					break;
			}
		
			activeObjects.RemoveAt(index);
		}
	}
}
