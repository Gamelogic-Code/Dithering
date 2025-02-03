using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Extensions.Samples
{
	public class ExampleUI : GLMonoBehaviour
	{
		[SerializeField] private string title = null;
		[SerializeField] private Text titleText = null;
		
		public void Start() => ResetTileText();

		public void OnValidate()
		{
			Debug.Assert(titleText != null, "Title text is not set");
			ResetTileText();
		}

		private void ResetTileText() => titleText.text = title;
	}
}
