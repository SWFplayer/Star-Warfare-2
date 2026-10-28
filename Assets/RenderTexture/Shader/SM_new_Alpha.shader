#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
Shader "StarWarfare/SNDM_new_Alpha" {
	Properties{
		_maincolor("Main Color", Color) = (1,1,1,1)
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_specColor("Specular Color", Color) = (0,0,0,1)
		_specTex_Gloss("Specular(RGB)Gloss(A)", 2D) = "black" {}
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_shininess("Shininess", Range(0,1)) = 0.984127
		_normalmap("Normalmap", 2D) = "bump" {}
		_paintColor("Paint Color", Color) = (1,0,0,1)
		_useCustomColor("Use Custom Color (bool)", Range(0,1)) = 0
		_Alpha("Alpha", Range(0,1)) = 1
	}
		SubShader{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
			LOD 400

			Pass {
				Name "FORWARD"
				Tags { "LIGHTMODE" = "ForwardBase" "SHADOWSUPPORT" = "true" "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
				Fog { Mode Off }
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				sampler2D _RGBA; float4 _RGBA_ST;
				sampler2D _lightTex;
				sampler2D _normalmap; float4 _normalmap_ST;
				sampler2D _specTex_Gloss; float4 _specTex_Gloss_ST;

				float4 _maincolor; float4 _specColor; float4 _paintColor;
				float _lightRate; float _specMultiple; float _shininess; float _useCustomColor; float _Alpha;

				struct v2f {
					float4 pos : SV_POSITION;
					float4 uv : TEXCOORD0;       
					float2 uvSpec : TEXCOORD1;
					float3 lightDir : TEXCOORD2;
					float3 shlight : TEXCOORD3;
					float3 viewDir : TEXCOORD4;
					LIGHTING_COORDS(5,6)
				};

				v2f vert(appdata_tan v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					TANGENT_SPACE_ROTATION;

					o.uv.xy = TRANSFORM_TEX(v.texcoord, _RGBA);
					o.uv.zw = TRANSFORM_TEX(v.texcoord, _normalmap);
					o.uvSpec = TRANSFORM_TEX(v.texcoord, _specTex_Gloss);

					o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
					o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

					float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
					o.shlight = ShadeSH9(float4(worldN, 1.0));

					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				float4 frag(v2f i) : SV_Target {
					float4 texBase = tex2D(_RGBA, i.uv.xy);
					float4 texLight = tex2D(_lightTex, i.uv.xy);
					float4 texNormal = tex2D(_normalmap, i.uv.zw);
					float4 texSpec = tex2D(_specTex_Gloss, i.uvSpec);

					float factor = _useCustomColor * texNormal.w;
					float3 baseCol = (texLight.rgb * _lightRate) + ((_maincolor.rgb * texBase.rgb) * (1.0 - factor)) + (_paintColor.rgb * factor);

					float gloss = _shininess + texSpec.a;
					float3 specCol = (texSpec.rgb + _specColor.rgb) * _specMultiple;

					float3 normal = normalize(texNormal.xyz * 2.0 - 1.0);
					float3 lightDir = i.lightDir;
					float3 viewDir = normalize(i.viewDir);

					float atten = LIGHT_ATTENUATION(i);

					float3 res;
					res = _LightColor0.xyz * max(0.0, dot(lightDir, normal));
					float lum = dot(_LightColor0.xyz, float3(0.22, 0.707, 0.071));
					float spec = pow(max(0.0, dot(normal, normalize(lightDir + viewDir))), gloss * 128.0) * lum;

					res *= 2.0;

					float3 c = (baseCol * res * atten) + (res * atten * (spec * specCol));
					c += baseCol * i.shlight;

					return float4(c, _Alpha);
				}
				ENDCG
			}

			Pass {
				Name "FORWARD"
				Tags { "LIGHTMODE" = "ForwardAdd" "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
				ZWrite Off
				Fog { Mode Off }
				Blend One One

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdadd_fullshadows
				#pragma fragmentoption ARB_precision_hint_fastest
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				sampler2D _RGBA; float4 _RGBA_ST;
				sampler2D _lightTex;
				sampler2D _normalmap; float4 _normalmap_ST;
				sampler2D _specTex_Gloss; float4 _specTex_Gloss_ST;

				float4 _maincolor; float4 _specColor; float4 _paintColor;
				float _lightRate; float _specMultiple; float _shininess; float _useCustomColor; float _Alpha;

				struct v2f {
					float4 pos : SV_POSITION;
					float4 uv : TEXCOORD0;
					float2 uvSpec : TEXCOORD1;
					float3 lightDir : TEXCOORD2;
					float3 viewDir : TEXCOORD3;
					LIGHTING_COORDS(4,5)
				};

				v2f vert(appdata_tan v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					TANGENT_SPACE_ROTATION;

					o.uv.xy = TRANSFORM_TEX(v.texcoord, _RGBA);
					o.uv.zw = TRANSFORM_TEX(v.texcoord, _normalmap);
					o.uvSpec = TRANSFORM_TEX(v.texcoord, _specTex_Gloss);

					o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
					o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				float4 frag(v2f i) : SV_Target {
					float4 texBase = tex2D(_RGBA, i.uv.xy);
					float4 texLight = tex2D(_lightTex, i.uv.xy);
					float4 texNormal = tex2D(_normalmap, i.uv.zw);
					float4 texSpec = tex2D(_specTex_Gloss, i.uvSpec);

					float factor = _useCustomColor * texNormal.w;
					float3 baseCol = (texLight.rgb * _lightRate) + ((_maincolor.rgb * texBase.rgb) * (1.0 - factor)) + (_paintColor.rgb * factor);

					float gloss = _shininess + texSpec.a;
					float3 specCol = (texSpec.rgb + _specColor.rgb) * _specMultiple;

					float3 normal = normalize(texNormal.xyz * 2.0 - 1.0);
					float3 lightDir = normalize(i.lightDir);
					float3 viewDir = normalize(i.viewDir);

					float atten = LIGHT_ATTENUATION(i);

					float3 res;
					res = _LightColor0.xyz * max(0.0, dot(lightDir, normal));
					float lum = dot(_LightColor0.xyz, float3(0.22, 0.707, 0.071));
					float spec = pow(max(0.0, dot(normal, normalize(lightDir + viewDir))), gloss * 128.0) * lum;

					res *= (atten * 2.0);

					float3 c = (baseCol * res) + (res * (spec * specCol));

					return float4(c, 0.0);     
				}
				ENDCG
			}

					Pass {
						Name "PREPASS"
						Tags { "LIGHTMODE" = "PrePassBase" "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
						Fog { Mode Off }
						Blend SrcAlpha OneMinusSrcAlpha

						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag
						#include "UnityCG.cginc"

						sampler2D _RGBA; float4 _RGBA_ST;
						sampler2D _normalmap; float4 _normalmap_ST;
						sampler2D _specTex_Gloss; float4 _specTex_Gloss_ST;
						float _shininess;
						float _Alpha;

						struct v2f {
							float4 pos : SV_POSITION;
							float4 uv : TEXCOORD0;
							float3 TtoW0 : TEXCOORD1;
							float3 TtoW1 : TEXCOORD2;
							float3 TtoW2 : TEXCOORD3;
						};

						v2f vert(appdata_tan v) {
							v2f o;
							o.pos = UnityObjectToClipPos(v.vertex);
							TANGENT_SPACE_ROTATION;

							o.uv.xy = TRANSFORM_TEX(v.texcoord, _normalmap);
							o.uv.zw = TRANSFORM_TEX(v.texcoord, _specTex_Gloss);

							o.TtoW0 = mul(rotation, ((float3x3)unity_ObjectToWorld)[0].xyz) * 1.0;
							o.TtoW1 = mul(rotation, ((float3x3)unity_ObjectToWorld)[1].xyz) * 1.0;
							o.TtoW2 = mul(rotation, ((float3x3)unity_ObjectToWorld)[2].xyz) * 1.0;
							return o;
						}

						float4 frag(v2f i) : SV_Target {
							float4 texNormal = tex2D(_normalmap, i.uv.xy);
							float4 texSpec = tex2D(_specTex_Gloss, i.uv.zw);

							float3 normal = normalize(texNormal.xyz * 2.0 - 1.0);
							float3 worldN;
							worldN.x = dot(i.TtoW0, normal);
							worldN.y = dot(i.TtoW1, normal);
							worldN.z = dot(i.TtoW2, normal);

							float gloss = _shininess + texSpec.a;

							return float4(worldN * 0.5 + 0.5, gloss);
						}
						ENDCG
					}

					Pass {
						Name "PREPASS"
						Tags { "LIGHTMODE" = "PrePassFinal" "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
						ZWrite Off
						Fog { Mode Off }
						Blend SrcAlpha OneMinusSrcAlpha

						CGPROGRAM
						#pragma vertex vert
						#pragma fragment frag
						#pragma multi_compile_prepassfinal
						#include "UnityCG.cginc"
						#include "Lighting.cginc"
						#include "AutoLight.cginc"

						sampler2D _RGBA; float4 _RGBA_ST;
						sampler2D _lightTex;
						sampler2D _normalmap; float4 _normalmap_ST;
						sampler2D _specTex_Gloss; float4 _specTex_Gloss_ST;
						sampler2D _LightBuffer;

						float4 _maincolor; float4 _specColor; float4 _paintColor;
						float _lightRate; float _specMultiple; float _useCustomColor; float _Alpha;

						struct v2f {
							float4 pos : SV_POSITION;
							float4 uv : TEXCOORD0;
							float2 uvSpec : TEXCOORD1;
							float4 screenPos : TEXCOORD2;
							float3 shlight : TEXCOORD3;
							float2 lmap : TEXCOORD4;
							float4 shadowFade : TEXCOORD5;
						};

						v2f vert(appdata_tan v) {
							v2f o;
							o.pos = UnityObjectToClipPos(v.vertex);
							o.screenPos = ComputeScreenPos(o.pos);

							o.uv.xy = TRANSFORM_TEX(v.texcoord, _RGBA);
							o.uv.zw = TRANSFORM_TEX(v.texcoord, _normalmap);
							o.uvSpec = TRANSFORM_TEX(v.texcoord, _specTex_Gloss);

							float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
							o.shlight = ShadeSH9(float4(worldN, 1.0));

							#ifdef LIGHTMAP_ON
								float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
								o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
								o.shadowFade.xyz = (worldPos - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
								o.shadowFade.w = (-mul(UNITY_MATRIX_MV, v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
							#endif
							return o;
						}

						float4 frag(v2f i) : SV_Target {
							float4 texBase = tex2D(_RGBA, i.uv.xy);
							float4 texLight = tex2D(_lightTex, i.uv.xy);
							float4 texNormal = tex2D(_normalmap, i.uv.zw);
							float4 texSpec = tex2D(_specTex_Gloss, i.uvSpec);

							float factor = _useCustomColor * texNormal.w;
							float3 baseCol = (texLight.rgb * _lightRate) + ((_maincolor.rgb * texBase.rgb) * (1.0 - factor)) + (_paintColor.rgb * factor);

							float3 specCol = (texSpec.rgb + _specColor.rgb) * _specMultiple;

							float4 light = tex2Dproj(_LightBuffer, i.screenPos);

							#ifdef HDR_LIGHT_PREPASS_ON
								light = max(light, 0.001);
							#else
								light = -(log2(max(light, 0.001)));
							#endif

							float3 lightColor = light.xyz;

							#ifdef LIGHTMAP_ON
								float lmFade = (sqrt(dot(i.shadowFade, i.shadowFade)) * unity_LightmapFade.z) + unity_LightmapFade.w;
								float3 lmFull = 2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap).xyz;
								float3 lmIndirect = 2.0 * UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap, i.lmap).xyz;
								lightColor += mix(lmIndirect, lmFull, saturate(lmFade));
							#else
								lightColor += i.shlight;
							#endif

							float3 c = (baseCol * lightColor) + (lightColor * (light.w * specCol));

							return float4(c, _Alpha);
						}
						ENDCG
					}
		}
			Fallback "Diffuse"
}