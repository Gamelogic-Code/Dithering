using System;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Represents a shader keyword, and whether it is present ot not. 
	/// </summary>
	[Serializable]
	public class Keyword
	{
		public string name;
		public bool enabled;
	}
}