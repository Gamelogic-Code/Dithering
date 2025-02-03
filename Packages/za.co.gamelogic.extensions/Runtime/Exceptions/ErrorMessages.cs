using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Provides error messages for this namespace.
	/// </summary>
	[Version(4, 0, 0)]
	internal static class ErrorMessages
	{
		#region ThrowHelper
		internal const string ArgumentCannotBeNegative = "Argument cannot be negative.";
		
		// {0} minimum
		// {1} maximum
		internal const string ArgumentMustBeInRange = "The value must be between {0} and {1}.";
		
		internal const string ContainerEmpty = "The container is empty.";
		internal const string ContainerFull = "The container is full.";
		
		#endregion
		
		#region Algorithms
		internal const string IndexAlreadyAssigned = "Index is already assigned.";
		internal const string NoObjectAtIndex = "No object at index.";
		internal const string CollectionIsEmpty = "Collection is empty.";
		#endregion
		
		#region Pools
		internal const string ObjectNotInPool = "The given object is not in pool.";
		internal const string ObjectAlreadyInactive = "The pool object is already inactive.";
		internal const string ObjectAlreadyActive = "The pool object is already active.";
		internal const string NoInactiveObjects = "No inactive objects are available.";
		internal const string TooManyObjectsReleased = "More objects were released than were acquired.";
		
		internal const string TypeNotAssignableFromType 
			= "The given type {0} is not assignable from the required type {1}";

		#endregion
		
		#region Popup Property Drawer

		public const string NoRetrieversForType = "No retrievers registered for type {0}.";
		public const string NoRetrieverForTypeWithKey = "No retriever registered for name {0} with key {1}.";
		
		public const string GetValuesByOtherMethodNotImplemented 
			= "The GetValuesByOtherMethod method must be implemented if the retrieval method for the attribute is set " +
				"to Other.";
		#endregion
	}
}
