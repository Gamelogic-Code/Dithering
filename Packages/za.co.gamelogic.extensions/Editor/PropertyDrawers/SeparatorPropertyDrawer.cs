// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using Gamelogic.Extensions.Internal;
using UnityEditor;
using UnityEngine;

namespace Gamelogic.Extensions.Editor
{
	[CustomPropertyDrawer(typeof(SeparatorAttribute))]
	[Version(4, 3, 0)]
	public class SeparatorDrawer : DecoratorDrawer
	{
		SeparatorAttribute Attribute => (SeparatorAttribute) attribute;
		
		public override float GetHeight()
		{
			return Attribute.Height;
		}

		public override void OnGUI(Rect position)
		{
			EditorGUI.DrawRect(position, Attribute.Color);
		}
	}
}
