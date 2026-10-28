Shader "StarWarfare/Middle/SNDM_new_LOD100" {
	Properties{
		_maincolor("Main Color", Color) = (1,1,1,1)
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_shininess("Shininess", Range(0,1)) = 0.984127
		_paintColor("Paint Color", Color) = (1,0,0,1)
		_useCustomColor("Use Custom Color (bool)", Range(0,1)) = 0
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }

			Pass {
				Tags { "LightMode" = "ForwardBase" }

				Lighting On
				Fog { Mode Off }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#include "UnityCG.cginc"
				#include "Lighting.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 lightColor : TEXCOORD1;
				};

				sampler2D _RGBA;
				sampler2D _lightTex;
				float _lightRate;

				v2f vert(appdata v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;

					float3 viewPos = mul(UNITY_MATRIX_MV, v.vertex).xyz;
					
					float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));

					float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;

					for (int i = 0; i < 4; i++) {
						float3 lightDir = _WorldSpaceLightPos0.xyz - (viewPos * _WorldSpaceLightPos0.w);
						
						float distSq = dot(lightDir, lightDir);
						float atten = 1.0 / (1.0 + distSq * unity_LightAtten[i].z);

						float NdotL = max(0.0, dot(viewNormal, normalize(lightDir)));
						lightColor += unity_LightColor[i].xyz * (NdotL * atten);
					}

					o.lightColor = lightColor * 2.0;

					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					fixed4 baseTex = tex2D(_RGBA, i.uv);
					fixed4 lightTex = tex2D(_lightTex, i.uv);

					fixed4 c = baseTex + (lightTex * _lightRate);

					fixed3 resultRGB = c.xyz * i.lightColor;

					return fixed4(resultRGB, c.w);
				}
				ENDCG
			}
		}

			Fallback "Mobile/Diffuse"
}