using System;
using System.Linq;
using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A base class for creating custom popup list drawers in the Unity Editor.
	/// </summary>
	/// <typeparam name="T">The type of the values in the popup list.</typeparam>
	[Version(4, 3, 0)]
	public abstract class PopupListPropertyDrawer<T> : PropertyDrawer
	{
		/// <summary>
		/// Gets the <see cref="PopupListAttribute"/> associated with the property.
		/// </summary>
		public PopupListAttribute Attribute => (PopupListAttribute)attribute;

		protected T[] values;
		
		/// <summary>
		/// Renders the property in the Unity Editor with a popup list if the values are available.
		/// </summary>
		/// <param name="position">The position in the editor window where the property should be drawn.</param>
		/// <param name="property">The serialized property being drawn.</param>
		/// <param name="label">The label for the property.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (values == null)
			{
				GetValues();
			}
			
			if(values == null)
			{
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			DrawField(position, property, label);
		}

		/// <summary>
		/// Draws the popup field in the editor.
		/// </summary>
		/// <param name="position">The position in the editor window where the field should be drawn.</param>
		/// <param name="property">The serialized property associated with the field.</param>
		/// <param name="label">The label for the field.</param>
		protected void DrawField(Rect position, SerializedProperty property, GUIContent label)
		{
			int selectedIndex = Array.IndexOf(values, GetValue(property));
			
			if(selectedIndex == -1)
			{
				selectedIndex = 0;
			}

			var style = new GUIStyle(EditorStyles.popup);
			style.imagePosition = ImagePosition.ImageLeft;

			Assert.IsNotNull(values);
			selectedIndex = EditorGUI.Popup(
				position, new GUIContent(label.text), selectedIndex, values.Select(GetContent).ToArray(),
				style);

			if (selectedIndex >= 0 && selectedIndex < values.Length)
			{
				SetPropertyValue(property, values[selectedIndex]);
			}
		}

		/// <summary>
		/// Gets the <see cref="GUIContent"/> for a given value in the popup list.
		/// </summary>
		/// <param name="value">The value to convert to <see cref="GUIContent"/>.</param>
		/// <returns>The <see cref="GUIContent"/> for the given value.</returns>
		protected abstract GUIContent GetContent(T value);
		
		/// <summary>
		/// Sets the value of the serialized property based the given value.
		/// </summary>
		/// <param name="property">The serialized property to set the value for.</param>
		/// <param name="value">The value to set.</param>
		/// <example>
		/// If <typeparamref name="T"/> is <see cref="string"/>, this is simply implemented as
		/// <code>
		/// <![CDATA[
		/// override protected void SetPropertyValue(SerializedProperty property, string value) => property.stringValue = value;
		/// ]]>
		/// </code>
		/// </example>
		protected abstract void SetPropertyValue(SerializedProperty property, T value);
		
		/// <summary>
		/// Gets the current value of the property from the serialized property.
		/// </summary>
		/// <param name="property">The serialized property to get the value from.</param>
		/// <returns>The current value of the serialized property.</returns>
		/// <example>
		/// If <typeparamref name="T"/> is <see cref="string"/>, this is simply implemented as
		/// <code>
		/// <![CDATA[
		/// override protected string GetValue(SerializedProperty property) => property.stringValue;
		/// ]]>
		/// </code>
		/// </example>
		protected abstract T GetValue(SerializedProperty property);

		/// <summary>
		/// Gets the values to be displayed in the popup list by a method other one of the system-provided ones.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <remarks>
		/// Derived drawers for popup lists where the <see cref="PopupListAttribute.RetrievalMethod"/>
		/// is set to <see cref="ValuesRetrievalMethod.Other"/> needs to be implemented in this method.
		/// 
		/// </remarks>
		protected virtual T[] GetValuesByOtherMethod() 
			=> throw new NotImplementedException(ErrorMessages.GetValuesByOtherMethodNotImplemented);

		/// <summary>
		/// Retrieves the values to be displayed in the popup list, based on the retrieval method specified in the attribute.
		/// </summary>
		/// <remarks>
		/// The values are retrieved by calling editor code, which cannot be done from the attribute itself.
		/// </remarks>
		private void GetValues()
		{
			switch (Attribute.RetrievalMethod)
			{
				case ValuesRetrievalMethod.FuncKey:
					values = PropertyDrawerData.GetValues<T>(Attribute.PopupListData.ValuesRetrieverKey);
					break;
				case ValuesRetrievalMethod.ValueList:
					values = ((PopupListData<T>)Attribute.PopupListData).Values;
					break;
				case ValuesRetrievalMethod.Func:
					values = ((PopupListData<T>)Attribute.PopupListData).ListRetriever().ToArray();
					break;
				case ValuesRetrievalMethod.Other:
					values = GetValuesByOtherMethod();
					
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
