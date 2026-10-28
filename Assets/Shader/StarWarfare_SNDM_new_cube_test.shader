Shader "StarWarfare/SNDM_new_cube_test" {
	Properties{
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_specColor("Specular Color", Color) = (0,0,0,1)
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_shininess("Shininess", Range(0,1)) = 0.984127
		_normalmap("Normalmap", 2D) = "bump" {}
		_paintColor("Paint Color", Color) = (1,0,0,1)
		_useCustomColor("Use Custom Color (bool)", Range(0,1)) = 0
		_seXiang("se xiang", Range(0,1)) = 0
		_seXiang_vector1("se xiang opt vector1", Vector) = (0,0,0,0)
		_seXiang_vector2("se xiang opt vector2", Vector) = (0,0,0,0)
		_Cube("Reflection Cubemap", CUBE) = "" { TexGen CubeReflect }
		_CubeColor("Reflection Color", Color) = (0,0,0,1)
	}
		SubShader{
			LOD 400
			Tags { "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }

			Pass {
				Name "FORWARD"
				Tags { "LIGHTMODE" = "ForwardBase" "SHADOWSUPPORT" = "true" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
				Fog { Mode Off }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_precision_hint_fastest

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				sampler2D _RGBA;
				float4 _RGBA_ST;
				float4 _specColor;
				sampler2D _lightTex;
				float _lightRate;
				float _specMultiple;
				float _shininess;
				sampler2D _normalmap;
				float4 _paintColor;
				float _useCustomColor;
				float _seXiang;
				float4 _seXiang_vector1;
				float4 _seXiang_vector2;
				samplerCUBE _Cube;
				float4 _CubeColor;

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 TtoW0 : TEXCOORD1;
					float4 TtoW1 : TEXCOORD2;
					float4 TtoW2 : TEXCOORD3;
					float3 lightDir : TEXCOORD4;
					float3 shlight : TEXCOORD5;
					float3 viewDir : TEXCOORD6;
					SHADOW_COORDS(7)
				};

				v2f vert(appdata_tan v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					TANGENT_SPACE_ROTATION;
					float3 worldNormal = mul(rotation, v.normal);
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					float3 worldViewDir = worldPos - _WorldSpaceCameraPos;

					o.TtoW0 = float4(rotation[0], worldViewDir.x);
					o.TtoW1 = float4(rotation[1], worldViewDir.y);
					o.TtoW2 = float4(rotation[2], worldViewDir.z);

					o.uv = TRANSFORM_TEX(v.texcoord, _RGBA);
					o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
					o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

					float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
					o.shlight = ShadeSH9(float4(worldN, 1.0));

					TRANSFER_SHADOW(o);
					return o;
				}

				float4 frag(v2f i) : SV_Target {
					
					float4 texBase = tex2D(_RGBA, i.uv);
					float4 texLight = tex2D(_lightTex, i.uv);
					float4 texNormal = tex2D(_normalmap, i.uv);

					float4 multiply0 = texLight * _lightRate;

					float factor = texNormal.w * _useCustomColor;

					float mx = max(max(texBase.x, texBase.y), texBase.z);
					float mn = min(min(texBase.x, texBase.y), texBase.z);
					float sat = (mx - mn) / (mx + 1e-05);
					float invSat = 1.0 - sat;
					float invSatZ = 1.0 - (sat * _seXiang_vector2.z);
					float invSatW = 1.0 - (sat * _seXiang_vector2.w);

					float3 seXiangColor;
					seXiangColor.x = dot(_seXiang_vector2.xy, float2(1,1)) + dot(_seXiang_vector1, float4(invSatW, invSat, invSat, invSatZ));
					seXiangColor.y = dot(_seXiang_vector2.xy, float2(invSat, invSatW)) + dot(_seXiang_vector1, float4(invSat, invSatZ, 1, 1));
					seXiangColor.z = dot(_seXiang_vector2.xy, float2(invSatZ, invSat)) + dot(_seXiang_vector1, float4(1, 1, invSatW, invSat));

					float4 colorTint;
					colorTint.w = 1.0;
					colorTint.xyz = seXiangColor * mx;

					multiply0 = multiply0 + ((colorTint * factor) + (texBase * (1.0 - factor)));

					float3 worldNormal = float3(i.TtoW0.z, i.TtoW1.z, i.TtoW2.z);
					float3 worldView = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
					float3 worldRefl = worldView - 2.0 * dot(worldNormal, worldView) * worldNormal;
					float4 reflCol = texCUBE(_Cube, worldRefl);
					float4 multiply1 = _specColor * _specMultiple;

					float3 finalColor = multiply0 + ((reflCol * texLight.w) * _CubeColor);

					float3 normal = normalize(texNormal.xyz * 2.0 - 1.0);
					float3 lightDir = i.lightDir;
					float3 viewDir = normalize(i.viewDir);

					float4 res;
					res.xyz = _LightColor0.xyz * max(0.0, dot(lightDir, normal));
					float lum = dot(_LightColor0.xyz, float3(0.22, 0.707, 0.071));
					res.w = pow(max(0.0, dot(normal, normalize(lightDir + viewDir))), _shininess * 128.0) * lum;
					res *= 2.0;

					float atten = SHADOW_ATTENUATION(i);

					float4 c;
					c.xyz = (finalColor * res.xyz) + (res.xyz * (res.w * multiply1.xyz));
					c.w = 1.0;

					c.xyz = c.xyz + (finalColor * i.shlight);

					return c;
				}
				ENDCG
			}

			Pass {
				Name "FORWARD"
				Tags { "LIGHTMODE" = "ForwardAdd" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
				ZWrite Off
				Fog { Mode Off }
				Blend One One

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdadd_fullshadows

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				sampler2D _RGBA;
				float4 _RGBA_ST;
				float4 _specColor;
				sampler2D _lightTex;
				float _lightRate;
				float _specMultiple;
				float _shininess;
				sampler2D _normalmap;
				float _useCustomColor;
				float4 _seXiang_vector1;
				float4 _seXiang_vector2;
				samplerCUBE _Cube;
				float4 _CubeColor;
#if !defined(UNITY_DECLARE_LIGHT_COORDS_INCLUDED) && !defined(POINT) && !defined(SPOT) && !defined(POINT_COOKIE) && !defined(DIRECTIONAL_COOKIE)
				float4x4 unity_WorldToLight;
#endif

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 TtoW0 : TEXCOORD1;
					float4 TtoW1 : TEXCOORD2;
					float4 TtoW2 : TEXCOORD3;
					float3 lightDir : TEXCOORD4;
					float3 viewDir : TEXCOORD5;
					float4 lightCoord : TEXCOORD6;
				};

				v2f vert(appdata_tan v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					TANGENT_SPACE_ROTATION;
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					float3 worldViewDir = worldPos - _WorldSpaceCameraPos;

					o.TtoW0 = float4(rotation[0], worldViewDir.x);
					o.TtoW1 = float4(rotation[1], worldViewDir.y);
					o.TtoW2 = float4(rotation[2], worldViewDir.z);

					o.uv = TRANSFORM_TEX(v.texcoord, _RGBA);
					o.lightDir = mul(rotation, ObjSpaceLightDir(v.vertex));
					o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));
					o.lightCoord = mul(unity_WorldToLight, mul(unity_ObjectToWorld, v.vertex));
					return o;
				}

				float4 frag(v2f i) : SV_Target {
					float4 texBase = tex2D(_RGBA, i.uv);
					float4 texLight = tex2D(_lightTex, i.uv);
					float4 texNormal = tex2D(_normalmap, i.uv);

					float4 multiply0 = texLight * _lightRate;
					float factor = texNormal.w * _useCustomColor;

					float mx = max(max(texBase.x, texBase.y), texBase.z);
					float mn = min(min(texBase.x, texBase.y), texBase.z);
					float sat = (mx - mn) / (mx + 1e-05);
					float invSat = 1.0 - sat;
					float invSatZ = 1.0 - (sat * _seXiang_vector2.z);
					float invSatW = 1.0 - (sat * _seXiang_vector2.w);

					float3 seXiangColor;
					seXiangColor.x = dot(_seXiang_vector2.xy, float2(1,1)) + dot(_seXiang_vector1, float4(invSatW, invSat, invSat, invSatZ));
					seXiangColor.y = dot(_seXiang_vector2.xy, float2(invSat, invSatW)) + dot(_seXiang_vector1, float4(invSat, invSatZ, 1, 1));
					seXiangColor.z = dot(_seXiang_vector2.xy, float2(invSatZ, invSat)) + dot(_seXiang_vector1, float4(1, 1, invSatW, invSat));

					float4 colorTint;
					colorTint.w = 1.0;
					colorTint.xyz = seXiangColor * mx;

					multiply0 = multiply0 + ((colorTint * factor) + (texBase * (1.0 - factor)));

					float3 worldNormal = float3(i.TtoW0.z, i.TtoW1.z, i.TtoW2.z);
					float3 worldView = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
					float3 worldRefl = worldView - 2.0 * dot(worldNormal, worldView) * worldNormal;
					float4 reflCol = texCUBE(_Cube, worldRefl);
					float4 multiply1 = _specColor * _specMultiple;

					float3 finalColor = multiply0 + ((reflCol * texLight.w) * _CubeColor);

					float3 normal = normalize(texNormal.xyz * 2.0 - 1.0);
					float3 lightDir = normalize(i.lightDir);
					float3 viewDir = normalize(i.viewDir);

					float atten;
					#if defined(POINT)
						float2 lightUV = i.lightCoord.xy;
						atten = tex2D(_LightTexture0, lightUV).w;
					#elif defined(SPOT)
						float2 lightUV = (i.lightCoord.xy / i.lightCoord.w) + 0.5;
						atten = tex2D(_LightTexture0, lightUV).w * tex2D(_LightTextureB0, dot(i.lightCoord.xyz, i.lightCoord.xyz)).w;
					#elif defined(POINT_COOKIE)
						atten = tex2D(_LightTextureB0, dot(i.lightCoord.xyz, i.lightCoord.xyz)).w * texCUBE(_LightTexture0, i.lightCoord.xyz).w;
					#elif defined(DIRECTIONAL_COOKIE)
						atten = tex2D(_LightTexture0, i.lightCoord.xy).w;
					#else
						atten = 1.0;
					#endif

					float4 res;
					res.xyz = _LightColor0.xyz * max(0.0, dot(lightDir, normal));
					float lum = dot(_LightColor0.xyz, float3(0.22, 0.707, 0.071));
					res.w = pow(max(0.0, dot(normal, normalize(lightDir + viewDir))), _shininess * 128.0) * lum;
					res *= atten * 2.0;

					float4 c;
					c.xyz = (finalColor * res.xyz) + (res.xyz * (res.w * multiply1.xyz));
					c.w = 0.0;
					return c;
				}
				ENDCG
			}

			Pass {
				Name "PREPASS"
				Tags { "LIGHTMODE" = "PrePassBase" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
				Fog { Mode Off }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_prepassbase

				#include "UnityCG.cginc"

				sampler2D _RGBA;
				float4 _RGBA_ST;
				float _shininess;
				sampler2D _normalmap;

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 TtoW0 : TEXCOORD1;
					float3 TtoW1 : TEXCOORD2;
					float3 TtoW2 : TEXCOORD3;
				};

				v2f vert(appdata_tan v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					TANGENT_SPACE_ROTATION;
					o.uv = TRANSFORM_TEX(v.texcoord, _RGBA);
					o.TtoW0 = mul(rotation, ((float3x3)unity_ObjectToWorld)[0].xyz) * 1.0;
					o.TtoW1 = mul(rotation, ((float3x3)unity_ObjectToWorld)[1].xyz) * 1.0;
					o.TtoW2 = mul(rotation, ((float3x3)unity_ObjectToWorld)[2].xyz) * 1.0;
					return o;
				}

				float4 frag(v2f i) : SV_Target {
					float4 texNormal = tex2D(_normalmap, i.uv);
					float3 normal = normalize(texNormal.xyz * 2.0 - 1.0);
					float3 worldN;
					worldN.x = dot(i.TtoW0, normal);
					worldN.y = dot(i.TtoW1, normal);
					worldN.z = dot(i.TtoW2, normal);
					float3 res = worldN * 0.5 + 0.5;
					return float4(res, _shininess);
				}
				ENDCG
			}

			Pass {
				Name "PREPASS"
				Tags { "LIGHTMODE" = "PrePassFinal" "IGNOREPROJECTOR" = "False" "RenderType" = "Opaque" }
				ZWrite Off
				Fog { Mode Off }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_prepassfinal

				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				sampler2D _RGBA;
				float4 _RGBA_ST;
				float4 _specColor;
				sampler2D _lightTex;
				float _lightRate;
				float _specMultiple;
				sampler2D _normalmap;
				float _useCustomColor;
				float4 _seXiang_vector1;
				float4 _seXiang_vector2;
				samplerCUBE _Cube;
				float4 _CubeColor;
				sampler2D _LightBuffer;

				float4 unity_LightmapFade;
				
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 screenPos : TEXCOORD1;
					float4 TtoW0 : TEXCOORD2;
					float4 TtoW1 : TEXCOORD3;
					float4 TtoW2 : TEXCOORD4;
					float3 shlight : TEXCOORD5;
					float2 lmap : TEXCOORD6;
					float4 shadowFade : TEXCOORD7;
				};

				struct appdata_full_custom {
					float4 vertex : POSITION;
					float4 tangent : TANGENT;
					float3 normal : NORMAL;
					float4 texcoord : TEXCOORD0;
					float4 texcoord1 : TEXCOORD1;
					float4 color : COLOR;
				};

				v2f vert(appdata_full_custom v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.screenPos = ComputeScreenPos(o.pos);

					TANGENT_SPACE_ROTATION;
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					float3 worldViewDir = worldPos - _WorldSpaceCameraPos;

					o.TtoW0 = float4(rotation[0], worldViewDir.x);
					o.TtoW1 = float4(rotation[1], worldViewDir.y);
					o.TtoW2 = float4(rotation[2], worldViewDir.z);

					o.uv = TRANSFORM_TEX(v.texcoord, _RGBA);

					float3 worldN = mul((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
					o.shlight = ShadeSH9(float4(worldN, 1.0));

					#ifdef LIGHTMAP_ON
						o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
						o.shadowFade.xyz = (worldPos - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w;
						o.shadowFade.w = (-mul(UNITY_MATRIX_MV, v.vertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w);
					#endif

					return o;
				}

				float4 frag(v2f i) : SV_Target {
					float4 texBase = tex2D(_RGBA, i.uv);
					float4 texLight = tex2D(_lightTex, i.uv);
					float4 texNormal = tex2D(_normalmap, i.uv);

					float4 multiply0 = texLight * _lightRate;
					float factor = texNormal.w * _useCustomColor;

					float mx = max(max(texBase.x, texBase.y), texBase.z);
					float mn = min(min(texBase.x, texBase.y), texBase.z);
					float sat = (mx - mn) / (mx + 1e-05);
					float invSat = 1.0 - sat;
					float invSatZ = 1.0 - (sat * _seXiang_vector2.z);
					float invSatW = 1.0 - (sat * _seXiang_vector2.w);

					float3 seXiangColor;
					seXiangColor.x = dot(_seXiang_vector2.xy, float2(1,1)) + dot(_seXiang_vector1, float4(invSatW, invSat, invSat, invSatZ));
					seXiangColor.y = dot(_seXiang_vector2.xy, float2(invSat, invSatW)) + dot(_seXiang_vector1, float4(invSat, invSatZ, 1, 1));
					seXiangColor.z = dot(_seXiang_vector2.xy, float2(invSatZ, invSat)) + dot(_seXiang_vector1, float4(1, 1, invSatW, invSat));

					float4 colorTint;
					colorTint.w = 1.0;
					colorTint.xyz = seXiangColor * mx;

					multiply0 = multiply0 + ((colorTint * factor) + (texBase * (1.0 - factor)));

					float3 worldNormal = float3(i.TtoW0.z, i.TtoW1.z, i.TtoW2.z);
					float3 worldView = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
					float3 worldRefl = worldView - 2.0 * dot(worldNormal, worldView) * worldNormal;
					float4 reflCol = texCUBE(_Cube, worldRefl);
					float4 multiply1 = _specColor * _specMultiple;

					float3 finalColor = multiply0 + ((reflCol * texLight.w) * _CubeColor);

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

						lightColor += lerp(lmIndirect, lmFull, saturate(lmFade));
					
					#else
						lightColor += i.shlight;
					#endif

					float4 c;
					c.xyz = (finalColor * lightColor) + (lightColor * (light.w * multiply1.xyz));
					c.w = 1.0;
					return c;
				}
				ENDCG
				}
		}
		Fallback "StarWarfare/Middle/SNDM_new"
}