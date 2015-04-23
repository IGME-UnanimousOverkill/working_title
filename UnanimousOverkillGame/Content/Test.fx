SamplerState TextureSampler : register(s0);
SamplerState CustomSampler : register(s1);

Texture2D mario : register(t0);
Texture2D normalMap : register(t1);

float3 lightPos;
float4 lightColor;

float4 main(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	// "Scale" the uv coordinates

	// Sample normal map texture and unpack the vector
	float4 normal = normalMap.Sample(CustomSampler, texCoord) * 2 - 1;
	normal.z *= 1;
	normal = normalize(normal);


	// Do lighting calculations
	float3 vecToLight = lightPos - position.xyz;
	vecToLight.y *= -1;
	float lightDist = length(vecToLight);

	float3 dirToLight = normalize(vecToLight);
	float lightAmount = saturate(dot(normal.xyz, dirToLight))+.02f;
	

	// Now sample mario's texture
	float4 texColor = mario.Sample(TextureSampler, texCoord);
	
	//texColor = lerp(texColor, lightColor, lightAmount);
	texColor.rgb *= lightAmount * lightColor.rgb;
	
	return texColor;
}


technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_5_0 main();
	}
}