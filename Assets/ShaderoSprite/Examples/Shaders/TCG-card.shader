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
ZoomUV_Zoom_1("ZoomUV_Zoom_1", Range(0.2, 4)) = 1.026392
ZoomUV_PosX_1("ZoomUV_PosX_1", Range(-3, 3)) = 1.346154
ZoomUV_PosY_1("ZoomUV_PosY_1", Range(-3, 3)) =-0.03846156
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
_ColorHSV_Hue_1("_ColorHSV_Hue_1", Range(0, 360)) = 180
_ColorHSV_Saturation_1("_ColorHSV_Saturation_1", Range(0, 2)) = 1
_ColorHSV_Brightness_1("_ColorHSV_Brightness_1", Range(0, 2)) = 1
_NewTex_4("NewTex_4(RGB)", 2D) = "white" { }
_SourceNewTex_2("_SourceNewTex_2(RGB)", 2D) = "white" { }
_SourceNewTex_3("_SourceNewTex_3(RGB)", 2D) = "white" { }
_RGBA_Mul_Fade_1("_RGBA_Mul_Fade_1", Range(0, 2)) = 1.011534
_PatternMovement_PosX_2("_PatternMovement_PosX_2", Range(-2, 2)) = 0
_PatternMovement_PosY_2("_PatternMovement_PosY_2", Range(-2, 2)) = -0.09230707
_PatternMovement_Speed_2("_PatternMovement_Speed_2", Range(1, 16)) = 0.9794906
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_RenderTex_1("RenderTex_1(RGB)", 2D) = "white" { }
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
_SourceNewTex_4("_SourceNewTex_4(RGB)", 2D) = "white" { }
SpriteSheetFrameUV_Size_1("SpriteSheetFrameUV_Size_1", Range(2, 16)) = 4
SpriteSheetFrameUV_Frame_1("SpriteSheetFrameUV_Frame_1", Range(0, 1)) = 10
SpriteSheetFrameUV_Size_2("SpriteSheetFrameUV_Size_2", Range(2, 16)) = 4
SpriteSheetFrameUV_Frame_2("SpriteSheetFrameUV_Frame_2", Range(0, 1)) = 8
_Burn_Value_1("_Burn_Value_1", Range(0, 1)) = 0
_Burn_Speed_1("_Burn_Speed_1", Range(-8, 8)) = -3.394876
_Destroyer_Value_1("_Destroyer_Value_1", Range(0, 1)) = 0
_Destroyer_Speed_1("_Destroyer_Speed_1", Range(0, 1)) =  0.5
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
ZWrite Off Blend One OneMinusSrcAlpha Cull Off

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
sampler2D _SourceNewTex_1;
float _ColorHSV_Hue_1;
float _ColorHSV_Saturation_1;
float _ColorHSV_Brightness_1;
sampler2D _NewTex_4;
sampler2D _SourceNewTex_2;
sampler2D _SourceNewTex_3;
float _RGBA_Mul_Fade_1;
float _PatternMovement_PosX_2;
float _PatternMovement_PosY_2;
float _PatternMovement_Speed_2;
sampler2D _NewTex_2;
sampler2D _NewTex_1;
sampler2D _RenderTex_1;
sampler2D _NewTex_3;
sampler2D _SourceNewTex_4;
float SpriteSheetFrameUV_Size_1;
float SpriteSheetFrameUV_Frame_1;
float SpriteSheetFrameUV_Size_2;
float SpriteSheetFrameUV_Frame_2;
float _Burn_Value_1;
float _Burn_Speed_1;
float _Destroyer_Value_1;
float _Destroyer_Speed_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
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
float DSFXr (float2 c, float seed)
{
return frac(43.*sin(c.x+7.*c.y)*seed);
}

float DSFXn (float2 p, float seed)
{
float2 i = floor(p), w = p-i, j = float2 (1.,0.);
w = w*w*(3.-w-w);
return lerp(lerp(DSFXr(i, seed), DSFXr(i+j, seed), w.x), lerp(DSFXr(i+j.yx, seed), DSFXr(i+1., seed), w.x), w.y);
}

