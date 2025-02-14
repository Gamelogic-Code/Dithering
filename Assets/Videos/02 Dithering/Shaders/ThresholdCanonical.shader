﻿/*
	A post-process shader for a threshold effect, where all colors are quantized to 2. 
*/

Shader "Gamelogic/Fx/Dithering/Threshold (Canonical)"
{
	/** This example shows the simplest way that each of the dithering shaders could be implemented.
	
		However, to manage the code as bit better, I refactored it so that code can be shared. You can 
		see hte file Threshold.shader to see how this looks. 
	*/ 
	
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
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Threshold;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed lightness = dot(col.rgb, float3(0.299, 0.587, 0.114));

				col.rgb = float3(1, 1, 1) * step(_Threshold, lightness);
				col.a = 1;
				
				return col;
			}
			ENDCG
		}
	}
}
