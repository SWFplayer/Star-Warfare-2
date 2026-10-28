Shader "Hidden/ColorCorrectionSelective" {
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
				float4 selColor;
				float4 targetColor;

				fixed4 frag(v2f_img i) : SV_Target {
					fixed4 color = tex2D(_MainTex, i.uv);

				float3 diff = color.rgb - selColor.rgb;

				float dist = 2.0 * sqrt(dot(diff, diff));

				float blend = clamp(dist, 0.0, 1.0);

				return lerp(targetColor, color, blend);
			}
			ENDCG
		}
	}
		Fallback Off
}
