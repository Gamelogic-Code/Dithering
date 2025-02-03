using System.Collections;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
	[SerializeField, ValidateNotNull] private MovingSceneObject birdPrefab = null;
	[SerializeField] private float smallSpawnInterval = 0.1f;
	[SerializeField] private float bigSpawnInterval = 0.5f;
	
	/*	This generates Fibonacci numbers, which is a good sequence that kinda looks random and grows exponentially.
	*/
	private readonly IGenerator<int> birdCountGenerator 
		= Generator.Iterate(1, 1, (x, y) => x + y);

	private IPool<MovingSceneObject> birds;
	private WaitForSeconds smallWait;
	private WaitForSeconds bigWait;

	public void Awake()
	{
		birds = new HashPool<MovingSceneObject>(
			10, () =>
			{
				var bird = Instantiate(birdPrefab, transform);
				bird.LeftEdge = -1200;
				bird.OnOutOfBounds += () => birds.Release(bird);
				bird.GetRequiredComponent<ScalingSceneObject>().OnScaleComplete += () => birds.Release(bird);

				return bird;
			},
			_ => { },

			b =>
			{
				b.gameObject.SetActive(true);
				b.transform.ResetLocalPosition();
				b.GetRequiredComponent<ScalingSceneObject>().Reset();

				if (!birds.HasAvailableObject)
				{
					birds.IncreaseCapacity(1);
				}
			},

			b => b.gameObject.SetActive(false));
		
		smallWait = new WaitForSeconds(smallSpawnInterval);
		bigWait = new WaitForSeconds(bigSpawnInterval);
	}

	public void Start() => StartCoroutine(SpawnBirds());

	public IEnumerator SpawnBirds()
	{
		while(Application.isPlaying)
		{
			int count = birdCountGenerator.Next();
			
			for (int i = 0; i < count; i++)
			{
				birds.Get();

				yield return smallWait;
			}
			
			yield return bigWait;
		}
	}
}
