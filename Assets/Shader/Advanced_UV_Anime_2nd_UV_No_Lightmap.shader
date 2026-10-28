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
					float2 uv2 : TEXCOORD1; // 2nd UV set (used as a frame atlas)
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;   // Scrolled MainTex UVs
					float2 uv2 : TEXCOORD1;  // Scrolled DetailTex UVs (fract)
					float4 multiplier : TEXCOORD2; // Detail layer multiplier (0 or _AMultiplier)
					float2 frameOffset : TEXCOORD3; // Floor of the 2nd UV / 0.25
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

					// 1. MainTex UVs: Transform and scroll
					float2 scroll1 = float2(_ScrollX, _ScrollY);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex) + frac(scroll1 * _Time.x);

					// 2. DetailTex UVs: Transform and scroll (before frame offset)
					float2 uvDetail = TRANSFORM_TEX(v.uv2, _DetailTex);
					float2 scroll2 = float2(_Scroll2X, _Scroll2Y);
					o.uv2 = uvDetail + frac(scroll2 * _Time.x);

					// 3. Frame Offset: Floor the 2nd UV by 0.25 (for a 4x4 atlas)
					o.frameOffset = floor(uvDetail / float2(0.25, 0.25));

					// 4. Sleep/Active cycle
					float totalCycle = _SleepTime + _ActiveTime;
					float cycleTime = frac(abs(_Time.y / totalCycle)) * totalCycle;
					if (_Time.y < 0) cycleTime = -cycleTime;

					// Detail layer only visible during the active phase
					float active = (cycleTime > _SleepTime) ? _AMultiplier : 0.0;
					o.multiplier = float4(active, active, active, active);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					// 1. Reconstruct the DetailTex UVs using the frame atlas logic
					// (fract(uv2) * 0.25) + (frameOffset * 0.25)
					float2 detailUV = (frac(i.uv2) * 0.25) + (i.frameOffset * 0.25);

					// 2. Sample both textures
					fixed4 mainColor = tex2D(_MainTex, i.uv);
					fixed4 detailColor = tex2D(_DetailTex, detailUV);

					// 3. Combine: MainTex + (DetailTex * multiplier)
					return mainColor + (detailColor * i.multiplier);
				}
				ENDCG
			}
		}
}