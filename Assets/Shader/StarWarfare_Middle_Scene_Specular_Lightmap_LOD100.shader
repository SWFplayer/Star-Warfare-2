Shader "StarWarfare/Middle/Scene/Specular_Lightmap_LOD100"
{
	Properties
	{
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_LightPower("lightPower", Float) = 1
		_Attenuation("Attenuation", Range(0,1)) = 0.5
	}

		SubShader
		{
			LOD 100
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
				fixed4 _Color;

				#ifdef LIGHTMAP_ON
				// sampler2D unity_Lightmap;
				// float4 unity_LightmapST;
				float _LightMap;
				#endif

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
					fixed4 col = tex2D(_MainTex, i.uv) * _Color;

					#ifdef LIGHTMAP_ON
					fixed3 lm = (2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lmUV).rgb) * _LightMap;
					col.rgb *= lm;
					#endif

					col.a = 0.0;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
			Fallback "Mobile/Diffuse"
}