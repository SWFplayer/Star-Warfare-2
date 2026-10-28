Shader "StarWarfareShader/Smoke" {
	Properties{
		_MainTex("Base layer (RGB)", 2D) = "white" {}
		_DetailTex("2nd layer (RGB)", 2D) = "white" {}
		_ScrollX("Base layer Scroll speed X", Float) = 1
		_ScrollY("Base layer Scroll speed Y", Float) = 0
		_Scroll2X("2nd layer Scroll speed X", Float) = 1
		_Scroll2Y("2nd layer Scroll speed Y", Float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_MMultiplier("Layer Multiplier", Float) = 2
	}

		SubShader{
			LOD 100
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Mode Off }
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color  : COLOR;
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
				float _MMultiplier;
				fixed4 _Color;

				v2f vert(appdata_t v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);

					float2 scroll1 = float2(_ScrollX, _ScrollY);
					float2 scroll2 = float2(_Scroll2X, _Scroll2Y);

					float2 baseUV = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
					baseUV += frac(scroll1 * _Time.xy);

					float2 detailUV = v.uv * _DetailTex_ST.xy + _DetailTex_ST.zw;
					detailUV += frac(scroll2 * _Time.xy);

					o.uv = float4(baseUV, detailUV);

					o.color = _MMultiplier * _Color * v.color;

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