using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// A script that sets the frame rate of the application.
	/// </summary>
	public class SetFrameRate : MonoBehaviour
	{
		[SerializeField] private int framesPerSecond;
		public void Awake()
		{
			Application.targetFrameRate = framesPerSecond;
		}
	}
}
