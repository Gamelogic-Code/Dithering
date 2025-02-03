using System;
using Gamelogic.Extensions.Support;
using Gamelogic.Extensions.Algorithms;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Represents a snapshot of a value, capturing its current and previous states.
	/// </summary>
	/// <typeparam name="T">The type of the value being tracked.</typeparam>
	/// <remarks>This type is mostly useful as a base for other types. See for example <see cref="ObservedValue{T}"/>.</remarks>
	[Version(4,1, 0)]
	public class ValueSnapshot<T>
	{
		private const string ValueNotSet = "[Value not set]";
	
		private const string ValueNotAvailable = nameof(Value) + "is not available until the value has been set at least once.";
		private const string PreviousValueNotAvailable = nameof(PreviousValue) + "is not available until the value has been set at least twice.";

		/// <summary>
		/// Buffer to store the current and previous values.
		/// </summary>
		private readonly Capacity2Buffer<T> buffer = new Capacity2Buffer<T>();

		/// <summary>
		/// Gets or sets the current value of this snapshot. 
		/// </summary>
		/// <remarks>If this snapshot has been constructed or reset without an initial value, setting the value for the first time will not raise any events.</remarks>
		/// <exception cref="InvalidOperationException">Thrown when trying to get a value that hasn't been set yet.</exception>
		public virtual T Value
		{
			get => HasValue ? buffer.Last : throw new InvalidOperationException(ValueNotAvailable);
			set => buffer.Insert(value);
		}

		/// <summary>
		/// Gets the previous value assigned to this snapshot.
		/// </summary>
		/// <exception cref="InvalidOperationException">Thrown when trying to get a previous value that hasn't been set yet.</exception>
		public T PreviousValue
			=> HasPreviousValue ? buffer.First : throw new InvalidOperationException(PreviousValueNotAvailable);

		/// <summary>
		/// Gets a value indicating whether the <see cref="Value"/> has been set.
		/// </summary>
		[MemberNotNullWhen(true, nameof(Value))]
		public bool HasValue => buffer.Count > 0;

		/// <summary>
		/// Gets a value indicating whether the <see cref="Value"/> has been set more than once, and thus has a <see cref="PreviousValue"/>.
		/// </summary>
		[MemberNotNullWhen(true, nameof(PreviousValue))]
		public bool HasPreviousValue => buffer.Count > 1;

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueSnapshot{T}"/> class.
		/// </summary>
		public ValueSnapshot()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValueSnapshot{T}"/> class with an initial value.
		/// </summary>
		/// <param name="initialValue">The initial value to set.</param>
		public ValueSnapshot(T initialValue)
		{
			buffer.Insert(initialValue);
		}

		/// <summary>
		/// Resets the value of this snapshot to its initial state.
		/// </summary>
		/// <remarks><see cref="HasValue"/> will be <see langword="false"/> until <see cref="Value"/> has been assigned.</remarks>
		public void Reset() => buffer.Clear();

		/// <summary>
		/// Resets the value of this snapshot with an initial value.
		/// </summary>
		/// <param name="initialValue">The value this snapshot is initialized to.</param>
		/// <remarks><see cref="HasValue"/> will be <see langword="true"/>, but <see cref="HasPreviousValue"/> will be <see langword="false"/> until <see cref="Value"/> has been assigned again.</remarks>
		public void Reset(T initialValue)
		{
			Reset();
			buffer.Insert(initialValue);
		}

		/// <inheritdoc/>
		// TODO: Delegate this to a formatter class that handles empty and null values appropriately.
		public override string ToString() 
			=> !HasValue 
				? ValueNotSet 
				: Value.ToString();
	}
}
