Shader "Hidden/ColorCorrectionCurves" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "" {}
		_RgbTex("_RgbTex (RGB)", 2D) = "" {}
		_ZCurve("_ZCurve (RGB)", 2D) = "" {}
		_RgbDepthTex("_RgbDepthTex (RGB)", 2D) = "" {}
	}

		SubShader{
			Pass {
				ZTest Always
				ZWrite Off
				Cull Off
				Fog { Mode Off }

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 depthUV : TEXCOORD1;
				};

				sampler2D _MainTex;
				sampler2D _RgbTex;
				sampler2D _ZCurve;
				sampler2D _RgbDepthTex;
				sampler2D _CameraDepthTexture;

				float4 _CameraDepthTexture_ST;
				float4 _ZBufferParamss;
				float _Saturation;

				v2f vert(appdata_img v) {
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.texcoord.xy;
					o.depthUV = v.texcoord.xy * _CameraDepthTexture_ST.xy + _CameraDepthTexture_ST.zw;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					
					fixed4 color = tex2D(_MainTex, i.uv);

				fixed3 red = tex2D(_RgbTex, float2(color.r, 0.125)).rgb * fixed3(1,0,0);
				fixed3 green = tex2D(_RgbTex, float2(color.g, 0.375)).rgb * fixed3(0,1,0);
				fixed3 blue = tex2D(_RgbTex, float2(color.b, 0.625)).rgb * fixed3(0,0,1);

				float rawDepth = tex2D(_CameraDepthTexture, i.depthUV).r;
				
				float linearDepth = 1.0 / (_ZBufferParamss.x * rawDepth + _ZBufferParamss.y);

				float zval = tex2D(_ZCurve, float2(linearDepth, 0.5)).r;

				fixed3 depthRed = tex2D(_RgbDepthTex, float2(color.r, 0.125)).rgb * fixed3(1,0,0);
				fixed3 depthGreen = tex2D(_RgbDepthTex, float2(color.g, 0.375)).rgb * fixed3(0,1,0);
				fixed3 depthBlue = tex2D(_RgbDepthTex, float2(color.b, 0.625)).rgb * fixed3(0,0,1);

				fixed3 finalRGB = lerp(red + green + blue, depthRed + depthGreen + depthBlue, zval);

				float lum = dot(finalRGB, float3(0.22, 0.707, 0.071));
				color.rgb = lerp(float3(lum, lum, lum), finalRGB, _Saturation);

				return color;
			}
			ENDCG
		}
	}
		Fallback Off
}