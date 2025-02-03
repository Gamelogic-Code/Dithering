using System;
using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;
// ReSharper disable InvalidXmlDocComment - this is for docs inherited so the warning does not apply. 

namespace Gamelogic.Extensions
{
	/// <summary>
	/// A factory class for creating instances of types derived from <typeparamref name="TBase"/>.
	/// </summary>
	/// <typeparam name="TBase">The base type that all created instances must derive from.</typeparam>
	/// <remarks><typeparamref name="TBase"/> is often a common interface or abstract type, and
	/// the registered instances of different types that implements the base type. This is useful when
	/// you want to choose the implementation based on a type parameter.
	/// </remarks>
	/// <example>
	/// One use case is to make it easier to write unit tests for a set of types that share an interface.
	///
	/// [!code-csharp[ImplementationFactoryTestExample](../../Assets/DocumentationCode/Editor/ImplementationFactoryTestExample.cs)]
	/// </example>
	[Version(3, 2, 0)]
	public class ImplementationFactory<TBase> : IEnumerable<Func<TBase>>
	{
		private readonly Dictionary<Type, Func<TBase>> factories = new Dictionary<Type, Func<TBase>>();

		/// <summary>
		/// Registers a factory method for creating instances of <typeparamref name="TImplementation"/>.
		/// </summary>
		/// <typeparam name="TImplementation">The type of object to be created by the factory method.</typeparam>
		/// <param name="factory">The factory method that creates instances of <typeparamref name="TImplementation"/>.
		/// </param>
		public void Add<TImplementation>(Func<TImplementation> factory)
			where TImplementation : TBase
		{
			factories.Add(typeof(TImplementation), () => factory());
		}

		/// <summary>
		/// Gets an instance of <typeparamref name="TImplementation"/> using the registered factory method.
		/// </summary>
		/// <typeparam name="TImplementation">The type of object to be created.</typeparam>
		/// <returns>An instance of <typeparamref name="TImplementation"/>.</returns>
		/// <exception cref="InvalidOperationException">Thrown when no factory method is registered for the specified type.</exception>
		public TImplementation GetInstance<TImplementation>()
			where TImplementation : TBase
		{
			var type = typeof(TImplementation);

			if (factories.TryGetValue(type, out var constructor))
			{
				return (TImplementation)constructor();
			}

			throw new InvalidOperationException($"No constructor found for type: {type}.");
		}

		/// <summary>
		/// Returns an enumerator that iterates through the registered factory methods.
		/// </summary>
		/// <returns>An enumerator for the registered factory methods.</returns>
		public IEnumerator<Func<TBase>> GetEnumerator() => factories.Values.GetEnumerator();

		/// <summary>
		/// Returns an enumerator that iterates through the registered factory methods.
		/// </summary>
		/// <returns>An enumerator for the registered factory methods.</returns>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <inheritdoc cref="ImplementationFactory{TBase}"/>
	/// <typeparam name="T1">The type of argument1 for factory methods.</typeparam>
	[Version(3, 2, 0)]
	public class ImplementationFactory<T1, TBase> : IEnumerable<Func<T1, TBase>>
	{
		private readonly Dictionary<Type, Func<T1, TBase>> factories = new Dictionary<Type, Func<T1, TBase>>();

		/// <inheritdoc cref="ImplementationFactory{TBase}.Add{TImplementation}"/>
		public void Add<TImplementation>(Func<T1, TImplementation> factory)
			where TImplementation : TBase
		{
			factories.Add(typeof(TImplementation), arg1 => factory(arg1));
		}

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetInstance{TImplementation}"/>
		/// <param name="arg1">The first argument to pass to the factory method.</param>
		public TImplementation GetInstance<TImplementation>(T1 arg1)
			where TImplementation : TBase
		{
			var type = typeof(TImplementation);

			if (factories.TryGetValue(type, out var constructor))
			{
				return (TImplementation)constructor(arg1);
			}

			throw new InvalidOperationException($"No constructor found for type: {type}.");
		}

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		public IEnumerator<Func<T1, TBase>> GetEnumerator() => factories.Values.GetEnumerator();

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <inheritdoc cref="ImplementationFactory{TBase, T1}"/>
	/// <typeparam name="T2">The type of argument2 for factory methods.</typeparam>
	[Version(3, 2, 0)]
	public class ImplementationFactory<T1, T2, TBase> : IEnumerable<Func<T1, T2, TBase>>
	{
		private readonly Dictionary<Type, Func<T1, T2, TBase>> factories = new Dictionary<Type, Func<T1, T2, TBase>>();

