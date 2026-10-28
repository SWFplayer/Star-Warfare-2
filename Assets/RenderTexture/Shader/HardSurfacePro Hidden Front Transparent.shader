Shader "Hidden/Hardsurface Pro Front Transparent"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Float) = 0.078125
		_Gloss("Gloss", Float) = 0
		_Metalics("Metalics", Float) = 0
		_Reflection("Reflection", Float) = 0
		_EdgeAlpha("Edge Alpha", Float) = 0
		_FrezPow("Fresnel Power", Float) = 1
		_FrezFalloff("Fresnel Falloff", Float) = 4
		_Cube("Cube", Cube) = "white" {}
	}

		SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			sampler2D _MainTex;          
			samplerCUBE _Cube;

			fixed4 _Color;
			
			float _Shininess;
			float _Gloss;
			float _Metalics;
			float _Reflection;
			float _EdgeAlpha;
			float _FrezPow;
			float _FrezFalloff;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 normalDir : TEXCOORD0;
				float3 tangentDir : TEXCOORD1;
				float3 bitangentDir : TEXCOORD2;
				float3 viewDir : TEXCOORD3;
				float3 lightDir : TEXCOORD4;
				float3 shLight : TEXCOORD5;
				float4 projPos : TEXCOORD6;
				UNITY_FOG_COORDS(7)
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
				o.bitangentDir = cross(o.normalDir, o.tangentDir) * v.tangent.w;

				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.lightDir = UnityWorldSpaceLightDir(mul(unity_ObjectToWorld, v.vertex).xyz);

				#ifndef LIGHTMAP_ON
				o.shLight = ShadeSH9(float4(o.normalDir, 1.0));
				#else
				o.shLight = float3(0,0,0);
				#endif

				#ifdef SHADOWS_SCREEN
				o.projPos = ComputeScreenPos(o.pos);
				#endif

				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 viewDir = normalize(i.viewDir);
				float3 normalDir = normalize(i.normalDir);

				float3 fakeNormal = float3(
					dot(i.tangentDir, float3(0,0,1)),
					dot(i.bitangentDir, float3(0,0,1)),
					dot(normalDir, float3(0,0,1))
				);
				fakeNormal = normalize(fakeNormal);

				float3 worldRefl = normalize(reflect(-viewDir, fakeNormal));
				float fresnel = pow(1.0 - abs(dot(worldRefl, fakeNormal)), _FrezFalloff);

				fixed4 col = _Color;
				col.a = _Color.a * (1.0 - (fresnel * _EdgeAlpha));

				float frezPow = fresnel * _FrezPow;
				float reflectionAmount = _Reflection + frezPow;

				fixed3 cubeTex = texCUBE(_Cube, worldRefl).rgb * reflectionAmount;

				float cubeLum = clamp(dot(cubeTex, float3(0.22, 0.707, 0.071)), 0.0, 1.0);
				float gloss = _Gloss + frezPow;
				float alphaBlend = min(col.a + (cubeLum * cubeLum), 1.0);

				fixed3 baseColor = col.rgb * (1.0 - clamp(reflectionAmount, 0.0, 1.0));

				float3 lightDir = normalize(i.lightDir);
				float NdotL = dot(float3(0,0,1), lightDir);    
				NdotL = max(NdotL, abs(NdotL) * (1.0 - alphaBlend));    

				float3 halfDir = normalize(lightDir + viewDir);
				float NdotH = dot(float3(0,0,1), halfDir);
				NdotH = max(0.0, NdotH);

				float spec = pow(NdotH, _Shininess * 128.0) * gloss;

				fixed3 lighting = (baseColor * _LightColor0.rgb * NdotL) + (_LightColor0.rgb * _SpecColor.rgb * spec);
				lighting *= 2.0;     

				fixed4 result;
				result.rgb = lighting + (baseColor * i.shLight);

				result.rgb += lerp(cubeTex, cubeTex * col.rgb, 1.0 - (cubeLum * cubeLum) * _Metalics);
				result.a = alphaBlend;

				UNITY_APPLY_FOG(i.fogCoord, result);
				return result;
			}
			ENDCG
		}
	}
		Fallback "Diffuse"
}