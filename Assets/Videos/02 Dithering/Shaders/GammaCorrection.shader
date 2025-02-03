/*
	Post effect shader to control the gamma. 
*/
Shader "Gamelogic/Fx/Gamma Correction"
{
	Properties
	{
		_MainTex("Render Texture", 2D) = "white" {}
		_Gamma("Gamma Value", Float) = 2.2
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Overlay" }
		Pass
		{
			ZWrite Off
			Cull Off
			ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Gamma;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 color = tex2D(_MainTex, i.uv);
				color.rgb = pow(color.rgb, 1.0 / _Gamma);
				return color;
			}
			ENDCG
		}
	}
	Fallback Off
}
