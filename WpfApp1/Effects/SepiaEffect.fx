sampler2D input : register(s0);

float4 main(float2 coord : TEXCOORD) : COLOR
{
	float4 source = tex2D(input, coord);
	
	float4 result;
	result.r = source.r * 0.393 + source.g * 0.769 + source.b * 0.189;
	result.g = source.r * 0.349 + source.g * 0.686 + source.b * 0.168;
	result.b = source.r * 0.272 + source.g * 0.534 + source.b * 0.131;
	result.a = source.a;
	
	return result;
}