using System;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Represents a matrix that is internally handled as a float array. 
	/// </summary>
	[Serializable]
	public class MatrixArray
	{
		public Vector2Int dimensions = new(4, 4);
		public float[] values;
	}
}