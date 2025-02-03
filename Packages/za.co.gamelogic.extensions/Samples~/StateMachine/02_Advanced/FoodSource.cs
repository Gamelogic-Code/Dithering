using System;
using UnityEngine;

namespace Gamelogic.Extensions.Samples.StateMachine
{
	/// <summary>
	/// Represents a food source in the world.
	/// </summary>
	[Serializable]
	public class FoodSource
	{
		public Vector2 position;
		public float radius;
		
		public Transform Transform { get; set; }
	}
}
