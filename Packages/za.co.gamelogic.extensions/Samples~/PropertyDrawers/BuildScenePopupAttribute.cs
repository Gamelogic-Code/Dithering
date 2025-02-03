using System.Linq;

namespace Gamelogic.Extensions.Samples
{
	/// <summary>
	/// An attribute used to mark a string field that should be drawn as a popup list of scene paths in the Unity editor.
	/// </summary>
	public class BuildScenePopupAttribute : PopupListAttribute
	{
		/// <summary>
		/// Marks a string field that should be drawn as a popup list containing the paths of scenes included in the build settings.
		/// </summary>
		public BuildScenePopupAttribute()
			: base(new PopupListData<string>(GetScenes))
		{
		}
		
#if UNITY_EDITOR
		private static string[] GetScenes() => UnityEditor.EditorBuildSettings.scenes.Select(s => s.path).ToArray();
#else
		private static string[] GetScenes() => System.Array.Empty<string>();
#endif
	}
}
