using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Represents a set of game objects.
	/// </summary>
	[Serializable]
	public class GameObjectSet : IEnumerable<GameObject>
	{
		/// <summary>
		/// The set of game objects.
		/// </summary>
		public List<GameObject> set;
		
		/// <inheritdoc />
		public IEnumerator<GameObject> GetEnumerator() => set.GetEnumerator();
		
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}