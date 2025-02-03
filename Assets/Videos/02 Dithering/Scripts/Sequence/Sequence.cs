using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions;
using Gamelogic.Extensions.Algorithms;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Switches game objects or sets of game objects on sequentially at a given interval.
	/// </summary>
	/// <remarks>
	/// This is useful for making recording for example by changing the post effect every second.
	///
	/// When the next game object is switched on, the previous game object in the sequence is switched off.
	///
	/// Optionally a number can be shown that counts along the sequence (1, 2, 3 etc.) by setting the
	/// <see cref="LabelSettings"/> appropriately.   
	/// </remarks>
	public class Sequence : GLMonoBehaviour
	{
		[SerializeField] private float switchIntervalSec = 1;
		[SerializeField] private float firstIntervalSec = 1;
		[SerializeField] private List<GameObject> singleObjectSequence;
		[SerializeField] private List<GameObjectSet> sequence;
		[SerializeField] private bool switchOffLast;
		[SerializeField] private bool randomOrder;
		[SerializeField] private bool stopWhenDone;
		[SerializeField] private LabelSettings labelSettings;
		
		private List<GameObjectSet> finalSequence;

		private class SequenceSwitcher
		{
			private readonly List<GameObjectSet> sequence;
			private readonly LabelSettings labelSettings;
			
			public SequenceSwitcher(List<GameObject> singleObjectSequence, List<GameObjectSet> sequence, bool randomOrder,
				LabelSettings labelSettings)
			{
				this.labelSettings = labelSettings;
				
				this.sequence = singleObjectSequence
					.Select(go => new GameObjectSet {set = new List<GameObject> {go}})
					.Concat(sequence)
					.ToList();
				
				if(randomOrder)
				{
					this.sequence.Shuffle();
				}
				
				Debug.Log(this.sequence.Count + " items in sequence");
			}
			
			public IEnumerator PlaySequence(
				float firstIntervalSec, 
				float switchIntervalSec,
				bool switchOffLast,
				bool stopWhenDone
				)
			{
				if (labelSettings.numberLabelRoot != null)
				{
					labelSettings.numberLabelRoot.SetActive(labelSettings.showNumberLabels);
				}
				SwitchOffEverything();
		
				Switch(0, true);
				yield return new WaitForSeconds(firstIntervalSec);

				if (!Application.isPlaying)
				{
					SwitchOffEverything();
					yield break;
				}
			
				for(int i = 1; i < sequence.Count; i++)
				{
					Switch(i - 1, false);
					Switch(i, true);
					yield return new WaitForSeconds(switchIntervalSec);
					
					if (!Application.isPlaying)
					{
						SwitchOffEverything();
						yield break;
					}
				}
		
				if(switchOffLast)
				{
					Switch(sequence.Count - 1, false);

					if (labelSettings.numberLabelRoot != null)
					{
						labelSettings.numberLabelRoot.SetActive(false);
					}
				}

				if (stopWhenDone)
				{
					#if UNITY_EDITOR
					UnityEditor.EditorApplication.ExitPlaymode();
					#else
					Application.Quit();
					#endif
					
				}
			}

			private void SwitchOffEverything()
			{
				foreach (var go in sequence.SelectMany(set => set))
				{
					go.SetActive(false);
				}
			}
	
			private void Switch(int index, bool on)
			{
				if(on && labelSettings.showNumberLabels)
				{
					int displayNumber = index + 1;
					labelSettings.numberLabel.text = displayNumber.ToString();
				}
			
				
				foreach (var go in sequence[index])
				{
					go.SetActive(on);
				}
			}
		}
		
		public void Start()
		{
			var sequenceSwitcher = new SequenceSwitcher(singleObjectSequence, sequence, randomOrder, labelSettings);
			StartCoroutine(sequenceSwitcher.PlaySequence(firstIntervalSec, switchIntervalSec, switchOffLast, stopWhenDone));
		}
	}
}
