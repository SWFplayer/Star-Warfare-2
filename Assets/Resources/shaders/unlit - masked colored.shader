Shader "Unlit/Masked Colored" {
	Properties{
		_MainTex("Base (RGB) Mask (A)", 2D) = "white" {}
		_Color("Tint Color", Color) = (1,1,1,1)
	}

		SubShader{
			LOD 200
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Mode Off }
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
				float4 _MainTex_ST;
				float4 _Color;

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					
					fixed4 col = tex2D(_MainTex, i.uv) * i.color;

				fixed3 finalRGB = lerp(col.xyz, col.xyz * _Color.xyz, col.w);

				return fixed4(finalRGB, col.w);
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
						AlphaTest Greater 0.01
						ColorMask RGB
						ColorMaterial AmbientAndDiffuse

						SetTexture[_MainTex] { combine texture * primary }
						SetTexture[_MainTex] { ConstantColor[_Color] combine previous * constant }
					}
			}
}