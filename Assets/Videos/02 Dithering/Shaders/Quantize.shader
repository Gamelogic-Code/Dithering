/*
	Post effect shader to quantize the lightness of the image.
*/

Shader "Gamelogic/Fx/Quantize"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LevelCount ("Level Count", Range(2, 256)) = 8
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

			int _LevelCount;

			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float lighness = clamp(to_lightness(color.rgb), 0, 0.999);
				float quantized_color = floor(lighness * _LevelCount) / (_LevelCount - 1);
				
				return float4(quantized_color, quantized_color, quantized_color, 1);
			}
			ENDCG
		}
	}
}
