//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_PlasmaFX_Fade_1("_PlasmaFX_Fade_1", Range(0, 1)) = 0.3525641
_PlasmaFX_Speed_1("_PlasmaFX_Speed_1", Range(0, 1)) = 0.5961538
_Add_Fade_1("_Add_Fade_1", Range(0, 4)) = 0
_Generate_Shape_PosX_1("_Generate_Shape_PosX_1", Range(-1, 2)) = -1
_Generate_Shape_PosY_1("_Generate_Shape_PosY_1", Range(-1, 2)) = 0.5000013
_Generate_Shape_Size_1("_Generate_Shape_Size_1", Range(-1, 1)) = 0.0256418
_Generate_Shape_Dist_1("_Generate_Shape_Dist_1", Range(0, 1)) = 0.515385
_Generate_Shape_Side_1("_Generate_Shape_Side_1", Range(1, 32)) = 2
_Generate_Shape_Rotation_1("_Generate_Shape_Rotation_1", Range(-360, 360)) = 36
_Add_Fade_2("_Add_Fade_2", Range(0, 4)) = 0.3435919
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

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
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
sampler2D _NewTex_1;
float _PlasmaFX_Fade_1;
float _PlasmaFX_Speed_1;
float _Add_Fade_1;
float _Generate_Shape_PosX_1;
float _Generate_Shape_PosY_1;
float _Generate_Shape_Size_1;
float _Generate_Shape_Dist_1;
float _Generate_Shape_Side_1;
float _Generate_Shape_Rotation_1;
float _Add_Fade_2;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


inline float RBFXmod(float x,float modu)
{
return x - floor(x * (1.0 / modu)) * modu;
}

float3 RBFXrainbow(float t)
{
t= RBFXmod(t,1.0);
float tx = t * 8;
float r = clamp(tx - 4.0, 0.0, 1.0) + clamp(2.0 - tx, 0.0, 1.0);
float g = tx < 2.0 ? clamp(tx, 0.0, 1.0) : clamp(4.0 - tx, 0.0, 1.0);
float b = tx < 4.0 ? clamp(tx - 2.0, 0.0, 1.0) : clamp(6.0 - tx, 0.0, 1.0);
return float3(r, g, b);
}

float4 Plasma(float4 txt, float2 uv, float _Fade, float speed)
{
float _TimeX=_Time.y * speed;
float a = 1.1 + _TimeX * 2.25;
float b = 0.5 + _TimeX * 1.77;
float c = 8.4 + _TimeX * 1.58;
float d = 610 + _TimeX * 2.03;
float x1 = 2.0 * uv.x;
float n = sin(a + x1) + sin(b - x1) + sin(c + 2.0 * uv.y) + sin(d + 5.0 * uv.y);
n = RBFXmod(((5.0 + n) / 5.0), 1.0);
float4 nx=txt;
n += nx.r * 0.2 + nx.g * 0.4 + nx.b * 0.2;
float4 ret=float4(RBFXrainbow(n),txt.a);
return lerp(txt,ret,_Fade);
}
float4 Generate_Shape(float2 uv, float posX, float posY, float Size, float Smooth, float number, float black, float rot)
{
uv = uv - float2(posX, posY);
float angle = rot * 0.01744444;
float a = atan2(uv.x, uv.y) +angle, r = 6.28318530718 / int(number);
float d = cos(floor(0.5 + a / r) * r - a) * length(uv);
float dist = 1.0 - smoothstep(Size, Size + Smooth, d);
float4 result = float4(1, 1, 1, dist);
if (black == 1) result = float4(dist, dist, dist, 1);
return result;
}
float4 frag (v2f i) : COLOR
{
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 _PlasmaFX_1 = Plasma(NewTex_1,i.texcoord,_PlasmaFX_Fade_1,_PlasmaFX_Speed_1);
NewTex_1 = lerp(NewTex_1,NewTex_1*NewTex_1.a + _PlasmaFX_1*_PlasmaFX_1.a,_Add_Fade_1);
float4 _Generate_Shape_1 = Generate_Shape(i.texcoord,_Generate_Shape_PosX_1,_Generate_Shape_PosY_1,_Generate_Shape_Size_1,_Generate_Shape_Dist_1,_Generate_Shape_Side_1,0,_Generate_Shape_Rotation_1);
NewTex_1 = lerp(NewTex_1,NewTex_1*NewTex_1.a + _Generate_Shape_1*_Generate_Shape_1.a,_Add_Fade_2 * NewTex_1.a);
float4 FinalResult = NewTex_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
