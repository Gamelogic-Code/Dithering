using System;
using System.Collections.Generic;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Wrapper that can raise an event when a value changes. 
	/// </summary>
	/// <typeparam name="T">The type of value to track.</typeparam>
	public sealed class ObservedValue<T> : ValueSnapshot<T>
	{
		private readonly IEqualityComparer<T> comparer;
	
		/// <summary>
		/// Gets or sets and sets the value of this <see cref="ObservedValue{T}"/>, and fire the
		/// <see cref="OnValueChange"/> event if the event has changed from its last value if
		/// <see cref="ObservedValue{T}.TrackingEnabled"/> is <see langword="true"/>.
		/// </summary>
		/// <remarks>If this <see cref="ObservedValue{T}"/> has been constructed or reset without an initial value,
		/// setting the value
		/// for the first time will not raise the <see cref="OnValueChange"/>.</remarks>
		public override T Value
		{
			get => base.Value;
		
			set
			{
				base.Value = value;

				if (TrackingEnabled && HasPreviousValue && !comparer.Equals(Value, PreviousValue))
				{
					OnValueChange?.Invoke();
				}
			}
		}
	
		/// <summary>
		/// Gets or sets a value indicating whether tracking is enabled for this <see cref="ObservedValue{T}"/>.
		/// </summary>
		/// <remarks>When <see langword="false"/>, <see cref="OnValueChange"/> will not be raised when the value of these
		/// <see cref="ObservedValue{T}"/> changes.</remarks>
		public bool TrackingEnabled { get; set; }

		// ReSharper disable once InvalidXmlDocComment
		// Used to document the action argument
	
		/// <summary>
		/// Raised when <see cref="Value"/> has changed from its previous value and <see cref="TrackingEnabled"/> is true.
		/// </summary>
		/// <param name="arg">The caller.</param>
		public event Action OnValueChange;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservedValue{T}"/> class.
		/// </summary>
		/// <param name="comparer">The comparer used tp compare values. If not provided, the
		/// <see cref="EqualityComparer{T}.Default"/> will be used.</param>
		/// <remarks>
		/// <para><see cref="OnValueChange"/>will not be raised the first time <see cref="Value"/> is being assigned.
		/// </para>
		/// <para><see cref="TrackingEnabled"/> is <see langword="true"/> initially.</para>
		/// </remarks>
		public ObservedValue(IEqualityComparer<T> comparer = null)
		{
			TrackingEnabled = true;
			this.comparer = comparer ?? EqualityComparer<T>.Default;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObservedValue{T}"/> class.
		/// </summary>
		/// <param name="initialValue">The value this change detector is initialized to.</param>
		/// <param name="comparer">The comparer used tp compare values. If not provided, the
		/// <see cref="EqualityComparer{T}.Default"/> will be used.</param>
		/// <remarks><see cref="TrackingEnabled"/> is <see langword="true"/> initially.</remarks>
		public ObservedValue(T initialValue, IEqualityComparer<T> comparer = null)
			: this(comparer)
			=> Value = initialValue;

		/// <inheritdoc/>
		public override string ToString() => HasValue ? Value.ToString() : string.Empty;
		
		/// <summary>
		/// Sets the value without notification.
		/// </summary>
		/// <param name="value">The value.</param>
		public void SetSilently(T value)
		{
			// Set base value to skip notification.
			base.Value = value; 
		}
	}
}
