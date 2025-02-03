/*
	Per-object dithering shader. 
*/

Shader "Gamelogic/Fx/Dithering/Object Dithering"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DitherPatternTex ("Dither Pattern", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}
		LOD 200

		CGPROGRAM
		#pragma surface surf Dither fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DitherPatternTex;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_DitherPatternTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 d = tex2D(_DitherPatternTex, IN.uv_DitherPatternTex);

			o.Albedo = c.rgb;
			o.Alpha = d;
		}

		half4 LightingDither(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);
			half diff = max (0, dot (s.Normal, lightDir));
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 48.0);
			
			half3 color = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			float lightness = DotClamped(color, float3(0.299, 0.587, 0.114));
			float bias = lerp(-0.4, 0.4, s.Alpha);
			lightness = floor((lightness + bias) * 2) / 2.0;
			half4 c;
			c.rgb = float3(lightness, lightness, lightness);
			c.a = 1;
			return c;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
