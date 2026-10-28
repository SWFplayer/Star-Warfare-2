Shader "Hidden/BrightPassFilter2" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
	}

		SubShader{

		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Threshhold;

			fixed4 frag(v2f_img i) : SV_Target {
				fixed4 color = tex2D(_MainTex, i.uv);

			color.rgb = max(float3(0,0,0), color.rgb - _Threshhold.xxx);
			return color;
		}
		ENDCG
	}


		Pass {
			ZTest Always
			ZWrite Off
			Cull Off
			Fog { Mode Off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Threshhold;

			fixed4 frag(v2f_img i) : SV_Target {
				fixed4 color = tex2D(_MainTex, i.uv);

			color.rgb = max(float3(0,0,0), color.rgb - _Threshhold.xyz);
			return color;
		}
		ENDCG
	}
	}

		Fallback Off
}