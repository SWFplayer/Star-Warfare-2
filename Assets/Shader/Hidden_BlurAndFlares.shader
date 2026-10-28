Shader "Hidden/BlurAndFlares"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NonBlurredTex("Base (RGB)", 2D) = "white" {}
	}

		CGINCLUDE
#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	float4 _Offsets;
	float _StretchWidth;
	float4 _TintColor;
	float2 _Threshhold;
	float _Saturation;

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f_simple
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f_7tap
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float2 uv3 : TEXCOORD3;
		float2 uv4 : TEXCOORD4;
		float2 uv5 : TEXCOORD5;
		float2 uv6 : TEXCOORD6;
	};

	struct v2f_9tap
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float4 uv1 : TEXCOORD1;
		float4 uv2 : TEXCOORD2;
		float4 uv3 : TEXCOORD3;
		float4 uv4 : TEXCOORD4;
	};

	v2f_simple vert_simple(appdata v)
	{
		v2f_simple o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	v2f_7tap vert_7tap(appdata v)
	{
		v2f_7tap o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;

		float2 texel = _MainTex_TexelSize.xy;
		float2 cse1 = 0.5 * texel;
		float2 cse2 = 1.5 * texel;
		float2 cse3 = 2.5 * texel;

		o.uv1 = v.uv + cse1 * _Offsets.xy;
		o.uv2 = v.uv - cse1 * _Offsets.xy;
		o.uv3 = v.uv + cse2 * _Offsets.xy;
		o.uv4 = v.uv - cse2 * _Offsets.xy;
		o.uv5 = v.uv + cse3 * _Offsets.xy;
		o.uv6 = v.uv - cse3 * _Offsets.xy;
		return o;
	}


	v2f_9tap vert_9tap(appdata v)
	{
		v2f_9tap o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;

		float4 offsets = _Offsets.xyxy * float4(1.0, 1.0, -1.0, -1.0);
		o.uv1 = v.uv.xyxy + offsets;
		o.uv2 = v.uv.xyxy + (offsets * 2.0);
		o.uv3 = v.uv.xyxy + (offsets * 3.0);
		o.uv4 = v.uv.xyxy + (offsets * 5.0);
		return o;
	}
	ENDCG

		SubShader
	{
		ZTest Always
		ZWrite Off
		Cull Off
		Fog { Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_simple
			#pragma fragment frag_brightness
			#pragma target 2.0

			fixed4 frag_brightness(v2f_simple i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed lum = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
				return col / (1.5 + lum);
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_stretch
			#pragma fragment frag_stretch
			#pragma target 2.0

			struct v2f_stretch
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
				float2 uv4 : TEXCOORD4;
				float2 uv5 : TEXCOORD5;
				float2 uv6 : TEXCOORD6;
			};

			v2f_stretch vert_stretch(appdata v)
			{
				v2f_stretch o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				float cse1 = _StretchWidth * 2.0;
				float cse2 = _StretchWidth * 4.0;
				float cse3 = _StretchWidth * 6.0;

				o.uv1 = v.uv + (cse1 * _Offsets.xy);
				o.uv2 = v.uv - (cse1 * _Offsets.xy);
				o.uv3 = v.uv + (cse2 * _Offsets.xy);
				o.uv4 = v.uv - (cse2 * _Offsets.xy);
				o.uv5 = v.uv + (cse3 * _Offsets.xy);
				o.uv6 = v.uv - (cse3 * _Offsets.xy);
				return o;
			}

			fixed4 frag_stretch(v2f_stretch i) : SV_Target
			{
				fixed4 c0 = tex2D(_MainTex, i.uv);
				fixed4 c1 = tex2D(_MainTex, i.uv1);
				fixed4 c2 = tex2D(_MainTex, i.uv2);
				fixed4 c3 = tex2D(_MainTex, i.uv3);
				fixed4 c4 = tex2D(_MainTex, i.uv4);
				fixed4 c5 = tex2D(_MainTex, i.uv5);
				fixed4 c6 = tex2D(_MainTex, i.uv6);
				return max(max(max(max(max(max(c0, c1), c2), c3), c4), c5), c6);
			}
			ENDCG
		}

				Pass
				{
					CGPROGRAM
					#pragma vertex vert_7tap
					#pragma fragment frag_threshold
					#pragma target 2.0

					fixed4 frag_threshold(v2f_7tap i) : SV_Target
					{
						fixed4 c0 = tex2D(_MainTex, i.uv);
						fixed4 c1 = tex2D(_MainTex, i.uv1);
						fixed4 c2 = tex2D(_MainTex, i.uv2);
						fixed4 c3 = tex2D(_MainTex, i.uv3);
						fixed4 c4 = tex2D(_MainTex, i.uv4);
						fixed4 c5 = tex2D(_MainTex, i.uv5);
						fixed4 c6 = tex2D(_MainTex, i.uv6);

						fixed4 avg = (c0 + c1 + c2 + c3 + c4 + c5 + c6) / 7.0;
						fixed4 thresholded = max(avg - _Threshhold.xxxx, fixed4(0,0,0,0));

						fixed lum = dot(thresholded.rgb, fixed3(0.22, 0.707, 0.071));
						thresholded.rgb = lerp(fixed3(lum, lum, lum), thresholded.rgb, _Saturation) * _TintColor.rgb;

						return thresholded;
					}
					ENDCG
				}

				Pass
				{
					CGPROGRAM
					#pragma vertex vert_7tap
					#pragma fragment frag_blur
					#pragma target 2.0

					fixed4 frag_blur(v2f_7tap i) : SV_Target
					{
						fixed4 col = tex2D(_MainTex, i.uv);
						col += tex2D(_MainTex, i.uv1);
						col += tex2D(_MainTex, i.uv2);
						col += tex2D(_MainTex, i.uv3);
						col += tex2D(_MainTex, i.uv4);
						col += tex2D(_MainTex, i.uv5);
						col += tex2D(_MainTex, i.uv6);

						fixed lum = dot(col.rgb, fixed3(0.22, 0.707, 0.071));
						return col / (7.5 + lum);
					}
					ENDCG
				}

						Pass
						{
							CGPROGRAM
							#pragma vertex vert_9tap
							#pragma fragment frag_wideblur
							#pragma target 2.0

							fixed4 frag_wideblur(v2f_9tap i) : SV_Target
							{
								fixed4 col = 0.225 * tex2D(_MainTex, i.uv);
								col += 0.15 * tex2D(_MainTex, i.uv1.xy);
								col += 0.15 * tex2D(_MainTex, i.uv1.zw);
								col += 0.11 * tex2D(_MainTex, i.uv2.xy);
								col += 0.11 * tex2D(_MainTex, i.uv2.zw);
								col += 0.075 * tex2D(_MainTex, i.uv3.xy);
								col += 0.075 * tex2D(_MainTex, i.uv3.zw);
								col += 0.0525 * tex2D(_MainTex, i.uv4.xy);
								col += 0.0525 * tex2D(_MainTex, i.uv4.zw);
								return col;
							}
							ENDCG
						}
	}
		Fallback Off
}