using System;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Properties than can be set on a shader. 
	/// </summary>
	[Serializable]
	public enum ShaderPropertyType
	{
		Int,
		Float,
		Texture,
		Keyword,
		KeywordSet,
		Color,
		Vector, 
		FloatArray,
		MatrixArray,
		ScreenTextureSize,
	}
}