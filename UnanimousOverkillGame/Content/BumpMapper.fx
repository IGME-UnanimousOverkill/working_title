SamplerState TextureSampler : register(s0);
SamplerState CustomSampler : register(s1);

Texture2D diffuse : register(t0);
Texture2D bump : register(t1);

// Eye position
float3 camPos;
float2 mouseOffset;


float4 main(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	int maxSamples = 64;

	// Adjust the texture coordinate based on the mouse (and slow it down slightly)
	texCoord.xy -= mouseOffset * 0.5f;

	// Normalized vector from cam to current pixel
	float3 camToPixel = normalize(float3(position.xy, 0) - camPos) / maxSamples * 0.02f;
	float3 samplePos = float3(texCoord, 1);

	// How far "into" the texture each step
	float depthPerSample = 1.0f / maxSamples;

	// Keep sampling the bump map
	for (int i = 0; i < maxSamples; i++)
	{
		// Sample the current depth, which tells us the depth 
		// "under" the ray
		float depth = bump.Sample(CustomSampler, samplePos.xy).r;
		if (depth > samplePos.z)
		{
			// We've crossed the "boundary"
			break;
		}

		// Adjust our sample position
		samplePos.xy += camToPixel.xy;
		samplePos.z -= depthPerSample;
	}

	// Sample the actual texture (and tint it with the bump map - fake shadows!)
	return diffuse.Sample(CustomSampler, samplePos.xy);// *bump.Sample(CustomSampler, samplePos.xy);
}


technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_5_0 main();
	}
}