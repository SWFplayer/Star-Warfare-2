Shader "StarWarfare/Scene/Blend_Detail_Normal_Lightmap_Shadow_Parallax"
{
	Properties
	{
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
		_Parallax("Height", Range(0.005,0.08)) = 0.02
	}

		SubShader
		{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			Pass
			{
				Name "CONTENTBASE"
				Tags { "LIGHTMODE" = "Always" "SHADOWSUPPORT" = "true" "RenderType" = "Opaque" }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0

			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_ON
			#pragma multi_compile SHADOWS_OFF SHADOWS_SCREEN SHADOWS_NATIVE
			#pragma multi_compile VERTEXLIGHT_OFF VERTEXLIGHT_ON

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MainTexB;
			float4 _MainTexB_ST;
			sampler2D _NormalTex;
			sampler2D _DetailNormal;
			sampler2D _DetailNormalB;
			sampler2D _MaskMap;
			sampler2D _ShadowMap;
			float4 _Color;
			float4 _SpecColor;
			float4 _ShadowColor;
			float4 _LightDir;
			float4 _LightDirB;
			float _SpecularPower;
			float _Shininess;
			float _GlossPower;
			float _LightMap;
			float _Attenuation;
			float _ShadowDarkness;
			float _BlendRate;
			float _Parallax;
			float _UVDetailNormalTile;
			float _DetailNormalMulti;
			float _UVDetailNormalTileB;
			float _DetailNormalMultiB;

			float4x4 _LocalToShadowMatrix;

			struct appdata
			{
				float4 vertex  : POSITION;
				float4 color   : COLOR;
				float3 normal  : NORMAL;
				float4 tangent : TANGENT;
				float2 uv      : TEXCOORD0;
				float2 uvLM    : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos         : SV_POSITION;
				float4 uv          : TEXCOORD0;       
				float4 shadowCoord : TEXCOORD1;
				float2 uvLM        : TEXCOORD2;
				float2 uvColor     : TEXCOORD3;      
				float3 tangentView : TEXCOORD4;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv.xy = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.uv * _MainTexB_ST.xy + _MainTexB_ST.zw;

				o.uvLM = v.uvLM * unity_LightmapST.xy + unity_LightmapST.zw;

				o.shadowCoord = mul(_LocalToShadowMatrix, v.vertex);

				o.uvColor = v.color.xy;

				float3 normal = normalize(v.normal);
				float3 tangent = normalize(v.tangent.xyz);
				float3 bitangent = cross(normal, tangent) * v.tangent.w;
				float3x3 tbn = float3x3(tangent, bitangent, normal);

				float3 worldCam = _WorldSpaceCameraPos;
				float3 objCam = mul(unity_WorldToObject, float4(worldCam, 1.0)).xyz;
				float3 objView = objCam - v.vertex.xyz;

				o.tangentView = mul(tbn, objView);

				return o;
			}

			float SampleProjectedShadow(float4 shadowCoord)
			{
				float2 uv = (shadowCoord.xy / shadowCoord.w) * 0.5 + 0.5;

				float2 d = abs(uv - 0.5);
				float isVisible = max(sign(0.5 - max(d.x, d.y)), 0.0);

				float4 sm = tex2D(_ShadowMap, uv);
				return sm.x * isVisible;
			}

			float4 frag(v2f i) : SV_Target
			{
				float h = tex2D(_NormalTex, i.uv.xy).w;
				h = (h - 0.5) * 2.0;
				float3 v = normalize(i.tangentView);
				v.z += 0.42;
				float2 offset = ((h * _Parallax) - (_Parallax / 2.0)) * (v.xy / v.z);

				float4 parallaxUV = i.uv + float4(offset, offset);

				float4 mainTexA = tex2D(_MainTex, parallaxUV.xy);
				float3 diffA = mainTexA.xyz;
				float  specA = mainTexA.w;

				float2 detailUVA = parallaxUV.xy * _UVDetailNormalTile;
				float3 normalA = (tex2D(_NormalTex, parallaxUV.xy).xyz * 2.0) - 1.0;
				float3 detailNA = (tex2D(_DetailNormal, detailUVA).xyz * 2.0) - 1.0;
				normalA.xy += detailNA.xy * _DetailNormalMulti;
				normalA = normalize(normalA);

				float3 shadow3 = float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);

				#ifdef LIGHTMAP_ON
					float4 lm4 = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM);
					float3 lm = (2.0 * lm4.xyz) * _LightMap;
					float3 shadowLM = min(lm, shadow3 * 0.8);
					float NdotL = max(0.0, (dot(normalA, _LightDir.xyz) * 0.5) + 1.0);
					float3 diff = (diffA * shadowLM * NdotL) * _Color.xyz;
					float3 spec = (specA * shadowLM) * _SpecularPower * _SpecColor.xyz;
					float3 resultA = diff + spec;
				#else
					float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
					float NdotL = max(0.0, dot(normalA, _LightDir.xyz));
					float3 mixed = lerp(shadow3 * NdotL, ambient + 1.0, ambient);
					float3 diff = diffA * mixed * _Color.xyz;
					float3 spec = (specA * shadow3) * _SpecularPower * _SpecColor.xyz;
					float3 resultA = diff + spec;
				#endif

					float4 mainTexB = tex2D(_MainTexB, parallaxUV.zw);
					float3 diffB = mainTexB.xyz;
					float  specB = mainTexB.w;

					float2 detailUVB = parallaxUV.zw * _UVDetailNormalTileB;
					float4 detailNB = tex2D(_DetailNormalB, detailUVB);
					float3 normalB;
					normalB.z = (detailNB.z * 2.0) - 1.0;
					normalB.xy = (detailNB.xy * 2.0 - 1.0) * _DetailNormalMultiB;
					normalB = normalize(normalB);

					#ifdef LIGHTMAP_ON
						float NdotLB = max(0.0, (dot(normalB, _LightDirB.xyz) * 0.5) + 1.0);
						float3 diffB_final = (diffB * shadowLM * NdotLB) * _Color.xyz;
						float3 specB_final = (specB * shadowLM) * _SpecularPower * _SpecColor.xyz;
						float3 resultB = diffB_final + specB_final;
					#else
						float NdotLB = max(0.0, dot(normalB, _LightDirB.xyz));
						float3 mixedB = lerp(shadow3 * NdotLB, ambient + 1.0, ambient);
						float3 diffB_final = diffB * mixedB * _Color.xyz;
						float3 specB_final = (specB * shadow3) * _SpecularPower * _SpecColor.xyz;
						float3 resultB = diffB_final + specB_final;
					#endif

						float mask = tex2D(_MaskMap, i.uvColor).w;
						float3 blendedColor = lerp(resultB, resultA, mask);

						float shadowVis = SampleProjectedShadow(i.shadowCoord);
						float shadowW = shadowVis * _ShadowDarkness;
						float3 outColor = blendedColor * (1.0 - shadowW) + _ShadowColor.xyz * shadowW;

						return float4(outColor, 0.0);
					}
					ENDCG
				}
		}
}