Shader "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap_Combine3" {
	Properties{
		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_GlossPower("Gloss", Float) = 1
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_MainTexB("Diffuse (RGB) Specular (A) blend", 2D) = "white" {}
		_LightPowerOfB("Light Power of B map", Float) = 1
		_BlendRate("Blend Rate For Test", Range(0,1)) = 0
		_MaskMap("Blend Mask map (RGB)", 2D) = "white" {}

		// Unused properties kept for material compatibility
		_UVDetailNormalTile("UV Detail Normal Tile", Float) = 1
	}

		SubShader{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf CustomBlendLightNoReflection fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _MainTexB;
			sampler2D _MaskMap;

			fixed4 _Color;

			half _SpecularPower;
			half _Attenuation;
			half _LightPowerOfB;
			float3 _LightDir;

			struct Input {
				float2 uv_MainTex;
				float2 uv_MainTexB;
				float4 color : COLOR; // Vertex color (used for mask UV)
			};

			void surf(Input IN, inout SurfaceOutput o) {
				// Sample main textures and mask
				fixed4 mainTexA = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 mainTexB = tex2D(_MainTexB, IN.uv_MainTexB);
				fixed4 mask = tex2D(_MaskMap, IN.uv_MainTex);

				// Setup Surface Output
				o.Albedo = mainTexA.rgb * _Color.rgb;
				o.Specular = mask.y;
				o.Gloss = 1.0;
				o.Alpha = 1.0;
			}

			// Custom Lighting Function
			inline fixed4 LightingCustomBlendLightNoReflection(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				// Setup
				// The GLSL uses a fixed vector for lighting direction (0.57735, 0.57735, 0.57735)
				// which is the normalized (1,1,1) direction.
				half3 customLightDir = normalize(float3(0.57735, 0.57735, 0.57735));
				half3 shadow = _Attenuation * 2.0;
				half3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				// Calculate Set A
				half NdotL = max(0.0, dot(s.Normal, customLightDir));
				half3 diffLight = lerp(shadow * NdotL, ambient + 1.0, ambient);
				half3 diffuseA = s.Albedo * diffLight;
				half3 specularA = (s.Specular * shadow * _SpecularPower) * _SpecColor.rgb;
				half3 colorA = diffuseA + specularA;

				// Calculate Set B
				// The GLSL calculates Set B using the same ambient but different lighting for the main diffuse
				// It uses _LightPowerOfB to scale the lighting
				half3 diffuseB = (tex2D(_MainTexB, s.Albedo.xy).rgb * shadow * _LightPowerOfB) * _Color.rgb;
				half3 specularB = (tex2D(_MaskMap, s.Albedo.xy).z * shadow * _SpecularPower) * _SpecColor.rgb;
				half3 colorB = diffuseB + specularB;

				// Blend based on the MaskMap red channel
				// We sample the mask using the vertex color XY as UVs
				half maskVal = tex2D(_MaskMap, s.Albedo.xy).x;

				fixed4 c;
				c.rgb = lerp(colorB, colorA, maskVal);
				c.rgb *= atten;
				c.a = 1.0;
				return c;
			}
			ENDCG
		}

			Fallback "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap_Combine3_LOD100"
}