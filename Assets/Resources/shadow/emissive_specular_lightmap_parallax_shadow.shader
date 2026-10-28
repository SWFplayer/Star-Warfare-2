Shader "StarWarfare/Scene/Emissive_Specular_Lightmap_Parallax_Shadow"
{
	Properties
	{
		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_LightPower("lightPower", Float) = 1
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_Emissive("Emissive", 2D) = "black" {}
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
			sampler2D _SpecTex;
			sampler2D _Emissive;
			sampler2D _BumpMap;
			sampler2D _ParallaxMap;
			sampler2D _ShadowMap;
			float4 _Color;
			float4 _SpecColor;
			float4 _ShadowColor;
			float4 _LightDir;
			float _LightPower;
			float _Attenuation;
			float _SpecularPower;
			float _Shininess;
			float _LightMap;
			float _Parallax;
			float _ShadowDarkness;

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
				float3 viewDir     : TEXCOORD2;
				float4 shadowCoord : TEXCOORD3;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

				o.uvLM = v.uvLM * unity_LightmapST.xy + unity_LightmapST.zw;

				float3 worldCam = _WorldSpaceCameraPos;
				float3 objCam = mul(unity_WorldToObject, float4(worldCam, 1.0)).xyz;
				o.viewDir = normalize(objCam - v.vertex.xyz);

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
				float h = tex2D(_ParallaxMap, i.uv).x;
				float height = _Parallax;
				float3 v = normalize(i.viewDir);
				v.z += 0.42;
				float2 parallaxUV = i.uv + ((h * height) - (height / 2.0)) * (v.xy / v.z);

				float3 diffTex = tex2D(_MainTex, parallaxUV).xyz;
				float  specTex = tex2D(_SpecTex, parallaxUV).x;
				float3 normalTex = (tex2D(_BumpMap, parallaxUV).xyz * 2.0) - 1.0;
				float3 emissiveTex = tex2D(_Emissive, parallaxUV).xyz;

				float shadowVis = SampleProjectedShadow(i.shadowCoord);

				float3 resultColor;

				#ifdef LIGHTMAP_ON
				float4 lm4 = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM);
				float3 lm = (2.0 * lm4.xyz) * _LightMap;
				float3 shadow3 = min(lm, float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0) * 0.8);

				float NdotL = max(0.0, (dot(normalTex, _LightDir.xyz) * 0.5) + 1.0);

				float3 diff = (diffTex * shadow3 * NdotL) * _Color.xyz;
				float3 spec = (specTex * shadow3) * _SpecularPower * _SpecColor.xyz;
				resultColor = diff + spec;
			#else
				float3 shadow3 = float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;  

				float NdotL = max(0.0, dot(normalTex, _LightDir.xyz));

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