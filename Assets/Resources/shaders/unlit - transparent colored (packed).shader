Shader "Unlit/Transparent Colored (Packed)" {
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
				Blend SrcAlpha OneMinusSrcAlpha
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
				};

				sampler2D _MainTex;

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
				
					fixed4 mask = tex2D(_MainTex, i.uv);

				fixed4 activeChannels = clamp(ceil(i.color - 0.5), 0.0, 1.0);

				fixed4 rgbCalc = clamp(((activeChannels * 0.51) - i.color) / -0.49, 0.0, 1.0);
				fixed3 finalRGB = rgbCalc.xyz;

				fixed4 maskedTex = mask * activeChannels;

				float finalAlpha = (maskedTex.x + maskedTex.y + maskedTex.z + maskedTex.w);

				return fixed4(finalRGB, rgbCalc.w * finalAlpha);
			}
			ENDCG
		}
	}

		Fallback Off
}