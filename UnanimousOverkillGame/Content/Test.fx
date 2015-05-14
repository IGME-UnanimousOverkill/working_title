SamplerState TextureSampler : register(s0);
SamplerState CustomSampler : register(s1);

Texture2D tex : register(t0);
Texture2D normalMap : register(t1);

float3 lightPos;
float4 lightColor;

float4 main(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{

	// Sample normal map texture and unpack the vector
	//float4 normal = normalMap.Sample(CustomSampler, texCoord) * 2 - 1;
	//normal.z *= 1;
	//normal = normalize(normal);

	// Do lighting calculations
	float3 vecToLight = lightPos - position.xyz;
	vecToLight.y *= -1;
	float lightDist = length(vecToLight);

	//float3 dirToLight = normalize(vecToLight);
	float lightAmount = saturate(dot(.8, 1));
	
	//Decides the size of the lightcircle around the player
	lightAmount -= lightDist*.002;

	// Sample texture
	float4 texColor = tex.Sample(TextureSampler, texCoord);
	
	//texColor = lerp(texColor, lightColor, lightAmount);
	texColor.rgb *= lightAmount * lightColor.rgb;
	
	return texColor * color;
}


technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_5_0 main();
	}
}