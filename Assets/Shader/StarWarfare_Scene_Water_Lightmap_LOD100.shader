Shader "StarWarfare/Scene/Water_Lightmap_LOD100"
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
			LOD 100
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
				sampler2D _Emissive;

				#ifdef LIGHTMAP_ON
				// sampler2D unity_Lightmap;
				// float4 unity_LightmapST;
				float _LightMap;
				#endif

				float _alpha;

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
					#ifdef LIGHTMAP_ON
					float2 lmUV : TEXCOORD1;
					#endif
					UNITY_FOG_COORDS(2)
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);

					#ifdef LIGHTMAP_ON
					o.lmUV = v.uv2.xy * unity_LightmapST.xy + unity_LightmapST.zw;
					#endif

					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// 1. Sample Main Texture
					fixed3 color = tex2D(_MainTex, i.uv).rgb;

				// 2. Apply Lightmap (if enabled)
				#ifdef LIGHTMAP_ON
					fixed4 lmTex = UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmUV);
					fixed3 lm = (2.0 * lmTex.rgb) * _LightMap;
					color *= lm;
				#endif

					// 3. Add Emissive
					color += tex2D(_Emissive, i.uv).rgb;

					// 4. Output with constant alpha
					fixed4 result = fixed4(color, _alpha);

					UNITY_APPLY_FOG(i.fogCoord, result);
					return result;
				}
				ENDCG
			}
		}
			Fallback "Diffuse"
}