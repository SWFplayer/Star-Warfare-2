Shader "Unlit/Dynamic Font" {
	Properties{
		_MainTex("Alpha (A)", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1) 
	}

		SubShader{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Mode Off }
				Blend SrcAlpha OneMinusSrcAlpha
				AlphaTest Greater 0.01
				

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
					fixed4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed4 col;
					col.xyz = i.color.xyz;
					col.w = i.color.w * tex2D(_MainTex, i.uv).w;
					return col;
				}
				ENDCG
			}
		}

			
					SubShader{
						Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

						Pass {
							Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

							BindChannels {
								Bind "vertex", Vertex
								Bind "color", Color
								Bind "texcoord", TexCoord
							}

							ZTest Always
							ZWrite Off
							Cull Off
							Fog { Mode Off }
							Blend SrcAlpha OneMinusSrcAlpha

							SetTexture[_MainTex] {
								ConstantColor[_Color]
								combine constant * primary, constant alpha * texture alpha
							}
						}
				}
}