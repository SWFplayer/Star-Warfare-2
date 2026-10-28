Shader "StarWarfare/Scene/Blend_Detail_Normal_Lightmap_LOD100" {
	Properties{
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_MainTexB("Diffuse (RGB) Specular (A) blend", 2D) = "white" {}
		_MaskMap("Blend Mask map", 2D) = "white" {}

		// Unused properties kept for material compatibility
		_Shininess("Shininess for Light Map", Range(0.03,1)) = 1
		_SpecColor("Specular Color", Color) = (1,1,1,1)
		_SpecularPower("Specular", Float) = 1
		_GlossPower("Gloss", Float) = 1
		_UVDetailNormalTile("UV Detail Normal Tile", Float) = 1
		_DetailNormalMulti("Detail Normal Multi", Float) = 1
		_NormalTex("Normal (RGB)", 2D) = "bump" {}
		_DetailNormal("Detail Normal map", 2D) = "white" {}
		_LightDir("LightDirection", Vector) = (0,0,0,0)
		_Attenuation("Attenuation", Range(0,1)) = 0.5
		_DetailNormalB("Detail Normal map blend", 2D) = "black" {}
		_UVDetailNormalTileB("UV Detail Normal Tile blend", Float) = 1
		_DetailNormalMultiB("Detail Normal Multi blend", Float) = 1
		_LightDirB("LightDirection of blend", Vector) = (0,0,0,0)
		_BlendRate("Blend Rate For Test", Range(0,1)) = 0
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
				float4 color : COLOR; // Vertex color (used for mask UV)
			};

			void surf(Input IN, inout SurfaceOutput o) {
				// 1. Sample Textures
				fixed3 mainTex = tex2D(_MainTex, IN.uv_MainTex).rgb;
				fixed3 mainTexB = tex2D(_MainTexB, IN.uv_MainTexB).rgb;

				// 2. Blend using the Mask (GLSL uses vertex color XY for mask UV, then reads W)
				float mask = tex2D(_MaskMap, IN.color.xy).w;

				// 3. Final Albedo
				o.Albedo = lerp(mainTexB, mainTex, mask) * _Color.rgb;
			}
			ENDCG
		}

			Fallback "Mobile/Diffuse"
}