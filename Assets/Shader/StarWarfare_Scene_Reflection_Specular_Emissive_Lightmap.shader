Shader "StarWarfare/Scene/Reflection_Specular_Emissive_Lightmap" {
	Properties{
		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_Emissive("Emissive", 2D) = "black" {}
		_ReflectionCube("Reflection Cubemap", CUBE) = "_Skybox" {}
		_ReflectionAlpha("Reflection Alpha", 2D) = "black" {}
		_ReflectionPower("Reflection Power", Range(0,1)) = 0
		_EyeDir("EyeDirection", Vector) = (0,0,0,0)
	}

		SubShader{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf CustomReflectionSpecular vertex:vert fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _SpecTex;
			sampler2D _Emissive;
			sampler2D _ReflectionAlpha;
			samplerCUBE _ReflectionCube;

			fixed4 _Color;

			half _SpecularPower;
			half _Attenuation;
			half _ReflectionPower;

			// Custom SurfaceOutput to carry reflection data to the lighting function
			struct SurfaceOutputRefl {
				fixed3 Albedo;
				fixed3 Normal;
				fixed3 Emission;
				half Specular;
				fixed Gloss;
				fixed Alpha;
				fixed3 Reflection;   // The cubemap reflection color
				half ReflectionMask; // The reflection blend mask (alpha * power)
			};

			struct Input {
				float2 uv_MainTex;
				float3 reflectionDir;
			};

			void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);

				// Transform to world space
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);

				// The GLSL computes the reflection in object space, then transforms to world
				// Let's replicate the exact math:
				float4 viewPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				float3 eyeDirObj = (viewPos.xyz * 1.0) - v.vertex.xyz;
				float3 eyeDirObjN = normalize(eyeDirObj);
				float3 reflObj = eyeDirObjN - (2.0 * dot(v.normal, eyeDirObjN) * v.normal);

				// Transform to world space
				float3 reflWorld = mul((float3x3)unity_ObjectToWorld, reflObj);

				// The GLSL flips X and Y of the reflection vector
				o.reflectionDir = float3(-reflWorld.x, -reflWorld.y, reflWorld.z);
			}

			void surf(Input IN, inout SurfaceOutputRefl o) {
				// Sample textures
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 specTex = tex2D(_SpecTex, IN.uv_MainTex);
				fixed4 reflectAlpha = tex2D(_ReflectionAlpha, IN.uv_MainTex);
				fixed4 emissive = tex2D(_Emissive, IN.uv_MainTex);
				fixed3 reflection = texCUBE(_ReflectionCube, IN.reflectionDir).rgb;

				// Albedo and specular
				o.Albedo = mainTex.rgb;
				o.Specular = specTex.r;
				o.Gloss = 1.0;

				// Carry reflection data to the lighting function
				o.Reflection = reflection;
				o.ReflectionMask = reflectAlpha.r * _ReflectionPower;

				// Emissive is added at the very end (after the reflection mix)
				o.Emission = emissive.rgb;

				o.Alpha = 1.0;
			}

			// Custom Lighting Function with the exact GLSL logic
			inline fixed4 LightingCustomReflectionSpecular(SurfaceOutputRefl s, fixed3 lightDir, half3 viewDir, fixed atten) {
				// The GLSL uses a hardcoded light direction (1,1,1)
				half3 customLightDir = normalize(float3(1.0, 1.0, 1.0));
				half3 shadow = _Attenuation * 2.0;
				half3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				// Diffuse
				half NdotL = max(0.0, dot(s.Normal, customLightDir));
				half3 diffLight = lerp(shadow * NdotL, ambient + 1.0, ambient);
				half3 diffuse = s.Albedo * diffLight * _Color.rgb;

				// Specular
				half3 specular = (s.Specular * shadow * _SpecularPower) * _SpecColor.rgb;

				// Lit color (before reflection)
				half3 litColor = diffuse + specular;

				// Reflection mix (replaces lit color based on mask)
				half3 finalColor = lerp(litColor, s.Reflection, s.ReflectionMask);

				// Emissive is added at the end
				finalColor += s.Emission;

				// Apply attenuation
				fixed4 c;
				c.rgb = finalColor * atten;
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}

			Fallback "Diffuse"
}