using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	#region Documentation_StringPopupAttribute
	/// <summary>
	/// Used to mark a string field that should be drawn as a popup list.
	/// </summary>
	[Version(4, 3, 0)]
	public class StringPopupAttribute : PopupListAttribute
	{
		/// <summary>
		/// Marks a string field that should be drawn as a popup list using the given key to retrieve the values.
		/// </summary>
		/// <param name="key">The key used to retrieve the values for the popup list. The retriever function should be
		/// registered with <see cref="PropertyDrawerData.RegisterValuesRetriever{T}"/>. See
		/// (Property Drawers)[PropertyDrawers.md] for more details.
		/// </param>
		public StringPopupAttribute(string key) : base(new PopupListData<string>(key))
		{
		}
		
		/// <summary>
		/// Marks a string field that should be drawn as a popup list using the given list of values.
		/// </summary>
		/// <param name="values"></param>
		public StringPopupAttribute(string[] values) : base(new PopupListData<string>(values))
		{
		}
	}

	#endregion
}
