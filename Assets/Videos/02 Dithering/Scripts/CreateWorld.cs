using Gamelogic.Extensions;
using Unity.Mathematics;
using UnityEngine;

namespace Gamelogic.Experimental
{
	[ExecuteInEditMode]
	public class CreateWorld : GLMonoBehaviour
	{
		[SerializeField] private GameObject sandPrefab;
		[SerializeField] private GameObject grassPrefab;
		[SerializeField] private float threshold = 0.5f;
		[SerializeField] private Vector2 cellSize = new Vector2(1, 1);
	
		[SerializeField] private GameObject worldRoot;
	
		[SerializeField] private Vector2Int dimensions;
		[SerializeField] private float sampleScale = 1;
		[SerializeField] private Texture2D ditherPattern;
		[SerializeField] private float minDither;
		[SerializeField] private float maxDither;

		[SerializeField] private Vector2 offset;
	
	
		[InspectorButton]
		public void BuildWorld()
		{
			worldRoot.transform.DestroyChildrenUniversal();
		
			for (int x = 0; x < dimensions.x; x++)
			{
				for (int y = 0; y < dimensions.y; y++)
				{
					var sample = new Vector2(x, y) * sampleScale + offset;
					float terrainValue = noise.pnoise(sample, (Vector2) dimensions);
					float ditherValue = ditherPattern.GetPixel(x % ditherPattern.width, y % ditherPattern.height).r;
				
					var prefab = terrainValue + Mathf.Lerp(minDither, maxDither, ditherValue) < threshold ? sandPrefab : grassPrefab;
				
					var obj = Instantiate(prefab, worldRoot.transform);
					obj.transform.position = new Vector3(x * cellSize.x, 0, y * cellSize.y);
				}
			}	
		}
	}
}
