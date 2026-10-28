Shader "StarWarfare/Enemy/Bumped_Specular"
{
	Properties
	{
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Range(0.03,1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_SpecMulti("Specular Multi", Float) = 1
	}

		SubShader
		{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma surface surf BlinnPhong
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _BumpMap;
			half _Shininess;
			half _SpecMulti;

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				// 1. Sample Textures
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed3 normalTex = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

				// 2. Set Surface Outputs
				o.Albedo = mainTex.rgb;
				o.Normal = normalTex;

				// The Alpha channel of the main texture acts as the Gloss mask, scaled by _SpecMulti
				o.Gloss = mainTex.a * _SpecMulti;

				// Shininess property
				o.Specular = _Shininess;

				// The original shader used "Blend SrcAlpha OneMinusSrcAlpha" and wrote alpha
				o.Alpha = 1.0;
			}
			ENDCG
		}
			Fallback "Mobile/VertexLit"
}