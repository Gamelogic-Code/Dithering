using Gamelogic.Extensions.Algorithms;
using UnityEngine;

namespace Gamelogic.Extensions.Samples.StateMachine
{
	/// <summary>
	/// Spawns a number of organisms at random positions.
	/// </summary>
	public class Spawner : MonoBehaviour
	{
		[ValidateNotNull]
		[SerializeField] private GameObject organismPrefab = null;
		
		[ValidateNotNegative]
		[SerializeField] private int organismCount = 10;
		
		[ValidateNotNull]
		[SerializeField] private Transform root = null;
		
		[ValidateNotNegative]
		[SerializeField] private float range = 100f;
		
		public void Start()
		{
			var rotationGenerator = Generator
				.UniformRandomFloat()
				.Select(x => x * 360);
			
			var positionGenerator = Generator
				.UniformVector2InCircle(range)
				.Select(x => x.To3DXY());
			
			for (int i = 0; i < organismCount; i++)
			{
				var organism = Instantiate(organismPrefab, root);
				organism.transform.Rotate(0, 0, rotationGenerator.Next());
				organism.transform.position = positionGenerator.Next();
			}
		}
	}
}
