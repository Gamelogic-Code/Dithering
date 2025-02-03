using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Settings that control labels.
	/// </summary>
	[Serializable]
	public class LabelSettings
	{
		/// <summary>
		/// Whether to show number labels.
		/// </summary>
		public bool showNumberLabels;
		
		/// <summary>
		/// The root of the number label.
		/// </summary>
		/// <remarks>
		/// This object controls the visibility of the entire label (and should include for example the background if there is one). 
		/// </remarks> 
		public GameObject numberLabelRoot;
		
		/// <summary>
		/// The text component of the number label.
		/// </summary>
		public Text numberLabel;
	}
}