Shader "Hidden/LensFlareCreate"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off
			Fog { Mode Off }
			Blend One One

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 colorA;
			float4 colorB;
			float4 colorC;
			float4 colorD;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float2 centeredUV = v.uv - 0.5;

				o.uv0 = (centeredUV * -0.85) + 0.5;
				o.uv1 = (centeredUV * -1.45) + 0.5;
				o.uv2 = (centeredUV * -2.55) + 0.5;
				o.uv3 = (centeredUV * -4.15) + 0.5;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c0 = tex2D(_MainTex, i.uv0) * colorA;
				fixed4 c1 = tex2D(_MainTex, i.uv1) * colorB;
				fixed4 c2 = tex2D(_MainTex, i.uv2) * colorC;
				fixed4 c3 = tex2D(_MainTex, i.uv3) * colorD;

				return c0 + c1 + c2 + c3;
			}
			ENDCG
		}
	}
		Fallback Off
}