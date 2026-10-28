Shader "MADFINGER/Environment/Scroll 2 Layers Sine AlphaBlended"
{
	Properties
	{
		_MainTex("Base layer (RGB)", 2D) = "white" {}
		_DetailTex("2nd layer (RGB)", 2D) = "white" {}
		_ScrollX("Base layer Scroll speed X", Float) = 1
		_ScrollY("Base layer Scroll speed Y", Float) = 0
		_Scroll2X("2nd layer Scroll speed X", Float) = 1
		_Scroll2Y("2nd layer Scroll speed Y", Float) = 0
		_SineAmplX("Base layer sine amplitude X", Float) = 0.5
		_SineAmplY("Base layer sine amplitude Y", Float) = 0.5
		_SineFreqX("Base layer sine freq X", Float) = 10
		_SineFreqY("Base layer sine freq Y", Float) = 10
		_SineAmplX2("2nd layer sine amplitude X", Float) = 0.5
		_SineAmplY2("2nd layer sine amplitude Y", Float) = 0.5
		_SineFreqX2("2nd layer sine freq X", Float) = 10
		_SineFreqY2("2nd layer sine freq Y", Float) = 10
		_Color("Color", Color) = (1,1,1,1)
		_MMultiplier("Layer Multiplier", Float) = 2
	}

		SubShader
		{
			LOD 100
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "true"
				"RenderType" = "Transparent"
			}

			ZWrite Off
			Cull Off
			Fog { Mode Off }
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_fog

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _DetailTex;
				float4 _DetailTex_ST;

				float _ScrollX, _ScrollY;
				float _Scroll2X, _Scroll2Y;
				float _SineAmplX, _SineAmplY;
				float _SineFreqX, _SineFreqY;
				float _SineAmplX2, _SineAmplY2;
				float _SineFreqX2, _SineFreqY2;
				float4 _Color;
				float _MMultiplier;

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float4 uv : TEXCOORD0;         
					fixed4 color : COLOR;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					float2 baseUV = TRANSFORM_TEX(v.uv, _MainTex);
					float2 baseScroll = float2(_ScrollX, _ScrollY) * _Time.xy;
					baseUV += frac(baseScroll);

					baseUV.x += sin(_Time.y * _SineFreqX) * _SineAmplX;
					baseUV.y += sin(_Time.y * _SineFreqY) * _SineAmplY;

					float2 detailUV = TRANSFORM_TEX(v.uv, _DetailTex);
					float2 detailScroll = float2(_Scroll2X, _Scroll2Y) * _Time.xy;
					detailUV += frac(detailScroll);

					detailUV.x += sin(_Time.y * _SineFreqX2) * _SineAmplX2;
					detailUV.y += sin(_Time.y * _SineFreqY2) * _SineAmplY2;

					o.uv = float4(baseUV, detailUV);

					o.color = (_MMultiplier * _Color) * v.color;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv.xy) * tex2D(_DetailTex, i.uv.zw) * i.color;
					return col;
				}
				ENDCG
			}
		}
}