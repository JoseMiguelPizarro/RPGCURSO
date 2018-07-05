//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Previews/GenerateXATXQ1"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float3 Generate_Voronoi_hash3(float2 p, float seed)
{
float3 q = float3(dot(p, float2(127.1, 311.7)),
dot(p, float2(269.5, 183.3)),
dot(p, float2(419.2, 371.9)));
return frac(sin(q) * 43758.5453 * seed);
}
float4 Generate_Voronoi(float2 uv, float size,float seed, float black)
{
float2 p = floor(uv*size);
float2 f = frac(uv*size);
float k = 1.0 + 63.0*pow(1.0, 4.0);
float va = 0.0;
float wt = 0.0;
for (int j = -2; j <= 2; j++)
{
for (int i = -2; i <= 2; i++)
{
float2 g = float2(float(i), float(j));
float3 o = Generate_Voronoi_hash3(p + g, seed)*float3(1.0, 1.0, 1.0);
float2 r = g - f + o.xy;
float d = dot(r, r);
float ww = pow(1.0 - smoothstep(0.0, 1.414, sqrt(d)), k);
va += o.z*ww;
wt += ww;
}
}
float4 result = saturate(va / wt);
result.a = saturate(result.a + black);
return result;
}
float4 frag (v2f i) : COLOR
{
float4 uv_rgba_1 =  float4(i.texcoord.r,i.texcoord.g,0,1);
float4 FinalResult = uv_rgba_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
