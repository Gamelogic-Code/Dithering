using System;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Mark fields in a MonoBehaviour with this attribute to give a specific warning 
	/// when the field is not set.
	/// </summary>
	/// <example>
	/// <code>
	/// public class MyMonoBehaviour : MonoBehaviour
	/// {
	/// 	[WarningIfNull("Assign the prefab")]
	/// 	public GameObject playerPrefab;
	/// 	
	/// 	//...
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field)]
	[Obsolete("Use ValidateNotNullAttribute instead.")]
	public class WarningIfNullAttribute : ValidateNotNullAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WarningIfNullAttribute"/> class.
		/// </summary>
		public WarningIfNullAttribute()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="WarningIfNullAttribute"/> class with a custom warning message.
		/// </summary>
		/// <param name="message">The custom warning message.</param>
		public WarningIfNullAttribute(string message)
		{
			Message = message;
		}
	}
}
