using System.Collections;
using Gamelogic.Extensions;
using UnityEngine;
using Gamelogic.Extensions.Algorithms;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Class that spawns objects according to some distribution. 
	/// </summary>
	public class Spawner : GLMonoBehaviour
	{
		public enum SpawnType
		{
			White,
			Blue,
			Red,
			ErrorDiffusion,
			Bayer,
		}
	
		[SerializeField] private GameObject sprite1;
		[SerializeField] private GameObject sprite2;
	
		[SerializeField] private SpawnType spawnType;
		[SerializeField] private float sprite1Probability = 0.5f;
		[SerializeField] private float intervalSecs;
	
		public void Start() => StartCoroutine(Spawn());

		private IEnumerator Spawn()
		{
			var noise = spawnType switch
			{
				SpawnType.Blue => XGenerator.BlueNoise.Select(Quantize),
				SpawnType.Red => XGenerator.RedNoise.Select(Quantize),
				SpawnType.White => Generator.UniformRandomFloat().Select(Quantize),
				SpawnType.ErrorDiffusion => XGenerator.ErrorDiffusion(sprite1Probability),
				SpawnType.Bayer => XGenerator.Bayer8.Select(Quantize),
			};
		
			while(Application.isPlaying)
			{
				float value = noise.Next();
				var sprite = value == 0 ? sprite1 : sprite2;
				var obj = Instantiate(sprite, transform);
				obj.transform.position = transform.position;
			
				//yield return new WaitForSeconds(intervalSecs);
				//This is to make video recording stable
				yield return new WaitForFrames(Mathf.FloorToInt(intervalSecs * 30));
			}
		}
	
		private int Quantize(float x) => x < sprite1Probability ? 0 : 1;
	}
}
