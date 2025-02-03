using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// The base class for all popup list attributes. 
	/// </summary>
	/// <remarks>
	/// You can extend from this class if you want to create a custom popup list attribute.
	/// </remarks>
	[Version(4, 3, 0)]
	public abstract class PopupListAttribute : PropertyAttribute
	{
		public PopupListData PopupListData { get; set; }
		
		public ValuesRetrievalMethod RetrievalMethod => PopupListData.RetrievalMethod;
		
		protected PopupListAttribute(PopupListData popupListData) => PopupListData = popupListData;
	}
}
