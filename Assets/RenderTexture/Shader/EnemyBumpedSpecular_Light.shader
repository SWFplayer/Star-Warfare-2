Shader "StarWarfare/Enemy/Bumped_Specular_Light"
{
	Properties
	{
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Range(0.03,1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_SpecMulti("Specular Multi", Float) = 1
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
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
			sampler2D _lightTex;
			half _Shininess;
			half _SpecMulti;
			half _lightRate;

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 lightTex = tex2D(_lightTex, IN.uv_MainTex);
				fixed3 normalTex = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

				fixed3 finalAlbedo = mainTex.rgb + (lightTex.rgb * _lightRate);

				o.Albedo = finalAlbedo;
				o.Normal = normalTex;

				o.Gloss = mainTex.a * _SpecMulti;

				o.Specular = _Shininess;

				o.Alpha = 1.0;
			}
			ENDCG
		}
			Fallback "Mobile/Diffuse"
}