//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/FireCircle"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_Color ("Tint", Color) = (1,1,1,1)
[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
ZoomUV_Zoom_1("ZoomUV_Zoom_1", Range(0.2, 4)) = 1.803
ZoomUV_PosX_1("ZoomUV_PosX_1", Range(-3, 3)) = 0.551
ZoomUV_PosY_1("ZoomUV_PosY_1", Range(-3, 3)) =0.5
KaleidoscopeUV_PosX_1("KaleidoscopeUV_PosX_1",  Range(-2, 2)) = 0.497
KaleidoscopeUV_PosY_1("KaleidoscopeUV_PosY_1",  Range(-2, 2)) = 0.536
KaleidoscopeUV_Number_1("KaleidoscopeUV_Number_1", Range(0, 6)) = 3.912
_Generate_Fire_PosX_1("_Generate_Fire_PosX_1", Range(-1, 2)) = -0.035
_Generate_Fire_PosY_1("_Generate_Fire_PosY_1", Range(-1, 2)) = 0.408
_Generate_Fire_Precision_1("_Generate_Fire_Precision_1", Range(0, 1)) = 0.05
_Generate_Fire_Smooth_1("_Generate_Fire_Smooth_1", Range(0, 1)) = 0.308
_Generate_Fire_Speed_1("_Generate_Fire_Speed_1", Range(-2, 2)) = 0.703
_RGBA_Add_Fade_1("_RGBA_Add_Fade_1", Range(0, 4)) = 2.433
_CircleFade_PosX_1("_CircleFade_PosX_1", Range(-1, 2)) = 0.607
_CircleFade_PosY_1("_CircleFade_PosY_1", Range(-1, 2)) = 1.238
_CircleFade_Size_1("_CircleFade_Size_1", Range(-1, 1)) = 0.976
_CircleFade_Dist_1("_CircleFade_Dist_1", Range(0, 1)) = 0.265
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
sampler2D _SourceNewTex_1;
float ZoomUV_Zoom_1;
float ZoomUV_PosX_1;
float ZoomUV_PosY_1;
float KaleidoscopeUV_PosX_1;
float KaleidoscopeUV_PosY_1;
float KaleidoscopeUV_Number_1;
float _Generate_Fire_PosX_1;
float _Generate_Fire_PosY_1;
float _Generate_Fire_Precision_1;
float _Generate_Fire_Smooth_1;
float _Generate_Fire_Speed_1;
float _RGBA_Add_Fade_1;
float _CircleFade_PosX_1;
float _CircleFade_PosY_1;
float _CircleFade_Size_1;
float _CircleFade_Dist_1;

void vert(inout appdata_full v, out Input o)
{
v.vertex.xy *= _Flip.xy;
#if defined(PIXELSNAP_ON)
v.vertex = UnityPixelSnap (v.vertex);
#endif
UNITY_INITIALIZE_OUTPUT(Input, o);
o.color = v.color * _Color * _RendererColor;
}


float2 ZoomUV(float2 uv, float zoom, float posx, float posy)
{
float2 center = float2(posx, posy);
uv -= center;
uv = uv * zoom;
uv += center;
return uv;
}
float4 Circle_Fade(float4 txt, float2 uv, float posX, float posY, float Size, float Smooth)
{
float2 center = float2(posX, posY);
float dist = 1.0 - smoothstep(Size, Size + Smooth, length(center - uv));
txt.a *= dist;
return txt;
}
float Generate_Fire_hash2D(float2 x)
{
return frac(sin(dot(x, float2(13.454, 7.405)))*12.3043);
}

float Generate_Fire_voronoi2D(float2 uv, float precision)
{
float2 fl = floor(uv);
float2 fr = frac(uv);
float res = 1.0;
for (int j = -1; j <= 1; j++)
{
for (int i = -1; i <= 1; i++)
{
float2 p = float2(i, j);
float h = Generate_Fire_hash2D(fl + p);
float2 vp = p - fr + h;
float d = dot(vp, vp);
res += 1.0 / pow(d, 8.0);
}
}
return pow(1.0 / res, precision);
}

float4 Generate_Fire(float2 uv, float posX, float posY, float precision, float smooth, float speed, float black)
{
uv += float2(posX, posY);
float t = _Time*60*speed;
float up0 = Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(0, -t), precision);
float up1 = 0.5 + Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(42, -t ) + 30.0, precision);
float finalMask = up0 * up1  + (1.0 - uv.y);
finalMask += (1.0 - uv.y)* 0.5;
finalMask *= 0.7 - abs(uv.x - 0.5);
float4 result = smoothstep(smooth, 0.95, finalMask);
result.a = saturate(result.a + black);
return result;
}
float4 Color_PreGradients(float4 rgba, float4 a, float4 b, float4 c, float4 d, float offset, float fade, float speed)
{
float gray = (rgba.r + rgba.g + rgba.b) / 3;
gray += offset+(speed*_Time*20);
float4 result = a + b * cos(6.28318 * (c * gray + d));
result.a = rgba.a;
result.rgb = lerp(rgba.rgb, result.rgb, fade);
return result;
}
float4 OperationBlendMask(float4 origin, float4 overlay, float4 mask, float blend)
{
float4 o = origin; 
origin.rgb = overlay.a * overlay.rgb + origin.a * (1 - overlay.a) * origin.rgb;
origin.a = overlay.a + origin.a * (1 - overlay.a);
origin.a *= mask;
origin = lerp(o, origin,blend);
return origin;
}
float4 PatternMovementMask(float2 uv, sampler2D source, float4 rgba, float4 mask, float posx, float posy, float speed)
{
float t = _Time * 20 * speed;
uv = fmod(abs(uv+float2(posx*t, posy*t)),1);
float4 result = tex2D(source, uv);
result.a = result.a * rgba.a * mask.r;
return result;
}
float2 KaleidoscopeUV(float2 uv, float posx, float posy, float number)
{
uv = uv - float2(posx, posy);
float r = length(uv);
float a = abs(atan2(uv.y, uv.x));
float sides = number;
float tau = 3.1416;
a = fmod(a, tau / sides);
a = abs(a - tau / sides / 2.);
uv = r * float2(cos(a), sin(a));
return uv;
}
void surf(Input i, inout SurfaceOutput o)
{
float4 SourceRGBA_1 = tex2D(_SourceNewTex_1, i.uv_MainTex);
float2 ZoomUV_1 = ZoomUV(i.uv_MainTex,ZoomUV_Zoom_1,ZoomUV_PosX_1,ZoomUV_PosY_1);
float2 KaleidoscopeUV_1 = KaleidoscopeUV(ZoomUV_1,KaleidoscopeUV_PosX_1,KaleidoscopeUV_PosY_1,KaleidoscopeUV_Number_1);
float4 _Generate_Fire_1 = Generate_Fire(KaleidoscopeUV_1,_Generate_Fire_PosX_1,_Generate_Fire_PosY_1,_Generate_Fire_Precision_1,_Generate_Fire_Smooth_1,_Generate_Fire_Speed_1,0);
float4 _PatternMovementMask_1 = PatternMovementMask(i.uv_MainTex,_SourceNewTex_1,SourceRGBA_1,_Generate_Fire_1,-0.064,-0.415,0.503);
_PatternMovementMask_1.rg += _RGBA_Add_Fade_1;
float4 _CircleFade_2 = Circle_Fade(_Generate_Fire_1,i.uv_MainTex,0.5,0.5,0.3,0.2);
float4 _PremadeGradients_1 = Color_PreGradients(_CircleFade_2,float4(1,0,0.13,1),float4(0.42,0.95,0,1),float4(0.99,0.68,0.99,1),float4(0.39,0.39,1,1),-0.256,1,0);
float4 OperationBlendMask_1 = OperationBlendMask(_PremadeGradients_1, _PatternMovementMask_1, _CircleFade_2, 1); 
float4 _CircleFade_1 = Circle_Fade(OperationBlendMask_1,i.uv_MainTex,_CircleFade_PosX_1,_CircleFade_PosY_1,_CircleFade_Size_1,_CircleFade_Dist_1);
float4 FinalResult = _CircleFade_1;
o.Albedo = FinalResult.rgb* i.color.rgb;
o.Alpha = FinalResult.a * _SpriteFade * i.color.a;
clip(o.Alpha - 0.05);
}

ENDCG
}
Fallback "Sprites /Default"
}
