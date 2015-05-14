SamplerState TextureSampler : register(s0);

Texture2D regularTexture : register(t0);

// Pixel size
float2 pixelSize;
int blurAmount;
int rampCount;

float4 main(float4 position : SV_Position, float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{

	float2 uvOffset[9];
	uvOffset[0] = float2(0, 0);
	uvOffset[1] = float2(-pixelSize.x, -pixelSize.y);
	uvOffset[2] = float2(-pixelSize.x, 0);
	uvOffset[3] = float2(-pixelSize.x, pixelSize.y);
	uvOffset[4] = float2(0,            pixelSize.y);
	uvOffset[5] = float2(pixelSize.x,  pixelSize.y);
	uvOffset[6] = float2(pixelSize.x,  0);
	uvOffset[7] = float2(pixelSize.x, -pixelSize.y);
	uvOffset[8] = float2(0,           -pixelSize.y);

	// Sample all pixels
	float4 finalColor = regularTexture.Sample(TextureSampler, texCoord);
	for(int blur = 0; blur < blurAmount; blur++)
	{
		[unroll]
		for(int i = 1; i < 9; i++)
		{
			finalColor += regularTexture.Sample(TextureSampler, texCoord + uvOffset[i] * blur);
		}
	}
	finalColor /= max(1, 9 * blurAmount);

	// Ramp color
	if( rampCount > 0 )
		finalColor = round(finalColor * rampCount) / rampCount;

	// All done
	return finalColor;
}


technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_5_0 main();
	}
}