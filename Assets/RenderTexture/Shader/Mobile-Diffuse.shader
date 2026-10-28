Shader "Mobile/Diffuse"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader
	{
		LOD 150
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
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
		Fallback "Mobile/VertexLit"
}