/* 
	The post processing shader of a hybrid shader technique. See also HybridDitheringObject.shader. 
*/

Shader "Gamelogic/Fx/HybridDitheringPost"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Dither0Tex ("Dither Texture 0", 2D) = "white" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_NoiseTiling ("Noise Tiling", Vector) = (1, 1, 1, 1 )
		_Offset0 ("Offset 0", Float) = 0
		_Offset1 ("Offset 1", Float) = 1
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
			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile_fragment TORUS_ON TORUS_OFF
			
			#include "Dither.cginc"

			float2 _NoiseTiling;
			sampler2D _Dither0Tex;

			sampler2D _NoiseTex;

			float _Offset0;
			float _Offset1;
			float2 _UvOffset;
			float _Rotation;;
			

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				float lightness = col.r;
				
				float2 sampleIndex = col.gb * _NoiseTiling;
				
				//sampleIndex.y += sampleIndex.x;
				//sampleIndex.y += 0.5 * sin(0.2*sampleIndex.x);
				//sampleIndex.y += 4*(1 - lightness) * sin(0.2*sampleIndex.x);
				//sampleIndex.y += 4*(ddy(lightness) + ddx(lightness)) * sin(0.2*sampleIndex.x);

				#if TORUS_ON
				sampleIndex = transform_uv(sampleIndex, _UvOffset, _Rotation);
				#endif

				float noise = tex2D(_NoiseTex, sampleIndex).r;
				lightness = clamp(lightness + lerp(_Offset0, _Offset1, noise), 0, 0.99);

				col.rgb = tex2D(_Dither0Tex, float2(lightness, 0.5)).rgb;
				
				return col;
			}
			ENDCG
		}
	}
}
