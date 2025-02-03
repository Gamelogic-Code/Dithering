using System;
using System.Linq;
using UnityEngine;

namespace Gamelogic.Extensions.Samples
{
	/*	This example shows how to use an event tracker.
		The interesting code is in PoisonablePlayer.
	*/
	public class EventTrackerExampleMain : GLMonoBehaviour
	{
		#region Serialized Fields
		[Header("Player")]
		[ValidateNotNull]
		[SerializeField] private PoisonableCharacter character = null;

		[Header("Button Lists")]
		[SerializeField] private Color poisonColor = Color.green;
		[ValidateNotNull]
		[SerializeField] private ButtonListUI poisonList = null;

		[Space]
		[SerializeField] private Color antidoteColor = Color.magenta;
		[ValidateNotNull]
		[SerializeField] private ButtonListUI antidotesList = null;
		#endregion

		#region Messages
		public void Start()
		{
			var poisons = 
				Enum.GetValues(typeof(Poison))
					.Cast<Poison>()
					.ToList();

			poisonList.Init(poisons, poison => character.EatPoison(poison), poisonColor);
			antidotesList.Init(poisons, poison => character.EatAntidote(poison), antidoteColor);
		}
		#endregion
	}
}
