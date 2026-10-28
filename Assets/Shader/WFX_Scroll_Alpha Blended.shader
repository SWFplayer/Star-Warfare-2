Shader "WFX/Scroll/Alpha Blended" {
	Properties{
		_MainTex("Looped Texture + Alpha Mask", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,3)) = 1
		_ScrollSpeed("Scroll Speed", Float) = 2
	}

		SubShader{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Color(0,0,0,0) }
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMask RGB

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
				float _ScrollSpeed;

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

					float alpha = color.a * tex2D(_MainTex, i.uv).a;

					float scroll = frac(_Time.x * _ScrollSpeed);

					float2 scrolledUV = float2(i.uv.x, i.uv.y - scroll);
					fixed3 rgb = color.rgb * tex2D(_MainTex, scrolledUV).rgb;

					return fixed4(rgb, alpha);
				}
				ENDCG
			}
		}
}