using Gamelogic.Extensions;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// This script should be placed on a <see cref="Camera"/>. It calls <see cref="PostProcess.OnRenderImage"/>
	/// on all children with <see cref="PostProcess"/> of the <c>postProcessRoot</c> configured in this inspector.
	/// This allows you to have post-processes in separate objects and make it easier to manage if you are experimenting
	/// with many processes at the same time.  
	/// </summary>
	[ExecuteInEditMode]
	public class PostProcessPropagation : GLMonoBehaviour
	{
		[SerializeField] private Transform postProcessRoot;
		
		#if UNITY_EDITOR // Only for debugging
		[SerializeField, ReadOnly]
		#endif
		private PostProcess[] postProcesses;
		
		public void OnRenderImage(RenderTexture sourceTexture, RenderTexture destinationTexture)
		{
			postProcesses = postProcessRoot.GetComponentsInChildren<PostProcess>();
	
			RenderTexture currentSource = sourceTexture;

			foreach (var postProcess in postProcesses)
			{
				if (!postProcess.enabled)
				{
					continue;
				}

				// Get a new temporary render texture
				var temporary = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);

				// Perform post-processing
				postProcess.OnRenderImage(currentSource, temporary);

				// Release the current source if it was a temporary texture
				if (currentSource != sourceTexture)
				{
					RenderTexture.ReleaseTemporary(currentSource);
				}

				// Swap textures
				currentSource = temporary;
			}

			// Blit the final texture to the destination
			Graphics.Blit(currentSource, destinationTexture);

			// Release the last temporary texture
			if (currentSource != sourceTexture)
			{
				RenderTexture.ReleaseTemporary(currentSource);
			}
		}
	}
	
	
}
