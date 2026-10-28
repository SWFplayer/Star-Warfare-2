Shader "StarWarfare/Scene/Emissive_Detail_Normal_Lightmap_Shadow"
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
		_EyeDir("EyeDirection", Vector) = (0,0,0,0)
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_Emissive("Emissive", 2D) = "black" {}
		_ShadowMap("Shadow Map", 2D) = "black" {}
		_ShadowDarkness("Shadow Darkness", Range(0,1)) = 0.5
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
			sampler2D _NormalTex;
			sampler2D _DetailNormal;
			sampler2D _SpecTex;
			sampler2D _Emissive;
			sampler2D _ShadowMap;
			float4 _Color;
			float4 _SpecColor;
			float4 _ShadowColor;
			float4 _LightDir;
			float4 _EyeDir;
			float _Attenuation;
			float _SpecularPower;
			float _Shininess;
			float _GlossPower;
			float _LightMap;
			float _ShadowDarkness;
			float _UVDetailNormalTile;
			float _DetailNormalMulti;

			float4x4 _LocalToShadowMatrix;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
				float2 uvLM   : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos         : SV_POSITION;
				float2 uv          : TEXCOORD0;
				float2 uvLM        : TEXCOORD1;
				float4 shadowCoord : TEXCOORD2;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

				o.uvLM = v.uvLM * unity_LightmapST.xy + unity_LightmapST.zw;

				o.shadowCoord = mul(_LocalToShadowMatrix, v.vertex);

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
				float3 diffTex = tex2D(_MainTex, i.uv).xyz;
				float  specTex = tex2D(_SpecTex, i.uv).x;
				float3 emissiveTex = tex2D(_Emissive, i.uv).xyz;

				float2 detailUV = i.uv * _UVDetailNormalTile;
				float3 normalBase = (tex2D(_NormalTex, i.uv).xyz * 2.0) - 1.0;
				float3 normalDetail = (tex2D(_DetailNormal, detailUV).xyz * 2.0) - 1.0;

				normalBase.xy += normalDetail.xy * _DetailNormalMulti;
				float3 normal = normalize(normalBase);

				float shadowVis = SampleProjectedShadow(i.shadowCoord);

				float3 resultColor;

				#ifdef LIGHTMAP_ON
				float4 lm4 = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM);
				float3 lm = (2.0 * lm4.xyz) * _LightMap;
				float3 shadow3 = min(lm, float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0) * 0.8);

				float NdotL = max(0.0, (dot(normal, _LightDir.xyz) * 0.5) + 1.0);

				float3 diff = (diffTex * shadow3 * NdotL) * _Color.xyz;
				float3 spec = (specTex * shadow3) * _SpecularPower * _SpecColor.xyz;
				resultColor = diff + spec;
			#else
				float3 shadow3 = float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;  

				float NdotL = max(0.0, dot(normal, _LightDir.xyz));

				float3 mixed = lerp(shadow3 * NdotL, ambient + 1.0, ambient);

				float3 diff = diffTex * mixed * _Color.xyz;
				float3 spec = (specTex * shadow3) * _SpecularPower * _SpecColor.xyz;
				resultColor = diff + spec;
			#endif

				float shadowW = shadowVis * _ShadowDarkness;
				float3 outColor = resultColor * (1.0 - shadowW) + _ShadowColor.xyz * shadowW;

				outColor += emissiveTex;

				return float4(outColor, 0.0);
			}
			ENDCG
		}
		}

			Fallback "Diffuse"
}