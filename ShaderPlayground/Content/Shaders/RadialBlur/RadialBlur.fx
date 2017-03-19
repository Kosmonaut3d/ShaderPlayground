﻿
//Transition shader, TheKosmonaut 2017

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VARIABLES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture2D Screen;

float Timer = 0;

float AspectRatio = 1.6f;

float2 MousePosition = float2(0.5f, 0.5f);

float BlurIntensity = 0.2f;

float BlurSamples = 10;
float InvBlurSamples = 0.1f;

SamplerState texSampler
{
    Texture = (Screen);
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = POINT;
    MinFilter = POINT;
    Mipfilter = POINT;
};
 
////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  STRUCTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct VertexShaderInput
{
    float3 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  FUNCTIONS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  VERTEX SHADER
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = float4(input.Position, 1);
    output.TexCoord = input.TexCoord;
    return output;

}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  PIXEL SHADER
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//  HELPER FUNCTIONS
		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//  Main function
		////////////////////////////////////////////////////////////////////////////////////////////////////////////

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	
	float2 vec = MousePosition - input.TexCoord;

	//vec.x *= AspectRatio;

	float4 color;

	float invBlurSamples = 1.0f / BlurSamples;

	for (int i = 0; i < BlurSamples; i++)
	{
		color += Screen.Sample(texSampler, input.TexCoord + vec * i * invBlurSamples * BlurIntensity);
	}

	color *= invBlurSamples;

	return float4(color.rgb, 1) ;
}

float4 PixelShaderFunctionTex(VertexShaderOutput input) : COLOR
{
	return Screen.Sample(texSampler, input.TexCoord);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  TECHNIQUES
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Blur
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}

technique Texture
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionTex();
	}
}

