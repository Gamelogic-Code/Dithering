using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gamelogic.Extensions.Internal;
using Gamelogic.Extensions.Support;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Contains static variables and methods that are used by the property drawers.
	/// </summary>
	/// <remarks>
	/// These are meant to be set or called globally when the Editor loads. You can define a class such as
	/// <c>PropertyDrawerDataInitializer</c> that initializes these values, and as decorated with the
	/// <see cref="UnityEditor.InitializeOnLoadAttribute"/>.
	///
	/// See [Property Drawers](../content/PropertyDrawers.md) for more details.
	/// </remarks>
	[Version(4, 3, 0)]
	public static class PropertyDrawerData
	{
		public static string ValueIsInvalidMessage = "Value is invalid.";
		
		/// <summary>
		/// Color used to draw warnings in the inspector.
		/// </summary>
		public static Color WarningColor = Branding.Lemon;
		
		/// <summary>
		/// Color used to highlight fields in the inspector.
		/// </summary>
		public static Color HighlightColor = Branding.Aqua;
		
		/// <summary>
		/// Color used to draw separators.
		/// </summary>
		[Version(4, 4, 0)]
		public static Color SeparatorColor = Branding.Gray6;
		
		///<summary>
		/// The height used to draw separators.
		/// </summary>
		[Version(4, 4, 0)]
		public static int SeparatorHeight = 1;
		
		/// <summary>
		/// Whether to warn in the inspector when a value is invalid.
		/// </summary>
		public static bool WarnInInspector = true;
		
		/// <summary>
		/// Whether to warn in the console when a value is invalid.
		/// </summary>
		public static bool WarnInConsole = false;
		
		/// <summary>
		/// Whether to force the value to be valid, provided the attributes allows it. 
		/// </summary>
		public static bool ForceValue = true;
		
		/* Implementation note:
				Question: Why do we store retrievers by type? Since we cast them anyways, this adds complexity.
				Answer: It allows us to be a bit more type safe, and make it easier for users to know that they are
				trying to retrieve values using the wring type. 
		*/
		private static readonly Dictionary<Type, Dictionary<string, Func<IEnumerable>>> Retrievers 
			= new Dictionary<Type, Dictionary<string, Func<IEnumerable>>>();
		
		/// <summary>
		/// Registers a new function to retrieve a list of values of the given type. 
		/// </summary>
		/// <param name="key">The key to associate with the values that can be used to get the retriever function.</param>
		/// <param name="retriever">The function that retrieves the list of values.</param>
		/// <typeparam name="T">The type of the values.</typeparam>
		/// <remarks>This is used by the property drawers of the subclasses of <see cref="PopupListAttribute"/>>,
		/// such as <see cref="StringPopupAttribute"/> and <see cref="ColorPopupAttribute"/>.</remarks>
		public static void RegisterValuesRetriever<T>(string key, Func<IEnumerable<T>> retriever)
		{
			var valuesType = typeof(T);

			if (!Retrievers.ContainsKey(valuesType))
			{
				Retrievers[valuesType] = new Dictionary<string, Func<IEnumerable>>();
			}
			
			Retrievers[valuesType][key] = retriever;
		}
		
		/// <summary>
		/// Gets the list of values associated with the given key.
		/// </summary>
		/// <param name="key">The key of the values to get.</param>
		/// <typeparam name="T">The type of the values.</typeparam>
		/// <returns>The values associated with the given key.</returns>
		/// <exception cref="InvalidOperationException">No retrievers registered for type <typeparamref name="T"/>
		/// with the <paramref name="key"/>.</exception>
		public static T[] GetValues<T>(string key)
		{
			var valuesType = typeof(T);
			
			// TODO: Add throw if key not found to throw helper.
			// (Not doing it now because we meed to think how to phrase the error message so it can apply in this case.)
			if (!Retrievers.TryGetValue(valuesType, out var retrieversForType))
			{
				throw new KeyNotFoundException(string.Format(ErrorMessages.NoRetrieversForType, valuesType));
			}
			
			if(!retrieversForType.TryGetValue(key, out var retriever))
			{
				throw new KeyNotFoundException(string.Format(ErrorMessages.NoRetrieverForTypeWithKey, valuesType, key));
			}
			
			return retriever().Cast<T>().ToArray();
		}
	}
}
