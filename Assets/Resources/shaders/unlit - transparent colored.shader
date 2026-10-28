Shader "Unlit/Transparent Colored" {
	Properties{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "white" {}
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
					float2 uv     : TEXCOORD0;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata_t v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
					o.color = v.color;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					return tex2D(_MainTex, i.uv) * i.color;
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
							Blend SrcAlpha OneMinusSrcAlpha
							AlphaTest Greater 0.01
							ColorMask RGB
							ColorMaterial AmbientAndDiffuse
							

							SetTexture[_MainTex] { combine texture * primary }
						}
				}
}