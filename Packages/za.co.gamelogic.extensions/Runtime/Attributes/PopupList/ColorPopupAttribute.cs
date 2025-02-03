using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// An attribute used to mark a color field that should be drawn as a popup list in the Unity editor.
	/// </summary>
	public class ColorPopupAttribute : PopupListAttribute
	{
		/// <summary>
		/// Marks a color field that should be drawn as a popup list using the given key to retrieve the values.
		/// </summary>
		/// <param name="key">The key used to retrieve the color values for the popup list. The retriever function should be
		/// registered with <see cref="PropertyDrawerData.RegisterValuesRetriever{T}"/>.</param>
		public ColorPopupAttribute(string key) : base(new PopupListData<Color>(key))
		{
		}
	}
}
