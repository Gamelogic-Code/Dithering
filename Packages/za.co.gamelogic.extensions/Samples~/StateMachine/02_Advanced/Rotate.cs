using UnityEngine;

namespace Gamelogic.Extensions.Samples.StateMachine
{
	/// <summary>
	/// Rotates the object around the z-axis.
	/// </summary>
	public class Rotate : MonoBehaviour
	{
		[Tooltip("In degrees per second.")]
		[SerializeField] private float rotationSpeed = 10f;

		public void Update()
		{
			transform.Rotate(Vector3.forward * (Time.deltaTime * rotationSpeed));
		}
	}
}
