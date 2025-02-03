// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Provides useful extension methods for GameObjects.
	/// </summary>
	[Version(3, 0, 0)]
	public static class GameObjectExtensions
	{
		/// <summary>
		/// Gets a component of the given type on the game object, or fail if no such component can be found.
		/// </summary>
		/// <param name="go">The game object to check.</param>
		/// <typeparam name="T">The type of component to get.</typeparam>
		/// <returns>A component of type T attached to the given game object if it exists.</returns>
		/// <exception cref="InvalidOperationException">no component of the required type exist on the given game object.
		/// </exception>
		/// <remarks>Use this method when you are sure that the component exists on the game object. 
		/// </remarks>
		public static T GetRequiredComponent<T>(this GameObject go) where T : Component
		{
			var retrievedComponent = go.GetComponent<T>();

			if (retrievedComponent == null)
			{
				throw new InvalidOperationException(
					message: $"GameObject \"{go.name}\" does not have a component of type {typeof(T)}");
			}

			return retrievedComponent;
		}
		
		/// <summary>
		/// Gets a component of the given type in one of the children, or fail if no such component can be found.
		/// </summary>
		/// <param name="go">The game object to check.</param>
		/// <typeparam name="T">The type of component to get.</typeparam>
		/// <returns>A component of type T attached to the given game object if it exists.</returns>
		/// <exception cref="InvalidOperationException">no component of the required type exist on any of the given game
		/// object's children.</exception>
		/// <remarks>Use this method when you are sure that the component exists on the game object. 
		/// </remarks>
		public static T GetRequiredComponentInChildren<T>(this GameObject go) where T : Component
		{
			var retrievedComponent = go.GetComponentInChildren<T>();

			if (retrievedComponent == null)
			{
				throw new InvalidOperationException(
					message: $"GameObject \"{go.name}\" does not have a component of type {typeof(T)}");
			}

			return retrievedComponent;
		}
	}
}
