/*
	Full screen post process shader that shows a texture overlay. (Useful for debugging)
*/


Shader "Gamelogic/Fx/Texture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Overlay ("Texture", 2D) = "white" {}
		_Alpha ("Alpha", Range(0, 1)) = 1
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

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _Overlay;
			float _Alpha;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col1 = tex2D(_MainTex, i.uv);
				fixed4 col2 = tex2D(_Overlay, i.uv);

				return lerp(col1, col2, _Alpha);
			}
			ENDCG
		}
	}
}
