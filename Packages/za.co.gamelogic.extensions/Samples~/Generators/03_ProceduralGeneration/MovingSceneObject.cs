using System;
using UnityEngine;

namespace Gamelogic.Extensions
{
	public class MovingSceneObject : MonoBehaviour
	{
		public float speed = 1.0f;
		public Vector3 direction = Vector3.left;
	
		public float LeftEdge { get; set; }
	
		public event Action OnOutOfBounds;
	
		private void Update()
		{
			transform.position += direction.normalized * (speed * Time.deltaTime);
			
			if(transform.position.x < LeftEdge)
			{
				OnOutOfBounds?.Invoke();
			}
		}
	}
}
