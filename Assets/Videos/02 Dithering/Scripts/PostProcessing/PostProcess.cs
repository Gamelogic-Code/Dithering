using Gamelogic.Extensions;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Allows you to apply a shader as a full screen post process effect.
	/// </summary>
	/// <remarks>
	/// For this to work, you need a <see cref="PostProcessPropagation"/> script on your rendering camera.
	///
	/// Furthermore, you need to add the properties manually in the inspector.
	///
	/// There is little error checking on properties, and the shader will not work if the type if the property is
	/// set incorrectly.
	///
	/// Changing the type of a property can lead to problems, so when creating a new property change the type before
	/// changing the name. 
	/// </remarks>
	[ExecuteInEditMode]
	public class PostProcess : GLMonoBehaviour
	{
		[SerializeField] private Shader shader;
		[SerializeField] private ShaderProperty[] shaderProperties;

		private Material screenMaterial;
		private MaterialPropertyBlock materialPropertyBlock;

		public Material ScreenMaterial
		{
			get
			{
				if(shader == null)
				{
					Debug.LogError("No shader set for post process " + name);
				}
				
				
				if (screenMaterial == null)
				{
					screenMaterial = new Material(shader)
					{
						hideFlags = HideFlags.DontSave
					};
				}

				return screenMaterial;
			}
		}

		public void Start()
		{
			if (shader == null || !shader.isSupported)
			{
				enabled = false;
			}
		}

		public void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
		{
			if (shader != null)
			{
				SetMaterialProperties();
				Graphics.Blit(sourceTexture, destTexture, ScreenMaterial);
			}
			else
			{
				Graphics.Blit(sourceTexture, destTexture);
			}
		}

		/// <summary>
		/// Applies all the properties defined for this post process in the inspector to the <see cref="ScreenMaterial"/>. 
		/// </summary>
		[InspectorButton] 
		protected virtual void SetMaterialProperties()
		{
			foreach (var shaderProperty in shaderProperties)
			{
				shaderProperty.Apply(ScreenMaterial);
			}
		}

		public void OnDisable()
		{
			if (screenMaterial != null)
			{
				DestroyImmediate(screenMaterial);
			}
		}
	}
}
