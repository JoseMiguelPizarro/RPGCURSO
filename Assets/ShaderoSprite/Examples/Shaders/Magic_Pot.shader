//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Magic_Pot"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_NewTex_4("NewTex_4(RGB)", 2D) = "white" { }
_SourceNewTex_2("_SourceNewTex_2(RGB)", 2D) = "white" { }
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_SourceNewTex_3("_SourceNewTex_3(RGB)", 2D) = "white" { }
TwistUV_TwistUV_Bend_1("TwistUV_TwistUV_Bend_1", Range(-1, 1)) = 0
TwistUV_TwistUV_PosX_1("TwistUV_TwistUV_PosX_1", Range(-1, 2)) = 0.5
TwistUV_TwistUV_PosY_1("TwistUV_TwistUV_PosY_1", Range(-1, 2)) = 0.5
TwistUV_TwistUV_Radius_1("TwistUV_TwistUV_Radius_1", Range(0, 1)) = 0.5
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_FillColor_Color_1("_FillColor_Color_1", COLOR) = (0.05882353,0.2210956,1,1)
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
sampler2D _NewTex_4;
sampler2D _SourceNewTex_2;
sampler2D _NewTex_1;
sampler2D _SourceNewTex_3;
float TwistUV_TwistUV_Bend_1;
float TwistUV_TwistUV_PosX_1;
float TwistUV_TwistUV_PosY_1;
float TwistUV_TwistUV_Radius_1;
sampler2D _NewTex_3;
sampler2D _SourceNewTex_1;
sampler2D _NewTex_2;
float4 _FillColor_Color_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 AnimatedPingPongOffsetUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy, float speed)
{
float time = sin(_Time * 100* speed)  * 0.1;
speed *= time * 25;
uv += float2(offsetx, offsety)*speed;
uv = uv * float2(zoomx, zoomy);
return uv;
}
float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float4 UniColor(float4 txt, float4 color)
{
txt.rgb = lerp(txt.rgb,color.rgb,color.a);
return txt;
}
float2 TwistUV(float2 uv, float value, float posx, float posy, float radius)
{
float2 center = float2(posx, posy);
float2 tc = uv - center;
float dist = length(tc);
if (dist < radius)
{
float percent = (radius - dist) / radius;
float theta = percent * percent * 16.0 * sin(value);
float s = sin(theta);
float c = cos(theta);
tc = float2(dot(tc, float2(c, -s)), dot(tc, float2(s, c)));
}
tc += center;
return tc;
}
float2 ZoomUV(float2 uv, float zoom, float posx, float posy)
{
float2 center = float2(posx, posy);
uv -= center;
uv = uv * zoom;
uv += center;
return uv;
}
float4 DisplacementUV(float2 uv,sampler2D source,float x, float y, float value)
{
return tex2D(source,lerp(uv,uv+float2(x,y),value));
}
float4 OperationBlend(float4 origin, float4 overlay, float blend)
{
float4 o = origin; 
o.a = overlay.a + origin.a * (1 - overlay.a);
o.rgb = (overlay.rgb * overlay.a + origin.rgb * origin.a * (1 - overlay.a)) / (o.a+0.0000001);
o.a = saturate(o.a);
o = lerp(origin, o, blend);
return o;
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
float2 FlipUV_H(float2 uv)
{
uv.x = 1 - uv.x;
return uv;
}
float2 FlipUV_V(float2 uv)
{
uv.y = 1 - uv.y;
return uv;
}
float2 AnimatedMouvementUV(float2 uv, float offsetx, float offsety, float speed)
{
speed *=_Time*50;
uv += float2(offsetx, offsety)*speed;
uv = fmod(uv,1);
return uv;
}
float4 PatternMovement(float2 uv, sampler2D source, float4 rgba, float posx, float posy, float speed)
{
float t = _Time * 20 * speed;
uv = fmod(abs(uv+float2(posx*t, posy*t)),1);
float4 result = tex2D(source, uv);
result.a = result.a * rgba.a;
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
float4 frag (v2f i) : COLOR
{
float4 NewTex_4 = tex2D(_NewTex_4, i.texcoord);
float2 DistortionUV_2 = DistortionUV(FlipUV_V(i.texcoord),16.65335,17.80203,1,1,1);
float2 TwistUV_2 = TwistUV(DistortionUV_2,-1,0.5,0.7179495,0.5121561);
float2 ZoomUV_1 = ZoomUV(TwistUV_2,0.5239452,0.5,0.5);
float4 _PatternMovement_1 = PatternMovement(ZoomUV_1,_SourceNewTex_2,NewTex_4,0,0.589742,0.7433456);
NewTex_4 = lerp(NewTex_4,NewTex_4*NewTex_4.a + _PatternMovement_1*_PatternMovement_1.a,1.499897);
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float2 TwistUV_1 = TwistUV(i.texcoord,TwistUV_TwistUV_Bend_1,TwistUV_TwistUV_PosX_1,TwistUV_TwistUV_PosY_1,TwistUV_TwistUV_Radius_1);
float4 SourceRGBA_1 = tex2D(_SourceNewTex_3, TwistUV_1);
float4 NewTex_3 = tex2D(_NewTex_3, i.texcoord);
float4 _PatternMovementMask_1 = PatternMovementMask(i.texcoord,_SourceNewTex_3,SourceRGBA_1,NewTex_3,0.06666369,-0.612818,0.4051326);
_PatternMovementMask_1.rgb += 4;
float2 AnimatedPingPongOffsetUV_1 = AnimatedPingPongOffsetUV(TwistUV_1,-0.2563993,-0.8679954,1.069306,1,0.08585064);
float2 DistortionUV_1 = DistortionUV(AnimatedPingPongOffsetUV_1,4.346955,8.941807,0.2583196,0.4467809,0.54608);
float2 AnimatedMouvementUV_1 = AnimatedMouvementUV(DistortionUV_1,0.1564112,-0.3153843,0.3820519);
float4 NewTex_2 = tex2D(_NewTex_2,AnimatedMouvementUV_1);
float4 _Displacement_1 = DisplacementUV(DistortionUV_1,_SourceNewTex_1,0,NewTex_2.g*NewTex_2.a,-0.5384693);
float4 _Displacement_2 = DisplacementUV(FlipUV_H(DistortionUV_1),_SourceNewTex_1,NewTex_2.r*NewTex_2.a,NewTex_2.g*NewTex_2.a,-0.0205172);
float4 OperationBlendMask_1 = OperationBlendMask(_Displacement_2, _Displacement_1, NewTex_3, 1); 
float4 FillColor_1 = UniColor(OperationBlendMask_1,_FillColor_Color_1);
float4 _PremadeGradients_1 = Color_PreGradients(FillColor_1,float4(0.55,0.4,0.3,1),float4(0.5,0.51,0.35,1),float4(0.8,0.75,0.8,1),float4(0.075,0.33,0.67,1),0,0.3807623,1.025643);
float4 OperationBlend_2 = OperationBlend(_PatternMovementMask_1, _PremadeGradients_1, 1); 
float4 OperationBlend_3 = OperationBlend(OperationBlend_2, NewTex_1, 0.7006093); 
float4 OperationBlend_1 = OperationBlend(NewTex_4, OperationBlend_3, 1); 
float4 FinalResult = OperationBlend_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
