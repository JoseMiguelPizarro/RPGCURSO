//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/orb"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
DisplacementPack_2("DisplacementPack_2(RGB)", 2D) = "white" { }
DisplacementPack_1("DisplacementPack_1(RGB)", 2D) = "white" { }
OffsetUV_X_1("OffsetUV_X_1", Range(-1, 1)) = -0.0820516
OffsetUV_Y_1("OffsetUV_Y_1", Range(-1, 1)) = -0.173077
OffsetUV_ZoomX_1("OffsetUV_ZoomX_1", Range(0.1, 10)) = 1
OffsetUV_ZoomY_1("OffsetUV_ZoomY_1", Range(0.1, 10)) = 1
_NewTex_5("NewTex_5(RGB)", 2D) = "white" { }
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
_NewTex_4("NewTex_4(RGB)", 2D) = "white" { }
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
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
sampler2D DisplacementPack_2;
sampler2D DisplacementPack_1;
float OffsetUV_X_1;
float OffsetUV_Y_1;
float OffsetUV_ZoomX_1;
float OffsetUV_ZoomY_1;
sampler2D _NewTex_5;
sampler2D _NewTex_3;
sampler2D _NewTex_4;
sampler2D _NewTex_2;
sampler2D _NewTex_1;

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
float2 OffsetUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(uv * float2(zoomx, zoomy), 1);
return uv;
}

float2 OffsetUVClamp(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(clamp(uv * float2(zoomx, zoomy), 0.0001, 0.9999), 1);
return uv;
}
float2 ZoomUV(float2 uv, float zoom, float posx, float posy)
{
float2 center = float2(posx, posy);
uv -= center;
uv = uv * zoom;
uv += center;
return uv;
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
float4 HdrCreate(float4 txt,float value)
{
if (txt.r>0.98) txt.r=2;
if (txt.g>0.98) txt.g=2;
if (txt.b>0.98) txt.b=2;
return lerp(saturate(txt),txt, value);
}
float4 ColorFilters(float4 rgba, float4 red, float4 green, float4 blue, float fade)
{
float3 c_r = float3(red.r, red.g, red.b);
float3 c_g = float3(green.r, green.g, green.b);
float3 c_b = float3(blue.r, blue.g, blue.b);
float4 r = float4(dot(rgba.rgb, c_r) + red.a, dot(rgba.rgb, c_g) + green.a, dot(rgba.rgb, c_b) + blue.a, rgba.a);
return lerp(rgba, saturate(r), fade);

}
float4 DisplacementPack(float2 uv,sampler2D source,float x, float y, float value, float motion, float motion2)
{
float t=_Time.y;
float2 mov =float2(x*t,y*t)*motion;
float2 mov2 =float2(x*t*2,y*t*2)*motion2;
float4 rgba=tex2D(source, uv + mov);
float4 rgba2=tex2D(source, uv + mov2);
float r=(rgba2.r+rgba2.g+rgba2.b)/3;
r*=rgba2.a;
uv+=mov2*0.25;
return tex2D(source,lerp(uv,uv+float2(rgba.r*x,rgba.g*y),value*r));
}
float2 SimpleDisplacementUV(float2 uv,float x, float y, float value)
{
return lerp(uv,uv+float2(x,y),value);
}
float4 frag (v2f i) : COLOR
{
float2 ZoomUV_2 = ZoomUV(i.texcoord,0.2818581,0.5,0.5);
float4 _DisplacementPack_2 = DisplacementPack(ZoomUV_2,DisplacementPack_2,-0.001561429,-0.2021967,-1.217884,1,1);
float2 _Simple_Displacement_1 = SimpleDisplacementUV(i.texcoord,_DisplacementPack_2.r*_DisplacementPack_2.a,0,-0.1040089);
float2 DistortionUV_1 = DistortionUV(_Simple_Displacement_1,10,7.991311,0.2149961,0.07140644,0.3538568);
float2 DistortionUV_2 = DistortionUV(DistortionUV_1,10,20.10254,0.1391024,0.1301283,1);
float2 ZoomUV_1 = ZoomUV(DistortionUV_2,2.42604,0.5,0.5);
float2 FishEyeUV_1 = FishEyeUV(ZoomUV_1,0.3734018);
float4 _DisplacementPack_1 = DisplacementPack(FishEyeUV_1,DisplacementPack_1,-0.2890022,0.2186863,1.209905,1,1);
float4 _ColorFilters_1 = ColorFilters(_DisplacementPack_1,float4(0.5,1.41,-0.81,0.07),float4(-0.17,0.62,0.29,-0.34),float4(-1.59,-1.37,-2,-0.06),1);
float2 OffsetUV_1 = OffsetUVClamp(FishEyeUV_1,OffsetUV_X_1,OffsetUV_Y_1,OffsetUV_ZoomX_1,OffsetUV_ZoomY_1);
float4 NewTex_5 = tex2D(_NewTex_5,OffsetUV_1);
NewTex_5.r += 0.2538272;
_ColorFilters_1 = lerp(_ColorFilters_1,_ColorFilters_1*_ColorFilters_1.a + NewTex_5*NewTex_5.a,1.3128);
float4 NewTex_3 = tex2D(_NewTex_3, i.texcoord);
float4 NewTex_4 = tex2D(_NewTex_4,OffsetUV_1);
NewTex_3 = lerp(NewTex_3,NewTex_3 * NewTex_4,1);
_ColorFilters_1.a = lerp(NewTex_3.r, 1 - NewTex_3.r ,0);
float4 NewTex_2 = tex2D(_NewTex_2, i.texcoord);
float4 OperationBlend_2 = OperationBlend(_ColorFilters_1, NewTex_2, 1); 
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 OperationBlend_1 = OperationBlend(OperationBlend_2, NewTex_1, 1); 
float4 HdrCreate_1 = HdrCreate(OperationBlend_1,0.256379);
float4 FinalResult = HdrCreate_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
