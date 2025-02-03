/*
	Post process shader that uses a texture for the dither pattern for the RGB channels separately.  
*/
Shader "Gamelogic/Fx/Dithering/Texture RGB Dithering"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		
		_ColorTransformRTex ("Color Transform R", 2D) = "white" {}
		_ColorTransformGTex ("Color Transform G", 2D) = "white" {}
		_ColorTransformBTex ("Color Transform B", 2D) = "white" {}
		
		// For monochromatic dithering use the same texture for all channels
		_DitherPatternRTex ("Dither Pattern R", 2D) = "white" {}
		_DitherPatternGTex ("Dither Pattern G", 2D) = "white" {}
		_DitherPatternBTex ("Dither Pattern B", 2D) = "white" {}
		
		_DitherPatternTiling ("Dither Pattern Tiling", Vector) = (1, 1, 1, 1 )
		
		_DitherPatternSampleChannelOffset ("Dither Pattern Sample Channel Offset", Float) = 0.0
		
		_DitherAmountMin ("Dither Amount Min", Float) = -0.5
		_DitherAmountMax ("Dither Amount Max", Float) = 0.5
		
		_ColorScale ("Color Scale", Float) = 1
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
			#pragma multi_compile _DISPLACEMENT_NONE _DISPLACEMENT_WAVE _DISPLACEMENT_WAVE_LIGHTNESS _DISPLACEMENT_DIAGONAL
			
			#include "Dither.cginc"
			
			sampler2D _ColorTransformRTex;
			sampler2D _ColorTransformGTex;
			sampler2D _ColorTransformBTex;
			
			sampler2D _DitherPatternRTex;
			sampler2D _DitherPatternGTex;
			sampler2D _DitherPatternBTex;

			float2 _DitherPatternTiling;
			
			float _DitherPatternSampleChannelOffset;
			
			float _DitherAmountMin;
			float _DitherAmountMax;
			float _ColorScale = 1;

			fixed4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);

				float2 uv_r = i.uv;
				float2 uv_g = i.uv;
				float2 uv_b = i.uv;

				#if defined(_DISPLACEMENT_NONE)
				uv_r = uv_r * _DitherPatternTiling.xy;
				uv_g = uv_g * _DitherPatternTiling.xy;
				uv_b = uv_b * _DitherPatternTiling.xy;
				
				#elif defined(_DISPLACEMENT_DIAGONAL)
				uv_r = diagonal(uv_r) * _DitherPatternTiling.xy;
				uv_g = diagonal(uv_g) * _DitherPatternTiling.xy;
				uv_b = diagonal(uv_b) * _DitherPatternTiling.xy;
				#elif defined(_DISPLACEMENT_WAVE)
				uv_r = wave(uv_r, 0.03 , 10) * _DitherPatternTiling.xy;
				uv_g = wave(uv_g, 0.03 , 10) * _DitherPatternTiling.xy;
				uv_b = wave(uv_b, 0.03 , 10) * _DitherPatternTiling.xy;
				
				#elif defined(_DISPLACEMENT_WAVE_LIGHTNESS)
				uv_r = wave_lightness(uv_r, 0.03 , 10, color.r) * _DitherPatternTiling.xy;
				uv_g = wave_lightness(uv_g, 0.03 , 10, color.g) * _DitherPatternTiling.xy;
				uv_b = wave_lightness(uv_b, 0.03 , 10, color.b) * _DitherPatternTiling.xy;
				
				#endif
				
				float2 sample_index_r = float2(uv_r.x, uv_r.y);
				float2 sample_index_g = float2(uv_g.x + _DitherPatternSampleChannelOffset, uv_g.y);
				float2 sample_index_b = float2(uv_b.x + 2*_DitherPatternSampleChannelOffset, uv_b.y);

				float dither_pattern_r = tex2D(_DitherPatternRTex, sample_index_r ).r;
				float dither_pattern_g = tex2D(_DitherPatternGTex, sample_index_g).r;
				float dither_pattern_b = tex2D(_DitherPatternBTex, sample_index_b).r;
				float3 dither_amount = float3(dither_pattern_r, dither_pattern_g, dither_pattern_b);
				
				float3 new_color = clamp(color.rgb + lerp(_DitherAmountMin, _DitherAmountMax, dither_amount), 0, 0.99) ;
				color =
					_ColorScale * (
						tex2D(_ColorTransformRTex, float2(new_color.r, 0.5))
						+ tex2D(_ColorTransformGTex, float2(new_color.g, 0.5))
						+ tex2D(_ColorTransformBTex, float2(new_color.b, 0.5)));	
						
				
				return float4(color.rgb, 1);
			}
			ENDCG
		}
	}
}
