Shader "Advanced/Scroll_2Layers" {
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
		_MMultiplier("Layer Multiplier", Float) = 2
	}

		SubShader{
			LOD 100
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Color(0,0,0,0) }
				Blend One One

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 uv     : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float4 uv     : TEXCOORD0;          
					fixed4 color : TEXCOORD1;     
				};

				sampler2D _MainTex;
				sampler2D _DetailTex;
				float4 _MainTex_ST;
				float4 _DetailTex_ST;

				float _ScrollX;
				float _ScrollY;
				float _Scroll2X;
				float _Scroll2Y;
				float _SineAmplX;
				float _SineAmplY;
				float _SineFreqX;
				float _SineFreqY;
				float _SineAmplX2;
				float _SineAmplY2;
				float _SineFreqX2;
				float _SineFreqY2;
				float _MMultiplier;

				v2f vert(appdata_t v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					float2 scroll1 = float2(_ScrollX, _ScrollY);
					float2 scroll2 = float2(_Scroll2X, _Scroll2Y);

					float2 baseUV = v.uv * _MainTex_ST.xy + _MainTex_ST.zw
						+ frac(float2(_ScrollX, _ScrollY) * _Time.xy);
					float2 detailUV = v.uv * _DetailTex_ST.xy + _DetailTex_ST.zw
						+ frac(float2(_Scroll2X, _Scroll2Y) * _Time.xy);
					baseUV.x += sin(_Time.x * _SineFreqX)  * _SineAmplX;
					baseUV.y += sin(_Time.x * _SineFreqY)  * _SineAmplY;
					detailUV.x += sin(_Time.x * _SineFreqX2) * _SineAmplX2;
					detailUV.y += sin(_Time.x * _SineFreqY2) * _SineAmplY2;

					o.uv = float4(baseUV, detailUV);
					o.color = _MMultiplier;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed4 c = tex2D(_MainTex, i.uv.xy) * tex2D(_DetailTex, i.uv.zw) * i.color;
					return c;
				}
				ENDCG
			}
		}
}