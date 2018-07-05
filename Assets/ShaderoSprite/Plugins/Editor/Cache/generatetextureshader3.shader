//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Previews/GenerateXATXQ3"
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


float2 HumanBreathExUV(float2 uv, sampler2D smp, float intensity, float speed, float sideintensity)
{
float t = _Time * 15 * speed;
float i = intensity * 0.01;
float si = sideintensity * 0.01;
float val = (sin(t * 3.1415));
float val2 = exp(-sin(t * 3.1415));
float org = val * i - i / 2;
float4 n = tex2D(smp, uv+org);
uv.y = lerp(uv.y, uv.y+org, n.r);
n = tex2D(smp, uv);
uv.x = lerp(uv.x, uv.x + val2 * si, n.g);
uv.x = lerp(uv.x, uv.x - val2 * si, n.b);
return uv;
}
float4 GenerateSOFTexture(float4 t1, float4 t2, float4 t3, float4 t4, float4 t5, float4 t6)
{
int rt1 = (int)(t1 * 255) >> 4;
int rt2 = (int)(t2 * 255) >> 4;
int rt3 = (int)(t3 * 255) >> 4;
int rt4 = (int)(t4 * 255) >> 4;
int rt5 = (int)(t5 * 255) >> 4;
int rt6 = (int)(t6 * 255) >> 4;
int r = (rt2 << 4) + rt1;
int g = (rt4 << 4) + rt3;
int b = (rt6 << 4) + rt5;
return float4((float)r * 0.003921, (float)g * 0.003921, (float)b * 0.003921, 1);
}
float4 frag (v2f i) : COLOR
{
float4 SplitRGBA_1 = float4(0,0,0,1);
float4 FinalResult = SplitRGBA_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
