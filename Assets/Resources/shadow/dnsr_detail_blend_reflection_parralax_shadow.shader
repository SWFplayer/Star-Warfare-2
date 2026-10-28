Shader "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap_Parallax_Shadow"
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
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_MainTexB("Diffuse (RGB) Specular (A) blend", 2D) = "white" {}
		_LightPowerOfB("Light Power of B map", Float) = 1
		_BlendRate("Blend Rate For Test", Range(0,1)) = 0
		_MaskMap("Blend Mask map", 2D) = "white" {}
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_SpecTexB("Specular Tex B", 2D) = "Black" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_ParallaxMap("Heightmap (A)", 2D) = "black" {}
		_Parallax("Height", Range(0.005,0.08)) = 0.02
		_ShadowMap("Shadow Map", 2D) = "black" {}
		_ShadowDarkness("Shadow Darkness", Range(0,1)) = 1
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
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
			sampler2D _SpecTex;
			sampler2D _SpecTexB;
			sampler2D _BumpMap;
			sampler2D _ParallaxMap;
			sampler2D _MaskMap;
			sampler2D _ShadowMap;
			float4 _Color;
			float4 _SpecColor;
			float4 _ShadowColor;
			float4 _LightDir;
			float _SpecularPower;
			float _Shininess;
			float _GlossPower;
			float _LightMap;
			float _Attenuation;
			float _ShadowDarkness;
			float _BlendRate;
			float _UVDetailNormalTile;
			float _LightPowerOfB;
			float _Parallax;

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
				float2 uv          : TEXCOORD0;
				float4 shadowCoord : TEXCOORD1;
				float2 uvB         : TEXCOORD2;
				float2 uvLM        : TEXCOORD3;
				float2 uvMask      : TEXCOORD4;
				float3 tangentView : TEXCOORD5;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

				o.uvB = v.uv * _MainTexB_ST.xy + _MainTexB_ST.zw;

				o.uvLM = v.uvLM * unity_LightmapST.xy + unity_LightmapST.zw;

				o.uvMask = v.color.xy;

				o.shadowCoord = mul(_LocalToShadowMatrix, v.vertex);

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
				return tex2D(_ShadowMap, uv).x * isVisible;
			}

			float4 frag(v2f i) : SV_Target
			{
				float h = tex2D(_ParallaxMap, i.uv).x;
				float3 v = normalize(i.tangentView);
				v.z += 0.42;
				float2 offset = ((h * _Parallax) - (_Parallax / 2.0)) * (v.xy / v.z);
				float2 parallaxUV = i.uv + offset;

				float3 diffA = tex2D(_MainTex, parallaxUV).xyz;
				float  specA = tex2D(_SpecTex, parallaxUV).x;
				float3 normalA = (tex2D(_BumpMap, parallaxUV).xyz * 2.0) - 1.0;

				float3 diffB = tex2D(_MainTexB, i.uvB).xyz;
				float4 specTexB = tex2D(_SpecTexB, parallaxUV);

				float3 shadow3 = float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

				float3 resultA;
				#ifdef LIGHTMAP_ON
					float4 lm4 = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM);
					float3 lm = (2.0 * lm4.xyz) * _LightMap;
					float3 shadowLM = min(lm, shadow3 * 0.8);

					float NdotL = max(0.0, (dot(normalA, _LightDir.xyz) * 0.5) + 1.0);
					float3 diff = (diffA * shadowLM * NdotL) * _Color.xyz;
					float3 spec = (specA * shadowLM) * _SpecularPower * _SpecColor.xyz;
					resultA = diff + spec;
				#else
					float NdotL = max(0.0, dot(normalA, _LightDir.xyz));
					float3 mixed = lerp(shadow3 * NdotL, ambient + 1.0, ambient);

					float3 diff = diffA * mixed * _Color.xyz;
					float3 spec = (specA * shadow3) * _SpecularPower * _SpecColor.xyz;
					resultA = diff + spec;
				#endif

					float shadowVis = SampleProjectedShadow(i.shadowCoord);
					float shadowW = shadowVis * _ShadowDarkness;
					float3 finalA = resultA * (1.0 - shadowW) + _ShadowColor.xyz * shadowW;

					float3 resultB;
					#ifdef LIGHTMAP_ON
						float3 shadowLM_B = min((2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM).xyz) * _LightMap, shadow3 * 0.8);
						float3 diffB_final = (diffB * shadowLM_B * _LightPowerOfB) * _Color.xyz;
						float3 specB_final = (specTexB.w * shadowLM_B) * _SpecularPower * _SpecColor.xyz;
						resultB = diffB_final + specB_final;
					#else
					float3 mixedB = lerp(shadow3 * _LightPowerOfB, ambient + 1.0, ambient);
					float3 diffB_final = diffB * mixedB * _Color.xyz;
					float3 specB_final = (specTexB.w * shadow3) * _SpecularPower * _SpecColor.xyz;
					resultB = diffB_final + specB_final;
				#endif

					float mask = tex2D(_MaskMap, i.uvMask).w;
					float3 outColor = lerp(resultB, finalA, mask);

					return float4(outColor, 0.0);
				}
				ENDCG
			}
		}
}