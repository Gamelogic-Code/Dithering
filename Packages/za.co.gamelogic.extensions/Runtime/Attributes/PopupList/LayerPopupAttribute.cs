using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// An attribute used to mark a string field that should be drawn as a popup list of layers in the Unity editor.
	/// </summary>
	[Version(4, 3, 0)]
	public class LayerPopupAttribute : PopupListAttribute
	{
		/// <summary>
		/// Marks a string field that should be drawn as a popup list containing the layers defined in the Unity project.
		/// </summary>
		public LayerPopupAttribute() : base(new PopupListData<string>(GetLayers))
		{
		}
		
#if UNITY_EDITOR
		private static string[] GetLayers() => UnityEditorInternal.InternalEditorUtility.layers;
#else
		private static string[] GetLayers() => System.Array.Empty<string>();
#endif
	}
}
