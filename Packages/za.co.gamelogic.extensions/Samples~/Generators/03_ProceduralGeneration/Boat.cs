using UnityEngine;

public class Boat : MonoBehaviour
{
	[SerializeField] private float rotationExtent = 30;
	[SerializeField] private float rotationSpeed = 1;

	private void Update()
	{
		var rotation = Mathf.Sin(2 * Mathf.PI * Time.time * rotationSpeed) * rotationExtent;
		transform.rotation = Quaternion.Euler(0, 0, rotation);
	}
}
