using UnityEngine;

namespace Gamelogic.Extensions.Samples
{
	public class TweenExample : GLMonoBehaviour
	{
		[ValidateNotNull]
		[SerializeField] private Transform cube = null;
		
		[ValidateNotNull]
		[SerializeField] private Transform leftPosition = null;
		
		[ValidateNotNull]
		[SerializeField] private Transform rightPosition = null;

		public void GoLeft()
		{
			Tween(
				cube.position, 
				leftPosition.transform.position, 
				1, 
				Vector3.Lerp, 
				newPosition => { cube.position = newPosition; });
		}

		public void GoRight()
		{
			Tween(
				cube.position,
				rightPosition.transform.position,
				1,
				Vector3.Lerp,
				newPosition => { cube.position = newPosition; });
		}
	}
}
