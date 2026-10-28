Shader "Advanced/Scroll 2 Layers Sine AlphaBlended_No_Lightmap" {
	Properties{
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

		SubShader{
			LOD 100
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

			Pass {
				Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Mode Off }
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float4 uv : TEXCOORD0;       
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				sampler2D _DetailTex;
				float4 _MainTex_ST;
				float4 _DetailTex_ST;

				float _ScrollX;
				float _ScrollY;
				float _Scroll2X;
				float _Scroll2Y;
				float _MMultiplier;

				float _SineAmplX;
				float _SineAmplY;
				float _SineFreqX;
				float _SineFreqY;
				float _SineAmplX2;
				float _SineAmplY2;
				float _SineFreqX2;
				float _SineFreqY2;

				fixed4 _Color;

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					float2 scroll1 = float2(_ScrollX, _ScrollY);
					float2 scroll2 = float2(_Scroll2X, _Scroll2Y);

					o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex) + frac(scroll1 * _Time.xy);
					o.uv.zw = TRANSFORM_TEX(v.uv, _DetailTex) + frac(scroll2 * _Time.xy);

					o.uv.x += sin(_Time.x * _SineFreqX) * _SineAmplX;
					o.uv.y += sin(_Time.x * _SineFreqY) * _SineAmplY;
					o.uv.z += sin(_Time.x * _SineFreqX2) * _SineAmplX2;
					o.uv.w += sin(_Time.x * _SineFreqY2) * _SineAmplY2;

					o.color = (_MMultiplier * _Color) * v.color;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed4 mainTex = tex2D(_MainTex, i.uv.xy);
					fixed4 detailTex = tex2D(_DetailTex, i.uv.zw);

					return mainTex * detailTex * i.color;
				}
				ENDCG
			}
		}
}