////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  Draw meshes with color == id
//  Or
//  Draw selected meshes with a a color overlay and outlines

matrix  WorldViewProj;
matrix World;

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  STRUCT DEFINITIONS

struct DrawNormal_VSIn
{
	float4 Position : POSITION;
	float3 Normal : NORMAL0;
};

struct DrawNormal_VSOut
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL0;
};

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  FUNCTION DEFINITIONS

//For outlines extrude the backfacing normals
DrawNormal_VSOut SSSVertexShader(DrawNormal_VSIn input)
{
	DrawNormal_VSOut Output;

	float4 normal = mul(float4(input.Normal, 0), WorldViewProj);

	Output.Position = mul(input.Position, WorldViewProj);
	Output.Normal = normal.xyz;
	return Output;
}

//------------------------ PIXEL SHADER ----------------------------------------

float4 SSSPixelShader(DrawNormal_VSOut input) : SV_Target
{
	return float4(1,1,1, 0);
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////
//  TECHNIQUES

technique Tech
{
	pass Pass1
	{
		VertexShader = compile vs_4_0 SSSVertexShader();
		PixelShader = compile ps_4_0 SSSPixelShader();
	}
}
