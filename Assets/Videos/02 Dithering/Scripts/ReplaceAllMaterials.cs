using System.Linq;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Replaces all materials in the scene with those provided (choosing randomly among them for each material).
	/// </summary>
	public class ReplaceAllMaterials : GLMonoBehaviour
	{
		[SerializeField] private Transform worldRoot;
		[SerializeField] private Material[] newMaterial;
	
		public void Start()
		{
			var renderers = worldRoot.GetComponentsInChildren<Renderer>();
			int replaceCount = 0;
		
			foreach (var renderer1 in renderers)
			{
				var originalMaterials = renderer1.materials;
				var newMaterials = originalMaterials.Select(Replace).ToArray();
				renderer1.materials = newMaterials;
				replaceCount++;
			}
		
			Debug.Log($"Replaced {replaceCount} materials");
		}

		private Material Replace(Material original)
		{
			var color = original.color;
			var replaceMaterial = new Material(newMaterial.RandomItem())
			{
				color = color
			};

			return replaceMaterial;
		}
	}
}