		/// <inheritdoc cref="ImplementationFactory{TBase}.Add{TImplementation}"/>
		public void Add<TImplementation>(Func<T1, T2, TImplementation> factory)
			where TImplementation : TBase
		{
			factories.Add(typeof(TImplementation), (arg1, arg2) => factory(arg1, arg2));
		}

		/// <inheritdoc cref="ImplementationFactory{T1,TBase}.GetInstance{TImplementation}"/>
		/// <param name="arg2">The second argument to pass to the factory method.</param>
		public TImplementation GetInstance<TImplementation>(T1 arg1, T2 arg2)
			where TImplementation : TBase
		{
			var type = typeof(TImplementation);

			if (factories.TryGetValue(type, out var constructor))
			{
				return (TImplementation)constructor(arg1, arg2);
			}

			throw new InvalidOperationException($"No constructor found for type: {type}.");
		}

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		public IEnumerator<Func<T1, T2, TBase>> GetEnumerator() => factories.Values.GetEnumerator();
		
		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <inheritdoc cref="ImplementationFactory{TBase, T1, T2}"/>
	/// <typeparam name="T3">The type of argument3 for factory methods.</typeparam>
	[Version(3, 2, 0)]
	public class ImplementationFactory<T1, T2, T3, TBase> : IEnumerable<Func<T1, T2, T3, TBase>>
	{
		private readonly Dictionary<Type, Func<T1, T2, T3, TBase>> factories = new Dictionary<Type, Func<T1, T2, T3, TBase>>();

		/// <inheritdoc cref="ImplementationFactory{TBase}.Add{TImplementation}"/>
		public void Add<TImplementation>(Func<T1, T2, T3, TImplementation> factory)
			where TImplementation : TBase
		{
			factories.Add(typeof(TImplementation), (arg1, arg2, arg3) => factory(arg1, arg2, arg3));
		}

		/// <inheritdoc cref="ImplementationFactory{T1,T2,TBase}.GetInstance{TImplementation}"/>
		/// <param name="arg3">The third argument to pass to the factory method.</param>
		public TImplementation GetInstance<TImplementation>(T1 arg1, T2 arg2, T3 arg3)
			where TImplementation : TBase
		{
			var type = typeof(TImplementation);

			if (factories.TryGetValue(type, out var constructor))
			{
				return (TImplementation)constructor(arg1, arg2, arg3);
			}

			throw new InvalidOperationException($"No constructor found for type: {type}.");
		}

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		public IEnumerator<Func<T1, T2, T3, TBase>> GetEnumerator() => factories.Values.GetEnumerator();

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}

	/// <inheritdoc cref="ImplementationFactory{TBase, T1, T2, T3}"/>
	/// <typeparam name="T4">The type of argument4 for factory methods.</typeparam>
	[Version(3, 2, 0)]
	public class ImplementationFactory<T1, T2, T3, T4, TBase> : IEnumerable<Func<T1, T2, T3, T4, TBase>>
	{
		private readonly Dictionary<Type, Func<T1, T2, T3, T4, TBase>> factories = new Dictionary<Type, Func<T1, T2, T3, T4, TBase>>();

		/// <inheritdoc cref="ImplementationFactory{TBase}.Add{TImplementation}"/>
		public void Add<TImplementation>(Func<T1, T2, T3, T4, TImplementation> factory)
			where TImplementation : TBase
		{
			factories.Add(typeof(TImplementation), (arg1, arg2, arg3, arg4) => factory(arg1, arg2, arg3, arg4));
		}

		/// <inheritdoc cref="ImplementationFactory{T1,T2,T3,TBase}.GetInstance{TImplementation}"/>
		/// <param name="arg4">The fourth argument to pass to the factory method.</param>
		public TImplementation GetInstance<TImplementation>(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
			where TImplementation : TBase
		{
			var type = typeof(TImplementation);

			if (factories.TryGetValue(type, out var constructor))
			{
				return (TImplementation)constructor(arg1, arg2, arg3, arg4);
			}

			throw new InvalidOperationException($"No constructor found for type: {type}.");
		}

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		public IEnumerator<Func<T1, T2, T3, T4, TBase>> GetEnumerator() => factories.Values.GetEnumerator();

		/// <inheritdoc cref="ImplementationFactory{TBase}.GetEnumerator"/>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
