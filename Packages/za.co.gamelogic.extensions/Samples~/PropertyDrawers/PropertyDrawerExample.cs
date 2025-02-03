#pragma warning disable CS0169 // Field is never used. This file is to see the fields in the inspector only. 
#pragma warning disable CS0414 // The private field is assigned but its value is never used
// ReSharper disable UnusedMember.Local

using UnityEngine;

using Gamelogic.Extensions.Internal;
using System;
using Gamelogic.Extensions.Support;
using JetBrains.Annotations;
using UnityEngine.Serialization;

namespace Gamelogic.Extensions.Samples
{
	[Flags]
	public enum MonsterState
	{
		IsHungry = 1,
		IsThirsty = 2,
		IsAngry = 4,
		IsTired = 8
	}

	[Serializable]
	public class MonsterData
	{
		public string name;
		public string nickName;
		public Color color;
	}

	// This class would work exactly the same in the inspector
	// if it was extended from MonoBehaviour instead, except
	// for the InspectorButton
	
	// Some setup is done in the ThemeInitializer script
	public class PropertyDrawerExample : GLMonoBehaviour
	{
		[Header("Display")]
		[ReadOnly]
		public string readonlyString = "Cannot change in inspector";
		
		[Comment("This is a comment.")]
		public string fieldWithComment = "This field has a comment";
		
		[Highlight] // Uses default color in PropertyDrawerData set in PropertyDrawerDataInitializer.cs
		[Space]
		[SerializeField] private int highligtedInt;
		
		[Highlight(Branding.Hex.Coral)]
		[SerializeField] private int redInt;
		
		[Space(order = 0)]
		[Comment("Note the nickName is used for the labels of array items in the inspector", order = 1)]
		[LabelField("nickName")]
		[SerializeField] private MonsterData[] moreMonsters = 
		{
			new MonsterData{ name = "Vampire", nickName = "Vamp", color = Utils.Blue },
			new MonsterData{ name = "Werewolf", nickName = "Wolf", color = Utils.Red},
		};
		
		[FormerlySerializedAs("monsterState")]
		[InspectorFlags]
		[SerializeField] private MonsterState flags = MonsterState.IsAngry | MonsterState.IsHungry;
		
		[Space(order = 0)]
		[Separator(Branding.Hex.Coral, 5, order = 1)]
		[Header("Validation", order = 2)]
		[Comment("Can only be positive.", "Must be larger than 0.", order = 3)]
		
		[ValidatePositive]
		[SerializeField] private int positiveInt = 1;
		
		[ValidatePositive]
		[SerializeField] private float positiveFloat = 0.1f;

		[Space(order = 0)]
		[Comment("Cannot be negative", order = 1)]
		[ValidateNotNegative]
		[SerializeField] private int nonNegativeInt = 0;

		[ValidateNotNegative(WarnInInspector = true, HexColor = Branding.Hex.Coral)]
		[SerializeField] private float nonNegativeFloat = 0f;
		
		[Space(order = 0)]
		[Comment("Can only be between -1 and 1.", order = 1)]
		[ValidateRange(-1, 1)]
		[SerializeField] private float rangeFloat = 0f;
		
		[Space(order = 0)]
		[Comment("Can only be between -5 and 5.", order = 1)]
		[ValidateRange(-5, 5)]
		[SerializeField] private int rangeInt = 0;
		
		[Space(order = 0)]
		[Comment("Custom validation attribute: must be power of two.", order = 1)]
		[ValidatePowerOfTwo]
		[SerializeField] private int powerOfTwo = 1;
		
		[Space(order = 0)]
		[ValidateNotNull]
		[SerializeField] private GameObject notNull;
		
		[SerializeField, ValidateNotNull(WarnInConsole = true)] 
		private GameObject notNullWarningInConsole;

		[SerializeField, ValidateNotNull(HexColor = Branding.Hex.Coral)] 
		private GameObject notNullCustomColor;

		[ValidateNotNull] // No warning, since numbers are never null.
		[SerializeField] private int notNullNumber = 10;

		[Space(order = 0)]
		[Comment(@"Must match: ^#?(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$", order = 1)]
		[ValidateMatchRegularExpression(@"^#?(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{4}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$")] 
		[SerializeField] private string hexColor = "#fff";
		
		[ValidateNotEmpty]
		[SerializeField] private string nonEmptyString = "a";
		
		[ValidateNotWhiteSpaceOrEmpty]
		[SerializeField] private string nonWhiteSpaceEmptyString;
		
		[Space(order = 0)]
		[Separator(Branding.Hex.Coral, 5, order = 1)]
		[Header("Special Types", order = 2)]
		
		[SerializeField] private OptionalInt anOptionalInt = new OptionalInt
		{
			UseValue = true,
			Value = 3
		};
		
		[Space(order = 0)]
		[Comment("Value is between 0 and 1.", order = 1)]
		[SerializeField] private MinMaxFloat minMaxFloatWithDefaultRange = new MinMaxFloat(0.25f, 0.75f);

		[Space(order = 0)]
		[Comment("Value is between 0 and 100.", order = 1)]
		[MinMaxRange(0, 100)]
		[SerializeField] private MinMaxInt minMaxIntPercentage = new MinMaxInt(50, 70);
		
		[MinMaxRange(0, 100)]
		[SerializeField] private MinMaxFloat minMaxFloatPercentage = new MinMaxFloat(50f, 70f);
		
		[MinMaxRange(-100, 200)]
		[SerializeField] private MinMaxFloat minMaxFloatPercentageWarn = new MinMaxFloat(50f, 70f);
		
		[Space(order = 0)]
		[Separator(Branding.Hex.Coral, 5, order = 1)]
		[Header("Popups", order = 2)]
		
		[IntPopup(new[]{5, 10, 15})]
		[SerializeField] private int intPopupFromList = 5;
		
		[StringPopup(new[] { "Vampire", "Werewolf" })]
		[SerializeField] private string stringPopupFromList;
		
		[StringPopup(ContentExample.PowerUpsRetrieverKey)] 
		[SerializeField] private string popupFromContent;
		
		[ColorPopup(ContentExample.ColorsRetrieverKey)]
		[SerializeField] private Color colorPopup;
		
		[TagPopup]
		[SerializeField] private string cameraTagPopup;
		
		[LayerPopup]
		[SerializeField] private string layerPopup;
		
		[BuildScenePopup]
		[SerializeField] private string buildScenePopup;
		
		[Space(order = 0)]
		[Separator(Branding.Hex.Coral, 5, order = 1)]
		[Header("Inspector Buttons", order = 3)]
		[Comment("Buttons show only if your object extends from GLMonoBehaviour or uses a custom editor that draws them",
			order = 4)]
		[Dummy, UsedImplicitly]
		[SerializeField] private bool dummy; // Allows us to have decorators above the buttons.

		//This will only show as a button if you extend from GLMonoBehaviour.
		[InspectorButton]
		public static void LogHello() => Debug.Log("Hello");
	}
}
