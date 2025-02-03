using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Experimental.Editor
{
	/// <summary>
	/// An editor tool that allows you to reduce the actual color depth of an image with dithering, or the resolution with
	/// anti-aliasing. This is mostly to demonstrate the principle of color depth and resolution trade-off. 
	/// </summary>
	public class ResolutionColorDepthTradeoff : EditorWindow
	{
		private const string ResolutionColorDepthTradeoffName = "Resolution Color-Depth Tradeoff";
		private Texture2D inputTexture;
		private int quantizationLevels = 4;

		private enum TransformationMode
		{
			None,
			DownscaleAntialias,
			UpscaleDither
		}

		private TransformationMode transformationMode;
		private Texture2D outputTexture;

		[MenuItem("Gamelogic/Tools/" + ResolutionColorDepthTradeoffName)]
		public static void ShowWindow() => GetWindow<ResolutionColorDepthTradeoff>(ResolutionColorDepthTradeoffName);

		private void OnGUI()
		{
			GUILayout.Label("ResolutionColorDepthTradeoffName", EditorStyles.boldLabel);

			inputTexture = (Texture2D)EditorGUILayout.ObjectField("Input Texture", inputTexture, typeof(Texture2D), false);
			quantizationLevels = EditorGUILayout.IntField("Quantization Levels (POT)", quantizationLevels);
			transformationMode = (TransformationMode)EditorGUILayout.EnumPopup("Transformation Mode", transformationMode);

			if (inputTexture != null)
			{
				Debug.Log("Process Image: " + inputTexture.width + " " + inputTexture.height);
			}
		
			if (GUILayout.Button("Process Image"))
			{
				ProcessImage();
			}

			if (outputTexture != null)
			{
				GUILayout.Label("Output Preview:");
				GUILayout.Label(AssetPreview.GetAssetPreview(outputTexture), GUILayout.MinWidth(512), GUILayout.MinWidth(512));

				Debug.Log("Output Texture: " + outputTexture.width + " " + outputTexture.height);
			
				if (GUILayout.Button("Save Output Texture"))
				{
					string path = EditorUtility.SaveFilePanel("Save Texture", "", "output.png", "png");
					if (!string.IsNullOrEmpty(path))
					{
						SaveOutputTexture(path);
					}
				}
			}
		}

		private void ProcessImage()
		{
			if (inputTexture == null)
			{
				Debug.LogError("Input texture is not assigned.");
				return;
			}

			var grayscaleTexture = ConvertToGrayscale(inputTexture);
			var quantizedTexture = QuantizeTexture(grayscaleTexture, quantizationLevels);

			outputTexture = transformationMode switch
			{
				TransformationMode.None => quantizedTexture,
				TransformationMode.DownscaleAntialias => DownscaleAntialias(quantizedTexture),
				TransformationMode.UpscaleDither => UpscaleDither(quantizedTexture, quantizationLevels),
				_ => outputTexture
			};

			Debug.Log("Image processing complete.");
		}

		private Texture2D ConvertToGrayscale(Texture2D texture)
		{
			var grayscale = new Texture2D(texture.width, texture.height);
			var pixels = texture.GetPixels();
		
			for (int i = 0; i < pixels.Length; i++)
			{
				float grayscaleValue = Vector3.Dot(new Vector3(pixels[i].r, pixels[i].g, pixels[i].b), new Vector3(0.2126f, 0.7152f, 0.0722f));
				pixels[i] = new Color(grayscaleValue, grayscaleValue, grayscaleValue, pixels[i].a);
			}

			grayscale.SetPixels(pixels);
			grayscale.Apply();
			return grayscale;
		}

		private Texture2D QuantizeTexture(Texture2D texture, int levelCount)
		{
			if (levelCount <= 0 || (levelCount & (levelCount - 1)) != 0)
			{
				Debug.LogError("Quantization levels must be a power of two.");
				return texture;
			}

			var quantized = new Texture2D(texture.width, texture.height);
			var pixels = texture.GetPixels();
		
			for (int i = 0; i < pixels.Length; i++)
			{
				float quantizedValue = Mathf.Floor(pixels[i].r * levelCount) / levelCount;
				pixels[i] = new Color(quantizedValue, quantizedValue, quantizedValue, pixels[i].a);
			}

			quantized.SetPixels(pixels);
			quantized.Apply();
			return quantized;
		}

		private Texture2D DownscaleAntialias(Texture2D texture)
		{
			int newWidth = texture.width / 2;
			int newHeight = texture.height / 2;
		
			var downSampled = new Texture2D(newWidth, newHeight);

			for (int i = 0; i < newWidth; i++)
			{
				for (int j = 0; j < newHeight; j++)
				{
					var color0 = texture.GetPixel(i * 2, j * 2);
					var color1 = texture.GetPixel(i * 2 + 1, j * 2);
					var color2 = texture.GetPixel(i * 2, j * 2 + 1);
					var color3 = texture.GetPixel(i * 2 + 1, j * 2 + 1);
				
					var averageColor = (color0 + color1 + color2 + color3) / 4;
					downSampled.SetPixel(i, j, averageColor);
				}
			}
		
			downSampled.Apply();
			return downSampled;
		}

		private Texture2D UpscaleDither(Texture2D texture, int levelCount)
		{
			int newWidth = texture.width * 2;
			int newHeight = texture.height * 2;
		
			var upscaled = new Texture2D(newWidth, newHeight);
			int[] matrix = {
				0, 2,
				3, 1
			};
		
			for (int i = 0; i < newWidth; i++)
			{
				for (int j = 0; j < newHeight; j++)
				{
					int x = i % 2;
					int y = j % 2;
		
					float ditherOffset = Mathf.Lerp(-0.2f, 0.2f, matrix[y * 2 + x] / 3f);
					float color = texture.GetPixel(i / 2, j / 2).r;
					float newColor = Mathf.Floor((color + ditherOffset) * levelCount) / levelCount;
					upscaled.SetPixel(i, j, new Color(newColor, newColor, newColor, 1));
				}
			}
		
			upscaled.Apply();
			return upscaled;
		}

		private void SaveOutputTexture(string path)
		{
			if (outputTexture == null)
			{
				Debug.LogError("No output texture to save.");
				return;
			}

			byte[] bytes = outputTexture.EncodeToPNG();
			File.WriteAllBytes(path, bytes);
			Debug.Log("Output texture saved to " + path);
		}
	}
}
