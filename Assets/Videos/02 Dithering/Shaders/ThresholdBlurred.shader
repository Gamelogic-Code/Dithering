/*
	A post process threshold shader, but with the pixels slightly blurred.  
*/
Shader "Gamelogic/Fx/Dithering/Threshold (Blurred)"
{
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
			#include "Dither.cginc"
			#pragma vertex vert
			#pragma fragment frag
			
			float _Threshold;

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				//fixed4 col = tex2D(_MainTex, i.uv);

				float image_dimensions = 512;
				float2 texel_size = float2(1.0 / image_dimensions, 1.0 / image_dimensions);
				
				float4 current_pixel = tex2D(_MainTex, i.uv);
				float4 left_pixel = tex2D(_MainTex, i.uv + float2(-texel_size.x, 0));
				float4 right_pixel = tex2D(_MainTex, i.uv + float2(texel_size.x, 0));
				float4 top_pixel = tex2D(_MainTex, i.uv + float2(0, -texel_size.y));
				float4 bottom_pixel = tex2D(_MainTex, i.uv + float2(0, texel_size.y));

				
				//float lightness = dot(col.rgb, float3(0.299, 0.587, 0.114));
				float current_lightness = dot(current_pixel.rgb, float3(0.299, 0.587, 0.114));
				float left_lightness = dot(left_pixel.rgb, float3(0.299, 0.587, 0.114));
				float right_lightness = dot(right_pixel.rgb, float3(0.299, 0.587, 0.114));
				float top_lightness = dot(top_pixel.rgb, float3(0.299, 0.587, 0.114));
				float bottom_lightness = dot(bottom_pixel.rgb, float3(0.299, 0.587, 0.114));

				float combinedLightness =
					0.2 * (current_lightness
					+ left_lightness
					+ right_lightness
					+ top_lightness
					+ bottom_lightness);

				if (combinedLightness > _Threshold)
				{
					current_pixel = float4(1, 1, 1, 1);
				}
				else
				{
					current_pixel = float4(0, 0, 0, 1);
				}
				
				return current_pixel;
			}
			ENDCG
		}
	}
}
