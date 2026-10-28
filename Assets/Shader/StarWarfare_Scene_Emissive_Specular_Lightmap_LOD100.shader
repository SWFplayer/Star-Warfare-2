Shader "StarWarfare/Scene/Emissive_Specular_Lightmap_LOD100" {
	Properties{
		_LightMap("Brightness Light Map ", Float) = 0.4
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_Emissive("Emissive", 2D) = "black" {}

		// Unused properties kept for material compatibility
		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_GlossPower("Gloss", Float) = 1
		_LightPower("lightPower", Float) = 1
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
	}

		SubShader{
			LOD 100
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf Lambert noforwardadd nolightmap nodynlightmap nodirlightmap
			#pragma target 2.0

			sampler2D _MainTex;
			sampler2D _Emissive;
			half _LightMap;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				// 1. Sample Main Texture
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);

				// 2. Albedo is the main texture (no _Color tint in this LOD 100 variant!)
				o.Albedo = mainTex.rgb;

				// 3. Emissive is added at the end
				o.Emission = tex2D(_Emissive, IN.uv_MainTex).rgb;

				o.Alpha = 1.0;
			}
			ENDCG
	}

		Fallback "Diffuse"
}