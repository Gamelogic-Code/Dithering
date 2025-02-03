using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// A  <see cref="YieldInstruction"/> that waits for a certain number of frames. 
	/// </summary>
	/// <remarks>
	/// This is useful when recording videos where the effective time is slowed down go do the recording, and you want the
	/// intervals to stay constant between recordings and non-recordings (on the same machine).  
	/// </remarks>
	public class WaitForFrames : CustomYieldInstruction
	{
		private readonly int targetFrame;

		public WaitForFrames(int frameCount)
		{
			targetFrame = Time.frameCount + frameCount;
		}

		public override bool keepWaiting => Time.frameCount < targetFrame;
	}
}
