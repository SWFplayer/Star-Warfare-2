// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "StarWarfare/Scene/Emissive_Reflection_Lightmap" {
	Properties{
		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_GlossPower("Gloss", Float) = 1
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_LightPower("lightPower", Float) = 1
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_Emissive("Emissive", 2D) = "black" {}
		_ReflectionAlpha("Reflection Alpha", 2D) = "black" {}
		_ReflectionPower("Reflection Power", Float) = 0
		_2DReflect("2D Reflection", 2D) = "black" {}
		_Param1("reflection Parameter 1", Float) = 0.6
		_Param2("reflection Parameter 2", Float) = 0.07
	}
		SubShader{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			Pass {
				Name "CONTENTBASE"
				Tags { "LIGHTMODE" = "Always" "SHADOWSUPPORT" = "true" "RenderType" = "Opaque" }
				Fog { Mode Off }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_precision_hint_fastest
				 #pragma multi_compile_fog
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				sampler2D _MainTex; float4 _MainTex_ST;
				sampler2D _SpecTex;
				sampler2D _2DReflect;
				sampler2D _Emissive;
				sampler2D _ReflectionAlpha;

				float4 _Color;

				float _SpecularPower;
				float _GlossPower;
				float _Shininess;
				float _LightMap;
				float _Attenuation;
				float _LightPower;
				float3 _LightDir;
				float _ReflectionPower;
				float _Param1;
				float _Param2;

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					float2 uv1 : TEXCOORD1; // Lightmap
					float2 reflectionUV : TEXCOORD2;
					LIGHTING_COORDS(3,4)
						UNITY_FOG_COORDS(5)
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);

					#ifdef LIGHTMAP_ON
						o.uv1 = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

						// Original screen-space 2D reflection math
						float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
						float3 reflectionPos = (-_WorldSpaceCameraPos * _Param1 + worldPos) * _Param2;

						// Pass the XY world coordinates as the UV for the 2D reflection map
						o.reflectionUV = reflectionPos.xy;

						TRANSFER_VERTEX_TO_FRAGMENT(o);
						UNITY_TRANSFER_FOG(o, o.pos);
						return o;
					}

					float4 frag(v2f i) : SV_Target {
						float4 mainTex = tex2D(_MainTex, i.uv0);
						float4 reflTex = tex2D(_2DReflect, i.reflectionUV);
						float4 reflAlpha = tex2D(_ReflectionAlpha, i.uv0);
						float specMask = tex2D(_SpecTex, i.uv0).x;
						float3 emissive = tex2D(_Emissive, i.uv0).rgb;

						// Base Attenuation color
						float3 shadow = _Attenuation * 2.0;
						float3 ambient = unity_AmbientSky.rgb;

						float3 lighting;

						#ifdef LIGHTMAP_ON
						// Lightmapped path
						float3 lm = (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1).xyz) * _LightMap;
						shadow = min(lm, (_Attenuation * 2.0) * 0.8);

						// Diffuse part: Base + Reflection blended by ReflectionAlpha.x
						float3 baseColor = mainTex.rgb + ((reflTex.rgb * _ReflectionPower) * reflAlpha.x);
						float3 diff = (baseColor * shadow * _LightPower) * _Color.rgb;

						// Specular part
						lighting = diff + ((specMask * shadow) * _SpecularPower * _SpecColor.rgb);
					#else
						// Dynamic (non-lightmapped) path
						float3 ambientMix = lerp(shadow * _LightPower, ambient + 1.0, ambient);

						// Diffuse part: Base + Reflection blended by ReflectionAlpha.x
						float3 baseColor = mainTex.rgb + ((reflTex.rgb * _ReflectionPower) * reflAlpha.x);
						float3 diff = (baseColor * ambientMix) * _Color.rgb;

						// Specular part
						lighting = diff + ((specMask * shadow) * _SpecularPower * _SpecColor.rgb);
					#endif

						// Final result with Emissive added on top
						float3 finalCol = lighting + emissive;

						fixed4 finalColor = fixed4(finalCol, 0.0);

						UNITY_APPLY_FOG(i.fogCoord, finalColor);

						return finalColor;
					}
					ENDCG
				}
		}
			Fallback "StarWarfare/Scene/Emissive_Reflection_Lightmap_LOD100"
}