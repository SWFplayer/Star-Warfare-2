Shader "StarWarfare/Scene/EmissiveWithTrigger_Reflection_Lightmap"
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
		_EyeDir("EyeDirection", Vector) = (0,0,0,0)
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_Emissive("Emissive", 2D) = "black" {}
		_EmissivePower("Emissvie Power", Float) = 1
		_ReflectionAlpha("Reflection Alpha", 2D) = "black" {}
		_ReflectionPower("Reflection Power", Float) = 0
		_2DReflect("2D Reflection", 2D) = "black" {}
		_Param1("reflection Parameter 1", Float) = 0.6
		_Param2("reflection Parameter 2", Float) = 0.07
	}

		SubShader
		{
			LOD 400
			Tags { "RenderType" = "Opaque" }

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
				sampler2D _2DReflect;

				#ifdef LIGHTMAP_ON
				float _LightMap;
				#endif

				fixed4 _Color;
				fixed4 _SpecColor;
				float _SpecularPower;
				float _Attenuation;
				float _ReflectionPower;
				float _EmissivePower;
				float _Param1;
				float _Param2;
				float3 _LightDir;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					#ifdef LIGHTMAP_ON
					float2 uv2 : TEXCOORD1;
					#endif
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 reflectUV : TEXCOORD1;
					#ifdef LIGHTMAP_ON
					float2 lmUV : TEXCOORD2;
					#endif
					UNITY_FOG_COORDS(3)
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					float3 cameraOffset = (-_WorldSpaceCameraPos * _Param1) + worldPos;
					o.reflectUV = (cameraOffset * _Param2).xy;

					#ifdef LIGHTMAP_ON
					o.lmUV = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 mainTex = tex2D(_MainTex, i.uv);
					fixed4 emissiveTex = tex2D(_Emissive, i.uv);
					fixed4 reflectTex = tex2D(_2DReflect, i.reflectUV);
					fixed4 reflectAlpha = tex2D(_ReflectionAlpha, i.uv);
					fixed specMask = tex2D(_SpecTex, i.uv).x;

					float3 normal = UnpackNormal(tex2D(_NormalTex, i.uv));

					fixed3 shadow = fixed3(_Attenuation * 2.0, _Attenuation * 2.0, _Attenuation * 2.0);

					#ifdef LIGHTMAP_ON
					fixed4 lmTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmUV);
					fixed3 lm = (2.0 * lmTex.xyz) * _LightMap;
					shadow = min(lm, shadow * 0.8);

					fixed NdotL = max(0.0, dot(normal, _LightDir) * 0.5 + 1.0);
				#else
					fixed NdotL = max(0.0, dot(normal, _LightDir));
				#endif

				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				fixed3 finalLight = lerp(shadow * NdotL, ambient + 1.0, ambient);

				fixed3 diffuse = (mainTex.rgb + (reflectTex.rgb * _ReflectionPower * reflectAlpha.x)) * finalLight * _Color.rgb;

				fixed3 spec = (specMask * shadow * _SpecularPower) * _SpecColor.rgb;

				fixed3 result = diffuse + spec;

				result += emissiveTex.rgb * _EmissivePower;

				fixed4 finalColor = fixed4(result, 0.0);

				UNITY_APPLY_FOG(i.fogCoord, finalColor);
				return finalColor;
			}
			ENDCG
		}
		}
}