using Gamelogic.Extensions.Internal;
using UnityEditor;

namespace Gamelogic.Extensions.Editor.Internal
{
	/// <summary>
	/// Provides utility methods for working with assets.
	/// </summary>
	[Version(4, 3, 0)]
	[Experimental]
	public static class Assets
	{
		/// <summary>
		/// Finds all assets of a given type in the asset database.
		/// </summary>
		/// <typeparam name="T">The type of asset to find.</typeparam>
		/// <returns>An array of assets of the given type.</returns>
		[ReuseCandidate]
		public static T[] FindByType<T>() where T : UnityEngine.Object
		{
			string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
			var assets = new T[guids.Length];

			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				assets[i] = AssetDatabase.LoadAssetAtPath<T>(path);
			}

			return assets;
		}
	}
}
