using System;
using Gamelogic.Extensions.Internal;
using UnityEngine;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// The base class for properties that can be validated. 
	/// </summary>
	/// <remarks>
	/// The default behaviour is determined by booleans in <see cref="PropertyDrawerData"/>: <see cref="PropertyDrawerData.ForceValue"/>,
	/// <see cref="PropertyDrawerData.WarnInConsole"/>, and <see cref="PropertyDrawerData.WarnInInspector"/>, but can be overriden
	/// for an attribute by setting the properties define in this attribute: <see cref="ForceValue"/>, <see cref="WarnInConsole"/>,
	/// and <see cref="WarnInInspector"/>. And since these properties are public, they can be set by the attribute user
	/// to override them for a specific field.
	///
	/// This attribute may be applied to multiple types. It is up to subclasses to use the right
	/// implement the methods correctly for the types that apply.
	///
	/// Custom <see cref="SerializableAttribute"/> types are not supported, and an error will
	/// be thrown in the editor when you apply this attribute to a field of a custom type. 
	/// 
	/// See [Property Drawers](../content/PropertyDrawers.md) for more details.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field)]
	[Version(4, 3, 0)]
	public abstract class ValidationAttribute : PropertyAttribute
	{
		/// <summary>
		/// Gets and sets whether to force the value to be valid, provided the attributes allows it.
		/// </summary>
		public bool ForceValue { get; set; } = PropertyDrawerData.ForceValue;

		/// <summary>
		/// Gets and sets whether to warn in the console when a value is invalid.
		/// </summary>
		/// <remarks>
		/// Only effective when not being forced.
		/// </remarks>
		public bool WarnInConsole { get; set; } = PropertyDrawerData.WarnInConsole;

		/// <summary>
		/// Gets and sets whether to warn in the inspector when a value is invalid.
		/// </summary>
		/// <remarks>
		/// Only effective when not being forced.
		/// </remarks>
		public bool WarnInInspector { get; set; } = PropertyDrawerData.WarnInInspector;

		/// <summary>
		/// Gets and set the message to display in the inspector or console when the value is invalid.
		/// </summary>
		/// <remarks>
		/// Only effective when not being forced, and <see cref="WarnInConsole"/> or
		/// <see cref="WarnInInspector"/> is <see langword="true"/>.
		/// </remarks>
		public string Message { get; set; } = PropertyDrawerData.ValueIsInvalidMessage;

		/// <summary>
		/// Gets and sets the color to use when drawing the property in the inspector when the value is invalid.
		/// </summary>
		/// <remarks>
		/// Only effective when not being forced, and <see cref="WarnInInspector"/> is <see langword="true"/>.
		/// </remarks>
		public Color Color { get; protected set; } = PropertyDrawerData.WarningColor;

		/// <summary>
		/// Gets and sets the color to use when drawing the property in the inspector when the value is invalid.
		/// </summary>
		/// <remarks>
		/// Only effective when not being forced, and <see cref="WarnInInspector"/> is <see langword="true"/>.
		///
		/// See <see cref="ColorExtensions.ParseHex"/> for details on the format of the string.
		/// </remarks>
		public string HexColor
		{
			get => ColorExtensions.ToRGBHex(Color);
			set => Color = ColorExtensions.ParseHex(value);
		}
		
#if UNITY_EDITOR
		/*
				Why is this editor code in the attribute?
				- This makes it the easiest to add new validation attributes that work.

				Why not avoid using serialized property so we do not have to include editor code in the attributes?
					- These alternatives have been considered:
						- Duplicating the SerializedProperty class for runtime code. This seems like a very
						drastic approach, and may be difficult to maintain as Unity continues adding new types
						to support.
						- Using a generic method, like IsValid<T>, and doing the conversion to the type from the serialized
						property. However, this is surprisingly tricky for lists and compound types, and after an initial attempt
						this strategy was abandoned. Besides, this method requires a lot of unnecessary boxing.
					Weighing up the pros and cons, it seems that the current approach is the best one.
			*/
		/// <summary>
		/// Checks whether the given property is valid.
		/// </summary>
		/// <param name="property">The property to check.</param>
		/// <returns><see langword="true"/> if the value is valid; otherwise, <see langword="false"/>.</returns>\
		/// <remarks>
		/// If the property type is not supported by this attribute, this method should return <see langword="true"/>.
		///
		/// This method is only available in the editor, and therefor subclasses should shield their implementations
		/// accordingly. 
		/// </remarks>
		/* Design note: we return true for unsupported types
			so that this check can always be implemented consistently with Constrain<T>.
		*/
		[EditorOnly]
		public abstract bool IsValid(UnityEditor.SerializedProperty property);
		
		/// <summary>
		/// Constrains a given value to be valid (i.e. pass <see cref="IsValid"/>).
		/// </summary>
		/// <remarks>
		/// If the given value is valid, it is simply returned. Otherwise, it is converted
		/// in some way to a valid value. For example, if the validation checks whether numeric values is
		/// in  range, this method should return the value clamped to the range.
		///
		/// The result of this method should return true when passed to <see cref="IsValid"/>.
		///
		/// If there is not a reasonable way to get a valid value from a general value,
		/// it is better to leave this method unimplemented (but then this attribute should not
		/// have <see cref="ForceValue"/> set to true, otherwise an exception will be thrown when this method
		/// is called).
		///
		/// This method should only change the values of the types supported by the attribute.
		///
		/// This method is only available in the editor, and therefor subclasses should shield their implementations
		/// accordingly. 
		/// </remarks>
		/// <returns>A valid value based on the given value.</returns>
		[EditorOnly]
		protected virtual void Constrain(UnityEditor.SerializedProperty property) => throw new NotImplementedException();
		
		/// <summary>
		/// Constrain and verify the value of the given property.
		/// </summary>
		/// <param name="property">The property to constrain and verify.<see cref="Constrain"/> for details on how the value is constrained, and
		/// <see cref="IsValid"/> for how the property is checked.</param>
		/// <exception cref="InvalidOperationException"></exception>
		[EditorOnly]
		public virtual void ConstrainAndVerify(UnityEditor.SerializedProperty property)
		{
			Constrain(property);
			
			if(!IsValid(property))
			{
				throw new InvalidOperationException(
					$"The value of {property.name} is invalid even after it has been constrained, indicating that " +
					$"{GetType()}.{nameof(Constrain)} is not implemented correctly.");
			}
		}
#endif
	}
}
