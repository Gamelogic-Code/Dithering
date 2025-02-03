using System.Collections;
using System.Linq;
using Gamelogic.Extensions.Algorithms;
using Gamelogic.Extensions.Internal;
using UnityEngine;
using UnityEngine.UI;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Component for displaying the median frame time, maximum frame time, and frame rate.
	/// </summary>
	/// <remarks>
	/// This is used in some examples, and is not intended for production use.
	/// </remarks>
	[Experimental]
	[Version(4, 0, 0)]
	public class FpsCounter : MonoBehaviour
	{
		private static class ToolTip
		{
			internal const string BufferLength = "How many samples to use to calculate the median frame time.";
			internal const string TextUpdateInterval = "How often to update the text in seconds.";
			internal const string MedianFrameTimeLabel = "The label for the median frame time.";
			internal const string MaxFrameTimeLabel = "The label for the maximum frame time.";
			internal const string FpsLabel = "The label for the frames per second.";
			internal const string MedianFrameTimeText = "The text for the median frame time.";
			internal const string MaxFrameTimeText = "The text for the maximum frame time.";
			internal const string FpsText = "The text for the frames per second.";
		}
		
		private const string MedianFrameTimeLabelText = "MED (MS)";
		private const string MaxFrameTimeLabelText = "MAX (MS)";
		private const string FpsLabelText = "FPS";

		private const int DefaultBufferLength = 60;
		
		private WaitForSeconds wait;
		private IBuffer<float> frameTimes;
		private float[] frameTimesCopy;  
		
		[Header("Operation")]
		[Tooltip(ToolTip.BufferLength)]
		[ValidatePositive]
		[SerializeField] private int bufferLength = DefaultBufferLength;
		
		[Tooltip(ToolTip.TextUpdateInterval)]
		[Min(0.1f)]
		[SerializeField] private float textUpdateInterval = 1.0f;

		[Header("UI")]
		[Tooltip(ToolTip.MedianFrameTimeLabel), ValidateNotNull]
		[SerializeField] private Text medianFrameTimeLabel = null;
		
		[Tooltip(ToolTip.MaxFrameTimeLabel), ValidateNotNull]
		[SerializeField] private Text maxFrameTimeLabel = null;
		
		[Tooltip(ToolTip.FpsLabel), ValidateNotNull]
		[SerializeField] private Text fpsLabel = null;
		
		[Tooltip(ToolTip.MedianFrameTimeText), ValidateNotNull]
		[SerializeField] private Text medianFrameTimeText = null;
		
		[Tooltip(ToolTip.MaxFrameTimeText), ValidateNotNull]
		[SerializeField] private Text maxFrameTimeText = null;
		
		[Tooltip(ToolTip.FpsText), ValidateNotNull]
		[SerializeField] private Text fpsText = null;

		private void Start()
		{
			medianFrameTimeLabel.text = MedianFrameTimeLabelText;
			maxFrameTimeLabel.text = MaxFrameTimeLabelText;
			fpsLabel.text = FpsLabelText;
			
			InitializeFpsCounter();
		}
		private void InitializeFpsCounter()
		{
			frameTimes = new RingBuffer<float>(bufferLength);
			wait = new WaitForSeconds(textUpdateInterval);
			frameTimesCopy = new float[bufferLength];
			StartCoroutine(UpdateText());
		}

		public void Update()
		{
			frameTimes.Insert(Time.deltaTime);
		}

		private IEnumerator UpdateText()
		{
			while (true)
			{
				UpdateCopy();

				float median = 1000 * Median(frameTimesCopy);
				float max = 1000 * frameTimesCopy.Max();
				float fps = 1000 / median;

				medianFrameTimeText.text = $"{median:0.0}";
				maxFrameTimeText.text = $"{max:0.0}";
				fpsText.text = $"{fps:0.00}";

				yield return wait; 
			}
			// ReSharper disable once IteratorNeverReturns
		}
		
		
		[ReuseCandidate]
		private static float Median(float[] list) => Median(list, 0, list.Length - 1);
		
		[ReuseCandidate]
		private static float Median(float[] list, int start, int end)
		{
			if (start >= end)
			{
				return list[start];
			}
			
			while (true)
			{
				int centerIndex = (start + end) / 2;

				SwapAt(list, start, centerIndex);
				int pivotIndex = start;

				pivotIndex = PivotIndex(list, start, end, pivotIndex);

				if (pivotIndex < centerIndex)
				{
					start = pivotIndex + 1;
				}
				else if (pivotIndex > centerIndex)
				{
					end = pivotIndex - 1;
				}
				else
				{
					return list[pivotIndex];
				}
			}
		}

		private static int PivotIndex(float[] list, int start, int end, int pivotIndex)
		{
			for (int i = start + 1; i <= end; i++)
			{
				if (!(list[i] < list[pivotIndex]))
				{
					continue;
				}
				// Move the item to the right of the pivot
				SwapAt(list, i, pivotIndex + 1);

				// Swap item with pivot
				SwapAt(list, pivotIndex + 1, pivotIndex);

				// Now the pivot is here
				pivotIndex++;
			}

			return pivotIndex;
		}

		private static void SwapAt(float[] list, int index0, int index1)
		{
			(list[index0], list[index1]) = (list[index1], list[index0]);
		}
		
		private void UpdateCopy()
		{
			foreach ((float item, int index) in frameTimes.WithIndices())
			{
				frameTimesCopy[index] = item;
			}
		}
	}
}
