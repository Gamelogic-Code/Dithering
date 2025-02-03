using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// An attribute used to mark an int field that should be drawn as a popup list in the Unity editor.
	/// </summary>
	[Version(4, 3, 0)]
	public class IntPopupAttribute : PopupListAttribute
	{
		/// <summary>
		/// Marks an int field that should be drawn as a popup list using the given key to retrieve the values.
		/// </summary>
		/// <param name="key">The key used to retrieve the integer values for the popup list. The retriever function should be
		/// registered with <see cref="PropertyDrawerData.RegisterValuesRetriever{T}"/>.</param>
		public IntPopupAttribute(string key) : base(new PopupListData<int>(key))
		{
		}

		/// <summary>
		/// Marks an int field that should be drawn as a popup list using the given list of values.
		/// </summary>
		/// <param name="values">The list of integer values for the popup list.</param>
		public IntPopupAttribute(int[] values) : base(new PopupListData<int>(values))
		{
		}
	}
}
