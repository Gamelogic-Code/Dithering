// Copyright Gamelogic Pty Ltd (c) http://www.gamelogic.co.za

using Gamelogic.Extensions.Internal;
using UnityEngine.UI;

namespace Gamelogic.Extensions
{
	/// <summary>
	/// Provides extension methods for Unity's Image class.
	/// </summary>
	public static class ImageExtensions
	{
		/// <summary>
		/// Sets the visibility of the image.
		/// </summary>
		/// <param name="image">The image to modify.</param>
		/// <param name="visible">A boolean value indicating whether the image should be visible.</param>
		/// <param name="visibleAlpha">The alpha value to use when the image is visible. Default is 1.0f.</param>
		/// <param name="invisibleAlpha">The alpha value to use when the image is invisible. Default is 0.0f.</param>
		[Version(3, 2, 0)]
		public static void SetVisible(this Image image, bool visible, float visibleAlpha = 1.0f,
			float invisibleAlpha = 0.0f)
			=> image.SetAlpha(visible ? visibleAlpha : invisibleAlpha);

		/// <summary>
		/// Sets the alpha value of the image.
		/// </summary>
		/// <param name="image">The image to modify.</param>
		/// <param name="alpha">The alpha value to set.</param>
		[Version(3, 2, 0)]
		public static void SetAlpha(this Image image, float alpha)
			=> image.color = image.color.WithAlpha(alpha);
	}
}
