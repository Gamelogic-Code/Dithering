using System;
using System.Collections.Generic;
using Gamelogic.Extensions.Internal;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Data used to draw a popup list.
	/// </summary>
	/// <remarks>
	/// This class is used to implement property drawers for popup lists. You should generally only
	/// need it if you implement a custom drawer, as explained in [Property Drawers](../content/PropertyDrawers.md),
	/// but even then you should probably use <see cref="PopupListData{T}"/>.
	/// </remarks>
	/* Design note: Earlier versions of C# do not support generic attributes. The generic subclass of this class is used
		to store data in a type-safe way, but this non-generic base class is used by the attribute.
	*/
	[Version(4, 3, 0)]
	public class PopupListData
	{
		private readonly string valuesRetrieverKey;
		
		/// <summary>
		/// The method used to retrieve the values for the popup list marked by this attribute.
		/// </summary>
		/* Design note: The attribute is not responsible for getting the values (since it cannot call editor code).
			Instead, the drawer retrieves them. This property tells the drawer what method to use.
		*/
		public ValuesRetrievalMethod RetrievalMethod { get; }
		
		/// <summary>
		/// Gets the key used to retrieve the values for the popup list, if <see cref="RetrievalMethod"/> is
		/// <see cref="ValuesRetrievalMethod.FuncKey"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="RetrievalMethod"/> is not <see cref="ValuesRetrievalMethod.FuncKey"/>.</exception>
		public string ValuesRetrieverKey
		{
			get
			{
				ValidateRetrievalMethod(ValuesRetrievalMethod.FuncKey);
				return valuesRetrieverKey;
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="PopupListData"/> class stores the key used to retrieve the values.
		/// </summary>
		/// <param name="valuesRetrieverKey">The key used to retrieve the values for the popup list. For the
		/// mechanism to work, they retriever should dbe registered with
		/// <see cref="PropertyDrawerData.RegisterValuesRetriever{T}"/>.</param>
		/// <remarks>
		/// <see cref="RetrievalMethod"/> will be set to <see cref="ValuesRetrievalMethod.FuncKey"/>.
		/// </remarks>
		public PopupListData(string valuesRetrieverKey)
		{
			this.valuesRetrieverKey = valuesRetrieverKey;
			RetrievalMethod = ValuesRetrievalMethod.FuncKey;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PopupListData"/> class that indicates that
		/// the given method will be used to retrieve values for the popup list.
		/// </summary>
		/// <param name="retrievalMethod">The method used to retrieve the values for the popup list.</param>
		/*	Design note: The constructor is protected since it is incomplete: additional data is necessary
			and should be provided by the subclass. It makes no sense for users to use the attribute with
			this constructor directly.
		*/
		protected PopupListData(ValuesRetrievalMethod retrievalMethod)
		{
			RetrievalMethod = retrievalMethod;
		}
		
		/// <summary>
		/// Checks that the <see cref="RetrievalMethod"/> is the expected one.
		/// </summary>
		/// <param name="expectedMethod">The expected method.</param>
		/// <exception cref="InvalidOperationException">The <see cref="RetrievalMethod"/> is not the expected one.</exception>
		/// <remarks>
		/// When overriding this class, and you provide properties associated with the retrieval method, you should
		/// call this method to ensure that the method is the expected one if that property is accessed. 
		/// </remarks>
		protected void ValidateRetrievalMethod(ValuesRetrievalMethod expectedMethod)
		{
			if (RetrievalMethod != expectedMethod)
			{
				throw new InvalidOperationException($"Expected list type {expectedMethod} but got {RetrievalMethod}");
			}
		}
	}
	
	/// <summary>
	/// Data used to draw a popup list of a specific type.
	/// </summary>
	/// <typeparam name="T">The type of the values in the list.</typeparam>
	/// <remarks>
	/// This class is used to implement property drawers for popup lists. You should generally only
	/// have use for this class if you want to implement your own property drawer. See
	/// [Property Drawers](../content/PropertyDrawers.md).
	/// 
	/// This class stores pieces of data that depends on which <see cref="ValuesRetrievalMethod"/> is used.
	/// 
	/// <table>
	/// <tr>
	/// <th>ValuesRetrievalMethod</th>
	/// <th>Usage</th>
	/// </tr>
	/// <tr>
	/// <td><see cref="ValuesRetrievalMethod.ValueList"/></td>
	/// <td>Use <see cref="Values"/> to get the list of values.</td>
	/// </tr>
	/// <tr>
	/// <td><see cref="ValuesRetrievalMethod.Func"/></td>
	/// <td>Use <see cref="ListRetriever"/> to get the function that can retrieve the values, and call it.</td>
	/// </tr>
	/// <tr>
	/// <td><see cref="ValuesRetrievalMethod.FuncKey"/></td>
	/// <td>
	/// Use <see cref="PopupListData.ValuesRetrieverKey"/> to get the key used to retrieve the values, and get the retriever 
	/// function from <see cref="PropertyDrawerData.GetValues{T}"/>.
	/// </td>
	/// </tr>
	/// <tr>
	/// <td><see cref="ValuesRetrievalMethod.Other"/></td>
	/// <td>
	/// Subclasses that are not part of the library can return this value if they use a method other than one of the
	/// ones above.
	/// </td>
	/// </tr>
	/// </table>  
	/// </remarks>
	[Version(4, 3, 0)]
	public class PopupListData<T> : PopupListData
	{
		private readonly T[] values;
		private readonly Func<IEnumerable<T>> listRetriever;
		
		/// <summary>
		/// Gets the list of values for this popup list, if <see cref="ValuesRetrievalMethod"/> is
		/// <see cref="ValuesRetrievalMethod.ValueList"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="ValuesRetrievalMethod"/> is not
		/// <see cref="ValuesRetrievalMethod.ValueList"/>.</exception>
		public T[] Values
		{
			get
			{
				ValidateRetrievalMethod(ValuesRetrievalMethod.ValueList);
				return values;
			}
		}
		
		/// <summary>
		/// Gets the function used to retrieve the list of values for this popup list, if <see cref="ValuesRetrievalMethod"/> is
		/// <see cref="ValuesRetrievalMethod.Func"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"><see cref="ValuesRetrievalMethod"/> is not
		/// <see cref="ValuesRetrievalMethod.Func"/>.</exception>
		public Func<IEnumerable<T>> ListRetriever
		{
			get
			{
				ValidateRetrievalMethod(ValuesRetrievalMethod.Func);
				return listRetriever;
			}
		}
		
		/// <summary>
		/// Creates a new instance of <see cref="PopupListData{T}"/> that stores the list retriever key.
		/// </summary>
		/// <param name="valuesRetrieverKey">The key of the list retriever
		/// used to retrieve the list of values for this popup list.</param>
		/// <remarks>
		/// <see cref="ValuesRetrievalMethod"/> will be set to <see cref="ValuesRetrievalMethod.FuncKey"/>.
		/// </remarks>
		public PopupListData(string valuesRetrieverKey)
			: base(valuesRetrieverKey)
		{
		}
		
		/// <summary>
		/// Creates a new instance of <see cref="PopupListData{T}"/> that stores the given list of values.
		/// </summary>
		/// <param name="values">The list of values for this popup list.</param>
		/// <remarks>
		/// <see cref="ValuesRetrievalMethod"/> will be set to <see cref="ValuesRetrievalMethod.ValueList"/>.
		/// </remarks>
		public PopupListData(T[] values) 
			: base(ValuesRetrievalMethod.ValueList)
			=> this.values = values;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PopupListData{T}"/> class stores the given function to retrieve
		/// the list of values.
		/// </summary>
		/// <param name="listRetriever">The function used to retrieve the list of values for this popup list.</param>
		/// <remarks>
		/// <see cref="ValuesRetrievalMethod"/> will be set to <see cref="ValuesRetrievalMethod.Func"/>.
		/// </remarks>
		public PopupListData(Func<IEnumerable<T>> listRetriever)
			: base(ValuesRetrievalMethod.Func)
		{
			this.listRetriever = listRetriever;
		}
	}
}
