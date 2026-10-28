Shader "Unlit/Premultiplied Colored (AlphaClip)" {
	Properties{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "white" {}
	}

		SubShader{
			LOD 200
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Mode Off }
				Blend One OneMinusSrcAlpha
				ColorMask RGB
				

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
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
					float2 clipUV : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = v.uv;

					
					o.clipUV = (v.vertex.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					
					fixed4 col = tex2D(_MainTex, i.uv) * i.color;

				float2 absClip = abs(i.clipUV);
				float maxVal = max(absClip.x, absClip.y);
				float clipAlpha = 1.0 - maxVal;

				if (clipAlpha < 0.0) {
					col = fixed4(0.0, 0.0, 0.0, 0.0);
				}

				return col;
			}
			ENDCG
		}
	}

				SubShader{
					LOD 100
					Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

					Pass {
						Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

						ZWrite Off
						Cull Off
						Fog { Mode Off }
						Blend One OneMinusSrcAlpha
						ColorMask RGB
						ColorMaterial AmbientAndDiffuse
						

						SetTexture[_MainTex] { combine texture * primary }
					}
			}
}