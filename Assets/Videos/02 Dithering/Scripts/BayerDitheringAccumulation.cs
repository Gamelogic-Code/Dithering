using UnityEngine;
using UnityEngine.Serialization;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Implements a Bayer dithering effect that accumulates error over time rather than space. 
	/// </summary>
	/// <remarks>
	/// This component sets all the necessary properties for the BayerDitheringAccumulation shader to work in
	/// the code, so no properties need to be added in the inspector. (This is not usual for <see cref="PostProcess"/>
	/// componets.)
	/// </remarks>
	[ExecuteInEditMode]
	public class BayerDitheringAccumulation : PostProcess
	{
		public Texture2D colorTransform;
		[FormerlySerializedAs("offset0")] public float ditherAmountMin = 0;
		[FormerlySerializedAs("offset1")] public float ditherAmountMax = 1;
		public bool accumulateError = true;

		private RenderTexture bufferA;
		private RenderTexture bufferB;
		private RenderTexture currentBuffer;

		private const int RenderModeImage = 0;
		private const int RenderModeError = 1;

		private int renderMode = RenderModeImage;

		protected override void SetMaterialProperties()
		{
			ScreenMaterial.SetTexture("_ColorTransformTex", colorTransform);
			ScreenMaterial.SetTexture("_ErrorTex", currentBuffer);
			ScreenMaterial.SetInt("_AccumulateError", accumulateError ? 1 : 0);

			ScreenMaterial.SetFloat("_Offset0", ditherAmountMin);
			ScreenMaterial.SetFloat("_Offset1", ditherAmountMax);
		}

		private new void Start()
		{
			EnsureBuffers();
			renderMode = RenderModeImage;
		}

		private void EnsureBuffers()
		{
			// Ensure both buffers are properly initialized
			if (bufferA == null || bufferA.width != Screen.width || bufferA.height != Screen.height)
			{
				if(bufferA != null)
				{
					bufferA.Release();
				}
				
				if(bufferB != null)
				{
					bufferB.Release();
				}
				
				bufferA = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
				bufferB = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);

				bufferA.Create();
				bufferB.Create();

				currentBuffer = bufferA; // Set the initial buffer
			}
		}

		public void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
		{
			if (ScreenMaterial != null)
			{
				EnsureBuffers();
				SetMaterialProperties();

				if (renderMode == RenderModeImage)
				{
					// Render the final image
					ScreenMaterial.SetInt("_RenderMode", RenderModeImage);
					Graphics.Blit(sourceTexture, destTexture, ScreenMaterial);
					renderMode = RenderModeError;
				}
				else
				{
					// Render the error into the buffer
					ScreenMaterial.SetInt("_RenderMode", RenderModeError);

					// Swap buffers: Write to the other buffer while sampling from the current one
					RenderTexture nextBuffer = (currentBuffer == bufferA) ? bufferB : bufferA;
					Graphics.Blit(sourceTexture, nextBuffer, ScreenMaterial);
					currentBuffer = nextBuffer; // Update the active buffer

					renderMode = RenderModeImage;
				}
			}
			else
			{
				Graphics.Blit(sourceTexture, destTexture);
			}
		}

		private void OnDisable()
		{
			if (bufferA != null)
			{
				bufferA.Release();
			}
			
			if (bufferB != null)
			{
				bufferB.Release();
			}
			
			bufferA = null;
			bufferB = null;
		}
	}
}
