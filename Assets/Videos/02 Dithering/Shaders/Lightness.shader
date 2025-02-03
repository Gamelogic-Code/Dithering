/*
	Post effect shader to convert RGB to lightness.  
*/
Shader "Gamelogic/Fx/Lightness"
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
			#include "Dither.cginc"
			#pragma vertex vert
			#pragma fragment frag

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				fixed lightness =to_lightness(color.rgb);

				return float4(lightness, lightness, lightness, 1);
			}
			ENDCG
		}
	}
}
