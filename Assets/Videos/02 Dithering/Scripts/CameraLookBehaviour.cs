using System.Collections;
using Gamelogic.Extensions;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Rotates the camera during runtime in various directions. USed to show effects related to camera movement, such as
	/// shimmering in dithering shaders. 
	/// </summary>
	public class CameraLookBehaviour : GLMonoBehaviour
	{
		[SerializeField] private float yRotationExtentDegrees;
		[SerializeField] private float xRotationExtentDegrees;
		[SerializeField] private float zRotationExtentDegrees;
		[SerializeField] private float time;
	
		private new Camera camera;
		private Vector3 initialCameraEulerAngles;
	
		public void Start()
		{
			camera = GetComponent<Camera>();
			initialCameraEulerAngles = camera.transform.rotation.eulerAngles;
			StartCoroutine(Animate());
		}

		private IEnumerator Animate()
		{
			yield return Tween(0f, 1f, time, Mathf.Lerp, CameraY);
			camera.transform.rotation = Quaternion.Euler(initialCameraEulerAngles);
		
			yield return Tween(0f, 1f, time, Mathf.Lerp, CameraX);
			camera.transform.rotation = Quaternion.Euler(initialCameraEulerAngles);
		
			yield return Tween(0f, 1f, time, Mathf.Lerp, CameraZ);
			camera.transform.rotation = Quaternion.Euler(initialCameraEulerAngles);
		
			yield return Tween(0f, 1f, time, Mathf.Lerp, CameraAll);
		
			camera.transform.rotation = Quaternion.Euler(initialCameraEulerAngles);
		
		}

		// Goes from 0 to 1 to 0 to -1 to 0 through a sine wave as t goes from 0 to 1.
		private float SineCycle(float t) => Mathf.Sin(2 * Mathf.PI * t);

		private void CameraY(float t) => SetRotationY(initialCameraEulerAngles.y + SineCycle(t) * yRotationExtentDegrees);

		private void CameraX(float t) => SetRotationX(initialCameraEulerAngles.x + SineCycle(t) * xRotationExtentDegrees);

		private void CameraZ(float t) => SetRotationZ(initialCameraEulerAngles.z + SineCycle(t) * zRotationExtentDegrees);

		private void CameraAll(float t)
		{
			CameraY(t);
			CameraX(t);
			CameraZ(t);
		}
	
		public void SetRotationY(float angle) 
			=> camera.transform.rotation = Quaternion.Euler(initialCameraEulerAngles.x, angle, initialCameraEulerAngles.z);
	
		public void SetRotationX(float angle)
			=> camera.transform.rotation = Quaternion.Euler(angle, initialCameraEulerAngles.y, initialCameraEulerAngles.z);
	
		public void SetRotationZ(float angle)
			=> camera.transform.rotation = Quaternion.Euler(initialCameraEulerAngles.x, initialCameraEulerAngles.y, angle);
	}
}
