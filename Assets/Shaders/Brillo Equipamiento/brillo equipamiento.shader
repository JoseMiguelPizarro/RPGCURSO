//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/brillo equipamiento"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_Color ("Tint", Color) = (1,1,1,1)
[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_Burn_Value_1("_Burn_Value_1", Range(0, 1)) = 0
_Burn_Speed_1("_Burn_Speed_1", Range(-8, 8)) = 0.672
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
_PatternMovement_PosX_1("_PatternMovement_PosX_1", Range(-2, 2)) = 0.164
_PatternMovement_PosY_1("_PatternMovement_PosY_1", Range(-2, 2)) = -1.643
_PatternMovement_Speed_1("_PatternMovement_Speed_1", Range(1, 16)) = 0.313
_Add_Fade_1("_Add_Fade_1", Range(0, 4)) = 4
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

}

SubShader
{
Tags
{
"Queue" = "Transparent"
"IgnoreProjector" = "True"
"RenderType" = "Transparent"
"PreviewType" = "Plane"
"CanUseSpriteAtlas" = "True"

}

Cull Off
Lighting Off
ZWrite Off
Blend SrcAlpha OneMinusSrcAlpha


CGPROGRAM

#pragma surface surf Lambert vertex:vert  nolightmap nodynlightmap keepalpha noinstancing
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
#include "UnitySprites.cginc"
struct Input
{
float2 uv_MainTex;
float4 color;
};

float _SpriteFade;
sampler2D _NewTex_1;
float _Burn_Value_1;
float _Burn_Speed_1;
sampler2D _SourceNewTex_1;
float _PatternMovement_PosX_1;
float _PatternMovement_PosY_1;
float _PatternMovement_Speed_1;
float _Add_Fade_1;

void vert(inout appdata_full v, out Input o)
{
v.vertex.xy *= _Flip.xy;
#if defined(PIXELSNAP_ON)
v.vertex = UnityPixelSnap (v.vertex);
#endif
UNITY_INITIALIZE_OUTPUT(Input, o);
o.color = v.color * _Color * _RendererColor;
}


float4 UniColor(float4 txt, float4 color)
{
txt.rgb = lerp(txt.rgb,color.rgb,color.a);
return txt;
}
float4 ColorRGBA(float4 txt, float4 color)
{
txt.rgb += color.rgb;
return txt;
}
float BFXr (float2 c, float seed)
{
return frac(43.*sin(c.x+7.*c.y)* seed);
}

float BFXn (float2 p, float seed)
{
float2 i = floor(p), w = p-i, j = float2 (1.,0.);
w = w*w*(3.-w-w);
return lerp(lerp(BFXr(i, seed), BFXr(i+j, seed), w.x), lerp(BFXr(i+j.yx, seed), BFXr(i+1., seed), w.x), w.y);
}

float BFXa (float2 p, float seed)
{
float m = 0., f = 2.;
for ( int i=0; i<9; i++ ){ m += BFXn(f*p, seed)/f; f+=f; }
return m;
}

float4 BurnFX(float4 txt, float2 uv, float value, float seed, float HDR)
{
float t = frac(value*0.9999);
float4 c = smoothstep(t / 1.2, t + .1, BFXa(3.5*uv, seed));
c = txt*c;
c.r = lerp(c.r, c.r*15.0*(1 - c.a), value);
c.g = lerp(c.g, c.g*10.0*(1 - c.a), value);
c.b = lerp(c.b, c.b*5.0*(1 - c.a), value);
c.rgb += txt.rgb*value;
c.rgb = lerp(saturate(c.rgb),c.rgb,HDR);
return c;
}
float4 PatternMovement(float2 uv, sampler2D source, float4 rgba, float posx, float posy, float speed)
{
float t = _Time * 20 * speed;
uv = fmod(abs(uv+float2(posx*t, posy*t)),1);
float4 result = tex2D(source, uv);
result.a = result.a * rgba.a;
return result;
}
void surf(Input i, inout SurfaceOutput o)
{
float4 _MainTex_1 = tex2D(_MainTex, i.uv_MainTex);
float4 NewTex_1 = tex2D(_NewTex_1, i.uv_MainTex);
float4 _Burn_1 = BurnFX(_MainTex_1,i.uv_MainTex,_Burn_Value_1,_Burn_Speed_1,0);
float4 _PatternMovement_1 = PatternMovement(i.uv_MainTex,_SourceNewTex_1,float4(0,0,0,1),_PatternMovement_PosX_1,_PatternMovement_PosY_1,_PatternMovement_Speed_1);
_Burn_1 = lerp(_Burn_1,_Burn_1*_Burn_1.a + _PatternMovement_1*_PatternMovement_1.a,_Add_Fade_1 * _Burn_1.a);
float4 FinalResult = _Burn_1;
o.Albedo = FinalResult.rgb* i.color.rgb;
o.Alpha = FinalResult.a * _SpriteFade * i.color.a;
o.Normal = UnpackNormal(NewTex_1);
clip(o.Alpha - 0.05);
}

ENDCG
}
Fallback "Sprites /Default"
}
