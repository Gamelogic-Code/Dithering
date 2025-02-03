using Gamelogic.Extensions;
using UnityEngine;

namespace Gamelogic.Experimental
{
	//Reuse-candidate
	public class InspectorTextAttribute : PropertyAttribute
	{
		// This attribute doesn't need any additional properties
	}
	
	//Reuse-candidate
	public class Readme : GLMonoBehaviour
	{
		[InspectorText] public string text;
	}
}
