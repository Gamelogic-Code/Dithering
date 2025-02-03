// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.ComponentModel;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{

	/// <summary>
	/// Provides support for <see cref="Singleton{T}"/>.
	/// </summary>
	[Version(3, 0, 0)]
	public static class Singleton
	{
		#region Types
		/// <summary>
		/// The result of the search for an instance of the singleton.
		/// </summary>
		// Todo: This could be more widely useful, maybe it should not be an inner class 
		[Version(3, 0, 0)]
		public enum FindResult
		{
			/// <summary>
			/// No instance was found in the open scenes.
			/// </summary>
			FoundNone = 0,
			
			/// <summary>
			/// A single instance was found in the open scenes.
			/// </summary>
			FoundOne = 1,
			
			/// <summary>
			/// More than one instance was found in the open scenes.
			/// </summary>
			FoundMoreThanOne = 2,
		}
		#endregion 
	}
	
	/// <summary>
	/// Provides a generic implementation of a singleton-like pattern for <see cref="MonoBehaviour"/> classes.
	/// This class automatically searches for an existing instance in the scene or logs an error if none or more than one
	/// are found.
	/// </summary>
	/// <typeparam name="T">The type of the Singleton class derived from <see cref="MonoBehaviour"/>.</typeparam>
	/// <remarks> Singletons usually manage their own creation, but it is common in Unity projects to have a singleton
	/// already placed in the scene. In this sense, this class really checks whether an instance can be used as a
	/// singleton, and then provides access to the instance.
	///
	/// If you want to create a singleton that is not already placed in the scene, you can use put
	/// <see cref="SingletonSpawner{T}"/> in the scene instead. 
	/// </remarks>
	/// <exception cref="InvalidOperationException">there are none or more than one instance in the scene.</exception>
	[Version(1, 0, 0)]
	public class Singleton<T> : GLMonoBehaviour 
		where T : MonoBehaviour
	{
		#region Private Fields
		// ReSharper disable once InconsistentNaming (too late to rename). 
		protected static T instance;
		#endregion
		
		#region Properties
		/// <summary>
		/// Returns the instance of this singleton.
		/// </summary>
		[Version(3, 0, 0)]
		public static T Instance
		{
			get
			{
				if (instance != null)
				{
					return instance;
				}
				
				var findResult = FindAndConnectInstance();

				switch (findResult)
				{
					case Singleton.FindResult.FoundNone:
						throw new InvalidOperationException($"An instance of {typeof(T)} is needed in the scene, but there is none.");
					case Singleton.FindResult.FoundMoreThanOne:
						throw new InvalidOperationException($"There is more than one instance of {typeof(T)} in the scene. Only one instance is allowed.");
					case Singleton.FindResult.FoundOne:
						return instance;
					default:
						throw new InvalidEnumArgumentException(nameof(findResult), (int)findResult, typeof(Singleton.FindResult));
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this singleton is ready, that is, a unique instance is available in the scene.
		/// </summary>
		[Version(3, 0, 0)]
		public static bool IsReady
		{
			get
			{
				if(instance != null)
				{
					return true;
				}

				return FindAndConnectInstance() == Singleton.FindResult.FoundOne;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this singleton has a unique instance in open scenes.
		/// </summary>
		[Version(3, 0, 0)]
		public static bool HasInstanceInOpenScenes => FindObjectOfType<T>() != null;

		#endregion

		#region Unity Messages
		[Version(3, 0, 0)]
		public void OnDestroy() => instance = null;
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Finds an instance of the given type in the open scenes and assign it to the instance field of the
		/// <see cref="Singleton{T}"/>.
		/// </summary>
		/// <returns>The result of the search. Only <see cref="Singleton.FindResult.FoundOne"/> is considered a success.</returns>
		/// <remarks>You should generally not need to call this method; this is used by code that span prefabs.
		/// </remarks>
		[Version(3, 0, 0)]
		public static Singleton.FindResult FindAndConnectInstance()
		{
			var instances = FindObjectsOfType<T>();

			if (instances.Length == 1)
			{
				instance = instances[0];
				return Singleton.FindResult.FoundOne;
			}
			
			if(instances.Length == 0)
			{
				return Singleton.FindResult.FoundNone;
			}

			if (instances.Length > 1)
			{
				return Singleton.FindResult.FoundMoreThanOne;
			}
			
			Debug.Assert(false, "This code should be unreachable");
			return default; // Still need to satisfy the compiler about returning a value
		}
		
		#endregion
	}
}
