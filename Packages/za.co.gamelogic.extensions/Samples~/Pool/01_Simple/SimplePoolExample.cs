using System.Collections;
using System.Collections.Generic;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamelogic.Extensions.Samples
{
	/// <summary>
	/// This simple example shows how to use a <see cref="MonoBehaviourPool{T}"/>.
	/// </summary>
	public class SimplePoolExample : GLMonoBehaviour
	{
	
		[ValidateNotNull]
		[SerializeField] private SpriteRenderer prefab = null;
		
		[SerializeField] private GameObject parent = null;
	
		[ValidatePositive]
		[SerializeField] private int maxObjectCount = 5000;

		[ValidatePositive]
		[SerializeField] private int minActivityRate = 5;
		
		[ValidatePositive]
		[SerializeField] private int maxActivityRate = 1000;
		
		[SerializeField] private List<Color> colors = null;
	
		private IPool<SpriteRenderer> pool;
		private IGenerator<Color> colorGenerator;
		private IGenerator<float> sizeGenerator;
		private List<SpriteRenderer> activeObjects;

		public void Awake()
		{
			activeObjects = new List<SpriteRenderer>();
			pool = new MonoBehaviourPool<SpriteRenderer>(prefab, parent, maxObjectCount, Activate, Deactivate);
		
			var alphaGenerator = Generator.UniformRandomFloat();
			var indexGenerator = Generator.UniformRandomInt(colors.Count);
			var baseColor = Generator.Choose(colors, indexGenerator);
			colorGenerator = Generator.Combine(baseColor, alphaGenerator, (color, alpha) => color.WithAlpha(alpha));
		
			sizeGenerator = Generator.UniformRandomFloat();
		
			StartCoroutine(Run());
		}
	
		private void Activate(SpriteRenderer obj)
		{
			obj.transform.position = Random.insideUnitSphere * 10;
			obj.gameObject.SetActive(true);
			obj.GetComponent<SpriteRenderer>().color = colorGenerator.Next();
			obj.transform.localScale = prefab.transform.localScale * sizeGenerator.Next();
		}
	
		private void Deactivate(SpriteRenderer obj)
		{
			obj.gameObject.SetActive(false);
		}
	
		public IEnumerator Run()
		{
			while (Application.isPlaying)
			{
				for (int i = 0; i < Random.Range(minActivityRate, maxActivityRate); i++)
				{
					AddObject();
				}
			
				for (int i = 0; i < Random.Range(minActivityRate, maxActivityRate); i++)
				{
					RemoveObject();
				}
			
				yield return null;
			}
		}

		private void AddObject()
		{
			if (!pool.HasAvailableObject)
			{
				return;
			}
			
			var obj = pool.Get();
			activeObjects.Add(obj);
		}
	
		private void RemoveObject()
		{
			if (activeObjects.Count == 0)
			{
				return;
			}
		
			int index = Random.Range(0, activeObjects.Count);
			var obj = activeObjects[index];
			pool.Release(obj);
		
			activeObjects.RemoveAt(index);
		}
	}
}
