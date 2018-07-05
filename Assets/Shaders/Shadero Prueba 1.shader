//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Shadero Prueba 1"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
SpriteSheetFrameUV_Size_1("SpriteSheetFrameUV_Size_1", Range(2, 16)) = 2
SpriteSheetFrameUV_Frame_1("SpriteSheetFrameUV_Frame_1", Range(0, 3)) = 0
_TintRGBA_Color_1("_TintRGBA_Color_1", COLOR) = (1,0,0,1)
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
#pragma multi_compile _ PIXELSNAP_ON
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
float SpriteSheetFrameUV_Size_1;
float SpriteSheetFrameUV_Frame_1;
float4 _TintRGBA_Color_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 TintRGBA(float4 txt, float4 color)
{
float3 tint = dot(txt.rgb, float3(.222, .707, .071));
tint.rgb *= color.rgb;
txt.rgb = lerp(txt.rgb,tint.rgb,color.a);
return txt;
}
float4 Four_Gradients(float4 txt, float2 uv, float4 col1, float4 col2, float4 col3, float4 col4)
{
float4 colorA = lerp(col3,col4, uv.x*1.1);
float4 colorB = lerp(col1,col2, uv.x*1.1);
float4 colorC = lerp(colorA,colorB, uv.y*1.1);
colorA = lerp(txt, colorC, colorC.a);
colorA.a = txt.a;
return colorA;
}
float2 SpriteSheetFrame(float2 uv, float size, float frame)
{
frame = int(frame);
uv /= size;
uv.x += fmod(frame,size) / size;
uv.y -=1/size;
uv.y += 1-floor(frame / size) / size;
return uv;
}
float4 frag (v2f i) : COLOR
{
float2 SpriteSheetFrameUV_1 = SpriteSheetFrame(i.texcoord,SpriteSheetFrameUV_Size_1,SpriteSheetFrameUV_Frame_1);
float4 _MainTex_1 = tex2D(_MainTex,SpriteSheetFrameUV_1);
float4 TintRGBA_1 = TintRGBA(_MainTex_1,_TintRGBA_Color_1);
float4 FinalResult = TintRGBA_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
