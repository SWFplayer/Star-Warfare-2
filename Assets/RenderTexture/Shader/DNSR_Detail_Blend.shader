Shader "StarWarfare/Scene/Blend_Detail_Normal_Lightmap" {
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
		  #pragma multi_compile_fog
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

			float4 _Color;
			float _SpecularPower; float _GlossPower; float _Shininess;
			float _LightMap; float _Attenuation;
			float _UVDetailNormalTile; float _DetailNormalMulti;
			float _UVDetailNormalTileB; float _DetailNormalMultiB;
			float3 _LightDir; float3 _LightDirB;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 maskUV : TEXCOORD2;
				float2 lmap : TEXCOORD3;
				LIGHTING_COORDS(4,5)
					UNITY_FOG_COORDS(6)
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.texcoord, _MainTexB);
				o.maskUV = v.color.xy;

				#ifdef LIGHTMAP_ON
					o.lmap = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif

				TRANSFER_VERTEX_TO_FRAGMENT(o);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			float4 frag(v2f i) : SV_Target {
				float4 mainTex = tex2D(_MainTex, i.uv0);
				float4 mainTexB = tex2D(_MainTexB, i.uv1);

				float4 detNorm = tex2D(_DetailNormal, i.uv0 * _UVDetailNormalTile);
				float4 detNormB = tex2D(_DetailNormalB, i.uv1 * _UVDetailNormalTileB);

				float3 normA = tex2D(_NormalTex, i.uv0).xyz * 2.0 - 1.0;
				normA.xy += ((detNorm.xy * 2.0) - 1.0) * _DetailNormalMulti;
				normA = normalize(normA);

				float3 normB = detNormB.xyz * 2.0 - 1.0;
				normB.xy = ((detNormB.xy * 2.0) - 1.0) * _DetailNormalMultiB;
				normB = normalize(normB);

				float3 amb = unity_AmbientSky.rgb;   
				float3 shadow = _Attenuation * 2.0;

				float NdotLA = max(0.0, dot(normA, _LightDir));
				float3 mixA = lerp(shadow * NdotLA, amb + 1.0, amb);    
				float3 diffA = (mainTex.rgb * mixA) * _Color.rgb;
				float3 resultA = diffA + ((mainTex.a * shadow) * _SpecularPower * _SpecColor.rgb);

				float NdotLB = max(0.0, dot(normB, _LightDirB));
				float3 mixB = lerp(shadow * NdotLB, amb + 1.0, amb);
				float3 diffB = (mainTexB.rgb * mixB) * _Color.rgb;
				float3 resultB = diffB + ((mainTexB.a * shadow) * _SpecularPower * _SpecColor.rgb);

				#ifdef LIGHTMAP_ON
					float3 lm = (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmap).xyz) * _LightMap;
					shadow = min(lm, (_Attenuation * 2.0) * 0.8);

					float NdotLA_lm = max(0.0, (dot(normA, _LightDir) * 0.5) + 1.0);
					diffA = ((mainTex.rgb * shadow) * NdotLA_lm) * _Color.rgb;
					resultA = diffA + ((mainTex.a * shadow) * _SpecularPower * _SpecColor.rgb);

					float NdotLB_lm = max(0.0, (dot(normB, _LightDirB) * 0.5) + 1.0);
					diffB = ((mainTexB.rgb * shadow) * NdotLB_lm) * _Color.rgb;
					resultB = diffB + ((mainTexB.a * shadow) * _SpecularPower * _SpecColor.rgb);
				#endif

					float blendMask = tex2D(_MaskMap, i.maskUV).w;
					float3 finalCol = lerp(resultB, resultA, blendMask);

					fixed4 finalColor = fixed4(finalCol, 0.0);
					UNITY_APPLY_FOG(i.fogCoord, finalColor);

					return finalColor;
				}
				ENDCG
			}

		Pass {
			Name "FORWARD"
			Tags { "LIGHTMODE" = "ForwardAdd" "RenderType" = "Opaque" }
			ZWrite Off
			Fog { Mode Off }
			Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd
			  #pragma multi_compile_fog
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

			float4 _Color; 
			float _SpecularPower; float _GlossPower; float _Shininess;
			float _LightMap; float _Attenuation;
			float _UVDetailNormalTile; float _DetailNormalMulti;
			float _UVDetailNormalTileB; float _DetailNormalMultiB;
			float3 _LightDir; float3 _LightDirB;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 maskUV : TEXCOORD2;
				float3 lightDir : TEXCOORD3;
				LIGHTING_COORDS(4,5)
					UNITY_FOG_COORDS(6)
			};

			v2f vert(appdata_full v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.texcoord, _MainTexB);
				o.maskUV = v.color.xy;

				o.lightDir = ObjSpaceLightDir(v.vertex);

				TRANSFER_VERTEX_TO_FRAGMENT(o);
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			float4 frag(v2f i) : SV_Target {
				float4 mainTex = tex2D(_MainTex, i.uv0);
				float4 mainTexB = tex2D(_MainTexB, i.uv1);

				float4 detNorm = tex2D(_DetailNormal, i.uv0 * _UVDetailNormalTile);
				float4 detNormB = tex2D(_DetailNormalB, i.uv1 * _UVDetailNormalTileB);

				float3 normA = tex2D(_NormalTex, i.uv0).xyz * 2.0 - 1.0;
				normA.xy += ((detNorm.xy * 2.0) - 1.0) * _DetailNormalMulti;
				normA = normalize(normA);

				float3 normB = detNormB.xyz * 2.0 - 1.0;
				normB.xy = ((detNormB.xy * 2.0) - 1.0) * _DetailNormalMultiB;
				normB = normalize(normB);

				float3 lightDir = normalize(i.lightDir);
				float atten = LIGHT_ATTENUATION(i);

				float NdotLA = max(0.0, dot(normA, lightDir));
				float3 diffA = (mainTex.rgb * _LightColor0.rgb * NdotLA) * _Color.rgb;

				float NdotLB = max(0.0, dot(normB, lightDir));
				float3 diffB = (mainTexB.rgb * _LightColor0.rgb * NdotLB) * _Color.rgb;

				float blendMask = tex2D(_MaskMap, i.maskUV).w;
				float3 finalCol = lerp(diffB, diffA, blendMask) * atten;

				fixed4 finalColor = fixed4(finalCol, 0.0);
				UNITY_APPLY_FOG(i.fogCoord, finalColor);

				return finalColor;
			}
			ENDCG
		}
	}
		Fallback "StarWarfare/Scene/Blend_Detail_Normal_Lightmap_LOD100"
}