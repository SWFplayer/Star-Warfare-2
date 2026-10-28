Shader "Unlit/Text (AlphaClip)" {
	Properties{
		_MainTex("Alpha (A)", 2D) = "white" {}
	}

		SubShader{
			LOD 200
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
					fixed4 color : COLOR;
					float2 uv0    : TEXCOORD0;
					float2 uv1    : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata_t v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv0 = v.uv;                                        
					o.uv1 = v.vertex.xy * _MainTex_ST.xy + _MainTex_ST.zw; 
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed4 col;
					col.xyz = i.color.xyz;
					col.w = i.color.w * tex2D(_MainTex, i.uv0).w;

					float2 a = abs(i.uv1);
					float edge = 1.0 - max(a.x, a.y);
					if (edge < 0.0) {
						col.w = 0.0;
					}
					return col;
				}
				ENDCG
			}
	}

		Fallback "Unlit/Text"
}