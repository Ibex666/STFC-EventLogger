sampler2D input : register(s0);

float4 main(float2 coord : TEXCOORD) : COLOR
{
	float4 source = tex2D(input, coord);
	
	float4 result;
	result.rgb = (source.r + source.g + source.b) / 3;
	result.a = source.a;
	
	return result;
}