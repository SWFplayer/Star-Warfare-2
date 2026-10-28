Shader "StarWarfare/Scene/Emissive_Reflection_Lightmap_Combine2" {
	Properties{
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
	}

		SubShader{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf CustomEmissiveReflectionCombine2 vertex:vert fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _SpecTex;
			sampler2D _Emissive;
			sampler2D _2DReflect;

			fixed4 _Color;

			half _SpecularPower;
			half _Attenuation;
			half _LightPower;
			half _ReflectionPower;
			float _Param1;
			float _Param2;

			struct Input {
				float2 uv_MainTex;
				float2 reflectionUV;
			};

			void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 reflectionPos = ((-_WorldSpaceCameraPos * _Param1) + worldPos) * _Param2;
				o.reflectionUV = reflectionPos.xy;
			}

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 reflectTex = tex2D(_2DReflect, IN.reflectionUV);
				fixed4 specTex = tex2D(_SpecTex, IN.uv_MainTex);

				// Combine2: Reflection alpha is now the Y (Green) channel of _SpecTex
				fixed3 reflection = (reflectTex.rgb * _ReflectionPower) * specTex.g;

				o.Albedo = mainTex.rgb + reflection;
				o.Albedo *= _Color.rgb;

				// Specular uses the X (Red) channel of _SpecTex
				o.Specular = specTex.r;

				// Emissive is added at the end
				o.Emission = tex2D(_Emissive, IN.uv_MainTex).rgb;
				o.Alpha = 1.0;
			}

			inline fixed4 LightingCustomEmissiveReflectionCombine2(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				half3 shadow = _Attenuation * 2.0;
				half3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				half NdotL = _LightPower;

				half3 diffLight = lerp(shadow * NdotL, ambient + 1.0, ambient);
				half3 diffuse = s.Albedo * diffLight;
				half3 specular = (s.Specular * shadow * _SpecularPower) * _SpecColor.rgb;

				fixed4 c;
				c.rgb = diffuse + specular;
				c.rgb += s.Emission;
				c.rgb *= atten;
				c.a = 1.0;
				return c;
			}
			ENDCG
		}

			Fallback "StarWarfare/Scene/Emissive_Reflection_Lightmap_LOD100"
}