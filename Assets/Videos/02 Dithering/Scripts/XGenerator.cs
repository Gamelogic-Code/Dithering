using Gamelogic.Extensions.Algorithms;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Contains various <see cref="IGenerator{TResult}"/> implementations. 
	/// </summary>
	// Reuse candidate
	public static class XGenerator
	{
		private class ErrorDiffusionImpl : IGenerator<int>
		{
			private const int NextRaw = 0;
			private readonly float threshold;
			private float accumulatedError = 0;
			private int current;
			
			public int Current => current;
			
			object IGenerator.Current => Current;

			public ErrorDiffusionImpl(float probability)
			{
				threshold = 1 - probability;
				current = NextRaw;
			}

			public void MoveNext()
			{
				accumulatedError += threshold - current;
				current = Quantize(NextRaw + accumulatedError);
			}
			
			private int Quantize(float x) => x < threshold ? 0 : 1;

			IGenerator IGenerator.CloneAndRestart() => CloneAndRestart();
			
			public IGenerator<int> CloneAndRestart() => new ErrorDiffusionImpl(threshold);
		}
		
		private static readonly int[] BayerMatrix = { 0, 4, 1, 5, 2, 6, 3, 7 };
		
		/*
			Blue and Red noise algorithms from

			https://blog.demofox.org/2023/03/06/uniform-1d-red-noise-blue-noise-part-2/
		*/

		private static readonly float[] BlueNoiseFilter = {0.5f, -1.0f, 0.5f};
		private static readonly float[] RedNoiseFilter = {0.25f, 0.5f, 0.25f};

		// ReSharper disable once IdentifierTypo
		private static readonly float[] HornerPolynomials = 
		{
			5.25964f, 0.039474f, 0.000708779f, 0.0f,
			-5.20987f, 7.82905f, -1.93105f, 0.159677f,
			-5.22644f, 7.8272f, -1.91677f, 0.15507f,
			5.23882f, -15.761f, 15.8054f, -4.28323f
		};

		/// <summary>
		/// Generates a stream of blue noise.
		/// </summary>
		/// <remarks>
		/// See: <see href="https://blog.demofox.org/2023/03/06/uniform-1d-red-noise-blue-noise-part-2/"/>.
		/// </remarks>
		public static IGenerator<float> BlueNoise => Generator
			.UniformRandomFloat()
			.Window(3)
			.Select(FilterBlueNoise)
			.Select(NormalizeBlueNoise)
			.Select(MakeUniform);
	
		/// <summary>
		/// Generates a stream of red noise.
		/// </summary>
		/// <remarks>
		/// See: <see href="https://blog.demofox.org/2023/03/06/uniform-1d-red-noise-blue-noise-part-2/"/>.
		/// </remarks>
		public static IGenerator<float> RedNoise => Generator
			.UniformRandomFloat()
			.Window(3)
			.Select(FilterRedNoise)
			.Select(MakeUniform);
		
		/// <summary>
		/// Generates a stream of Bayer 8x1 dithering matrix values.
		/// </summary>
		public static IGenerator<float> Bayer8 => Generator
			.Repeat(BayerMatrix)
			.Select(x => x / 8.0f);

		/// <summary>
		/// Generates a stream of error diffusion dithering values of 0s and 1s with the total given
		/// probability of 0s.
		/// </summary> 
		public static IGenerator<int> ErrorDiffusion(float probability) => new ErrorDiffusionImpl(probability);
	
		private static float Dot3(float[] vec1, float[] vec2) => vec1[0] * vec2[0] + vec1[1] * vec2[1] + vec1[2] * vec2[2];
		private static float FilterBlueNoise(float[] buffer) => Dot3(buffer, BlueNoiseFilter);
		private static float FilterRedNoise(float[] buffer) => Dot3(buffer, RedNoiseFilter);
		private static float NormalizeBlueNoise(float x) => (1 + x) * 0.5f;

		private static float MakeUniform(float x)
		{
			int first = Mathf.Min(Mathf.FloorToInt(x * 4.0f), 3) * 4;

			return HornerPolynomials[first + 3] 
					+ x * (HornerPolynomials[first + 2] 
							+ x * (HornerPolynomials[first + 1] 
									+ x * HornerPolynomials[first + 0]));
		}
	}
}
