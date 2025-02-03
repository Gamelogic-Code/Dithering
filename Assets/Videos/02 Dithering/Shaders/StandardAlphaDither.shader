/*
	Ordinary surface shader that uses dithered alpha cutoff instead of normal alpha blending. 
*/
Shader "Gamelogic/Fx/Dithering/Standard (Alpha Dithering)"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Alpha("Alpha", Range(0, 1)) = 1
		_Cutoff ("Alpha Cutoff", Range(0, 1)) = 0.5
		
		_DitherAmountMin ("Offset 0", Float) = -0.5
		_DitherAmountMax ("Offset 1", Float) = 0.5
		_PixelSize ("Pixel Size", Vector) = (1, 1, 0, 0)
	}
	SubShader
	{
		Tags
		{
			"Queue" = "AlphaTest" "RenderType" = "TransparentCutout"
		}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard alpha:test
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _NoiseTex_ST;

		float _Cutoff;
		float _Alpha;
		float _DitherAmountMin;
		float _DitherAmountMax;
		float2 _PixelSize;

		struct Input
		{
			float2 uv_MainTex;
			float4 screenPos;
		};

		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			const int matrix_size = 4; 
			float2 image_size = float2(1024, 1024);
			
			fixed4 color = tex2D(_MainTex, IN.uv_MainTex);
			color.a = color.a * _Alpha; 
			float2 screen_uv = IN.screenPos.xy / IN.screenPos.w;
			screen_uv = screen_uv * 0.5 + 0.5;
			
			int2 pixel = floor(screen_uv * image_size / _PixelSize) % matrix_size;

			float array[16] =
			{
				0,  8,  2, 10,
				12,  4, 14,  6,
				3, 11,  1,  9,
				15,  7, 13,  5
			};

			float offset = lerp(_DitherAmountMin, _DitherAmountMax, array[pixel.y * 4 + pixel.x] / 15);

			if (color.a + offset < _Cutoff)
			{
				discard; 
			}
			
			o.Albedo = color.rgb;
			o.Alpha = color.a; // Retain alpha for shadow casting
		}
		ENDCG
	}
	FallBack "Diffuse"
}
