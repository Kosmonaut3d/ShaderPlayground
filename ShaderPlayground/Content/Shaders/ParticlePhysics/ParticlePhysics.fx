
//Transition shader, TheKosmonaut 2017

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  VARIABLES
////////////////////////////////////////////////////////////////////////////////////////////////////////////

Texture2D TexBuffer;
float2 TexBufferSize = float2(4, 4);
float AspectRatio = 1.6f;

float2 Resolution = float2(1280, 800);
float2 InvResolution = float2(1.0f/1280, 1.0f/800);

float2 MousePosition = float2(800, 600);

float Size = 0.0006f;

float SpringConstant = 0.02f;
float DampeningConstant = 0.98f;

Texture2D Sprite;

SamplerState texSampler
{
	Texture = <Sprite>;
    AddressU = CLAMP;
    AddressV = CLAMP;
    MagFilter = LINEAR;
    MinFilter = LINEAR;
    Mipfilter = LINEAR;
};

 
////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  STRUCTS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

struct VertexShaderInput
{
    float3 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD;
};

struct VertexShaderColorOutput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD;
	float4 Color : COLOR;
};

//FULLSCREENQUAD

struct VertexShaderFSQInput
{
	float2 Position : POSITION0;
};


////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  FUNCTIONS
////////////////////////////////////////////////////////////////////////////////////////////////////////////

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  VERTEX SHADER
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

VertexShaderColorOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderColorOutput output;

	float edge = input.Position.z;

	// 0,2 is left - 1,3 is right
	float x = ((edge % 2) - 0.5f) * 2;

	//0,1 is bottom (+) - 2,3 is top (-)
	float y = -sign(edge - 1.5f);

	float2 texBufferPosition = input.Position.xy*0.5f + float2(0.5f,0.5f);
	/*texBufferPosition = 1 - texBufferPosition.y;*/

	//Physical Position
	float4 buffer = TexBuffer.Load(int3(texBufferPosition*TexBufferSize, 0));

	float2 pos = buffer.rg;
	float2 speed = buffer.ba;

	pos.y = 1 - pos.y;
	pos = pos * 2 - 1;

	float speedLength = length(speed);
	
	float size = Size * (1 + speedLength * 300);

    output.Position = float4(/*input.Position.xy + */pos + float2(x,y * AspectRatio) * size ,1, 1);

	output.TexCoord = float2(input.Position.xy + float2(x, (1-y) * AspectRatio) * size)*0.5f + 0.5f;
	
	float colorShift = saturate(speedLength * 200);

	output.Color = float4(1+colorShift,1, 2-colorShift, 0.5f);

    return output;
}

//FULLSCREENQUAD
//Pass a v4 position and v2 texcoord from only v2 input
VertexShaderOutput VertexShaderFSQFunction(VertexShaderFSQInput input)
{
	VertexShaderOutput output;

	output.Position = float4(input.Position.xy, 1, 1);
	output.TexCoord = input.Position.xy*0.5f + 0.5f;
	output.TexCoord.y = 1 - output.TexCoord.y;

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

float4 PixelShaderFunction(VertexShaderColorOutput input) : COLOR
{
	return Sprite.Sample(texSampler, input.TexCoord) *input.Color;
}

float4 PixelShaderPhysicsFunction(VertexShaderOutput input) : COLOR
{
	//default position 
	float2 defaultPosition = input.TexCoord * Resolution;

	//IsPosition
	float4 buffer = TexBuffer.Load(int3(input.TexCoord.xy*TexBufferSize, 0));
	float2 isPosition = buffer.rg * Resolution;
	float2 isSpeed = buffer.ba * Resolution;

	//Spring physics
	isSpeed += (-isPosition + defaultPosition) * SpringConstant;

	//Mouse
	float2 vec = MousePosition - isPosition;

	vec = float2(vec.y, -vec.x);

	float len = length(vec);
	if (len <= 200)
	{
		//linear from 0 to 1 in the center
		float x = (1 - len / 200);
		float y = 1 - abs(x - 0.5f) * 2;
		isSpeed += y * vec/len * 0.5f;
	}

	isSpeed *= DampeningConstant;

	isPosition += isSpeed;

	return float4(isPosition *InvResolution , isSpeed *InvResolution);
}

float4 PixelShaderPhysicsInitializeFunction(VertexShaderOutput input) : COLOR
{
	//default position 
	float2 defaultPosition = input.TexCoord;

	return float4(defaultPosition, 0,0);
}


////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//  TECHNIQUES
	////////////////////////////////////////////////////////////////////////////////////////////////////////////

technique Default
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFunction();
		PixelShader = compile ps_5_0 PixelShaderFunction();
	}
}

technique Physics
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFSQFunction();
		PixelShader = compile ps_5_0 PixelShaderPhysicsFunction();
	}
}

technique PhysicsInitialize
{
	pass Pass1
	{
		VertexShader = compile vs_5_0 VertexShaderFSQFunction();
		PixelShader = compile ps_5_0 PixelShaderPhysicsInitializeFunction();
	}
}

