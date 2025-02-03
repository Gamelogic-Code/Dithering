// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using System;
using System.ComponentModel;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	[Version(3, 0, 0)]
	public class SingletonSpawner<T> : GLMonoBehaviour 
		where T : MonoBehaviour
	{
		[SerializeField] private T prefab = null;
		[SerializeField] private bool dontDestroyOnLoad = false;

		public void Awake()
		{
			var result = Singleton<T>.FindAndConnectInstance();

			switch (result)
			{
				case Singleton.FindResult.FoundNone:
					var instance = Instantiate(prefab);
					instance.name = prefab.name;
					break;
				
				case Singleton.FindResult.FoundOne:
					// All good
					break;
				
				case Singleton.FindResult.FoundMoreThanOne:
					throw new InvalidOperationException(
						"There is already more than one instance of " + typeof(T) + 
						" in the scene. Only one instance is allowed.");

				default:
					throw new InvalidEnumArgumentException(nameof(result), (int)result, typeof(Singleton.FindResult));
			}
			
			result = Singleton<T>.FindAndConnectInstance();

			switch (result)
			{
				case Singleton.FindResult.FoundNone:
					throw new Exception("Instantiation failed, no instance of " + typeof(T) + " found.");
				
				case Singleton.FindResult.FoundOne:
					// All good
					break;
				
				case Singleton.FindResult.FoundMoreThanOne:
					Debug.Assert(false, "This code should be unreachable");
					return;
				
				default:
					throw new InvalidEnumArgumentException(nameof(result), (int)result, typeof(Singleton.FindResult));
			}
			
			/* We call this here so that if the prefab was already in the scene, it is also marked not to be destroyed.
			*/
			if (dontDestroyOnLoad)
			{
				DontDestroyOnLoad(Singleton<T>.Instance.gameObject);
			}
		}
	}
}
