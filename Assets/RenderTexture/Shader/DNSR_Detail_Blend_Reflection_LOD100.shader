Shader "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap_LOD100" {
	Properties{
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_MainTexB("Diffuse (RGB) Specular (A) blend", 2D) = "white" {}
		_MaskMap("Blend Mask map", 2D) = "white" {}

		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_GlossPower("Gloss", Float) = 1
		_UVDetailNormalTile("UV Detail Normal Tile", Float) = 1
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_LightPowerOfB("Light Power of B map", Float) = 1
		_BlendRate("Blend Rate For Test", Range(0,1)) = 0
		_SpecTex("Specular Tex", 2D) = "Black" {}
		_SpecTexB("Specular Tex B", 2D) = "Black" {}
	}

		SubShader{
			LOD 100
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf Lambert noforwardadd nolightmap nodynlightmap nodirlightmap
			#pragma target 2.0

			sampler2D _MainTex;
			sampler2D _MainTexB;
			sampler2D _MaskMap;
			fixed4 _Color;
			half _LightMap;

			struct Input {
				float2 uv_MainTex;
				float2 uv_MainTexB;
				float4 color : COLOR;       
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed3 mainTex = tex2D(_MainTex, IN.uv_MainTex).rgb;
				fixed3 mainTexB = tex2D(_MainTexB, IN.uv_MainTexB).rgb;

				float mask = tex2D(_MaskMap, IN.color.xy).w;

				o.Albedo = lerp(mainTexB, mainTex, mask) * _Color.rgb;
			}
			ENDCG
	}

		Fallback "Diffuse"
}