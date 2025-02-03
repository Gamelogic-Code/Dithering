using System;
using System.Collections.Generic;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Represents a set of mutually exclusive keywords of a shader. 
	/// </summary>
	[Serializable]
	public class KeywordSet
	{
		public List<Keyword> keywords = new();
	}
}