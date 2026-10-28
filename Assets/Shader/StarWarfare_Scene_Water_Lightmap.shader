Shader "StarWarfare/Scene/Water_Lightmap"
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
		_NormalTex("Normal (RGB)", 2D) = "bump" {}
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_Emissive("Emissive", 2D) = "black" {}
		_ReflectionCube("Reflection Cubemap", CUBE) = "_Skybox" {}
		_ReflectionAlpha("Reflection Alpha", 2D) = "black" {}
		_ReflectionPower("Reflection Power", Float) = 0
		_OriginPower("Origin Power", Float) = 1
		_alpha("Alpha", Range(0,1)) = 1
		_WaveScale("Wave scale", Range(0.02,0.15)) = 0.07
		WaveSpeed("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	}

		SubShader
		{
			LOD 400
			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				Name "CONTENTBASE"
				Tags { "LightMode" = "Always" }

				CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_fog
				#pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _NormalTex;
				sampler2D _SpecTex;
				sampler2D _Emissive;
				sampler2D _ReflectionAlpha;
				samplerCUBE _ReflectionCube;

				#ifdef LIGHTMAP_ON
				// sampler2D unity_Lightmap;
				// float4 unity_LightmapST;
				float _LightMap;
				#endif

				fixed4 _Color;
				fixed4 _SpecColor;
				float _SpecularPower;
				float _Attenuation;
				float _ReflectionPower;
				float _OriginPower;
				float _alpha;
				float3 _LightDir;

				float _WaveScale;
				float4 WaveSpeed;

				// Unity built-in sets _WaveOffset based on WaveSpeed and Time
				float4 _WaveOffset;

				struct appdata
				{
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
					#ifdef LIGHTMAP_ON
					float2 uv2 : TEXCOORD1;
					#endif
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float4 waveUVs : TEXCOORD1; // xy = wave1 UV, zw = wave2 UV
					float3 reflectVec : TEXCOORD2;
					#ifdef LIGHTMAP_ON
					float2 lmUV : TEXCOORD3;
					#endif
					UNITY_FOG_COORDS(4)
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					// --- Wave Normal UVs ---
					// The original shader uses _glesVertex.xy (object space position) 
					// multiplied by wave scale, plus a script-set _WaveOffset.
					o.waveUVs = (v.vertex.xyxy * _WaveScale) + _WaveOffset;

					// --- Fake Reflection Vector ---
					// Compute view direction, reflect it against the normal, 
					// transform to world space, and flip X/Y.
					float3 worldNormal = UnityObjectToWorldNormal(v.normal);
					float3 viewDir = WorldSpaceViewDir(v.vertex);
					float3 reflectVec = reflect(-viewDir, worldNormal);

					// Transform reflection vector to world space
					reflectVec = mul((float3x3)unity_ObjectToWorld, reflectVec);

					// Flip X/Y to match the decompiled shader's specific reflection mapping
					reflectVec.xy = -reflectVec.xy;

					o.reflectVec = reflectVec;

					#ifdef LIGHTMAP_ON
					o.lmUV = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// --- 1. Sample Textures ---
					fixed4 mainTex = tex2D(_MainTex, i.uv);
					fixed4 emissiveTex = tex2D(_Emissive, i.uv);
					fixed specMask = tex2D(_SpecTex, i.uv).x;
					fixed reflectAlpha = tex2D(_ReflectionAlpha, i.uv).x;

					// --- 2. Dual-Layer Normal Mapping ---
					fixed3 bump1 = UnpackNormal(tex2D(_NormalTex, i.waveUVs.xy));
					fixed3 bump2 = UnpackNormal(tex2D(_NormalTex, i.waveUVs.zw));

					// Average the two normals (as per original shader)
					fixed3 normal = normalize((bump1 + bump2) * 0.5);

					// --- 3. Lighting Math ---
					fixed3 shadow = fixed3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);

					#ifdef LIGHTMAP_ON
					// Custom Lightmap blending
					fixed4 lmTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmUV);
					fixed3 lm = (2.0 * lmTex.xyz) * _LightMap;
					shadow = min(lm, shadow * 0.8);

					// Half-Lambert
					fixed NdotL = max(0.0, dot(normal, _LightDir) * 0.5 + 1.0);
				#else
					// Standard Lambert
					fixed NdotL = max(0.0, dot(normal, _LightDir));
				#endif

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 finalLight = lerp(shadow * NdotL, ambient + 1.0, ambient);

				// Diffuse
				fixed3 diffuse = (mainTex.rgb * finalLight) * _Color.rgb;

				// Specular
				fixed3 spec = (specMask * shadow * _SpecularPower) * _SpecColor.rgb;

				fixed3 result = diffuse + spec;

				// --- 4. Cubemap Reflection ---
				// Reflect the already reflected view vector using the wave normal
				float3 refl = reflect(i.reflectVec, normal);
				fixed3 cubeReflect = texCUBE(_ReflectionCube, refl).rgb;

				// Add reflection (scaled by OriginPower)
				result = (result * _OriginPower) + (cubeReflect * reflectAlpha * _ReflectionPower);

				// Add Emissive
				result += emissiveTex.rgb;

				fixed4 finalColor = fixed4(result, _alpha);

				UNITY_APPLY_FOG(i.fogCoord, finalColor);
				return finalColor;
			}
			ENDCG
		}
		}
			Fallback "StarWarfare/Scene/Water_Lightmap_LOD100"
}