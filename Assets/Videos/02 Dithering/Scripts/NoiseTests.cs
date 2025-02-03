using System;
using System.Linq;
using System.Numerics;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Generates staistics an c harts to test the mean and frequency distribution of noise sequences.
	/// Used to verify the correctness of the noise generation streams.  
	/// </summary>
	public class NoiseTests : GLMonoBehaviour
	{
		[SerializeField] private int sampleCount;
		[SerializeField] private float threshold;

		[SerializeField] private Vector2Int dimensions;
		[SerializeField] private Color barColor;
		[SerializeField] private int barCount = 3;

		[SerializeField] private RawImage blueNoise;
		[SerializeField] private RawImage redNoise;
		[SerializeField] private RawImage uniformNoise;
		[SerializeField] private RawImage errorDiffusion;

		public void Start() => Test();

		[InspectorButton]
		private void Test()
		{
			var generators = new[]
			{
				XGenerator.BlueNoise.Select(Quantize),
				XGenerator.RedNoise.Select(Quantize),
				Generator.UniformRandomFloat().Select(Quantize),
				XGenerator.ErrorDiffusion(threshold)
			};
		
			var samples = generators.Select(Sample).ToArray();
			var fractionOnes = samples.Select(CalcFractionOnes).ToArray();	
		
			Debug.Log(fractionOnes.ListToString());

			var fft =
				samples
					.Select(ToComplexAr)
					.Select(ToFrequency)
					.Select(ToMagnitudeAr)
					.ToArray();
		
			Debug.Log(fft[0][0]);
			Debug.Log(fft[1][0]);
			Debug.Log(fft[2][0]);
			Debug.Log(fft[3][0]);
		
			var textures = fft.Select(GenerateSpectrumImage).ToArray();
			//var textures = samples
			//	.Select(ToFloatAr)
			//	.Select(GenerateSpectrumImage).ToArray();
		
			blueNoise.texture = textures[0];
			redNoise.texture = textures[1];
			uniformNoise.texture = textures[2];
			errorDiffusion.texture = textures[3];

			float ToFloat(int x) => x;
			float[] ToFloatAr(int[] sample) => sample.Select(ToFloat).ToArray();
			int[] Sample(IGenerator<int> generator) => generator.Next(sampleCount).ToArray();
			float CalcFractionOnes(int[] sample) => sample.Count(x => x == 1) / (float) sampleCount;
			Complex ToComplex(int x) => new(x, 0);
			Complex[] ToComplexAr(int[] sample) => sample.Select(ToComplex).ToArray();
			Complex[] ToFrequency(Complex[] sample) => DitFFT2(sample.ToArray(), sample.Length, 1, 0);
			float ToMagnitude(Complex c) => (float) c.Magnitude;
		
			float[] ToMagnitudeAr(Complex[] sample) => sample.Select(ToMagnitude).ToArray();

		}
	
		private int Quantize(float x) => x < threshold ? 0 : 1;
	
		// Overloaded method for odd elements
		private Complex[] DitFFT2(Complex[] x, int N, int s, int offset)
		{
			if (N == 1)
			{
				return new Complex[] { x[offset] }; // Return a new array with a single element
			}

			// Recursively compute FFT for even and odd indices
			Complex[] evenFFT = DitFFT2(x, N / 2, 2 * s, offset);
			Complex[] oddFFT = DitFFT2(x, N / 2, 2 * s, offset + s);

			Complex[] result = new Complex[N];

			// Combine results
			for (int k = 0; k < N / 2; k++)
			{
				Complex twiddle = Complex.Exp(new Complex(0, -2 * Math.PI * k / N)) * oddFFT[k];

				result[k] = evenFFT[k] + twiddle;
				result[k + N / 2] = evenFFT[k] - twiddle;
			}

			return result;
		}


		private Texture2D GenerateSpectrumImage(float[] amplitudes)
		{
			Texture2D texture = new Texture2D(dimensions.x, dimensions.y);
			Color backgroundColor = Color.white;

			// Clear image
			for (int x = 0; x < dimensions.x; x++)
			{
				for (int y = 0; y < dimensions.y; y++)
				{
					texture.SetPixel(x, y, backgroundColor);
				}
			}
		
			int barWidth = dimensions.x / barCount;

			float[] buckets = new float[barCount];

			for (int i = 0; i < amplitudes.Length; i++)
			{
				int bucketIndex = Mathf.FloorToInt(i / (float) amplitudes.Length * buckets.Length);

				if (bucketIndex == buckets.Length)
				{
					bucketIndex--;
				}
			
				buckets[bucketIndex] += amplitudes[i];
			}
		
			float maxAmplitude = buckets.Max();
			if(maxAmplitude == 0) return texture;

			//Debug.Log(maxAmplitude);
			//Debug.Log(buckets.ListToString());

			// Draw bars
			for (int i = 0; i < barCount; i++)
			{
				int xStart = i * barWidth;
				int barHeight = Mathf.RoundToInt(buckets[i] / maxAmplitude * dimensions.y);

				/*if (barHeight > 0)
			{
				Debug.Log("barHeight = " + barHeight);
			}*/

				for (int x = xStart; x < xStart + barWidth; x++)
				{
					for (int y = 0; y < barHeight; y++)
					{
						texture.SetPixel(x, y, barColor);
					}
				}
			}

			texture.Apply();
			return texture;
		}

	}
}
