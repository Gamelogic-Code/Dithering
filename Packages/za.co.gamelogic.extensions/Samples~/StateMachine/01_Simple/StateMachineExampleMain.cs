using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Extensions.Samples
{
	public class StateMachineExampleMain : GLMonoBehaviour
	{
		#region Types
		public enum PlayerState
		{
			Happy,
			Neutral,
			Sad
		}
		#endregion

		#region Serialized Fields
		[Header("UI")]
		[ValidateNotNull]
		[SerializeField] private GameObject happyIndicator = null;
		
		[ValidateNotNull]
		[SerializeField] private GameObject sadIndicator = null;
		
		[ValidateNotNull]
		[SerializeField] private Text healthText = null;
		#endregion

		#region Private Fields
		private StateMachine<PlayerState> stateMachine;
		private float health = 50;
		#endregion

		#region Messages
		public void Start()
		{
			stateMachine = new StateMachine<PlayerState>();

			stateMachine.AddState(
				PlayerState.Happy,
				() => { happyIndicator.SetActive(true); },
				UpdateHappy,
				() => { happyIndicator.SetActive(false); });

			stateMachine.AddState(PlayerState.Neutral);

			stateMachine.AddState(
				PlayerState.Sad,
				() => { sadIndicator.SetActive(true); },
				UpdateSad,
				() => { sadIndicator.SetActive(false); });
		}

		public void Update() => stateMachine.Update();

		#endregion

		#region Public Methods
		public void SetHappy() => stateMachine.CurrentState = PlayerState.Happy;

		public void SetNeutral() => stateMachine.CurrentState = PlayerState.Neutral;

		public void SetSad() => stateMachine.CurrentState = PlayerState.Sad;

		#endregion
		
		#region Private Methods
		private void UpdateHappy()
		{
			health = Mathf.Clamp(health + 1 * Time.deltaTime, 0, 100);
			healthText.text = ((int) health).ToString();
		}

		private void UpdateSad()
		{
			health = Mathf.Clamp(health - 1 * Time.deltaTime, 0, 100);
			healthText.text = ((int)health).ToString();
		}
		#endregion
	}
}
