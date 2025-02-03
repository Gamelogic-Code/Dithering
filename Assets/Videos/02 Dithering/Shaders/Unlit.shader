/*
	The body of a post process shader based on the unlit shader. 
*/
Shader "Gamelogic/Fx/Dithering/Unlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}
		LOD 100

		Pass
		{
			CGPROGRAM
			#include "Dither.cginc" // Includes data structures and generic vert function
			
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);

				// Dithering code comes here
				
				return color;
			}
			ENDCG
		}
	}
}
