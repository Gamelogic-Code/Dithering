/*
	Post process shader that uses a threshold matrix for the dither pattern for RGB channels separately.  
*/
Shader "Gamelogic/Fx/Dithering/Threshold Matrix RGB Dithering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ImageSize ("Image Size", Vector) = (1920, 1080, 0, 0)
		_ColorTransformRTex ("Color Transform R", 2D) = "white" {}
		_ColorTransformGTex ("Color Transform G", 2D) = "white" {}
		_ColorTransformBTex ("Color Transform B", 2D) = "white" {}
		_NoiseSampleChannelOffset ("Noise Sample Channel Offset", Float) = 0
		_DitherAmountMin ("Dither Amount Min", Float) = 0
		_DitherAmountMax ("Dither Amount Max", Float) = 0
		_ThresholdMatrixScale ("Threshold Matrix Scale", Float) = 0		
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

			#define CELL_SIZE 4

			float2 _ImageSize;
			sampler2D _ColorTransformRTex;
			sampler2D _ColorTransformGTex;
			sampler2D _ColorTransformBTex;

			float _NoiseSampleChannelOffset;

			float _DitherAmountMin;
			float _DitherAmountMax;

			float _ThresholdMatrixR[16];
			float _ThresholdMatrixG[16];
			float _ThresholdMatrixB[16];

			float _ThresholdMatrixScale;

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				int2 grid_index = pixel_mod(i.uv, _ImageSize.xy, uint2(CELL_SIZE, CELL_SIZE));

				float bias_r =
					_ThresholdMatrixR[grid_index.y * CELL_SIZE + grid_index.x]
					* _ThresholdMatrixScale;

				float bias_g =
					_ThresholdMatrixG[grid_index.y * CELL_SIZE + (_NoiseSampleChannelOffset + grid_index.x)]
					* _ThresholdMatrixScale;

				float biasB =
					_ThresholdMatrixB[(grid_index.y)* CELL_SIZE + (2*_NoiseSampleChannelOffset + grid_index.x)]
					* _ThresholdMatrixScale;

				float3 bias = float3(bias_r, bias_g, biasB);
				float3 new_col = clamp(col.rgb + lerp(_DitherAmountMin, _DitherAmountMax, bias), 0, 0.99);

				col =
					tex2D(_ColorTransformRTex, float2(new_col.r, 0.5))
					+ tex2D(_ColorTransformGTex, float2(new_col.g, 0.5))
					+ tex2D(_ColorTransformBTex, float2(new_col.b, 0.5));

				return col;
			}
			ENDCG
		}
	}
}
