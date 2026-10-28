Shader "StarWarfare/Scene/Emissive_Detail_Normal_Lightmap" {
	Properties{
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
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_Emissive("Emissive", 2D) = "black" {}

		// Unused property kept for compatibility
		_EyeDir("EyeDirection", Vector) = (0,0,0,0)
	}

		SubShader{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf CustomEmissiveLight fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _NormalTex;
			sampler2D _DetailNormal;
			sampler2D _SpecTex;
			sampler2D _Emissive;

			fixed4 _Color;

			half _SpecularPower;
			half _UVDetailNormalTile;
			half _DetailNormalMulti;
			half _Attenuation;
			float3 _LightDir;

			struct Input {
				float2 uv_MainTex;
				float2 uv_NormalTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				// 1. Sample Textures
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed3 normalTex = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));
				fixed3 detailNormalTex = UnpackNormal(tex2D(_DetailNormal, IN.uv_MainTex * _UVDetailNormalTile));

				// 2. Blend Normals
				normalTex.xy += detailNormalTex.xy * _DetailNormalMulti;

				// 3. Setup Surface Output
				o.Albedo = mainTex.rgb * _Color.rgb;
				o.Normal = normalize(normalTex);

				// The specular comes from a separate _SpecTex (only the Red channel is used)
				o.Specular = tex2D(_SpecTex, IN.uv_MainTex).r;

				// The emissive is added in the lighting function
				o.Emission = tex2D(_Emissive, IN.uv_MainTex).rgb;

				o.Alpha = 1.0;
			}

			// Custom Lighting Function
			inline fixed4 LightingCustomEmissiveLight(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				// Setup
				half3 customLightDir = normalize(_LightDir);
				half3 shadow = _Attenuation * 2.0;
				half3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				// Diffuse Calculation
				half NdotL = max(0.0, dot(s.Normal, customLightDir));
				half3 diffLight = lerp(shadow * NdotL, ambient + 1.0, ambient);
				half3 diffuse = s.Albedo * diffLight;

				// Specular Calculation
				half3 specular = (s.Specular * shadow * _SpecularPower) * _SpecColor.rgb;

				fixed4 c;
				c.rgb = diffuse + specular;

				// Add Emission
				c.rgb += s.Emission;

				// Apply standard Unity attenuation (shadows)
				c.rgb *= atten;

				c.a = 1.0;
				return c;
			}
			ENDCG
		}

			Fallback "Diffuse"
}