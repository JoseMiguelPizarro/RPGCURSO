//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Crystal"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
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

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "True" }
		ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

		// required for UI.Mask
		Stencil
	{
		Ref[_Stencil]
		Comp[_StencilComp]
		Pass[_StencilOp]
		ReadMask[_StencilReadMask]
		WriteMask[_StencilWriteMask]
	}

		Pass
	{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

		struct appdata_t {
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

	v2f vert(appdata_t IN)
	{
		v2f OUT;
		OUT.vertex = UnityObjectToClipPos(IN.vertex);
		OUT.texcoord = IN.texcoord;
		OUT.color = IN.color;
		return OUT;
	}


	float2 AnimatedOffsetUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy, float speed)
	{
		speed *= _Time * 25;
		uv += float2(offsetx*speed, offsety*speed);
		uv = fmod(uv * float2(zoomx, zoomy), 1);
		return uv;
	}
	float4 UniColor(float4 txt, float4 color)
	{
		txt.rgb = lerp(txt.rgb,color.rgb,color.a);
		return txt;
	}
	float4 Generate_Checker(float2 uv, float posX, float posY, float Size, float black)
	{
		uv += float2(posX, posY);
		uv = floor(Size * uv);
		float r = fmod(uv.x + uv.y, 2.);
		float4 result = float4(r, r, r, r);
		result.a = saturate(result.a + black);
		return result;
	}
	float4 Color_PreGradients(float4 rgba, float4 a, float4 b, float4 c, float4 d, float offset, float fade, float speed)
	{
		float gray = (rgba.r + rgba.g + rgba.b) / 3;
		gray += offset + (speed*_Time * 20);
		float4 result = a + b * cos(6.28318 * (c * gray + d));
		result.a = rgba.a;
		result.rgb = lerp(rgba.rgb, result.rgb, fade);
		return result;
	}
	float4 frag(v2f i) : COLOR
	{
		float2 AnimatedOffsetUV_1 = AnimatedOffsetUV(i.texcoord,-0.3615331,-0.4230714,10,10,-0.03461593);
		float4 _Generate_Checker_1 = Generate_Checker(AnimatedOffsetUV_1,0,0,8,1);
		float4 FillColor_1 = UniColor(_Generate_Checker_1, float4(0.25,0.216,0.247,0.922));
		float4 _PremadeGradients_1 = Color_PreGradients(FillColor_1,float4(0.5,0.5,0.5,1),float4(0.5,0.5,0.5,1),float4(1,1,1,1),float4(0,0.33,0.67,1),0,0.1814106,0.1307702);
		float4 FinalResult = _PremadeGradients_1;
		FinalResult.rgb *= i.color.rgb;
		FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
		return FinalResult;
	}

		ENDCG
	}
	}
		Fallback "Sprites/Default"
}
