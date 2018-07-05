//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.0.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Energy Light"
{
Properties
{
[HideInInspector] _MainTex("Base (RGB)", 2D) = "white" {}
_Generate_Fire_PosX_2("_Generate_Fire_PosX_2", Range(-1, 2)) = 0.1115382
_Generate_Fire_PosY_2("_Generate_Fire_PosY_2", Range(-1, 2)) = 0
_Generate_Fire_Precision_2("_Generate_Fire_Precision_2", Range(0, 1)) = 0.05
_Generate_Fire_Smooth_2("_Generate_Fire_Smooth_2", Range(0, 1)) = 0.3499995
_Generate_Fire_Speed_2("_Generate_Fire_Speed_2", Range(-2, 2)) = 1
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

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent"}
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

GrabPass { "_GrabTexture"  } 

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
# include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float2 screenuv : TEXCOORD1;
float4 color    : COLOR;
};

sampler2D _GrabTexture;
sampler2D _MainTex;
float _SpriteFade;
float _Generate_Fire_PosX_2;
float _Generate_Fire_PosY_2;
float _Generate_Fire_Precision_2;
float _Generate_Fire_Smooth_2;
float _Generate_Fire_Speed_2;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
float4 screenpos = ComputeGrabScreenPos(OUT.vertex);
OUT.screenuv = screenpos.xy / screenpos.w;
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 AnimatedTwistUV(float2 uv, float value, float posx, float posy, float radius, float speed)
{
float2 center = float2(posx, posy);
float2 tc = uv - center;
float dist = length(tc);
if (dist < radius)
{
float percent = (radius - dist) / radius;
float theta = percent * percent * 16.0 * sin(value);
float s = sin(theta + _Time.y * speed);
float c = cos(theta + _Time.y * speed);
tc = float2(dot(tc, float2(c, -s)), dot(tc, float2(s, c)));
}
tc += center;
return tc;
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
float4 DisplacementPlusUV(float2 uv,sampler2D source,float4 rgba ,float4 rgba2,float x, float y, float value)
{
float r=(rgba2.r+rgba2.g+rgba2.b)/3;
r*=rgba2.a;
return tex2D(source,lerp(uv,uv+float2(rgba.r*x,rgba.g*y),value*r));
}
float4 frag (v2f i) : COLOR
{
float2 AnimatedTwistUV_1 = AnimatedTwistUV(i.texcoord,0.4230769,0.5,0.5,0.698718,-2.435898);
float4 _Generate_Fire_1 = Generate_Fire(AnimatedTwistUV_1,0,-0.2903815,0.03910104,0.5653816,0.9846184,0);
float4 _CircleFade_1 = Circle_Fade(_Generate_Fire_1,i.texcoord,0.5,0.5,0.3461539,0.2);
float4 _Displacement_Plus_1 = DisplacementPlusUV(i.screenuv,_GrabTexture,_CircleFade_1,_CircleFade_1,0.2692308,0.1410256,0.1615382);
float2 AnimatedTwistUV_2 = AnimatedTwistUV(i.texcoord,0.6307698,0.5,0.5,1,4.897429);
float4 _Generate_Fire_2 = Generate_Fire(AnimatedTwistUV_2,_Generate_Fire_PosX_2,_Generate_Fire_PosY_2,_Generate_Fire_Precision_2,_Generate_Fire_Smooth_2,_Generate_Fire_Speed_2,0);
float4 _CircleFade_2 = Circle_Fade(_Generate_Fire_2,i.texcoord,0.5,0.5,0.2230769,0.2);
float4 _PremadeGradients_1 = Color_PreGradients(_CircleFade_2,float4(0.55,0.55,0.55,1),float4(0.8,0.8,0.8,1),float4(0.29,0.29,0.29,1),float4(0.54,0.59,0.6900001,1),0,1,0);
_Displacement_Plus_1 = lerp(_Displacement_Plus_1,_Displacement_Plus_1 + _PremadeGradients_1,1 * _PremadeGradients_1.a);
float4 FinalResult = _Displacement_Plus_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
