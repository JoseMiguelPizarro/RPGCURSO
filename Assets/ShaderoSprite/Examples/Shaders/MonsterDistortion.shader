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
TiltUpUV_TiltUpUV_Value_1("TiltUpUV_TiltUpUV_Value_1", Range(-2, 2)) = 0.0410258
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_Displacement_Plus_ValueX_1("_Displacement_Plus_ValueX_1", Range(-1, 1)) = 0
_Displacement_Plus_ValueY_1("_Displacement_Plus_ValueY_1", Range(-1, 1)) = -0.03076876
_Displacement_Plus_Size_1("_Displacement_Plus_Size_1", Range(-3, 3)) = 1
_RGBA_Add_Fade_1("_RGBA_Add_Fade_1", Range(0, 4)) = 0.4358959
_Burn_Value_1("_Burn_Value_1", Range(0, 1)) = 0
_Burn_Speed_1("_Burn_Speed_1", Range(-8, 8)) = -2.946151
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
float TiltUpUV_TiltUpUV_Value_1;
sampler2D _SourceNewTex_1;
sampler2D _NewTex_1;
float _Displacement_Plus_ValueX_1;
float _Displacement_Plus_ValueY_1;
float _Displacement_Plus_Size_1;
float _RGBA_Add_Fade_1;
float _Burn_Value_1;
float _Burn_Speed_1;

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
uv = fmod(uv * float2(zoomx, zoomy), 1);
return uv;
}
float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float2 ZoomUV(float2 uv, float zoom, float posx, float posy)
{
float2 center = float2(posx, posy);
uv -= center;
uv = uv * zoom;
uv += center;
return uv;
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
float4 Circle_Fade(float4 txt, float2 uv, float posX, float posY, float Size, float Smooth)
{
float2 center = float2(posX, posY);
float dist = 1.0 - smoothstep(Size, Size + Smooth, length(center - uv));
txt.a *= dist;
return txt;
}
float2 TiltUpUV(float2 uv, float offsetX)
{
uv += float2(offsetX*uv.y, 0);
return uv;
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
float4 DisplacementPlusUV(float2 uv,sampler2D source,float4 rgba ,float4 rgba2,float x, float y, float value)
{
float r=(rgba2.r+rgba2.g+rgba2.b)/3;
r*=rgba2.a;
return tex2D(source,lerp(uv,uv+float2(rgba.r*x,rgba.g*y),value*r));
}
float2 FlipUV_V(float2 uv)
{
uv.y = 1 - uv.y;
return uv;
}
float4 frag (v2f i) : COLOR
{
float2 TiltUpUV_1 = TiltUpUV(i.texcoord,TiltUpUV_TiltUpUV_Value_1);
float2 AnimatedPingPongOffsetUV_1 = AnimatedPingPongOffsetUV(TiltUpUV_1,0,0.1602559,1,1,0.1538459);
float2 ZoomUV_1 = ZoomUV(AnimatedPingPongOffsetUV_1,1.369138,0.5,0.2076871);
float2 DistortionUV_1 = DistortionUV(ZoomUV_1,9.846179,28.55381,0.846154,0.846154,1);
float4 SourceRGBA_1 = tex2D(_SourceNewTex_1, DistortionUV_1);
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 _Generate_Fire_1 = Generate_Fire(FlipUV_V(i.texcoord),0,0.4634622,0.05833326,0.1634611,-1.017947,0);
NewTex_1 = lerp(NewTex_1,NewTex_1*NewTex_1.a + _Generate_Fire_1*_Generate_Fire_1.a,1.74872);
float4 _Displacement_Plus_1 = DisplacementPlusUV(ZoomUV_1,_SourceNewTex_1,SourceRGBA_1,NewTex_1,_Displacement_Plus_ValueX_1,_Displacement_Plus_ValueY_1,_Displacement_Plus_Size_1);
float4 _CircleFade_1 = Circle_Fade(_Displacement_Plus_1,i.texcoord,0.5,0.476924,0.3000011,0.1884617);
_CircleFade_1.b += _RGBA_Add_Fade_1;
float4 _Burn_1 = BurnFX(_CircleFade_1,i.texcoord,_Burn_Value_1,_Burn_Speed_1,0.6564104);
float4 FinalResult = _Burn_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
