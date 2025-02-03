/*
	A post-process shader that uses Bayer dithering but accumulates error over time using error diffusion. 
*/
Shader "Gamelogic/Fx/Dithering/Time Accumulated Bayer Dithering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ErrorTex("Error", 2D) = "black" {}
		_RenderMode("Render Mode", Int) = 0
		_ColorTransformTex ("Color Transform", 2D) = "white" {}
		_AccumulateError ("Accumulate Error", Int) = 0

		_DitherAmountMin ("Offset 0", Float) = 0
		_DitherAmountMax ("Offset 1", Float) = 1
		
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

			#define ANIMATION_SPEED 0
			#define VISUALIZE_RESULT
			#define CELL_SIZE 4

			//#define VISUALIZE_ERROR

			sampler2D _ErrorTex;
			sampler2D _ColorTransformTex;
			
			float _DitherAmountMin;
			float _DitherAmountMax;
			uniform int _RenderMode;
			uint _AccumulateError;

			fixed4 frag(v2f i) : SV_Target
			{
				float4 imageColor = tex2D(_MainTex, i.uv);
				float4 error = tex2D(_ErrorTex, i.uv);
				float imageLightness = dot(imageColor.rgb, float3(0.299, 0.587, 0.114));

				//int2 gridIndex = pixelMod(i.uv, int2(512, 512), uint2(2, 2));
				//float array[2][2] = {{ 0, 2 }, { 3, 1 } };
				
				int imageSize = 512;
				int2 gridIndex = pixel_mod(i.uv, int2(imageSize, imageSize), uint2(CELL_SIZE, CELL_SIZE));
				float array[CELL_SIZE][CELL_SIZE] = {{0,  8,  2, 10},{12, 4, 14,  6}, {3, 11,  1,  9},{15, 7, 13, 5}};
				gridIndex.x = (gridIndex.x + int(_Time.y*ANIMATION_SPEED)) % CELL_SIZE;
				float bias = array[gridIndex.y][gridIndex.x]/15;
				float adjustedError = (error.r - error.g) * 0.1;
				//float lightness = imageLightness > 0.5 ? 1 : 0;
				float lightness = clamp(imageLightness + lerp(_DitherAmountMin, _DitherAmountMax, bias) + adjustedError, 0, 0.99);
				float4 col;
				col.rgb = tex2D(_ColorTransformTex, float2(lightness, 0.5)).rgb;
				//col.b = error;
				col.a = 1;

				float4 newError = float4(0, 0, 0, 0);
				
				newError.r = imageLightness - lightness;
				newError.g = lightness - imageLightness;
				newError.b = 0;
				newError.a = 1;
				
				float4 result = col;
				#ifdef VISUALIZE_ERROR
				result = float4(error.r, 0, 0, 1);
				#endif
				
				#ifdef  VISUALIZE_GRID
				result = float4(gridIndex.x / (float) CELL_SIZE, gridIndex.y / (float) CELL_SIZE, 0, 1);
				#endif

				#ifdef VISUALIZE_BIAS
				result = float4(bias, 0, 0, 1);
				#endif
				
				return _RenderMode == 0 ? result : newError * _AccumulateError;
			}
			ENDCG
		}
	}
}
