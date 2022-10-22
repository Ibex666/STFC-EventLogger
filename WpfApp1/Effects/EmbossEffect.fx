sampler2D input : register(s0);
float displacement : register(c0);
float strength : register(c1);

float4 main(float2 uv : TEXCOORD) : COLOR 
{ 
	float4 result = {0.5, 0.5, 0.5, 1.0}; //alpha 1 der rest mittelwert
	result = result - tex2D(input, uv - displacement) * strength;
	result = result + tex2D(input, uv + displacement) * strength;
	result.rgb = (result.r + result.b + result.g) / 3.0f; //Grayscale darüber legen
	
	return result; 
}