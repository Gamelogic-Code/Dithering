using System;
using UnityEngine;

namespace Gamelogic.Extensions
{
	public class ScalingSceneObject : MonoBehaviour
	{
		[SerializeField] private float shrinkSpeed = 30f;
		
		private Vector3 originalScale;
		private Vector3 scaleDirection;
		
		public event Action OnScaleComplete;
		
		private void Awake()
		{
			
			originalScale = transform.localScale;
			scaleDirection = originalScale.normalized;
		}
		
		private void Update()
		{
			if (transform.localScale.Dot(originalScale) > 0)
			{
				transform.localScale += scaleDirection * (shrinkSpeed * Time.deltaTime);
			}
			else
			{
				transform.localScale = Vector3.zero;
				OnScaleComplete?.Invoke();
			}
		}
		
		public void Reset() => transform.localScale = originalScale;
	}
}
