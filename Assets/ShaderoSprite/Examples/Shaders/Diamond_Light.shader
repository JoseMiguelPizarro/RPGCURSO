//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.0                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/Diamond_Light"
{
	Properties
	{
		[HideInInspector] _MainTex("Base (RGB)", 2D) = "white" {}
	_Generate_Fire_PosX_1("_Generate_Fire_PosX_1", Range(-1, 2)) = 0
		_Generate_Fire_PosY_1("_Generate_Fire_PosY_1", Range(-1, 2)) = -0.223077
		_Generate_Fire_Precision_1("_Generate_Fire_Precision_1", Range(0, 1)) = 0.1397436
		_Generate_Fire_Smooth_1("_Generate_Fire_Smooth_1", Range(0, 1)) = 0.4089745
		_Generate_Fire_Speed_1("_Generate_Fire_Speed_1", Range(-2, 2)) = 1.323078
		_CircleFade_PosX_1("_CircleFade_PosX_1", Range(-1, 2)) = 0.5
		_CircleFade_PosY_1("_CircleFade_PosY_1", Range(-1, 2)) = 0.8692334
		_CircleFade_Size_1("_CircleFade_Size_1", Range(-1, 1)) = 0.4256427
		_CircleFade_Dist_1("_CircleFade_Dist_1", Range(0, 1)) = 0.2
		_TurnAlphaToBlack_Fade_1("_TurnAlphaToBlack_Fade_1", Range(0, 1)) = 1
		_CircleFade_PosX_2("_CircleFade_PosX_2", Range(-1, 2)) = 0.5
		_CircleFade_PosY_2("_CircleFade_PosY_2", Range(-1, 2)) = 0.4942322
		_CircleFade_Size_2("_CircleFade_Size_2", Range(-1, 1)) = 0.325641
		_CircleFade_Dist_2("_CircleFade_Dist_2", Range(0, 1)) = 0.2
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

		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" }
		ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off

		GrabPass{ "_GrabTexture" }

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
# include "UnityCG.cginc"

		struct appdata_t {
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
	float _Generate_Fire_PosX_1;
	float _Generate_Fire_PosY_1;
	float _Generate_Fire_Precision_1;
	float _Generate_Fire_Smooth_1;
	float _Generate_Fire_Speed_1;
	float _CircleFade_PosX_1;
	float _CircleFade_PosY_1;
	float _CircleFade_Size_1;
	float _CircleFade_Dist_1;
	float _TurnAlphaToBlack_Fade_1;
	float _CircleFade_PosX_2;
	float _CircleFade_PosY_2;
	float _CircleFade_Size_2;
	float _CircleFade_Dist_2;

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


	float4 Circle_Fade(float4 txt, float2 uv, float posX, float posY, float Size, float Smooth)
	{
		float2 center = float2(posX, posY);
		float dist = 1.0 - smoothstep(Size, Size + Smooth, length(center - uv));
		txt.a *= dist;
		return txt;
	}
	float4 DisplacementUV(float2 uv,sampler2D source,float x, float y, float value)
	{
		return tex2D(source,lerp(uv,uv + float2(x,y),value));
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
		float t = _Time * 60 * speed;
		float up0 = Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(0, -t), precision);
		float up1 = 0.5 + Generate_Fire_voronoi2D(uv * float2(6.0, 4.0) + float2(42, -t) + 30.0, precision);
		float finalMask = up0 * up1 + (1.0 - uv.y);
		finalMask += (1.0 - uv.y)* 0.5;
		finalMask *= 0.7 - abs(uv.x - 0.5);
		float4 result = smoothstep(smooth, 0.95, finalMask);
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

	float4 TurnAlphaToBlack(float4 txt,float fade)
	{
		float3 gs = lerp(txt.rgb,float3(0,0,0), 1 - txt.a);
		return lerp(txt,float4(gs, 1), fade);
	}

	float4 frag(v2f i) : COLOR
	{
		float4 _Generate_Fire_1 = Generate_Fire(i.texcoord,_Generate_Fire_PosX_1,_Generate_Fire_PosY_1,_Generate_Fire_Precision_1,_Generate_Fire_Smooth_1,_Generate_Fire_Speed_1,0);
		float4 _CircleFade_1 = Circle_Fade(_Generate_Fire_1,i.texcoord,_CircleFade_PosX_1,_CircleFade_PosY_1,_CircleFade_Size_1,_CircleFade_Dist_1);
		float4 TurnAlphaToBlack_1 = TurnAlphaToBlack(_CircleFade_1,_TurnAlphaToBlack_Fade_1);
		float4 _Displacement_1 = DisplacementUV(i.screenuv,_GrabTexture,0,TurnAlphaToBlack_1.g,0.04923106);
		float4 _PremadeGradients_1 = Color_PreGradients(_Generate_Fire_1,float4(0.5,0.5,0.5,1),float4(0.5,0.5,0.5,1),float4(0.8,0.8,0.8,1),float4(0,0.33,0.67,1),-0.09871732,1,0.4538465);
		_Displacement_1 = lerp(_Displacement_1,_Displacement_1 + _PremadeGradients_1,0.5205141 * _PremadeGradients_1.a);
		float4 _CircleFade_2 = Circle_Fade(_Displacement_1,i.texcoord,_CircleFade_PosX_2,_CircleFade_PosY_2,_CircleFade_Size_2,_CircleFade_Dist_2);
		float4 FinalResult = _CircleFade_2;
		FinalResult.rgb *= i.color.rgb;
		FinalResult.a = FinalResult.a * _SpriteFade;
		return FinalResult;
	}

		ENDCG
	}
	}
		Fallback "Sprites/Default"
}
