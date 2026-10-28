Shader "StarWarfare/Scene/Emissive_Reflection_Lightmap_LOD100" {
	Properties{
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_Emissive("Emissive", 2D) = "black" {}

		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_GlossPower("Gloss", Float) = 1
		_LightPower("lightPower", Float) = 1
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_ReflectionPower("Reflection Power", Float) = 0
		_2DReflect("2D Reflection", 2D) = "black" {}
		_Param1("reflection Parameter 1", Float) = 0.6
		_Param2("reflection Parameter 2", Float) = 0.07
	}

		SubShader{
			LOD 100
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf Lambert noforwardadd nolightmap nodynlightmap nodirlightmap
			#pragma target 2.0

			sampler2D _MainTex;
			sampler2D _Emissive;
			fixed4 _Color;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);

				o.Albedo = mainTex.rgb * _Color.rgb;

				o.Emission = tex2D(_Emissive, IN.uv_MainTex).rgb;

				o.Alpha = 1.0;
			}
			ENDCG
		}

			Fallback "Diffuse"
}