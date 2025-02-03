/*
	Post process shader that uses a threshold matrix for the dither pattern.  
*/
Shader "Gamelogic/Fx/Dithering/Threshold matrix dithering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ImageSize("Image Size", Vector) = (1920, 1080, 0, 0)
		
		_ColorTransformTex ("Color Transform", 2D) = "white" {}
		_DitherAmountMin ("Dither Amount Min", Float) = 0
		_DitherAmountMax ("Dither Amount Max", Float) = 1
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
			#pragma multi_compile _NONE _CHECKER _BAYER_2 _BAYER_4 _BAYER_8 _BAYER_D4 _BAYER_H4 _BAYER_V4 _DOT _BLUE _DIAGONAL _HATCH _HATCH2 _HATCH3 _HATCH4 _HATCH5 _HATCH6
			#pragma multi_compile _DISPLACE_NONE _DISPLACE_XY _DISPLACE_WAVE_X _DISPLACE_WAVE_Y _DISPLACE_DIAGONAL_X _DISPLACE_DIAGONAL_Y _DISPLACE_CIRCLE
			
			#define ANIMATION_SPEED 0
			#define VISUALIZE_RESULT

			#if defined(_NONE)
				#define MATRIX_SIZE 1
				#define DIVISOR 1
			#elif defined(_BAYER_2)
				#define MATRIX_SIZE 2
				#define DIVISOR 3
			#elif defined(_BAYER_4)
				#define MATRIX_SIZE 4
				#define DIVISOR 15
			#elif defined(_BAYER_8)
				#define MATRIX_SIZE 8
				#define DIVISOR 63
			#elif defined(_CHECKER)			
				#define MATRIX_SIZE 2
				#define DIVISOR 1
			#elif defined(_BAYER_D4)
				#define MATRIX_SIZE 4
				#define DIVISOR 15
			#elif defined(_BAYER_H4)
				#define MATRIX_SIZE 4
				#define DIVISOR 4
			#elif defined(_BAYER_V4)
				#define MATRIX_SIZE 4
				#define DIVISOR 4
			#elif defined(_DOT)
				#define MATRIX_SIZE 6
				#define DIVISOR 17
			#elif defined(_BLUE)
				#define MATRIX_SIZE 4
				#define DIVISOR 1
			#elif defined(_DIAGONAL)
				#define MATRIX_SIZE 4
				#define DIVISOR 15
			#elif defined(_HATCH)
				#define MATRIX_SIZE 4
				#define DIVISOR 15
			#elif defined(_HATCH2)
				#define MATRIX_SIZE 4
				#define DIVISOR 6
			#elif defined(_HATCH3)
				#define MATRIX_SIZE 4
				#define DIVISOR 6
			#elif defined(_HATCH4)
				#define MATRIX_SIZE 4
				#define DIVISOR 6
			#elif defined(_HATCH5)
				#define MATRIX_SIZE 4
				#define DIVISOR 6
			#elif defined(_HATCH6)
				#define MATRIX_SIZE 6
				#define DIVISOR 4
			#endif
			
			sampler2D _ColorTransformTex;
			
			float _DitherAmountMin;
			float _DitherAmountMax;
			float2 _ImageSize;

			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float lightness = to_lightness(color.rgb);

				#ifdef VISUALIZE_LIGHTNESS
				return float4(lightness, lightness, lightness, 1);
				#endif

				#if defined(_NONE)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0}};
				#elif defined(_BAYER_2)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0,  2},{1, 3}};
				#elif defined(_BAYER_4)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0,  8,  2, 10}, {12, 4, 14,  6}, {3, 11,  1,  9},{15, 7, 13, 5}};
				#elif defined(_BAYER_8)
				float array[8][8] =
				{
					{ 0, 48, 12, 60,  3, 51, 15, 63 },
					{ 32, 16, 44, 28, 35, 19, 47, 31 },
					{ 8, 56,  4, 52, 11, 59,  7, 55 },
					{ 40, 24, 36, 20, 43, 27, 39, 23 },
					{ 2, 50, 14, 62,  1, 49, 13, 61 },
					{ 34, 18, 46, 30, 33, 17, 45, 29 },
					{ 10, 58,  6, 54,  9, 57,  5, 53 },
					{ 42, 26, 38, 22, 41, 25, 37, 21 }
				};
				#elif defined(_CHECKER)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0,  1},{1, 0}};
				#elif defined(_BAYER_D4)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0,  8,  2, 10},{6, 12, 4, 14}, {1, 9, 3, 11,},{7, 13, 5, 15}};
				#elif defined(_BAYER_H4)
				//float array[MATRIX_SIZE][MATRIX_SIZE] = {{0, 0, 0, 0}, {2, 2, 2, 2}, {1, 1, 1, 1}, {3, 3, 3, 3}};
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0, 0, 0, 0}, {1, 1, 1, 1}, {2, 2, 2, 2}, {1, 1, 1, 1}};
				#elif defined(_BAYER_V4)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0,  2,  1, 3}, {0,  2,  1, 3}, {0,  2,  1, 3}, {0,  2,  1, 3}};
				#elif defined(_DOT)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{9, 16, 12, 8, 1, 5}, {13, 17, 15, 4, 0, 2}, {10, 14, 11, 7, 3, 6}, {8, 1, 5, 9, 16, 12}, {4, 0, 2, 13, 17, 15}, {7, 3, 6, 10, 14, 11}};
				#elif defined(_BLUE)
				float array[MATRIX_SIZE][MATRIX_SIZE] = // Probably not really blue
				{
					{0.271f, 1.000f, 0.685f, 0.584f},
					{0.085f, 0.162f, 0.000f, 0.800f},
					{0.405f, 0.896f, 0.050f, 0.983f},
					{0.799f, 0.027f, 0.261f, 0.060f}
				};
				#elif defined(_DIAGONAL)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0, 9, 6, 15},{8, 5, 14, 3},{4,13,2,11},{12,1,10,7}};
				#elif defined(_HATCH)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{0, 8, 4, 12}, {2, 10, 6, 14}, {1, 9, 5, 13}, {3, 11, 7, 15}};

				#elif defined(_HATCH2)
				//float array[MATRIX_SIZE][MATRIX_SIZE] = {{4, 0, 2, 5}, {1, 0, 1, 1}, {3, 0, 2, 3}, {4, 0, 2, 6}};
				//float array [MATRIX_SIZE][MATRIX_SIZE] = {{4, 5, 2, 5}, {4, 6, 2, 6}, {3, 3, 2, 3}, {4, 6, 2, 6}};
				//float array [MATRIX_SIZE][MATRIX_SIZE] = {{4, 0, 2, 5}, {1, 0, 1, 1}, {3, 0, 2, 3}, {4, 0, 2, 6}};
				float array [MATRIX_SIZE][MATRIX_SIZE] = {{4, 6, 3, 6}, {4, 7, 3, 7}, {4, 5, 3, 5}, {4, 7, 3, 7}};
				#elif defined(_HATCH3)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{4, 2, 0, 5},{3, 2, 0, 3}, {1, 1, 0, 1}, {4, 2, 0, 6}};
				#elif defined(_HATCH4)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{4, 5, 2, 5}, {4, 3, 6, 2}, {2, 6, 6, 6}, {4, 2, 6, 3}};
				#elif defined(_HATCH5)
				float array[MATRIX_SIZE][MATRIX_SIZE] = {{2, 5, 5, 5}, {4, 3, 6, 2},{4, 6, 2, 6}, {4, 2, 6, 3}};
				#elif defined(_HATCH6)
				//float array[MATRIX_SIZE][MATRIX_SIZE] = {{0, 3, 3, 3, 1, 3}, {2, 0, 4, 1, 4, 4}, {2, 4, 0, 4, 4, 4}, {2, 1, 4, 0, 4, 4}, {1, 4, 4, 4, 0, 4}, {2, 4, 4, 4, 4, 0}};
				float array[MATRIX_SIZE][MATRIX_SIZE] =
				{
					{0, 3, 3, 3, 3, 3},
					{2, 1, 4, 4, 4, 0},
					{2, 4, 1, 4, 0, 4},
					{2, 4, 4, 0, 4, 4},
					{2, 4, 0, 4, 1, 4},
					{2, 0, 4, 4, 4, 1}
				};
				#endif
				
				int2 matrix_index = pixel_mod(i.uv, _ImageSize.xy, uint2(MATRIX_SIZE, MATRIX_SIZE));
				

				

				#if defined(_DISPLACE_XY)
				matrix_index = matrix_index.yx;
				#elif defined(_DISPLACE_WAVE_X)
				matrix_index.x = (matrix_index.x + 12 * (1 + cos(50 * i.uv.y))) % MATRIX_SIZE;
				#elif defined(_DISPLACE_DIAGONAL_X)
				matrix_index.x = (matrix_index.x + matrix_index.y) % MATRIX_SIZE;
				#elif defined(_DISPLACE_WAVE_Y)
				matrix_index.y = (matrix_index.y + 12 * (1 + cos(50 * i.uv.x))) % MATRIX_SIZE;
				#elif defined(_DISPLACE_DIAGONAL_Y)
				matrix_index.y = (matrix_index.y + matrix_index.x) % MATRIX_SIZE;
				#elif defined(_DISPLACE_CIRCLE)
				
				#endif
				
				float bias = array[matrix_index.y][matrix_index.x]/DIVISOR;
				float biased_lightness = clamp(lightness + lerp(_DitherAmountMin, _DitherAmountMax, bias), 0, 0.99);

				float4 dithered_color;
				dithered_color.rgb = tex2D(_ColorTransformTex, float2(biased_lightness, 0.5)).rgb;
				dithered_color.a = 1;

				#ifdef VISUALIZE_LIGHTNESS
				return float4(biased_lightness, biased_lightness, biased_lightness, 1);
				#endif

				#ifdef  VISUALIZE_MATRIX
				return float4(matrix_index.x / (float) MATRIX_SIZE, matrix_index.y / (float) MATRIX_SIZE, 0, 1);
				#endif

				#ifdef VISUALIZE_BIAS
				return float4(bias, 0, 0, 1);
				#endif
				
				return dithered_color;
			}
			ENDCG
		}
	}
}
