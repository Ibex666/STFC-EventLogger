sampler2D input : register(s0);

float4 main(float2 coord : TEXCOORD) : COLOR
{
	float4 source = tex2D(input, coord);
	
	float4 result;
	result.r = 1 - source.r;
	result.g = 1 - source.g;
	result.b = 1 - source.b;
	result.a = source.a;
	
	return result;
}