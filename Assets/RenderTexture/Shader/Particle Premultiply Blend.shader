Shader "Particles/Alpha Blended Premultiply"
{
	Properties
	{
		_MainTex("Particle Texture", 2D) = "white" {}
		_InvFade("Soft Particles Factor", Range(0.01,3)) = 1
	}

		SubShader
		{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" }

			Pass
			{
				ZWrite Off
				Cull Off
				Fog { Mode Off }
				Blend One OneMinusSrcAlpha
				ColorMask RGB

				CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_particles
				#pragma multi_compile SOFTPARTICLES_OFF SOFTPARTICLES_ON

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _InvFade;

				#ifdef SOFTPARTICLES_ON
				sampler2D _CameraDepthTexture;
				float4 _ZBufferParams;
				float4 _ProjectionParams;
				#endif

				struct appdata
				{
					float4 vertex : POSITION;
					float4 color : COLOR;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					fixed4 color : COLOR;
					float2 uv : TEXCOORD0;
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD1;
					#endif
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.color = v.color;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					#ifdef SOFTPARTICLES_ON
					float4 clipPos = o.pos;
					float4 o4;
					o4.xy = (clipPos.xy * 0.5);
					o4.y = o4.y * _ProjectionParams.x;
					o4.xy = o4.xy + clipPos.w;
					o4.zw = clipPos.zw;
					o.projPos.xyw = o4.xyw;
					o.projPos.z = -(UnityObjectToViewPos(v.vertex).z);
					#endif

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = i.color;

					#ifdef SOFTPARTICLES_ON
					float sceneDepth = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
					float linearDepth = 1.0 / (_ZBufferParams.z * sceneDepth + _ZBufferParams.w);
					float fade = saturate(_InvFade * (linearDepth - i.projPos.z));
					col.a *= fade;
					#endif

					fixed4 tex = tex2D(_MainTex, i.uv);
					fixed4 result = (col * tex) * col.a;
					return result;
				}
				ENDCG
			}
		}
}