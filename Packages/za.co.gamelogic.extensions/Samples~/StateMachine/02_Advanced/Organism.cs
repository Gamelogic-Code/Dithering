using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Gamelogic.Extensions.Samples.StateMachine
{
	/// <summary>
	/// Represents an organism that can feed, look for food, and explore.
	/// </summary>
	/*	This is an example of using a state machine to control the behaviour of an object.
	*/
	public class Organism : MonoBehaviour
	{
		private enum State
		{
			Feeding, 
			LookingForFood,
			Exploring,
		}
	
		[FormerlySerializedAs("feedingSpeed")]
		[Header("Stats")] 
		[SerializeField] private float regenerationRate = 1f;
		[SerializeField] private float exploringSpeed = 1f;
		[SerializeField] private float lookingForFoodSpeed = 2f;
		[SerializeField] private float rotationSpeed = 100f;
		[SerializeField] private float mouthDistance = 5f;
	
		[Header("UI")]
		[SerializeField, ValidateNotNull] private Image body = null;
		[SerializeField, ValidateNotNull] private Image energy = null;
	
		[Header("State Colors")]
		[SerializeField] private Color feedingColor = Color.green;
		[SerializeField] private Color lookingForFoodColor = Color.red;
		[SerializeField] private Color exploringColor = Color.blue;
	
		private StateMachine<State> stateMachine;
		private WorldInfo worldInfo;
		private float energyPercent = 50;
		private float boost;
		private Transform oldParent;

		public void Awake()
		{
			energyPercent = 50 + Random.Range(-20, 20);
			worldInfo = GLMonoBehaviour.FindRequiredObjectOfType<WorldInfo>();
			InitializeStateMachine();
			boost = Random.Range(1, 1.5f);
		}

		public void Update() => stateMachine.Update();
	
		private void InitializeStateMachine()
		{
			stateMachine = new StateMachine<State>();
			AddStates();
			stateMachine.CurrentState = State.Exploring;
		}
		
		private void AddStates()
		{
			stateMachine.AddState(State.Feeding, OnFeedingEnter, OnFeedingUpdate, OnFeedingExit);
			stateMachine.AddState(State.LookingForFood, OnLookingForFoodEnter, OnLookingForFoodUpdate);
			stateMachine.AddState(State.Exploring, OnExploringEnter, OnExploringUpdate);
		}
	
		private void OnFeedingEnter()
		{
			body.color = feedingColor;
			oldParent = body.transform.parent;
			body.transform.SetParent(worldInfo.FoodRoot);
			
			UpdateEnergyUI();
		}
		
		private void OnFeedingUpdate()
		{
			energyPercent += regenerationRate * Time.deltaTime;
			UpdateEnergyUI();
			
			if (energyPercent >= 100)
			{
				energyPercent = 100;
				stateMachine.CurrentState = State.Exploring;
			}
		}
		
		private void OnFeedingExit() => body.transform.SetParent(oldParent);
		
		private void OnLookingForFoodEnter() => body.color = lookingForFoodColor;

		private void OnLookingForFoodUpdate()
		{	
			var direction = worldInfo.GetClosestFoodSource(transform.position.To2DXY(), mouthDistance) - transform.position;
			float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			float currentAngle = transform.eulerAngles.z;
			float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle - 90, rotationSpeed * Time.deltaTime);
			transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
			
			transform.Translate(Vector3.up * (lookingForFoodSpeed * Time.deltaTime * boost));
			energyPercent -= 2 * regenerationRate * Time.deltaTime;
			UpdateEnergyUI();
			
			if (worldInfo.InRangeOfFood(transform.position.To2DXY(), mouthDistance))
			{
				stateMachine.CurrentState = State.Feeding;
			}
		}
		
		private void OnExploringEnter()
		{
			body.color = exploringColor;
			transform.Rotate(0, 0, 180 + Random.Range(-60, 60)); // Turn around
		}
	
		private void OnExploringUpdate()
		{
			transform.Translate(Vector3.up * (exploringSpeed * Time.deltaTime * boost));
			energyPercent -= regenerationRate * Time.deltaTime;
			UpdateEnergyUI();

			if (energyPercent < 33)
			{
				stateMachine.CurrentState = State.LookingForFood;
			}
		}
	
		private void UpdateEnergyUI() => energy.color = Color.Lerp(lookingForFoodColor, feedingColor, energyPercent / 100);
	}
}
