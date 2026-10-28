Shader "StarWarfare/Scene/Blend_Detail_Normal_Lightmap_Shadow" {
	Properties{
		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_GlossPower("Gloss", Float) = 1
		_UVDetailNormalTile("UV Detail Normal Tile", Float) = 1
		_DetailNormalMulti("Detail Normal Multi", Float) = 1
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_NormalTex("Normal (RGB)", 2D) = "bump" {}
		_DetailNormal("Detail Normal map", 2D) = "white" {}
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_MainTexB("Diffuse (RGB) Specular (A) blend", 2D) = "white" {}
		_DetailNormalB("Detail Normal map blend", 2D) = "black" {}
		_UVDetailNormalTileB("UV Detail Normal Tile blend", Float) = 1
		_DetailNormalMultiB("Detail Normal Multi blend", Float) = 1
		_LightDirB("LightDirection of blend", Vector) = (0,0,0,0)
		_BlendRate("Blend Rate For Test", Range(0,1)) = 0
		_MaskMap("Blend Mask map", 2D) = "white" {}
		_ShadowMap("Shadow Map", 2D) = "black" {}
		_ShadowDarkness("Shadow Darkness", Range(0,1)) = 0.5
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
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
				#include "UnityCG.cginc"
				#include "Lighting.cginc"
				#include "AutoLight.cginc"

				sampler2D _MainTex; float4 _MainTex_ST;
				sampler2D _MainTexB; float4 _MainTexB_ST;
				sampler2D _NormalTex;
				sampler2D _DetailNormal;
				sampler2D _DetailNormalB;
				sampler2D _MaskMap;
				sampler2D _ShadowMap;

				float4 _Color;
				
				float4 _ShadowColor;
				float _SpecularPower;
				float _GlossPower;
				float _Shininess;
				float _LightMap;
				float _Attenuation;
				float _ShadowDarkness;
				float _UVDetailNormalTile;
				float _DetailNormalMulti;
				float _UVDetailNormalTileB;
				float _DetailNormalMultiB;
				float3 _LightDir;
				float3 _LightDirB;
				float4x4 _LocalToShadowMatrix;      

				struct v2f {
					float4 pos : SV_POSITION;
					float4 uv : TEXCOORD0;       
					float4 screenPoint : TEXCOORD1;    
					float2 lmap : TEXCOORD2;
					float2 maskUV : TEXCOORD3;    
				};

				v2f vert(appdata_full v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.uv.zw = TRANSFORM_TEX(v.texcoord, _MainTexB);

					o.screenPoint = mul(_LocalToShadowMatrix, v.vertex);

					#ifdef LIGHTMAP_ON
						o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

						o.maskUV = v.color.xy;

						return o;
					}

					float4 frag(v2f i) : SV_Target {
						float4 mainTex = tex2D(_MainTex, i.uv.xy);
						float4 mainTexB = tex2D(_MainTexB, i.uv.zw);

						float4 detNorm = tex2D(_DetailNormal, i.uv.xy * _UVDetailNormalTile);
						float4 detNormB = tex2D(_DetailNormalB, i.uv.zw * _UVDetailNormalTileB);

						float3 normA = tex2D(_NormalTex, i.uv.xy).xyz * 2.0 - 1.0;
						normA.xy += ((detNorm.xy * 2.0) - 1.0) * _DetailNormalMulti;
						normA = normalize(normA);

						float3 normB = detNormB.xyz * 2.0 - 1.0;
						normB.xy = ((detNormB.xy * 2.0) - 1.0) * _DetailNormalMultiB;
						normB = normalize(normB);

						float3 ambient = unity_AmbientSky.rgb;
						float3 shadow = _Attenuation * 2.0;
						float3 resultA, resultB;

						#ifdef LIGHTMAP_ON
						float3 lm = (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap).xyz) * _LightMap;
						shadow = min(lm, (_Attenuation * 2.0) * 0.8);

						float NdotLA = max(0.0, (dot(normA, _LightDir) * 0.5) + 1.0);
						float3 diffA = (mainTex.rgb * shadow * NdotLA) * _Color.rgb;
						resultA = diffA + ((mainTex.a * shadow) * _SpecularPower * _SpecColor.rgb);

						float NdotLB = max(0.0, (dot(normB, _LightDirB) * 0.5) + 1.0);
						float3 diffB = (mainTexB.rgb * shadow * NdotLB) * _Color.rgb;
						resultB = diffB + ((mainTexB.a * shadow) * _SpecularPower * _SpecColor.rgb);
					#else
						float3 mixA = lerp(shadow * max(0.0, dot(normA, _LightDir)), ambient + 1.0, ambient);
						float3 diffA = (mainTex.rgb * mixA) * _Color.rgb;
						resultA = diffA + ((mainTex.a * shadow) * _SpecularPower * _SpecColor.rgb);

						float3 mixB = lerp(shadow * max(0.0, dot(normB, _LightDirB)), ambient + 1.0, ambient);
						float3 diffB = (mainTexB.rgb * mixB) * _Color.rgb;
						resultB = diffB + ((mainTexB.a * shadow) * _SpecularPower * _SpecColor.rgb);
					#endif

						float blendMask = tex2D(_MaskMap, i.maskUV).w;
						float3 finalCol = lerp(resultB, resultA, blendMask);

						float2 shadowUV = (i.screenPoint.xy / i.screenPoint.w) * 0.5 + 0.5;
						float4 shadowSample = tex2D(_ShadowMap, shadowUV);

						float2 absUV = abs(shadowUV - 0.5);
						float isVisible = max(sign(0.5 - max(absUV.x, absUV.y)), 0.0);

						float shadowWeight = (shadowSample.x * isVisible) * _ShadowDarkness;

						finalCol = (finalCol * (1.0 - shadowWeight)) + (_ShadowColor.rgb * shadowWeight);

						return float4(finalCol, 0.0);
					}
					ENDCG
				}
		}
}