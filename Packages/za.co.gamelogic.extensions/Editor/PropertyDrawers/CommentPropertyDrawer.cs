// Copyright Gamelogic (c) http://www.gamelogic.co.za

using UnityEngine;
using UnityEditor;

namespace Gamelogic.Extensions.Editor
{
	/// <summary>
	/// A property drawer for fields marked with the CommentAttribute. Similar to Header, but useful
	/// for longer descriptions.
	/// </summary>
	[CustomPropertyDrawer(typeof(CommentAttribute), useForChildren: true)]
	public class CommentPropertyDrawer : DecoratorDrawer
	{
		CommentAttribute CommentAttribute => (CommentAttribute)attribute;

		public override float GetHeight() => EditorStyles.wordWrappedLabel.CalcHeight(CommentAttribute.content, EditorGUIUtility.currentViewWidth - 30);

		public override void OnGUI(Rect position)
		{
			EditorGUI.BeginDisabledGroup(true);
			EditorGUI.LabelField(position, CommentAttribute.content, EditorStyles.wordWrappedLabel);
			EditorGUI.EndDisabledGroup();
		}
	}
}
