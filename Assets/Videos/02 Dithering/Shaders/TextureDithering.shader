/*
	Post process shader that uses a texture for the dither pattern.  
*/

Shader "Gamelogic/Fx/Dithering/Texture dithering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorTransformTex ("Color Transform", 2D) = "white" {}
		_DitherPattern ("Dither Pattern", 2D) = "white" {}
		_DitherPatternTiling ("Dither Pattern Tiling", Vector) = (1, 1, 1, 1 )
		_DitherAmountMin ("Dither Amount Min", Float) = 0
		_DitherAmountMax ("Dither Amount Max", Float) = 1
		_UvOffset ("UV Offset", Vector) = (0, 0, 0, 0)
		_Rotation ("Rotation", Float) = 0
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

			//#define VISUALIZE_DITHER_PATTERN
			
			#pragma vertex vert
			#pragma fragment frag

			//#pragma multi_compile_fragment TORUS_OFF TORUS_ON
			#pragma multi_compile _DISPLACEMENT_NONE _DISPLACEMENT_CIRCLE _DISPLACEMENT_WAVE _DISPLACEMENT_WAVE_LIGHTNESS _DISPLACEMENT_DIAGONAL _DISPLACEMENT_SCALE_LIGHTNESS _DISPLACEMENT_ANGLE_LIGHTNESS _DISPLACEMENT_ANIMATED_WAVE

			float2 _DitherPatternTiling;
			sampler2D _ColorTransformTex;

			sampler2D _DitherPattern;

			float _DitherAmountMin;
			float _DitherAmountMax;
			float2 _UvOffset;
			float _Rotation;
			
			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
				float lightness = to_lightness(color.rgb);

				float2 sample_index = i.uv * _DitherPatternTiling;
				
				#if defined(_DISPLACEMENT_WAVE)
					sample_index = wave(sample_index, 0.5, 0.5);
				#elif defined(_DISPLACEMENT_WAVE_LIGHTNESS)
					sample_index = wave_lightness(sample_index, 0.3, 1, lightness);
				#elif  defined(_DISPLACEMENT_ANIMATED_WAVE)
					sample_index.x += 10*_Time.y;
					sample_index = wave(sample_index, 0.5, 0.5);
					
				#elif defined(_DISPLACEMENT_DIAGONAL)
					sample_index = diagonal(sample_index);
				#elif defined(_DISPLACEMENT_CIRCLE)
					sample_index = circle(i.uv) * _DitherPatternTiling;
				#elif  defined(_DISPLACEMENT_SCALE_LIGHTNESS)
					sample_index = sample_index * lerp(4, 0.01, lightness);
				#elif defined (_DISPLACEMENT_ANGLE_LIGHTNESS)
					sample_index = rotate(sample_index, 2 * UNITY_PI * lightness);
				#endif

				#if TORUS_ON
				sample_index = transform_uv(sample_index, _UvOffset, _Rotation);
				#endif

				float dither_pattern = tex2D(_DitherPattern, sample_index).r;

				#ifdef VISUALIZE_DITHER_PATTERN
				return noise;
				#endif
				
				float biased_lightness = clamp(lightness + lerp(_DitherAmountMin, _DitherAmountMax, dither_pattern), 0, 1);

				float4 dithered_color;
				dithered_color.rgb = tex2D(_ColorTransformTex, float2(biased_lightness, 0.5)).rgb;
				dithered_color.a = 1;
				
				return dithered_color;
			}
			ENDCG
		}
	}
}
