Shader "Unlit/Premultiplied Colored" {
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
							Blend One OneMinusSrcAlpha
							ColorMask RGB
							ColorMaterial AmbientAndDiffuse
							

							SetTexture[_MainTex] { combine texture * primary }
						}
				}
}