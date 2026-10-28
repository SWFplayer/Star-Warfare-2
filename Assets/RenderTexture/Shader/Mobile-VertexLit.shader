Shader "Mobile/VertexLit"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader
	{
		LOD 80
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#include "UnityCG.cginc"
		#pragma surface surf Lambert
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	}
		Fallback "Diffuse"
}