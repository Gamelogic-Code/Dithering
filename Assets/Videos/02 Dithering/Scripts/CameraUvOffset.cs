using Gamelogic.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Set shader properties depending on camera rotation. Used by some shaders (see for example `ThresholdDithering.shader`) 
	/// </summary>
	[ExecuteInEditMode]
	public class CameraUvOffset : GLMonoBehaviour
	{
		[FormerlySerializedAs("postProcessing")] 
		[SerializeField] private PostProcess postProcess;
		[SerializeField] private Vector2 tiling;
		[SerializeField] private float rotationFactor;
	
		public void Update()
		{
			var rotation = Camera.main.transform.rotation.eulerAngles;
			float u = GLMathf.FloorMod(rotation.y / 360f, 1) * tiling.x;
			float v = GLMathf.FloorMod(rotation.x / 360f, 1) * tiling.y;
			postProcess.ScreenMaterial.SetVector("_UvOffset", new Vector4(u, v, 0, 0));
			postProcess.ScreenMaterial.SetFloat("_Rotation", rotation.z * rotationFactor);
		}
	}
}
