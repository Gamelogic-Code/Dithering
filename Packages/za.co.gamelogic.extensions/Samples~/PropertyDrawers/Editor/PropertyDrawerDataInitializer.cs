using System;
using System.Linq;
using Gamelogic.Extensions.Editor.Internal;
using Gamelogic.Extensions.Support;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Samples
{
	/* This class initializes colors and list retrievers for the property drawers in the project.
		It overrides some values in the PropertyDrawerData.
	*/
	[InitializeOnLoad]
	public static class PropertyDrawerDataInitializer
	{
		static PropertyDrawerDataInitializer()
		{
			/* We set the default colors used by the Warning and Highlight property drawers.
			*/
			PropertyDrawerData.HighlightColor = Branding.Apple;
			PropertyDrawerData.WarningColor = Branding.Lemon;
			
			PropertyDrawerData.ForceValue = false;

			/*	Then we register two functions that will be used to retrieve the values for popups.
				The functions use our content to get the values. 
			*/
			var content = Assets
				.FindByType<ContentExample>()
				.FirstOrDefault();
			
			PropertyDrawerData.RegisterValuesRetriever(ContentExample.PowerUpsRetrieverKey, 
				() => content == null ? Array.Empty<string>() : content.powerUps.Select(f => f.name)
				);
			
			PropertyDrawerData.RegisterValuesRetriever(ContentExample.ColorsRetrieverKey, 
				() => content == null ? Array.Empty<Color>() : content.colors
				);
		}
	}
}
