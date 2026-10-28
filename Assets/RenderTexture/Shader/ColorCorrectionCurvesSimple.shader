Shader "Hidden/ColorCorrectionCurvesSimple" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
		_RgbTex("_RgbTex (RGB)", 2D) = "" {}
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
				sampler2D _RgbTex;
				float _Saturation;

				fixed4 frag(v2f_img i) : SV_Target {
				
					fixed4 color = tex2D(_MainTex, i.uv);

				fixed3 red = tex2D(_RgbTex, float2(color.r, 0.125)).rgb * fixed3(1,0,0);
				fixed3 green = tex2D(_RgbTex, float2(color.g, 0.375)).rgb * fixed3(0,1,0);
				fixed3 blue = tex2D(_RgbTex, float2(color.b, 0.625)).rgb * fixed3(0,0,1);

				fixed3 correctedRGB = red + green + blue;

				float lum = dot(correctedRGB, float3(0.22, 0.707, 0.071));
				color.rgb = lerp(float3(lum, lum, lum), correctedRGB, _Saturation);

				return color;
			}
			ENDCG
		}
	}
		Fallback Off
}