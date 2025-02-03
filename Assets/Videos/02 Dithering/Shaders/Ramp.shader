/* 
	A post-process shader that renders a dark-to-light ramp, or a ramp for each RGB channel. This is useful to debug 
	other	post-process shaders.  
*/

Shader "Gamelogic/Fx/Ramp"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RGB("RGB ?", Int) = 0 // 0: Lightness, Otherwise: RGB
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
\
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
			int _RGB;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col;
				if (_RGB == 0)
				{
					col.rgb = float3(1, 1, 1) * i.uv.x;
				}
				else // _RGB == 1
				{
					if (i.uv.y < 0.333)
					{
						col.rgb = float3(1, 0, 0) *i.uv.x;
					}
					else if (i.uv.y < 0.66666)
					{
						col.rgb = float3(0, 1, 0) *i.uv.x;
					}
					else
					{
						col.rgb = float3(0, 0, 1)* i.uv.x;
					}
				}
				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
}
