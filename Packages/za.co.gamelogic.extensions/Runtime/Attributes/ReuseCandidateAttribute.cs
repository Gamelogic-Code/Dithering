using System;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Mark code that can potentially be reused.
	/// </summary>
	/// <remarks>
	/// This is usually used to mark code that is private, or public in a more specific library, to indicate that
	/// (after some refactoring) it may be more widely useful, and be made public or moved to a more general library. 
	/// </remarks>
	/*	Implementation note:
			-	Why not a comment? Using an attribute forces the form to be consistent and makes it easier to search for
	 			reuse candidates.
			-	Why not JIRA tickets? This allows the developer to mark code quickly as they think of it, reducing the
				chances that it will be forgotten once delayed.
	*/
	[Version(3, 1, 0)]
	[Experimental]
	[AttributeUsage(AttributeTargets.All)]
	class ReuseCandidateAttribute : Attribute
	{
		/// <summary>
		/// Where this code could be moved to.
		/// </summary>
		public string MoveToWhere { get; set; }
		
		/// <summary>
		/// A note about why this code could be useful, or what needs to be done to move it. 
		/// </summary>
		public string Note { get; set; }
	}
}
