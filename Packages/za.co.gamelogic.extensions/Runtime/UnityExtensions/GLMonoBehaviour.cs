// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using Gamelogic.Extensions.Internal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Provides some additional functions for MonoBehaviour.
	/// </summary>
	[Version(1, 0, 0)]
	[AddComponentMenu("Gamelogic/Extensions/GLMonoBehaviour")]
	public class GLMonoBehaviour : MonoBehaviour
	{
		#region Static Methods

#if !(UNITY_5 || UNITY_5_3_OR_NEWER || UNITY_2017_1_OR_NEWER)
		/// <summary>
		/// Instantiates the specified prefab.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prefab">The object.</param>
		/// <returns>T.</returns>
		public static T Instantiate<T>(T prefab) where T : Component
		{
			return (T) Object.Instantiate(prefab);
		}
#endif

#if !UNITY_5_5_OR_NEWER
		/// <summary>
		/// Instantiates an object at the 
		/// given position in the given orientation.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="prefab">The prefab to instantiate.</param>
		/// <param name="position">The position.</param>
		/// <param name="rotation">The rotation.</param>
		/// <returns>T.</returns>
		public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
		{
			var newObject = Instantiate<T>(prefab);

			newObject.transform.position = position;
			newObject.transform.rotation = rotation;

			return newObject;
		}
#endif

		/// <summary>
		/// Instantiates a prefab and attaches it to the given root. 
		/// </summary>
		public static T Instantiate<T>(T prefab, GameObject root) where T : Component
		{
			var newObject = Instantiate(prefab, root.transform, false);

			newObject.transform.ResetLocal();

			return newObject;
		}

		/// <summary>
		/// Instantiates a prefab, attaches it to the given root, and
		/// sets the local position and rotation.
		/// </summary>
		public static T Instantiate<T>(T prefab, GameObject root, Vector3 localPosition, Quaternion localRotation) where T : Component
		{
			var newObject = Instantiate(prefab, root.transform);

			var transform = newObject.transform;
			transform.localPosition = localPosition;
			transform.localRotation = localRotation;
			transform.ResetScale();

			return newObject;
		}

		/// <summary>
		/// Instantiates a prefab.
		/// </summary>
		/// <param name="prefab">The object.</param>
		/// <returns>GameObject.</returns>
		public static GameObject Instantiate(GameObject prefab)
		{
			return Object.Instantiate(prefab);
		}

		/// <summary>
		/// Instantiates the specified prefab.
		/// </summary>
		public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
		{
			var newObject = Object.Instantiate(prefab, position, rotation);

			return newObject;
		}

		/// <summary>
		/// Instantiates a prefab and parents it to the root.
		/// </summary>
		/// <param name="prefab">The prefab.</param>
		/// <param name="root">The root.</param>
		/// <returns>GameObject.</returns>
		public static GameObject Instantiate(GameObject prefab, GameObject root)
		{
			var newObject = Object.Instantiate(prefab, root.transform);
			newObject.transform.ResetLocal();

			return newObject;
		}

		/// <summary>
		/// Instantiates a prefab, attaches it to the given root, and
		/// sets the local position and rotation.
		/// </summary>
		/// <param name="prefab">The prefab.</param>
		/// <param name="root">The root.</param>
		/// <param name="localPosition">The local position.</param>
		/// <param name="localRotation">The local rotation.</param>
		/// <returns>GameObject.</returns>
		public static GameObject Instantiate(GameObject prefab, GameObject root, Vector3 localPosition, Quaternion localRotation)
		{
			var newObject = Object.Instantiate(prefab, root.transform);

			newObject.transform.localPosition = localPosition;
			newObject.transform.localRotation = localRotation;
			newObject.transform.ResetScale();

			return newObject;
		}

		#region Find

		/// <summary>
		/// Similar to FindObjectsOfType, except that it looks for components
		/// that implement a specific interface.
		/// </summary>
		public static List<TInterface> FindObjectsOfInterface<TInterface>() where TInterface : class
		{
			var monoBehaviours = FindObjectsOfType<MonoBehaviour>();

			return monoBehaviours.Select(behaviour => behaviour.GetComponent(typeof (TInterface))).OfType<TInterface>().ToList();
		}

		/// <summary>
		/// Finds a component of the given type in the scene, or fail if no such component can be found.
		/// </summary>
		/// <typeparam name="T">The type of component to find.</typeparam>
		/// <returns>The component of type T if it exists.</returns>
		/// <exception cref="InvalidOperationException">no component of the required type exist in the scene.</exception>
		/// <remarks>Use this method when you know the object exists. You do not need to check if it exists, since
		/// this method does and throws an exception if it does not.</remarks>
		[Version(3, 0, 0)]
		public static T FindRequiredObjectOfType<T>() where T : Object
		{
			var obj = FindObjectOfType<T>();

			if (obj == null)
			{
				throw new InvalidOperationException($"Required an object of type {typeof(T).Name} in the scene");
			}

			return obj;
		}
		#endregion

		#endregion

		#region Public Methods

		[Version(2, 3, 0)]
		public Coroutine Invoke(Action action, float time)
		{
			return MonoBehaviourExtensions.Invoke(this, action, time);
		}

		[Version(2, 3, 0)]
		public Coroutine InvokeRepeating(Action action, float time, float repeatTime)
		{
			return MonoBehaviourExtensions.InvokeRepeating(this, action, time, repeatTime);
		}

		[Version(2, 3, 0)]
		public Coroutine InvokeRepeating(Action action, IGenerator<float> repeatTime)
		{
			return MonoBehaviourExtensions.InvokeRepeating(this, action, repeatTime);
		}

		[Version(2, 3, 0)]
		public Coroutine Tween<T>(
			T start,
			T finish,
			float totalTime,
			Func<T, T, float, T> lerp,
			Action<T> action,
			Func<float> deltaTime)
		{
			return MonoBehaviourExtensions.Tween(this, start, finish, totalTime, lerp, action, deltaTime);
		}

		[Version(2, 3, 0)]
		public Coroutine Tween<T>(
			T start,
			T finish,
			float totalTime,
			Func<T, T, float, T> lerp,
			Action<T> action)
		{
			return MonoBehaviourExtensions.Tween(this, start, finish, totalTime, lerp, action);
		}

		/// <summary>
		/// Gets a component of the given type, or fail if no such component 
		/// is attached to the given component.
		/// </summary>
		/// <typeparam name="T">The type of component to get.</typeparam>
		/// <returns>A component of type T attached to the given component if it exists.
		/// </returns>
		/// <exception cref="InvalidOperationException">When the no component of the 
		/// required type exist on the given component.
		/// </exception>
		public T GetRequiredComponent<T>() where T : Component
		{
			return MonoBehaviourExtensions.GetRequiredComponent<T>(this);
		}

		/// <summary>
		/// Gets a component of the given type in one of the children, or fail if no such component 
		/// is attached to the given component.
		/// </summary>
		/// <typeparam name="T">The type of component to get.</typeparam>
		/// <returns>A component of type T attached to the given component if it exists.
		/// </returns>
		/// <exception cref="InvalidOperationException">When the no component of the 
		/// required type exist on any of the given components children.
		/// </exception>
		public T GetRequiredComponentInChildren<T>() where T : Component
		{
			return MonoBehaviourExtensions.GetRequiredComponentInChildren<T>(this);
		}

		/// <summary>
		/// Destroys given object using either Object.Destroy, or Object.DestroyImmediate,
		/// depending on whether Application.isPlaying is true or not. This is useful when 
		/// writing methods that is used by both editor tools and the game itself.
		/// </summary>
		/// <param name="obj">The object to destroy.</param>
		[Version(2, 5, 0)]
		public static void DestroyUniversal(Object obj)
		{
			if (Application.isPlaying)
			{
				Destroy(obj);
			}
			else
			{
				DestroyImmediate(obj); 
			}
		}

		#endregion
	}

	///<summary>
	/// Provides useful extension methods for MonoBehaviours.
	/// </summary>
	[Version(1, 0, 0)]
	public static class MonoBehaviourExtensions
	{
		#region Cloning

		/// <summary>
		/// Clones an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static T Clone<T>(this T obj) where T:MonoBehaviour
		{
			return Object.Instantiate(obj);
		}

		/// <summary>
		/// Clones an object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static List<T> Clone<T>(this T obj, int count) where T : MonoBehaviour
		{
			var list = new List<T>();

			for (int i = 0; i < count; i++)
			{
				list.Add(obj.Clone());
			}

			return list;
		}

		#endregion

		#region Typesafe methods for scheduling

		/// <summary>
		/// Invokes the given action after the given amount of time.
		/// </summary>
		public static Coroutine Invoke(this MonoBehaviour monoBehaviour, Action action, float time)
		{
			return monoBehaviour.StartCoroutine(InvokeImpl(action, time));
		}

		private static IEnumerator InvokeImpl(Action action, float time)
		{
			yield return new WaitForSeconds(time);

			action();
		}

		/// <summary>
		/// Invokes the given action after the given amount of time, and repeats the 
		/// action after every repeatTime seconds.
		/// </summary>
		public static Coroutine InvokeRepeating(this MonoBehaviour monoBehaviour, Action action, float time, float repeatTime)
		{
			return monoBehaviour.StartCoroutine(InvokeRepeatingImpl(action, time, repeatTime));
		}

		/// <summary>
		/// Invokes the given action after the given amount of time, and repeats the 
		/// action after every repeatTime seconds.
		/// </summary>
		public static Coroutine InvokeRepeating(this MonoBehaviour monoBehaviour, Action action, IGenerator<float> repeatTime)
		{
			return monoBehaviour.StartCoroutine(InvokeRepeatingImpl(action, repeatTime));
		}

		private static IEnumerator InvokeRepeatingImpl(Action action, float time, float repeatTime)
		{
			yield return new WaitForSeconds(time);

			while (true)
			{
				action();
				yield return new WaitForSeconds(repeatTime);
			}
			// ReSharper disable once IteratorNeverReturns
		}

		private static IEnumerator InvokeRepeatingImpl(Action action, IGenerator<float> repeatTime)
		{
			while (true)
			{
				yield return new WaitForSeconds(repeatTime.Next());

				action();
			}
			// ReSharper disable once IteratorNeverReturns
		}

		/// <summary>
		/// Cancels the action if it was scheduled.
		/// </summary>
		[Obsolete("The new Invoke is implemented as a coroutine. Store and cancel the coroutine instead.")]
		public static void CancelInvoke(this MonoBehaviour _1, Action _2)
		{
			throw new NotSupportedException();
		}


		/// <summary>
		/// Returns whether an invoke is pending on an action.
		/// </summary>
		[Obsolete("The new Invoke is implemented as a coroutine. Store and cancel the coroutine instead.")]
		public static bool IsInvoking(this MonoBehaviour _1, Action _2)
		{
			throw new NotSupportedException();
		}

		#endregion

		#region Tweening
		public static Coroutine Tween<T>(this MonoBehaviour monoBehaviour, T start, T finish, float totalTime, Func<T, T, float, T> lerp, Action<T> action)
		{
			return Tween(monoBehaviour, start, finish, totalTime, lerp, action, () => Time.deltaTime);
		}

		public static Coroutine Tween<T>(
			this MonoBehaviour monoBehaviour, 
			T start, 
			T finish, 
			float totalTime, 
			Func<T, T, float, T> lerp, 
			Action<T> action, 
			Func<float> deltaTime)
		{
			return monoBehaviour.StartCoroutine(TweenImpl(start, finish, totalTime, lerp, action, deltaTime));
		}

		private static IEnumerator TweenImpl<T>(
			T start,
			T finish,
			float totalTime,
			Func<T, T, float, T> lerp,
			Action<T> action,
			Func<float> deltaTime)
		{
			float time = 0;
			float t = 0;

			while (t < 1)
			{
				var current = lerp(start, finish, t);
				action(current);

				time += deltaTime();
				t = time / totalTime;

				yield return null;
			}

			action(finish);
		}

		#endregion

		#region Children

		public static GameObject FindChild(this Component component, string childName)
		{
			return component.transform.Find(childName).gameObject;
		}

		public static GameObject FindChild(this Component component, string childName, bool recursive)
		{
			if (recursive) return component.FindChild(childName);

			return FindChildRecursively(component.transform, childName);
		}

		private static GameObject FindChildRecursively(Transform target, string childName)
		{
			if (target.name == childName) return target.gameObject;

			for (var i = 0; i < target.childCount; ++i)
			{
				var result = FindChildRecursively(target.GetChild(i), childName);

				if (result != null) return result;
			}

			return null;
		}

		/// <summary>
		/// Finds a component of the type T in on the same object, or on a child down the hierarchy. This method also works
		/// in the editor and when the game object is inactive.
		/// </summary>
		[Version(1, 1, 0)]
		public static T GetComponentInChildrenAlways<T>(this Component component) where T : Component
		{
			foreach (var child in component.transform.SelfAndAllChildren())
			{
				var componentInChild = child.GetComponent<T>();

				if (componentInChild != null)
				{
					return componentInChild;
				}
			}

			return null;
		}

		/// <summary>
		/// Finds all components of the type T on the same object and on a children down the hierarchy. This method also works
		///	in the editor and when the game object is inactive.
		/// </summary>
		[Version(1,1, 0)]
		public static T[] GetComponentsInChildrenAlways<T>(this Component component) where T : Component
		{
			var components = new List<T>();

			foreach (var child in component.transform.SelfAndAllChildren())
			{
				var componentsInChild = child.GetComponents<T>();

				if (componentsInChild != null)
				{
					components.AddRange(componentsInChild);
				}
			}

			return components.ToArray();
		}

		#endregion

		#region Components

		/// <summary>
		/// Gets a component of the given type, or fail if no such component 
		/// is attached to the given component.
		/// </summary>
		/// <typeparam name="T">The type of component to get.</typeparam>
		/// <param name="component">The component to check.</param>
		/// <returns>A component of type T attached to the given component if it exists.
		/// </returns>
		/// <exception cref="InvalidOperationException">When the no component of the 
		/// required type exist on the given component.
		/// </exception>
		public static T GetRequiredComponent<T>(this Component component) where T : Component
		{
			var retrievedComponent = component.GetComponent<T>();

			if (retrievedComponent == null)
			{
				throw new InvalidOperationException(
					$"GameObject \"{component.name}\" ({component.GetType()}) does not have a component of type {typeof(T)}");
			}

			return retrievedComponent;
		}

		/// <summary>
		/// Gets a component of the given type in one of the children, or fail if no such component 
		/// is attached to the given component.
		/// </summary>
		/// <typeparam name="T">The type of component to get.</typeparam>
		/// <param name="component">The component to check.</param>
		/// <returns>A component of type T attached to the given component if it exists.
		/// </returns>
		/// <exception cref="InvalidOperationException">When the no component of the 
		/// required type exist on any of the given components children.
		/// </exception>
		public static T GetRequiredComponentInChildren<T>(this Component component) where T : Component
		{
			var retrievedComponent = component.GetComponentInChildren<T>();

			if (retrievedComponent == null)
			{
				throw new InvalidOperationException(
					$"GameObject \"{component.name}\" ({component.GetType()}) does not have a child with component of type {typeof(T)}");
			}

			return retrievedComponent;
		}

		/// <summary>
		/// Gets an attached component that implements the interface of the type parameter.
		/// </summary>
		/// <typeparam name="TInterface">The type of the t interface.</typeparam>
		/// <param name="component">The component to get the interface component from.</param>
		/// <returns>TInterface.</returns>
		public static TInterface GetInterfaceComponent<TInterface>(this Component component) where TInterface : class
		{
			return component.GetComponent(typeof(TInterface)) as TInterface;
		}

		#endregion
	}
}
