using System;
using UnityEngine;

namespace Gamelogic.Extensions.Samples
{
	[Serializable]
	public class PowerUp
	{
		public string name;
		public Color color;
	}
	
	/* This is an example of data that may be used in a game.
		We use it to provide data for the property drawers (to make certain popups) in the ContentExample.
	*/
	[CreateAssetMenu]
	public class ContentExample : ScriptableObject
	{
		/* These constants are used as keys to store the retriever functions we use to get the values
			for the power ups and colors. The mapping is done in the PropertyDrawerDataInitializer, and 
			we the popups that use them are declared in the ContentExample.
		*/
		public const string PowerUpsRetrieverKey = "PowerUps";
		public const string ColorsRetrieverKey = "Color";
		
		public PowerUp[] powerUps;
		public Color[] colors;
	}
}
