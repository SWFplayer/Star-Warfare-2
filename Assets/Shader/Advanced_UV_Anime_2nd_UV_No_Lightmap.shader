Shader "Advanced/UV_Anime_2nd_UV_No_Lightmap" {
	Properties{
		_MainTex("Base layer (RGB)", 2D) = "white" {}
		_DetailTex("2nd layer (RGB)", 2D) = "white" {}
		_ScrollX("Base layer Scroll speed X", Float) = 1
		_ScrollY("Base layer Scroll speed Y", Float) = 0
		_Scroll2X("2nd layer Scroll speed X", Float) = 1
		_Scroll2Y("2nd layer Scroll speed Y", Float) = 0
		_AMultiplier("Layer Multiplier", Float) = 0.5
		_SleepTime("2nd UV Sleep Time", Float) = 3
		_ActiveTime("2nd UV Active Time", Float) = 1
	}

		SubShader{
			LOD 100
			Tags { "QUEUE" = "Geometry+10" "RenderType" = "Opaque" }
			Fog { Mode Off }

			Pass {
				Tags { "QUEUE" = "Geometry+10" "RenderType" = "Opaque" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;         
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;      
					float2 uv2 : TEXCOORD1;      
					float4 multiplier : TEXCOORD2;       
					float2 frameOffset : TEXCOORD3;        
				};

				sampler2D _MainTex;
				sampler2D _DetailTex;
				float4 _MainTex_ST;
				float4 _DetailTex_ST;

				float _ScrollX;
				float _ScrollY;
				float _Scroll2X;
				float _Scroll2Y;
				float _AMultiplier;
				float _SleepTime;
				float _ActiveTime;

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					float2 scroll1 = float2(_ScrollX, _ScrollY);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex) + frac(scroll1 * _Time.x);

					float2 uvDetail = TRANSFORM_TEX(v.uv2, _DetailTex);
					float2 scroll2 = float2(_Scroll2X, _Scroll2Y);
					o.uv2 = uvDetail + frac(scroll2 * _Time.x);

					o.frameOffset = floor(uvDetail / float2(0.25, 0.25));

					float totalCycle = _SleepTime + _ActiveTime;
					float cycleTime = frac(abs(_Time.y / totalCycle)) * totalCycle;
					if (_Time.y < 0) cycleTime = -cycleTime;

					float active = (cycleTime > _SleepTime) ? _AMultiplier : 0.0;
					o.multiplier = float4(active, active, active, active);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					float2 detailUV = (frac(i.uv2) * 0.25) + (i.frameOffset * 0.25);

					fixed4 mainColor = tex2D(_MainTex, i.uv);
					fixed4 detailColor = tex2D(_DetailTex, detailUV);

					return mainColor + (detailColor * i.multiplier);
				}
				ENDCG
			}
		}
}