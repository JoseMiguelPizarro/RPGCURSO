//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Shadero_Slim"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_SpriteSheetUVAnimPack_Size_1("_SpriteSheetUVAnimPack_Size_1", Range(2, 16)) = 4
_SpriteSheetUVAnimPack_Frame1_1("_SpriteSheetUVAnimPack_Frame1_1", Range(0, 15)) = 0
_SpriteSheetUVAnimPack_Frame2_1("_SpriteSheetUVAnimPack_Frame2_1", Range(0, 15)) = 0
_SpriteSheetUVAnimPack_Blend_1("_SpriteSheetUVAnimPack_Blend_1", Range(0, 1)) = 0.801642
SpriteSheetUVAnimPack_1("SpriteSheetUVAnimPack_1(RGB)", 2D) = "white" { }
_NewTex_5("NewTex_5(RGB)", 2D) = "white" { }
_NewTex_4("NewTex_4(RGB)", 2D) = "white" { }
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_AutomaticLerp_Speed_1("_AutomaticLerp_Speed_1", Range(0, 1)) = 0.4051357
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_ColorHSV_Hue_1("_ColorHSV_Hue_1", Range(0, 360)) = 180
_ColorHSV_Saturation_1("_ColorHSV_Saturation_1", Range(0, 2)) = 1
_ColorHSV_Brightness_1("_ColorHSV_Brightness_1", Range(0, 2)) = 1
_NewTex_3("NewTex_3(RGB)", 2D) = "white" { }
_FourGradients_Color1_1("_FourGradients_Color1_1", COLOR) = (1,0,0,0)
_FourGradients_Color2_1("_FourGradients_Color2_1", COLOR) = (0,1,0,0)
_FourGradients_Color3_1("_FourGradients_Color3_1", COLOR) = (1,0.616,0,0.172)
_FourGradients_Color4_1("_FourGradients_Color4_1", COLOR) = (1,0.4852941,0,0.316)
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
float _SpriteSheetUVAnimPack_Size_1;
float _SpriteSheetUVAnimPack_Frame1_1;
float _SpriteSheetUVAnimPack_Frame2_1;
float _SpriteSheetUVAnimPack_Blend_1;
sampler2D SpriteSheetUVAnimPack_1;
sampler2D _NewTex_5;
sampler2D _NewTex_4;
sampler2D _NewTex_1;
float _AutomaticLerp_Speed_1;
sampler2D _NewTex_2;
float _ColorHSV_Hue_1;
float _ColorHSV_Saturation_1;
float _ColorHSV_Brightness_1;
sampler2D _NewTex_3;
float4 _FourGradients_Color1_1;
float4 _FourGradients_Color2_1;
float4 _FourGradients_Color3_1;
float4 _FourGradients_Color4_1;

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
float2 RotationUV(float2 uv, float rot, float posx, float posy, float speed)
{
rot=rot+(_Time*speed*360);
uv = uv - float2(posx, posy);
float angle = rot * 0.01744444;
float sinX = sin(angle);
float cosX = cos(angle);
float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
uv = mul(uv, rotationMatrix) + float2(posx, posy);
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
float4 Four_Gradients(float4 txt, float2 uv, float4 col1, float4 col2, float4 col3, float4 col4)
{
float4 colorA = lerp(col3,col4, uv.x*1.1);
float4 colorB = lerp(col1,col2, uv.x*1.1);
float4 colorC = lerp(colorA,colorB, uv.y*1.1);
colorA = lerp(txt, colorC, colorC.a);
colorA.a = txt.a;
return colorA;
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
float2 AnimatedMouvementUV(float2 uv, float offsetx, float offsety, float speed)
{
speed *=_Time*50;
uv += float2(offsetx, offsety)*speed;
uv = fmod(uv,1);
return uv;
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
float2 SpriteSheetAnimationUV(float2 uv, float size, float speed)
{
uv /= size;
uv.x += floor(fmod(_Time * speed, 1.0) * size) / size;
uv.y -= 1/size;
uv.y += (1 - floor(fmod(_Time * speed / size, 1.0) * size) / size);
return uv;
}
float2 FixSidesUV(float2 uv, float2 uv2)
{
float smooth = 0.08f;
float r = 1 - smoothstep(0.0, smooth, uv2.x);
r += smoothstep(1.- smooth, 1., uv2.x);
r += 1 - smoothstep(0.0, smooth, uv2.y);
r += smoothstep(1 - smooth, 1., uv2.y);
r = saturate(r);
uv = lerp(uv, uv2, r);
return uv;
}
float2 SimpleDisplacementUV(float2 uv,float x, float y, float value)
{
return lerp(uv,uv+float2(x,y),value);
}
float2 SpriteSheetUVAnimPack(float2 uv,sampler2D source,float size, float frame1, float frame2, float blend)
{
frame1 = int(frame1);
frame2 = int(frame2);
uv /= size;
uv.y -=1/size;
float2 uv2=uv;
uv.x += fmod(frame1,size) / size;
uv.y += 1-floor(frame1 / size) / size;
uv2.x += fmod(frame2,size) / size;
uv2.y += 1-floor(frame2 / size) / size;
uv = tex2D(source, uv).rg;
uv2 = tex2D(source, uv2).rg;
uv = lerp(uv,uv2,blend);
return uv;
}
float4 frag (v2f i) : COLOR
{
float2 DistortionUV_2 = DistortionUV(i.texcoord,17.6406,20.59443,0.2384597,0.1038443,0.261531);
float2 _SpriteSheetUVAnimPack_1 = SpriteSheetUVAnimPack(DistortionUV_2,SpriteSheetUVAnimPack_1,_SpriteSheetUVAnimPack_Size_1,_SpriteSheetUVAnimPack_Frame1_1,_SpriteSheetUVAnimPack_Frame2_1,_SpriteSheetUVAnimPack_Blend_1);
_SpriteSheetUVAnimPack_1 = FixSidesUV(_SpriteSheetUVAnimPack_1, i.texcoord);
float2 ZoomUV_1 = ZoomUV(i.texcoord,0.4668003,0.5,0.5);
float2 DistortionUV_1 = DistortionUV(ZoomUV_1,24.61619,45.94952,0.3333396,0.4230832,0.4077101);
float2 ResizeUV_1 = ResizeUV(DistortionUV_1,0,0.194872,1,0.6725633);
float2 AnimatedMouvementUV_1 = AnimatedMouvementUV(ResizeUV_1,0,0.24102,0.03845606);
float4 NewTex_5 = tex2D(_NewTex_5,AnimatedMouvementUV_1);
float2 _Simple_Displacement_1 = SimpleDisplacementUV(_SpriteSheetUVAnimPack_1,0,NewTex_5.g*NewTex_5.a,0.07076474);
float2 FixUV_1 = FixSidesUV(_Simple_Displacement_1, i.texcoord);
float4 NewTex_4 = tex2D(_NewTex_4,FixUV_1);
_SpriteSheetUVAnimPack_1 = FixSidesUV(_SpriteSheetUVAnimPack_1, i.texcoord);
float4 NewTex_1 = tex2D(_NewTex_1,FixUV_1);
float _AutomaticLerp_Fade_1 = (1+cos(_Time.y *4*_AutomaticLerp_Speed_1))/2;
NewTex_4 = lerp(NewTex_4,NewTex_1, _AutomaticLerp_Fade_1);
float2 RotationUV_1 = RotationUV(i.texcoord,0,0.5,0.5,-0.3795072);
float2 SpriteSheetAnimationUV_1 = SpriteSheetAnimationUV(RotationUV_1,8,32);
float4 NewTex_2 = tex2D(_NewTex_2,SpriteSheetAnimationUV_1);
NewTex_4 = lerp(NewTex_4,NewTex_4*NewTex_4.a + NewTex_2*NewTex_2.a,0.7179503 * NewTex_4.a);
float4 _ColorHSV_1 = ColorHSV(NewTex_4,_ColorHSV_Hue_1,_ColorHSV_Saturation_1,_ColorHSV_Brightness_1);
_SpriteSheetUVAnimPack_1 = FixSidesUV(_SpriteSheetUVAnimPack_1, i.texcoord);
float4 NewTex_3 = tex2D(_NewTex_3,_SpriteSheetUVAnimPack_1);
float4 OperationBlend_1 = OperationBlend(_ColorHSV_1, NewTex_3, 1); 
float4 _FourGradients_1 = Four_Gradients(OperationBlend_1,i.texcoord,_FourGradients_Color1_1,_FourGradients_Color2_1,_FourGradients_Color3_1,_FourGradients_Color4_1);
float4 FinalResult = _FourGradients_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
