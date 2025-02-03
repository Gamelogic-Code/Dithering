/*
	Post effect shader that transforms the lightness of the image using a ramp texture. 
*/
Shader "Gamelogic/Fx/Color Transform (Ramp)"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ColorTransformTex ("Ramp Texture", 2D) = "white" {}
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
			
			sampler2D _ColorTransformTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				float lightness = to_lightness(color.rgb);
				fixed4 transformed_color = sample_ramp(_ColorTransformTex, lightness);
				return transformed_color;
			}
			ENDCG
		}
	}
}