float DSFXa (float2 p, float seed)
{
float m = 0., f = 2.;
for ( int i=0; i<9; i++ ){ m += DSFXn(f*p, seed)/f; f+=f; }
return m;
}

float4 DestroyerFX(float4 txt, float2 uv, float value, float seed, float HDR)
{
float t = frac(value*0.9999);
float4 c = smoothstep(t / 1.2, t + .1, DSFXa(3.5*uv, seed));
c = txt*c;
c.r = lerp(c.r, c.r*120.0*(1 - c.a), value);
c.g = lerp(c.g, c.g*40.0*(1 - c.a), value);
c.b = lerp(c.b, c.b*5.0*(1 - c.a) , value);
c.rgb = lerp(saturate(c.rgb),c.rgb,HDR);
return c;
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
float4 ColorHSV(float4 RGBA, float HueShift, float Sat, float Val)
{

float4 RESULT = float4(RGBA);
float a1 = Val*Sat;
float a2 = HueShift*3.14159265 / 180;
float VSU = a1*cos(a2);
float VSW = a1*sin(a2);

RESULT.x = (.299*Val + .701*VSU + .168*VSW)*RGBA.x
+ (.587*Val - .587*VSU + .330*VSW)*RGBA.y
+ (.114*Val - .114*VSU - .497*VSW)*RGBA.z;

RESULT.y = (.299*Val - .299*VSU - .328*VSW)*RGBA.x
+ (.587*Val + .413*VSU + .035*VSW)*RGBA.y
+ (.114*Val - .114*VSU + .292*VSW)*RGBA.z;

RESULT.z = (.299*Val - .3*VSU + 1.25*VSW)*RGBA.x
+ (.587*Val - .588*VSU - 1.05*VSW)*RGBA.y
+ (.114*Val + .886*VSU - .203*VSW)*RGBA.z;

return RESULT;
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
float4 PatternMovement(float2 uv, sampler2D source, float4 rgba, float posx, float posy, float speed)
{
float t = _Time * 20 * speed;
uv = fmod(abs(uv+float2(posx*t, posy*t)),1);
float4 result = tex2D(source, uv);
result.a = result.a * rgba.a;
return result;
}

float4 TurnAlphaToBlack(float4 txt,float fade)
{
float3 gs = lerp(txt.rgb,float3(0,0,0), 1-txt.a);
return lerp(txt,float4(gs, 1), fade);
}


float4 TurnBlackToAlpha(float4 txt, float force, float fade)
{
float3 gs = dot(txt.rgb, float3(1., 1., 1.));
gs=saturate(gs);
return lerp(txt,float4(force*txt.rgb, gs.r), fade);
}

float2 ResizeUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(uv * float2(zoomx*zoomx, zoomy*zoomy), 1);
return uv;
}

float2 ResizeUVClamp(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(clamp(uv * float2(zoomx*zoomx, zoomy*zoomy), 0.0001, 0.9999), 1);
return uv;
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
float2 OffsetUV_1 = OffsetUV(i.texcoord,-0.123077,-0.4230717,1.38077,1.837692);
float2 ZoomUV_1 = ZoomUV(OffsetUV_1,ZoomUV_Zoom_1,ZoomUV_PosX_1,ZoomUV_PosY_1);
float4 _Generate_Fire_1 = Generate_Fire(ZoomUV_1,0,0,0.0948718,0.1358962,1,1);
float4 _Displacement_Plus_1 = DisplacementPlusUV(ZoomUV_1,_SourceNewTex_1,_Generate_Fire_1,float4(1,1,1,1),-0.07692308,0.03846153,0.41795);
float4 _ColorHSV_1 = ColorHSV(_Displacement_Plus_1,_ColorHSV_Hue_1,_ColorHSV_Saturation_1,_ColorHSV_Brightness_1);
float4 NewTex_4 = tex2D(_NewTex_4,ZoomUV_1);
float4 Mask2RGBA_1 = lerp(_ColorHSV_1,_Generate_Fire_1, lerp(NewTex_4.r, 1 - NewTex_4.r ,0.01666142));
float4 SourceRGBA_3 = tex2D(_SourceNewTex_2, ZoomUV_1);
float4 SourceRGBA_1 = tex2D(_SourceNewTex_3, ZoomUV_1);
float4 _PatternMovement_1 = PatternMovement(i.texcoord,_SourceNewTex_3,SourceRGBA_1,0.07692778,-0.3999994,0.9743543);
float4 TurnAlphaToBlack_1 = TurnAlphaToBlack(_PatternMovement_1,1);
float4 _Displacement_Plus_2 = DisplacementPlusUV(ZoomUV_1,_SourceNewTex_2,TurnAlphaToBlack_1,NewTex_4,0,-0.8692244,0.3307745);
_Displacement_Plus_2.r *= _RGBA_Mul_Fade_1;
float4 OperationBlend_6 = OperationBlend(SourceRGBA_3, _Displacement_Plus_2, 1); 
float4 OperationBlend_1 = OperationBlend(Mask2RGBA_1, OperationBlend_6, 1); 
float4 _PatternMovement_2 = PatternMovement(i.texcoord,_SourceNewTex_3,SourceRGBA_1,_PatternMovement_PosX_2,_PatternMovement_PosY_2,_PatternMovement_Speed_2);
_PatternMovement_2.a = lerp(SourceRGBA_3.r * _PatternMovement_2.a, (1 - SourceRGBA_3.r) * _PatternMovement_2.a,0);
_PatternMovement_2.r += 1.756415;
OperationBlend_1 = lerp(OperationBlend_1,OperationBlend_1*OperationBlend_1.a + _PatternMovement_2*_PatternMovement_2.a,0.4461576 * _PatternMovement_2.a);
float4 NewTex_2 = tex2D(_NewTex_2, i.texcoord);
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 Mask2RGBA_2 = lerp(OperationBlend_1,NewTex_2, lerp(NewTex_1.r, 1 - NewTex_1.r ,1));
float2 ResizeUV_3 = ResizeUVClamp(i.texcoord,0.001279777,0.01538419,1,1.655969);
float4 RenderTex_1 = tex2D(_RenderTex_1,ResizeUV_3);
float4 TurnBlackToAlpha_1 = TurnBlackToAlpha(RenderTex_1,0,1);
float4 NewTex_3 = tex2D(_NewTex_3, i.texcoord);
float4 OperationBlend_2 = OperationBlend(TurnBlackToAlpha_1, NewTex_3, 1); 
float2 ResizeUV_1 = ResizeUVClamp(i.texcoord,-0.7487159,-0.07435843,2.851282,2.832692);
float2 SpriteSheetFrameUV_1 = SpriteSheetFrame(ResizeUV_1,SpriteSheetFrameUV_Size_1,SpriteSheetFrameUV_Frame_1);
float4 SourceRGBA_2 = tex2D(_SourceNewTex_4, SpriteSheetFrameUV_1);
float2 ResizeUV_2 = ResizeUVClamp(i.texcoord,-0.1435895,-0.07564525,2.853135,2.858974);
float2 SpriteSheetFrameUV_2 = SpriteSheetFrame(ResizeUV_2,SpriteSheetFrameUV_Size_2,SpriteSheetFrameUV_Frame_2);
float4 SourceRGBA_4 = tex2D(_SourceNewTex_4, SpriteSheetFrameUV_2);
float4 OperationBlend_4 = OperationBlend(SourceRGBA_2, SourceRGBA_4, 1); 
float4 OperationBlend_5 = OperationBlend(OperationBlend_2, OperationBlend_4, 1); 
float4 _Burn_1 = BurnFX(OperationBlend_5,i.texcoord,_Burn_Value_1,_Burn_Speed_1,2);
float4 OperationBlend_3 = OperationBlend(Mask2RGBA_2, _Burn_1, 1); 
float4 _Destroyer_1 = DestroyerFX(OperationBlend_3,i.texcoord,_Destroyer_Value_1,_Destroyer_Speed_1,0.9154201);
float4 FinalResult = _Destroyer_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
