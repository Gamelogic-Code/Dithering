using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Extensions.Samples.StateMachine
{
	/// <summary>
	/// Represents the information about the world.
	/// </summary>
	public class WorldInfo : MonoBehaviour
	{
		[ValidateNotNull]
		[SerializeField] private Image foodSourcePrefab = null;
		
		[LabelField("position")]
		[SerializeField] private List<FoodSource> foodSources = null;
		
		[ValidateNotNull]
		[SerializeField] private Transform root = null;

		public Transform FoodRoot => root;

		public bool InRangeOfFood(Vector2 position, float mouthDistance)
			=> foodSources.Any(foodSource => Vector2.Distance(position, foodSource.Transform.position.To2DXY()) <
														foodSource.radius + mouthDistance);

		public void Awake()
		{
			foreach (var foodSource in foodSources)
			{
				var foodSourceImage = Instantiate(foodSourcePrefab, foodSource.position, Quaternion.identity, root);
				foodSourceImage.rectTransform.sizeDelta = new Vector2(foodSource.radius * 2, foodSource.radius * 2);
				foodSource.Transform = foodSourceImage.transform;
			}
		}

		public Vector3 GetClosestFoodSource(Vector2 position, float mouthDistance)
		{
			var closestFoodSource = foodSources.MinBy(foodSource =>
				Vector2.Distance(position, foodSource.Transform.position.To2DXY()) - foodSource.radius - mouthDistance);
			return closestFoodSource.position;
		}
	}
}
