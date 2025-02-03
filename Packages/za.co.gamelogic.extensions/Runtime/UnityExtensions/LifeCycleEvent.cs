using System;
using JetBrains.Annotations;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Flags for Unity lifecycle events.
	/// </summary>
	/// <remarks>
	/// Use this to specify what events logic should execute in. This is useful when you want this to be configurable in
	/// the inspector. <see cref="LifeCycleEventExtensions.IfMatchesExecute"/> for an example.  
	/// </remarks>
	/// <seealso cref="LifeCycleEventExtensions"/>
	[Flags]
	public enum LifeCycleEvent
	{
		None = 0,
		Awake = 1,
		OnEnable = 1 << 1,
		Start = 1 << 2,
		Update = 1 << 3,
		LateUpdate = 1 << 4,
		FixedUpdate = 1 << 5,
		OnDisable = 1 << 6,
		OnDestroy = 1 << 7,
		UserDefined = 1 << 8,
	}

	/// <summary>
	/// Provides extension methods for <see cref="LifeCycleEvent"/>.
	/// </summary>
	public static class LifeCycleEventExtensions
	{
		/// <summary>
		/// Executes an action if the current event matches a given event.
		/// </summary>
		/// <param name="eventToMatch">The event to match.</param>
		/// <param name="currentEvent">The current event.</param>
		/// <param name="action">The action to execute.</param>
		/// <example>
		/// In this example, the designer can configure in the inspector when to restart the game.
		/// 
		/// [!code-csharp[](../../Assets/DocumentationCode/LifeCycleExample.cs#Documentation_LifeCycleExample)]
		///
		/// </example>
		/* Design note: It would read better if this was an extension method on the action. However, since the actions
			are likely to be method groups, this would have been awkward. This syntax also works better with lambda 
			expressions.
		*/
		public static void IfMatchesExecute(
			this LifeCycleEvent eventToMatch, 
			LifeCycleEvent currentEvent, 
			[NotNull] Action action)
		{
			if (!eventToMatch.Matches(currentEvent))
			{
				return;
			}
			
			action.ThrowIfNull(nameof(action));
			action();
		}

		public static bool Matches(this LifeCycleEvent eventToMatch, LifeCycleEvent currentEvent) 
			=> (currentEvent & eventToMatch) != 0;
	}
}
