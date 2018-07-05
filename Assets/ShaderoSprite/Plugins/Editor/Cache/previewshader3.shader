//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Previews/PreviewXATXQ3"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
ZoomUV_Zoom_1("ZoomUV_Zoom_1", Range(0.2, 4)) = 1.209607
ZoomUV_PosX_1("ZoomUV_PosX_1", Range(-3, 3)) = 0.383846
ZoomUV_PosY_1("ZoomUV_PosY_1", Range(-3, 3)) =0.6876925
_Generate_Circle_PosX_1("_Generate_Circle_PosX_1", Range(-1, 2)) = 0.5
_Generate_Circle_PosY_1("_Generate_Circle_PosY_1", Range(-1, 2)) = 0.5
_Generate_Circle_Size_1("_Generate_Circle_Size_1", Range(-1, 1)) = 0.4368732
_Generate_Circle_Dist_1("_Generate_Circle_Dist_1", Range(0, 1)) = 0.06607945
_CircleHole_PosX_1("_CircleHole_PosX_1", Range(-1, 2)) = 0.4892273
_CircleHole_PosY_1("_CircleHole_PosY_1", Range(-1, 2)) = 0.5
_CircleHole_Size_1("_CircleHole_Size_1", Range(0, 1)) = 0.4417803
_CircleHole_Dist_1("_CircleHole_Dist_1", Range(0, 1)) = 0.4249997
_Generate_Shape_PosX_1("_Generate_Shape_PosX_1", Range(-1, 2)) = 0.5
_Generate_Shape_PosY_1("_Generate_Shape_PosY_1", Range(-1, 2)) = 0.6846156
_Generate_Shape_Size_1("_Generate_Shape_Size_1", Range(-1, 1)) = 0.4
_Generate_Shape_Dist_1("_Generate_Shape_Dist_1", Range(0, 1)) = 0.07513247
_Generate_Shape_Side_1("_Generate_Shape_Side_1", Range(1, 32)) = 1
_Generate_Shape_Rotation_1("_Generate_Shape_Rotation_1", Range(-360, 360)) = 179
_Mul_Fade_1("_Mul_Fade_1", Range(0, 1)) = 1
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
float ZoomUV_Zoom_1;
float ZoomUV_PosX_1;
float ZoomUV_PosY_1;
float _Generate_Circle_PosX_1;
float _Generate_Circle_PosY_1;
float _Generate_Circle_Size_1;
float _Generate_Circle_Dist_1;
float _CircleHole_PosX_1;
float _CircleHole_PosY_1;
float _CircleHole_Size_1;
float _CircleHole_Dist_1;
float _Generate_Shape_PosX_1;
float _Generate_Shape_PosY_1;
float _Generate_Shape_Size_1;
float _Generate_Shape_Dist_1;
float _Generate_Shape_Side_1;
float _Generate_Shape_Rotation_1;
float _Mul_Fade_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float4 TintRGBA(float4 txt, float4 color)
{
float3 tint = dot(txt.rgb, float3(.222, .707, .071));
tint.rgb *= color.rgb;
txt.rgb = lerp(txt.rgb,tint.rgb,color.a);
return txt;
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
float4 Generate_Circle(float2 uv, float posX, float posY, float Size, float Smooth, float black)
{
float2 center = float2(posX, posY);
float dist = 1.0 - smoothstep(Size, Size + Smooth, length(center - uv));
float4 result = float4(1,1,1,dist);
if (black == 1) result = float4(dist, dist, dist, 1);
return result;
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
float4 OperationBlend(float4 origin, float4 overlay, float blend)
{
float4 o = origin; 
o.a = overlay.a + origin.a * (1 - overlay.a);
o.rgb = (overlay.rgb * overlay.a + origin.rgb * origin.a * (1 - overlay.a)) / (o.a+0.0000001);
o.a = saturate(o.a);
o = lerp(origin, o, blend);
return o;
}
float2 FishEyeUV(float2 uv, float size)
{
float2 m = float2(0.5, 0.5);
float2 d = uv - m;
float r = sqrt(dot(d, d));
float power = (2.0 * 3.141592 / (2.0 * sqrt(dot(m, m)))) * (size+0.001);
float bind = sqrt(dot(m, m));
uv = m + normalize(d) * tan(r * power) * bind / tan(bind * power);
return uv;
}
float4 PatternMovementPack(float2 uv, sampler2D source, sampler2D source2, float posx, float posy, float speed, float v1, float v2)
{
float t = _Time * 20 * speed;
float2 mov =float2(posx*t,posy*t);
float2 muv = fmod(uv+mov,1);
float2 muv2 = fmod(uv+mov*0.7,1);
float4 rgba=tex2D(source, muv);
float4 mask=tex2D(source2, muv2);
uv = fmod(abs(uv+float2(posx*t, posy*t)),1);
float4 result = tex2D(source, uv);
result.a = lerp(0,result.a,v1) * rgba.a * lerp(mask.a,mask.r,v2);
return result;
}
float2 SimpleDisplacementUV(float2 uv,float x, float y, float value)
{
return lerp(uv,uv+float2(x,y),value);
}
float4 AlphaAsAura(float4 origin, float4 overlay, float blend)
{
float4 o = origin;
o = o.a;
if (o.a > 0.99) o = 0;
float4 aura = lerp(origin, origin + overlay, blend);
o = lerp(origin, aura, o);
return o;
}
float4 ShinyFX(float4 txt, float2 uv, float pos, float size, float smooth, float intensity, float speed)
{
pos = pos + 0.5+sin(_Time*20*speed)*0.5;
uv = uv - float2(pos, 0.5);
float a = atan2(uv.x, uv.y) + 1.4, r = 3.1415;
float d = cos(floor(0.5 + a / r) * r - a) * length(uv);
float dist = 1.0 - smoothstep(size, size + smooth, d);
txt.rgb += dist*intensity;
return txt;
}
float4 Circle_Hole(float4 txt, float2 uv, float posX, float posY, float Size, float Smooth)
{
float2 center = float2(posX, posY);
float dist = 1.0 - smoothstep(Size, Size + Smooth, 1-length(center - uv));
txt.a *= dist;
return txt;
}
float4 frag (v2f i) : COLOR
{
float2 ZoomUV_1 = ZoomUV(i.texcoord,ZoomUV_Zoom_1,ZoomUV_PosX_1,ZoomUV_PosY_1);
float4 _Generate_Circle_1 = Generate_Circle(ZoomUV_1,_Generate_Circle_PosX_1,_Generate_Circle_PosY_1,_Generate_Circle_Size_1,_Generate_Circle_Dist_1,0);
float4 _CircleHole_1 = Circle_Hole(_Generate_Circle_1,ZoomUV_1,_CircleHole_PosX_1,_CircleHole_PosY_1,_CircleHole_Size_1,_CircleHole_Dist_1);
float4 _Generate_Shape_1 = Generate_Shape(i.texcoord,_Generate_Shape_PosX_1,_Generate_Shape_PosY_1,_Generate_Shape_Size_1,_Generate_Shape_Dist_1,_Generate_Shape_Side_1,0,_Generate_Shape_Rotation_1);
_CircleHole_1 = lerp(_CircleHole_1,_CircleHole_1 * _Generate_Shape_1,_Mul_Fade_1);
float4 FinalResult = _CircleHole_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
