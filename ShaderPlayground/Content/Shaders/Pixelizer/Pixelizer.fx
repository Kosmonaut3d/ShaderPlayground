
//Transition shader, TheKosmonaut 2017

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VARIABLES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture2D Screen;

float AspectRatio = 1.6f;

float2 Resolution = float2(1280, 800);

float2 MousePosition = int2(-1, -1);

float3 LineColor = float3(0, 1, 0);

float SplitChance = 0.03f;

float EndChance = 0.95f;

float Time = 0;


float rows = 10;
float cols = 20;

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
	int2 texCoordInt = float2(input.TexCoord * Resolution);

	float4 color = Screen.Load(int3(texCoordInt,0));

	return color;
}



float4 PixelShaderFunctionTex(VertexShaderOutput input) : COLOR
{
	/*float invrows = 1 / rows;
	float invcols = 1 / cols;
	float2 localTexCoords = float2(input.TexCoord.x % invcols, input.TexCoord.y % invrows) * float2(cols, rows);

	if (localTexCoords.x + 1-localTexCoords.y > 1) return Screen.Sample(texSampler, input.TexCoord).rgba;
	*/

	//Idea -> encode bits
	//1 means goes vertical
	//0 1 means goes horizontal
	//0 0 means empty
	// if (a * 255) > 0 && < 64) it's still colored!

	const float frac64 = 1/256.0f; // 1/64

	const int2 offsets[] = {
		int2(1,0),
		int2(-1,0),
		int2(0,1),
		int2(0,-1),
	};
	

	int2 texCoordInt = float2(input.TexCoord * Resolution);

	float4 color = Screen.Load(int3(texCoordInt,0));

	//Timer -> decay


	//first delete the first values
	if (color.a > 0)
	{
		if (color.a > 0.25)//first significant bits are filled
		{
			color.a = 0.25f;
		}
		
		color.a -= frac64;

		color = float4(LineColor*color.a*4, color.a);

		//End here!
		return color;
	}

	//Alpha value stores direction and time

	//If we register a mouse click, draw pixel
	if (texCoordInt.x == MousePosition.x && texCoordInt.y == MousePosition.y)
	{
		return float4(LineColor, 1);
	}

	//Read neighbor
	float random = (frac(sin(dot(input.TexCoord, float2(15.8989f, 76.132f) * (1.0f+frac(Time)))) * 46336.23745f));

	if (random > EndChance) return float4(0, 0, 0, 0);

	//Horizontal
	[loop]
	for (int i = 0; i < 2; i++)
	{
		float4 neighbor = Screen.Load(int3(texCoordInt + offsets[i], 0));

		//int alpha = neighbor.a * 255;

		//////Check first bit
		//if (alpha >> 7 > 0)
		if(neighbor.a > 0.5f)
			color = float4(LineColor, random > SplitChance ? 0.6f : 0.3f);
	}

	//Vertical 

	[loop]
	for (int j = 2; j < 4; j++)
	{
		float4 neighbor = Screen.Load(int3(texCoordInt + offsets[j], 0));

		//int alpha = neighbor.a * 255;

		//////Check first bit
		//if (alpha >> 7 > 0)
		if ((neighbor.a > 0.25f && neighbor.a < 0.5f) || neighbor.a >=1)
			color = float4(LineColor, random > SplitChance ? 0.3f : 0.6f);
	}


	return color;
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  TECHNIQUES
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Texture
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}

technique Animate
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunctionTex();
	}
}

