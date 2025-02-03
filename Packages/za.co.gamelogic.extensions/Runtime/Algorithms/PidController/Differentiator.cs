using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Represents a differentiator that calculates the difference between two consecutive float values.
	/// </summary>
	/// <remarks>
	/// The differentiator assumes a constant sample rate. Therefore, while technically the derivative
	/// requires division by time, this constant can be absorbed by <see cref="PidController"/>, allowing
	/// the differentiator to focus solely on the difference between values.
	/// </remarks>
	[Version(4, 1, 0)]
	public sealed class Differentiator : ValueSnapshot<float>
	{
		private float difference;

		/// <summary>
		/// Gets the difference between the current and previous value of this <see cref="Differentiator"/>.
		/// </summary>
		/// <remarks>Technically to be a derivative we need to divide by the time. If we assume a constant
		/// sample rate, this is a constant, that can be absorbed by the PID filter. 
		/// </remarks>
		/// <exception cref="ValueSnapshot{T}.HasPreviousValue"><see cref="ValueSnapshot{T}"/> is false.</exception>
		public float Difference =>
			HasPreviousValue
				? difference
				: throw new InvalidOperationException("Not enough values set to calculate a derivative");

		/// <summary>
		/// Gets or sets the value of this <see cref="Differentiator"/>.
		/// </summary>
		public override float Value
		{
			get => base.Value;
			set
			{
				base.Value = value;

				if (HasPreviousValue)
				{
					difference = Value - PreviousValue;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Differentiator"/> class.
		/// </summary>
		public Differentiator()
		{
		}
	}
}
