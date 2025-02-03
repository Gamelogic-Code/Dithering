/*
	This writes lightness into R, and UV into BG.
	This can be used in a post-process dithering process (such as HybridDitheringPost.shader)
*/
Shader "Gamelogic/Fx/Dithering/Hybrid Dithering Object"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Opaque"
		}
		LOD 200

		CGPROGRAM
		#define VISUALIZE_RESULT
		
		#pragma surface surf Dither fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			float lightness = DotClamped(c, float3(0.299, 0.587, 0.114));
			o.Albedo = _Color.rgb; //float3(lightness, IN.uv_MainTex.x, IN.uv_MainTex.y);
			o.Alpha = IN.uv_MainTex.x;
			o.Gloss = IN.uv_MainTex.y;
		}

		half4 LightingDither(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);
			half diff = max (0, dot (s.Normal, lightDir));
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, 48.0);
			
			half3 color = (s.Albedo.r * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
			float lightness = DotClamped(color, float3(0.299, 0.587, 0.114)) + 0.05; //Cheat ambient
			half4 c;

			#ifdef VISUALIZE_LIGHTNESS
			c.rgb = float3(lightness, 0, 0);
			#endif
			
			#ifdef VISUALIZE_UV
			c.rgb = float3(0, s.Alpha, s.Gloss);
			#endif

			#ifdef VISUALIZE_RESULT
			c.rgb = float3(lightness, s.Alpha, s.Gloss);
			#endif

			
			
			c.a = 1;
			return c;
		}

		ENDCG
	}
	FallBack "Diffuse"
}
