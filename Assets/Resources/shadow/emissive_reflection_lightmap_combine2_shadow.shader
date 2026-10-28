// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "StarWarfare/Scene/Emissive_Reflection_Lightmap_Combine2_Shadow"
{
	Properties
	{
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
		_ReflectionPower("Reflection Power", Float) = 0
		_2DReflect("2D Reflection", 2D) = "black" {}
		_Param1("reflection Parameter 1", Float) = 0.6
		_Param2("reflection Parameter 2", Float) = 0.07
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

			// Match the original variant keywords
			#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_ON
			#pragma multi_compile SHADOWS_OFF SHADOWS_SCREEN SHADOWS_NATIVE
			#pragma multi_compile VERTEXLIGHT_OFF VERTEXLIGHT_ON

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _SpecTex;
			sampler2D _Emissive;
			sampler2D _2DReflect;
			sampler2D _ShadowMap;
			// sampler2D unity_Lightmap;
			// float4 unity_LightmapST;

			float4 _Color;
			float4 _SpecColor;
			float4 _ShadowColor;
			float _LightPower;
			float _Attenuation;
			float _SpecularPower;
			float _Shininess;
			float _GlossPower;
			float _LightMap;
			float _ShadowDarkness;
			float _ReflectionPower;
			float _Param1;
			float _Param2;

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
				float2 uvReflect   : TEXCOORD2;
				float4 shadowCoord : TEXCOORD3;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				// Original: uv = uv * _MainTex_ST.xy + _MainTex_ST.zw
				o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;

				// Lightmap UV
				o.uvLM = v.uvLM * unity_LightmapST.xy + unity_LightmapST.zw;

				// Projected shadow coordinate
				o.shadowCoord = mul(_LocalToShadowMatrix, v.vertex);

				// Fake 2D Screen-Space Reflection
				float3 worldSpace = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 reflectUV = (worldSpace - (_WorldSpaceCameraPos * _Param1)) * _Param2;
				o.uvReflect = reflectUV.xy;

				return o;
			}

			// Projected shadow: returns the sampled darkness weight (0..1)
			float SampleProjectedShadow(float4 shadowCoord)
			{
				float2 uv = (shadowCoord.xy / shadowCoord.w) * 0.5 + 0.5;

				// Box mask: isVisible = max(sign(0.5 - max(|uv-0.5|)), 0)
				float2 d = abs(uv - 0.5);
				float isVisible = max(sign(0.5 - max(d.x, d.y)), 0.0);

				float4 sm = tex2D(_ShadowMap, uv);
				return sm.x * isVisible;
			}

			float4 frag(v2f i) : SV_Target
			{
				// ----- Textures -----
				float4 mainTex = tex2D(_MainTex, i.uv);
				float4 specTex = tex2D(_SpecTex, i.uv);
				float3 reflectTex = tex2D(_2DReflect, i.uvReflect).xyz;
				float3 emissiveTex = tex2D(_Emissive, i.uv).xyz;

				// ----- Combine Diffuse with Reflection -----
				// Original: (mainTex.xyz + (reflectTex.xyz * _ReflectionPower) * specTex.y)
				float3 baseDiffuse = mainTex.xyz + (reflectTex * _ReflectionPower * specTex.y);

				// ----- Shadow (projected) -----
				float shadowVis = SampleProjectedShadow(i.shadowCoord);

				// ----- Lighting -----
				float3 resultColor;

				#ifdef LIGHTMAP_ON
				// Lightmap path
				float4 lm4 = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLM);
				float3 lm = (2.0 * lm4.xyz) * _LightMap;
				float3 shadow3 = min(lm, float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0) * 0.8);

				float3 diff = baseDiffuse * shadow3 * _LightPower * _Color.xyz;
				float3 spec = (specTex.x * shadow3) * _SpecularPower * _SpecColor.xyz;
				resultColor = diff + spec;
			#else
				// Ambient / non-lightmap path
				float3 shadow3 = float3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz; // glstate_lightmodel_ambient
				float3 mixed = lerp(shadow3 * _LightPower, ambient + 1.0, ambient);

				float3 diff = baseDiffuse * mixed * _Color.xyz;
				float3 spec = (specTex.x * shadow3) * _SpecularPower * _SpecColor.xyz;
				resultColor = diff + spec;
			#endif

				// ----- Apply projected shadow as a blend toward _ShadowColor -----
				float shadowW = shadowVis * _ShadowDarkness;
				float3 outColor = resultColor * (1.0 - shadowW) + _ShadowColor.xyz * shadowW;

				// ----- Add Emissive -----
				outColor += emissiveTex;

				return float4(outColor, 0.0);
			}
			ENDCG
		}
		}

			Fallback "StarWarfare/Scene/Emissive_Reflection_Lightmap_LOD100"
}