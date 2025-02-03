using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions.Algorithms
{
	/// <summary>
	/// Represents a Proportional-Integral-Derivative (PID) controller that calculates control values based on the
	/// difference between a desired set point and a measured process variable.
	/// </summary>
	[Version(4, 1, 0)]
	public sealed class PidController
	{
		private readonly Differentiator differentiator;
		private readonly float derivativeGain;
		private readonly float integralGain;
		private readonly Integrator integrator;
		private readonly float proportionalGain;

		/// <summary>
		/// Gets the filtered control value based on the PID gains and the current, differentiated, and integrated values.
		/// </summary>
		public float FilteredValue =>
			proportionalGain * Value
			+ derivativeGain * differentiator.Difference
			+ integralGain * integrator.Sum;

		/// <summary>
		/// Gets or sets the current value of the PID controller. Setting this value updates both the differentiator and
		/// integrator.
		/// </summary>
		public float Value
		{
			get => differentiator.Value; // Could also use integrator
			set
			{
				differentiator.Value = value;
				integrator.Value = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PidController"/> class with specified gains and integration window.
		/// </summary>
		/// <param name="integrationWindow">The number of values to consider in the integrator's sum.</param>
		/// <param name="proportionalGain">The gain for the proportional part of the PID controller.</param>
		/// <param name="derivativeGain">The gain for the derivative part of the PID controller.</param>
		/// <param name="integralGain">The gain for the integral part of the PID controller.</param>
		public PidController(int integrationWindow, float proportionalGain, float derivativeGain, float integralGain)
		{
			differentiator = new Differentiator();
			integrator = new Integrator(integrationWindow);

			this.proportionalGain = proportionalGain;
			this.derivativeGain = derivativeGain;
			this.integralGain = integralGain;
		}
	}
}
