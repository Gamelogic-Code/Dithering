using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// An attribute used to mark a string field that should be drawn as a popup list of tags in the Unity editor.
	/// </summary>
	[Version(4, 3, 0)]
	public class TagPopupAttribute : PopupListAttribute
	{
		/// <summary>
		/// Marks a string field that should be drawn as a popup list containing the tags defined in the Unity project.
		/// </summary>
		public TagPopupAttribute() : base(new PopupListData<string>(GetTags))
		{
		}
		
#if UNITY_EDITOR
		private static string[] GetTags() => UnityEditorInternal.InternalEditorUtility.tags;
#else
		private static string[] GetTags() => System.Array.Empty<string>();
#endif
	}
}
