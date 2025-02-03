// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using System;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// A data structure that can be used as a cache.
	/// </summary>
	/// <typeparam name="TKey">The type of the t key.</typeparam>
	/// <typeparam name="TValue">The type of the t value.</typeparam>
	public interface ICache<TKey, TValue>
	{
		#region Public Properties

		// Motivation: Data structures built on top of caches (such as LayeredCache) does not need to be implemented
		// in terms of the specific caches, so that they are interchangeable.

		/// <summary>
		/// Gets the number of elements in the cache .
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Gets a value indicating whether this cache is full.
		/// </summary>
		/// <value><c>true</c> if this cache is full; otherwise, <c>false</c>.</value>
		bool IsFull { get; }

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines whether this cache contains the specified key.
		/// </summary>
		/// <returns><c>true</c> if the cache contains the key; otherwise, <c>false</c>.</returns>
		bool ContainsKey(TKey key);

		/// <summary>
		/// Removes the element at the specified key from the cache.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>TValue.</returns>
		TValue Remove(TKey key);

		/// <summary>
		/// Removes the oldest item from the cache.
		/// </summary>
		/// <returns>KeyValuePair&lt;TKey, TValue&gt;.</returns>
		KeyValuePair<TKey, TValue> RemoveOldest();

		#endregion

		/// <summary>
		/// Gets or sets the value with the specified key. The method ContainsKey should always be called
		/// before getting the value of a key.
		/// </summary>
		TValue this[TKey key] { get; set; }
	}

	/// <summary>
	/// Represents a cache made out of two layers: a fast primary cache and a slow secondary cache. 
	/// Things requested that are in the slow cache gets moved to the fast cache, and the 
	/// oldest item in the fast cache moves to the slow cache.
	/// 
	/// New things are always added to the primary cache. The oldest item is moved to the slow cache
	/// if the primary cache is full. 
	/// 
	/// </summary>
	/// <typeparam name="TKey">The type of the t key.</typeparam>
	/// <typeparam name="TValue">The type of the t value.</typeparam>
	/// <seealso cref="ICache{TKey,TValue}" />
	public class LayeredCache<TKey, TValue> : ICache<TKey, TValue>
	{
		#region Constants

		private readonly ICache<TKey, TValue> fastPrimaryCache;
		private readonly ICache<TKey, TValue> slowSecondaryCache;

		#endregion

		#region Public Properties

		public int Count => fastPrimaryCache.Count + slowSecondaryCache.Count;

		public bool IsFull => fastPrimaryCache.IsFull && slowSecondaryCache.IsFull;

		#endregion

		#region Constructors

		public LayeredCache(int primaryCapacity, int secondaryCapacity)
		{
			fastPrimaryCache = new FixedSizeMemoryCache<TKey, TValue>(primaryCapacity);
			slowSecondaryCache = new FixedSizeMemoryCache<TKey, TValue>(secondaryCapacity);
		}

		#endregion

		#region Public Methods

		public bool ContainsKey(TKey key)
		{
			return fastPrimaryCache.ContainsKey(key) || slowSecondaryCache.ContainsKey(key);
		}

		public TValue Remove(TKey key)
		{
			if (fastPrimaryCache.ContainsKey(key))
			{
				return fastPrimaryCache.Remove(key);
			}

			Assert.IsTrue(slowSecondaryCache.ContainsKey(key));

			return slowSecondaryCache.Remove(key);
		}

		public KeyValuePair<TKey, TValue> RemoveOldest()
		{
			if (slowSecondaryCache.Count > 0)
			{
				return slowSecondaryCache.RemoveOldest();
			}

			Assert.IsTrue(fastPrimaryCache.Count > 0);

			return fastPrimaryCache.RemoveOldest();
		}

		#endregion

		public TValue this[TKey key]
		{
			get
			{
				if (fastPrimaryCache.ContainsKey(key))
				{
					return fastPrimaryCache[key];
				}

				// ContainsKey should be called before getting the value, 
				// so we know it is in the secondary cache when it is not in the primary cache.
				Assert.IsTrue(slowSecondaryCache.ContainsKey(key));

				// a key will only be in the secondary cache if the primary cache is full.
				Assert.IsTrue(fastPrimaryCache.IsFull);

				//Move the oldest value in the primary cache to the secondary cache
				//Move the value obtained from the secondary cache to the primary cache
				//Note: the order of statements is important here to prevent triggering cache re-organization

				var old = fastPrimaryCache.RemoveOldest();
				var val = slowSecondaryCache.Remove(key);

				fastPrimaryCache[key] = val;
				slowSecondaryCache[old.Key] = old.Value;

				return val;
			}

			set
			{
				if (fastPrimaryCache.ContainsKey(key) || !fastPrimaryCache.IsFull)
				{
					fastPrimaryCache[key] = value;
				}
				else
				{
					// primary cache does not have the key and primary cache is full

					var oldest = fastPrimaryCache.RemoveOldest();

					slowSecondaryCache[oldest.Key] = oldest.Value;

					fastPrimaryCache[key] = value;
				}

				if (IsFull) Debug.LogWarning("Layered Cache Full");
			}
		}
	}

	/// <summary>
	/// A cache maintained in memory that stays fixed in size.
	/// </summary>
	public class FixedSizeMemoryCache<TKey, TValue> : ICache<TKey, TValue>
	{
		#region Constants

		private readonly int capacity;

		private readonly LinkedList<TKey> queue; //? Use a heap instead.
		private readonly IDictionary<TKey, TValue> data;

		#endregion

		#region Public Properties

		public int Count => queue.Count;

		public bool IsFull => Count == capacity;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="FixedSizeMemoryCache{TKey,TValue}"/> class.
		/// </summary>
		/// <param name="capacity">The capacity of the cache, must be positive.</param>
		public FixedSizeMemoryCache(int capacity)
		{
			if (capacity <= 0)
			{
				throw new ArgumentException("Must be positive.", nameof(capacity));
			}

			this.capacity = capacity;

			queue = new LinkedList<TKey>();
			data = new Dictionary<TKey, TValue>();
		}

		#endregion

		#region Public Methods

		public bool ContainsKey(TKey key)
		{
			return data.ContainsKey(key);
		}

		public TValue Remove(TKey key)
		{
			queue.Remove(key);

			var val = data[key];

			data.Remove(key);

			return val;
		}

		public KeyValuePair<TKey, TValue> RemoveOldest()
		{
			var key = queue.Last.Value;
			var val = Remove(key);

			return new KeyValuePair<TKey, TValue>(key, val);
		}

		#endregion

		public TValue this[TKey key]
		{
			get
			{
				queue.Remove(key);
				queue.AddFirst(key);

				return data[key];
			}

			set
			{
				if (ContainsKey(key))
				{
					queue.Remove(key); //we will add it again at the front
				}
				else if (IsFull)
				{
					Assert.IsTrue(queue.Count > 0);

					var last = queue.Last.Value;
					queue.RemoveLast();
					data.Remove(last);
				}

				queue.AddFirst(key);

				data[key] = value;

				if (IsFull) Debug.Log("Fixed Size Memory Cache Full");
			}
		}
	}

	/// <summary>
	/// A cache maintained on disk.
	/// </summary>
	public class DiskCache<TKey, TValue> : ICache<TKey, TValue>
	{
		#region Constants

		public readonly int capacity;

		#endregion

		#region Public Properties

		public int Count => throw new NotImplementedException();

		public bool IsFull => throw new NotImplementedException();

		#endregion

		#region Constructors

		public DiskCache(int capacity)
		{
			this.capacity = capacity;
		}

		#endregion

		#region Public Methods

		public bool ContainsKey(TKey key)
		{
			throw new NotImplementedException();
		}

		public TValue Remove(TKey key)
		{
			throw new NotImplementedException();
		}

		public KeyValuePair<TKey, TValue> RemoveOldest()
		{
			throw new NotImplementedException();
		}

		#endregion

		public TValue this[TKey key]
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}
	}
}
