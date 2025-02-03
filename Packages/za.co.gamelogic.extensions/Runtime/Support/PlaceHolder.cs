using System;
using Gamelogic.Extensions.Internal;
using JetBrains.Annotations;

namespace Gamelogic.Extensions.Support
{
	/// <summary>
	/// Used to mark entities that are implemented in newer versions of .Net, Unity, or Rider. 
	/// </summary>
	/// <remarks>
	/// Placeholders are used to make the code easier to compile for different platforms. No attempt is made to mimic
	/// the actual behavior of the entities they represent, and therefore placeholders are used for entities that do not
	/// typically affect the execution of the application. These are often attributes used to guide code-analysers.    
	/// </remarks>
	[Version(4, 1, 0)]
	public class PlaceHolderAttribute : Attribute
	{
	}

	/// <summary>
	/// Specifies that a method will never return under any circumstance.
	/// </summary>
	/// See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.doesnotreturnattribute?view=net-8.0"/>.
	[PlaceHolder]
	[Version(4, 1, 0)]
	public class DoesNotReturn : Attribute
	{
	}

	/// <summary>
	/// Specifies that the method or property will ensure that the listed field and property members have non-null values
	/// when returning with the specified return value condition.
	/// </summary>
	/// See <see href="https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis.membernotnullwhenattribute?view=net-8.0"/>.
	[PlaceHolder]
	[Version(4, 1, 0)]
	public class MemberNotNullWhenAttribute : Attribute
	{
		[UsedImplicitly] // The arguments
		public MemberNotNullWhenAttribute(bool value, string member)
		{
		}
	}

#if !UNITY_6_0_OR_NEWER
	/// <summary>
	/// Specifies that the caller must be disposed of the return value.
	/// </summary>
	/// See <see href="https://www.jetbrains.com/help/resharper/NotDisposedResourceIsReturnedByProperty.html" />
	[PlaceHolder]
	[Version(4, 1, 0)]
	public class MustDisposeResourceAttribute : Attribute
	{
	}
#endif
}
