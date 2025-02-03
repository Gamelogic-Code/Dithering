/*
	A post-process shader for a threshold effect, where all colors are quantized to 2. 
*/

Shader "Gamelogic/Fx/Dithering/Threshold"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Threshold ("Threshold", Range(0, 1)) = 0.5
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
			#include "Dither.cginc"
			#pragma vertex vert
			#pragma fragment frag			

			float _Threshold;

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float lightness = to_lightness(color.rgb);
				float quantized_color = step(_Threshold, lightness);

				return float4(quantized_color, quantized_color, quantized_color, 1);
			}
			ENDCG
		}
	}
}
