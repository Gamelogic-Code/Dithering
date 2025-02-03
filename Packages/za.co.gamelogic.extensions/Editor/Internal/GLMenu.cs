// Copyright Gamelogic (c) http://www.gamelogic.co.za

using UnityEngine;
using UnityEditor;

namespace Gamelogic.Extensions.Menu.Editor.Internal
{
	/// <summary>
	/// Class with static functions for menu options.
	/// </summary>
	// ReSharper disable once PartialTypeWithSinglePart (Other parts are defined in other plugins)
	public static partial class GLMenu
	{
		public static void OpenUrl(string url) => Application.OpenURL(url);

		[MenuItem("Help/Gamelogic/Email Support")]
		public static void OpenSupportEmail()
		{
			OpenUrl("mailto:support@gamelogic.co.za");
		}

		[MenuItem("Help/Gamelogic/Extensions/API Documentation")]
		public static void OpenExtensionsAPI()
		{
			OpenUrl("http://www.gamelogic.co.za/documentation/extensions/");
		}
	}
}
