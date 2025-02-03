using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Extensions.Samples
{
	public class ClockExampleMain : GLMonoBehaviour
	{
		public const string PauseButtonText = "Pause";
		public const string UnpauseButtonText = "Unpause";
		public const string ResetButtonText = "Reset";
		public const string ClockExpiredMessage = "Clock Expired";

		[Header("UI")]
		[ValidateNotNull]
		[SerializeField] private Text clockCountText = null;
		
		[ValidateNotNull]
		[SerializeField] private Text message = null;
		
		[ValidateNotNull]
		[SerializeField] private Button resetButton = null;
		
		[ValidateNotNull]
		[SerializeField] private Button pauseButton = null;

		private Clock clock;

		public void Start()
		{
			clock = new Clock();
			clock.OnClockExpired += () => { message.text = ClockExpiredMessage; };
			clock.OnSecondsChanged += () => { clockCountText.text = clock.TimeInSeconds.ToString(); };
			ResetClock();
			clock.Unpause();
			SetChildText(resetButton, ResetButtonText);
			UpdateButtonsInitialText();
		}

		public void OnValidate() => UpdateButtonsInitialText();

		public void Update() => clock.Update(Time.deltaTime);

		public void ResetClock()
		{
			clock.Reset(10);
			message.text = string.Empty;
		}

		public void TogglePause()
		{
			if(clock.IsPaused)
			{
				clock.Unpause();
			}
			else
			{
				clock.Pause();
			}

			UpdatePauseButtonText();
		}

		private void UpdateButtonsInitialText()
		{
			SetChildText(resetButton, ResetButtonText);
			UpdatePauseButtonText();
		}

		// clock can be null when called from the OnValidate method in the editor. 
		private void UpdatePauseButtonText() 
			=> SetChildText(pauseButton, clock == null || clock.IsPaused ? UnpauseButtonText : PauseButtonText);

		private static void SetChildText(Component button, string text)
			=> button.GetRequiredComponentInChildren<Text>().text = text;
	}
}
