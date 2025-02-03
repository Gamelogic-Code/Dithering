using System;
using UnityEngine;

namespace Gamelogic.Experimental
{
	/// <summary>
	/// Represents a shader property. 
	/// </summary>
	/// <remarks>
	/// As is often the case in Unity, the class represents each type of variable with a different field, but on,y the
	/// relevant field is shown in the inspector.  
	/// </remarks>
	[Serializable]
	public class ShaderProperty
	{
		[SerializeField] private string name;
		[SerializeField] private ShaderPropertyType shaderPropertyType;
		[SerializeField] private int intValue;
		[SerializeField] private float floatValue;
		[SerializeField] private Texture textureValue;
		[SerializeField] private Keyword keywordValue;
		[SerializeField] private Color colorValue;
		[SerializeField] private Vector4 vectorValue;
		[SerializeField] private KeywordSet keywordSetValue;
		[SerializeField] private float[] floatArrayValue;
		[SerializeField] private MatrixArray matrixArrayValue;
		
		/// <summary>
		/// Applies this shader property to the given material that presumably supports it. 
		/// </summary>
		/// <param name="material"></param>
		public void Apply(Material material)
		{
			switch (shaderPropertyType)
			{
				case ShaderPropertyType.Int:
					material.SetInt(name, intValue);
					break;
				case ShaderPropertyType.Float:
					material.SetFloat(name, floatValue);
					break;
				case ShaderPropertyType.Texture:
					material.SetTexture(name, textureValue);
					break;
				case ShaderPropertyType.Keyword:
					if (keywordValue.enabled)
					{
						material.EnableKeyword(keywordValue.name);
					}
					else
					{
						material.DisableKeyword(keywordValue.name);
					}

					break;
				case ShaderPropertyType.KeywordSet:
					if (keywordSetValue == null) break;
					
					foreach (var keyword in keywordSetValue.keywords)
					{
						if (keyword.enabled)
						{
							material.EnableKeyword(keyword.name);
						}
						else
						{
							material.DisableKeyword(keyword.name);
						}
					}

					break;
				case ShaderPropertyType.Color:
					material.SetColor(name, colorValue);
					break;
				case ShaderPropertyType.Vector:
					material.SetVector(name, vectorValue);
					break;
				case ShaderPropertyType.FloatArray:
					material.SetFloatArray(name, floatArrayValue);
					break;
				case ShaderPropertyType.MatrixArray:
					material.SetFloatArray(name, matrixArrayValue.values);
					break;
				case ShaderPropertyType.ScreenTextureSize:
					float newWidth = Screen.width / vectorValue.x * vectorValue.z;
					float newHeight = Screen.height / vectorValue.y * vectorValue.z;
					
					material.SetVector(name, new Vector4(newWidth, newHeight, 0, 0));
					
					break;
			}
		}
	}
}