// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// This interface represents a piecewise linear curve, with input-output 
	/// pairs at the bends. 
	/// </summary>
	/// <typeparam name="T">The type of the output, usually a type that can be interpolated.</typeparam>
	/// <remarks>
	/// <para>This class is the base of the that described in AI Programming 
	/// Wisdom 1, "The Beauty of Response Curves", by Bob Alexander.</para>
	/// <para>The inputs need not be spread uniformly.</para>
	/// <para>You can implement this interface directly, or use the
	/// <see cref="ResponseCurveBase{T}"/> class as a base, which is often more convenient for interpolatable types.</para>
	/// <para>It can make sense to implement this interface for discrete types, although often broader concept is needed,
	/// making this interface unsuitable. For example, you could implement a class <c>InRange</c> that maps inputs to
	/// <see langword="true"/> if they are in a given range, and <see langword="false"/> otherwise. But this class would
	/// make sense for other inputs than float too, so limiting it by implementing this interface is not good.</para>
	/// </remarks>
	[Version(3, 2, 0)]
	public interface IResponseCurve<out T>
	{
		#region Properties

		/// <summary>
		/// Evaluates the curve at the given input and returns the result.
		/// </summary>
		/// <param name="input">The input for which output is sought.</param>
		/// <remarks>
		/// <para>
		/// If the input is below the inputMin given in the constructor, 
		/// the output is clamped to the first output sample.
		/// </para>
		/// <para>
		/// If the input is above the inputMax given in the constructor,
		/// the output is clamped to the last output sample.
		/// </para>
		/// <para>
		/// Otherwise, an index is calculated, and the output is interpolated
		/// between <c>outputSample[index]</c> and <c>outputSample[index + 1]</c>.
		/// </para>
		/// </remarks>
		T this[float input]
		{
			get;
		}

		#endregion

		#region Public methods
		
		/// <inheritdoc cref="this[float]"/>
		T Evaluate(float input);
		#endregion
	}
}
