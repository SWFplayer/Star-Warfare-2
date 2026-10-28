Shader "WFX/Additive Alpha8" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture (alpha)", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,3)) = 1
	}

		SubShader{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Color(0,0,0,0) }
				Blend One One

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile SOFTPARTICLES_OFF SOFTPARTICLES_ON
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 uv     : TEXCOORD0;
				};

				struct v2f {
					float4 vertex  : SV_POSITION;
					fixed4 color : COLOR;
					float2 uv      : TEXCOORD0;
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD1;
					#endif
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _TintColor;

				#ifdef SOFTPARTICLES_ON
				sampler2D _CameraDepthTexture;
				
				float _InvFade;
				#endif

				v2f vert(appdata_t v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					#ifdef SOFTPARTICLES_ON
					o.projPos = ComputeScreenPos(o.vertex);
					COMPUTE_EYEDEPTH(o.projPos.z);
					#endif

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed4 color = i.color;

					#ifdef SOFTPARTICLES_ON
					float sceneDepth = LinearEyeDepth(
						SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,
												  UNITY_PROJ_COORD(i.projPos)));
					float depthDelta = sceneDepth - i.projPos.z;
					color.a *= clamp(_InvFade * depthDelta, 0.0, 1.0);
					#endif

					float texAlpha = tex2D(_MainTex, i.uv).a;
					
					float mask = texAlpha * color.a;

					fixed4 col = (2.0 * color) * _TintColor * (mask * 2.0);
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
							ZWrite Off
							Cull Off
							Fog { Color(0,0,0,0) }
							Blend One One
							SetTexture[_MainTex] { ConstantColor[_TintColor] combine constant * primary }
							SetTexture[_MainTex] { combine texture alpha * previous double }
						}
				}
}