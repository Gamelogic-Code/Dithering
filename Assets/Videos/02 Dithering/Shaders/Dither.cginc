/*
	Shader code shared among shaders in the dithering video. 
*/

#include "UnityCG.cginc"

#define CENTER float2(0.5, 0.5)

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
};

struct v2f
{
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _MainTex_ST;

float4 sample_ramp(sampler2D ramp_texture, float sampleInput)
{
	return tex2D(ramp_texture, float2(sampleInput, 0.5));
}

v2f vert(appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	return o;
}

float to_lightness(float3 color)
{
	return dot(color, float3(0.299, 0.587, 0.114));
}

float quantize(float x, int levels)
{
	return floor(x * levels) / levels;
}

float2 quantize(float2 x, int levels)
{
	return floor(x * levels) / levels;
}

float3 quantize(float3 x, int levels)
{
	return floor(x * levels) / levels;
}

float4 quantize(float4 x, int levels)
{
	return floor(x * levels) / levels;
}

float rand(float2 uv)
{
	return frac(sin(dot(uv.xy, float2(12.9898, 78.233))) * 43758.5453);
}

float dither2(float2 uv, int levels)
{
	int2 uv_scaled = floor(uv * levels);
	return uv_scaled.x % 2 == uv_scaled.y % 2 ? 0 : 1;
}

int pixel_mod(float x, int image_width, int grid_width)
{
	return floor(x * image_width) % grid_width;
}


int2 pixel_mod(float2 uv, int2 image_size, int2 grid_size)
{
	return floor(uv * image_size) % grid_size;
}

float2 transform_uv(float2 uv, float2 offset, float rotation_in_degrees)
{
	float theta = radians(rotation_in_degrees); // Convert degrees to radians
	float cos_theta = cos(theta);
	float sin_theta = sin(theta);
				
	float2 centered_uv = uv - CENTER;
	float2 rotated_uv;
	rotated_uv.x = centered_uv.x * cos_theta - centered_uv.y * sin_theta;
	rotated_uv.y = centered_uv.x * sin_theta + centered_uv.y * cos_theta;
				
	return rotated_uv + CENTER + offset;
}

// Displacement functions
float2 nop(float2 uv)
{
	return uv;
}

float2 diagonal(float2 uv)
{
	return float2(uv.x, uv.y + 0.2*uv.x);
}

float2 wave(float2 uv, float amplitude, float frequency)
{
	return float2(uv.x, uv.y + amplitude * sin(frequency*uv.x));
}

float2 wave_lightness(float2 uv, float amplitude, float frequency, float lightness)
{
	return float2(uv.x, uv.y +  amplitude*8*(1 - lightness) * sin(frequency * uv.x));
}

float sqr(float x)
{
	return x * x;
}

float circle(float2 uv)
{
	float k = 1;
	float x = (uv.x * k) % 1;
	float y = (uv.y * k) % 1;
	float x0 = x + 0.2;
	float y0 = y - 0.5;
	float u = 2 * sqrt(sqr(x0) + sqr(y0));
	float v = atan2(y0, x0) / (2 * UNITY_PI) + 0.5;

	return float2(u, v);
}

float2 rotate(float2 uv, float angle)
{
	float cos_a = cos(angle);
	float sin_a = sin(angle);
	return float2(uv.x * cos_a - uv.y * sin_a, uv.x * sin_a + uv.y * cos_a);
}
