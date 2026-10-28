Shader "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap_Combine3_Shadow"
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
		_MaskMap("Blend Mask map (RGB)", 2D) = "white" {}
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

			float4x4 _LocalToShadowMatrix;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color  : COLOR;
				float2 uv     : TEXCOORD0;
				float2 uvLM   : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos         : SV_POSITION;
				float2 uv          : TEXCOORD0;
				float2 uvB         : TEXCOORD1;
				float2 uvLM        : TEXCOORD2;
				float2 uvMask      : TEXCOORD3;    
				float4 shadowCoord : TEXCOORD4;
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
				float3 diffA = tex2D(_MainTex, i.uv).xyz;
				float3 diffB = tex2D(_MainTexB, i.uvB).xyz;

				float4 maskUV = tex2D(_MaskMap, i.uv);
				float  specA = maskUV.y;       
				float  specB = maskUV.z;       

				float3 shadow3 = float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
				float3 hardcodedNormal = float3(0.57735, 0.57735, 0.57735);

				float3 resultA;
				#ifdef LIGHTMAP_ON
					float4 lm4 = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM);
					float3 lm = (2.0 * lm4.xyz) * _LightMap;
					float3 shadowLM = min(lm, shadow3 * 0.8);

					float NdotLA = max(0.0, (dot(hardcodedNormal, _LightDir.xyz) * 0.5) + 1.0);
					float3 diffALit = (diffA * shadowLM * NdotLA) * _Color.xyz;
					float3 specALit = (specA * shadowLM) * _SpecularPower * _SpecColor.xyz;
					resultA = diffALit + specALit;
				#else
					float NdotLA = max(0.0, dot(hardcodedNormal, _LightDir.xyz));
					float3 mixedA = lerp(shadow3 * NdotLA, ambient + 1.0, ambient);
					float3 diffALit = diffA * mixedA * _Color.xyz;
					float3 specALit = (specA * shadow3) * _SpecularPower * _SpecColor.xyz;
					resultA = diffALit + specALit;
				#endif

					float3 resultB;
					#ifdef LIGHTMAP_ON
						float3 shadowLM_B = min((2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM).xyz) * _LightMap, shadow3 * 0.8);
						float3 diffBLit = (diffB * shadowLM_B * _LightPowerOfB) * _Color.xyz;
						float3 specBLit = (specB * shadowLM_B) * _SpecularPower * _SpecColor.xyz;
						resultB = diffBLit + specBLit;
					#else
						float3 mixedB = lerp(shadow3 * _LightPowerOfB, ambient + 1.0, ambient);
						float3 diffBLit = diffB * mixedB * _Color.xyz;
						float3 specBLit = (specB * shadow3) * _SpecularPower * _SpecColor.xyz;
						resultB = diffBLit + specBLit;
					#endif

						float blendMask = tex2D(_MaskMap, i.uvMask).x;
						float3 outColor = lerp(resultB, resultA, blendMask);

						float shadowVis = SampleProjectedShadow(i.shadowCoord);
						float shadowW = shadowVis * _ShadowDarkness;
						outColor = outColor * (1.0 - shadowW) + _ShadowColor.xyz * shadowW;

						return float4(outColor, 0.0);
			}
				ENDCG
			}
		}

			Fallback "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap_Combine3_LOD100"
}