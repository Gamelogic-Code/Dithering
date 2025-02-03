// Copyright Gamelogic (c) http://www.gamelogic.co.za

using System;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// A class that can be used as the base of the implementation of a response curve. 
	/// </summary>
	/// <list type="T">The type of the output of the curve. This type should be interpolateable, so
	/// it is typicall a continuous type such as <see cref="float"/>, <see cref="Vector3"/>, or <see cref="Color"/>.</list>
	/// <remarks>This is a more convenient way to implement the <see cref="IResponseCurve{T}"/> interface, since all
	/// you need to do is specify how the type is interpolated by implementing the <see cref="Lerp(T,T,float)"/> method.
	/// <para>
	/// There are cases where it makes sense to implement <see cref="IResponseCurve{T}"/> for a discrete type T, but in
	/// those cases this class is not a suitable base, as it is obscure to describe the mapping using interpolation.
	/// </para>
	/// </remarks>
	[Version(1, 2, 0)]
	public abstract class ResponseCurveBase<T>: IResponseCurve<T>
	{
		#region Private Fields

		private readonly List<T> outputSamples;
		private readonly List<float> inputSamples;

		#endregion

		#region Properties
		/// <inheritdoc/>
		public T this[float input] => Evaluate(input);

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ResponseCurveBase{T}"/> class.
		/// </summary>
		/// <param name="inputSamples">Samples of input. Assumes input is monotonically increasing.</param>
		/// <param name="outputSamples">Samples of outputs.</param>
		protected ResponseCurveBase(IEnumerable<float> inputSamples, IEnumerable<T> outputSamples)
		{
			var input = inputSamples.AsList();
			var output = outputSamples.AsList();
			
			int minCount = Mathf.Min(input.Count, output.Count);

			if (minCount < 2)
			{
				throw new ArgumentException("There must be at least two samples");
			}

			if(!IsStrictlyMonotonic(input))
			{
				throw new ArgumentException("Input samples must be strictly monotonic");
			}

			this.outputSamples = new List<T>(output);
			this.inputSamples = new List<float>(input);
		}

		#endregion

		#region Protected Methods
		/// <summary>
		/// Linearly interpolates between the two given samples.
		/// </summary>
		/// <param name="outputSampleMin">The value when t is less than or equal to 0.</param>
		/// <param name="outputSampleMax">The value when t is greater than or equal to 1.</param>
		/// <param name="t">The fraction of the minimum sample to use.</param>
		protected abstract T Lerp(T outputSampleMin, T outputSampleMax, float t);

		#endregion

		#region Public methods		
		/// <summary>
		/// Evaluates the curve at the specified value.
		/// </summary>
		/// <param name="t">The value at which to evaluate the curve.</param>
		/// <remarks>Equivalent to <c>curve[t]</c>.</remarks>
		public T Evaluate(float t)
		{
			int index = SearchInput(t);

			float inputSampleMin = inputSamples[index];
			float inputSampleMax = inputSamples[index + 1];

			var outputSampleMin = outputSamples[index];
			var outputSampleMax = outputSamples[index + 1];

			return Lerp(t, inputSampleMin, inputSampleMax, outputSampleMin, outputSampleMax);
		}
		#endregion

		#region Private Methods

		/// <summary>
		/// Returns the biggest index i such that <c>mInput[i] &amp;= fInputValue</c>.
		/// </summary>
		private int SearchInput(float input)
		{
			if (input< inputSamples[0])
			{
				return 0;
			}

			if (input > inputSamples[inputSamples.Count - 2])
			{
				return inputSamples.Count - 2; //return the but-last node
			}

			return SearchInput(input, 0, inputSamples.Count - 2);
		}

		private int SearchInput(float input, int start, int end)
		{
			while (true)
			{
				if (end - start <= 1)
				{
					return start;
				}

				int mid = (end - start) / 2 + start;
				float midValue = inputSamples[mid];

				if (input.CompareTo(midValue) > 0)
				{
					start = mid;
				}
				else
				{
					end = mid;
				}
			}
		}

		private T Lerp(float input, float inputSampleMin, float inputSampleMax, T outputSampleMin,
			T outputSampleMax)
		{
			if (input <= inputSampleMin)
			{
				return outputSampleMin;
			}

			if (input >= inputSampleMax)
			{
				return outputSampleMax;
			}

			float t = (input - inputSampleMin) / (inputSampleMax - inputSampleMin);

			var output = Lerp(outputSampleMin, outputSampleMax, t);

			return output;
		}

		private bool IsStrictlyMonotonic(IReadOnlyList<float> list)
		{
			float previous = list.First();

			foreach(float item in list.Skip(1))
			{
				if (previous >= item)
				{
					return false;
				}

				previous = item;
			}

			return true;
		}
		#endregion
	}
}
