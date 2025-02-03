/*
	A post process threshold shader, but with the threshold smooth (using smooth step internally rather than step). 
*/
Shader "Gamelogic/Fx/Threshold (Smooth)"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Threshold ("Threshold", Range(0, 1)) = 0.5
		_Smoothness ("_Smoothness", Range(0, 1)) = 0.5
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
			float _Smoothness;

			float4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed lightness = dot(col.rgb, float3(0.299, 0.587, 0.114));

				col.rgb = float3(1, 1, 1) * smoothstep(_Threshold - _Smoothness, _Threshold + _Smoothness, lightness);
				col.a = 1;
				
				return col;
			}
			ENDCG
		}
	}
}
