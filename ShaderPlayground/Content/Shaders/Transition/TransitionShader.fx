
//Transition shader, TheKosmonaut 2017

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VARIABLES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture2D Screen;

float Timer = 0;

float AspectRatio = 1.6f;

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

float4 PixelShaderFunctionShear(VertexShaderOutput input) : COLOR
{
	float4 colorSample = Screen.Sample(texSampler, input.TexCoord);

	const float border = 0.5f;

	const float shear = 0.2f;

	float run = Timer * (1 + border + shear);

	float a = 1;

	float position = input.TexCoord.x + shear*input.TexCoord.y;

	if (position < run)
	{
		position -= run-border;
		
		a = saturate(position / border);
	}

	return float4(colorSample.rgb, a);
}

float4 PixelShaderFunctionCircle(VertexShaderOutput input) : COLOR
{
	float4 colorSample = Screen.Sample(texSampler, input.TexCoord);

	const float border = 0.2f;

	float run = Timer * (1 + border);

	float a = 1;

	const float2 middle = float2(0.5f, 0.5f);

	float2 value = input.TexCoord - middle;
	value.x *= AspectRatio;

	float position = length(value);

	if (position < run)
	{
		position -= run - border;

		a = saturate(position / border);
	}

	return float4(colorSample.rgb, a);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	float4 colorSample = Screen.Sample(texSampler, input.TexCoord);

	const float PI = 3.14159265359;
	const float PI2 = 6.28318530718;
	const float PI2inv = 0.159154943;

	const float border = 0.15f;

	const float2 middle = float2(0.5f, 0.5f);

	float2 value = input.TexCoord - middle;
	value.x *= AspectRatio;

	float position = atan2(value.y, value.x) + PI/2;

	float dist = length(value);

	position += -dist * 100 * (1 - Timer) - Timer;

	while (position < 0) position += PI2;
    while (position > PI2) position -= PI2;

	float a = 1;

	const float sections = 1;
	const float sectionsinv = 1 / sections;

	float run = Timer*(1 /*+ border * sections*/);
	run *= sectionsinv;

	position *= PI2inv;
	position %= sectionsinv;


	if (position < run)
	{
		/*position -= run - border;

		a = saturate(position / border);*/
		a = 0;
	}


	return float4(colorSample.rgb , a);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  TECHNIQUES
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Shear
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionShear();
	}
}

technique Circle
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionCircle();
	}
}

technique Spiral
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}


