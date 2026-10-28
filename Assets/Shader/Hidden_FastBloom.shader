Shader "Hidden/FastBloom"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Bloom("Bloom (RGB)", 2D) = "black" {}
	}

		CGINCLUDE
#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _Bloom;
	float4 _Parameter; // x = blurSpread, z = threshold, w = intensity

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

	struct v2f_downsample
	{
		float4 pos : SV_POSITION;
		float2 uv0 : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float2 uv3 : TEXCOORD3;
	};

	struct v2f_blur
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 blurDir : TEXCOORD1;
	};
	ENDCG

		SubShader
	{
		ZTest False
		ZWrite Off
		Cull Off
		Fog { Mode Off }

		// =====================================================
		// PASS 0: Combine Bloom and MainTex
		// =====================================================
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_simple
			#pragma fragment frag_combine
			#pragma target 2.0

			v2f_simple vert_simple(appdata v)
			{
				v2f_simple o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag_combine(v2f_simple i) : SV_Target
			{
				return tex2D(_MainTex, i.uv) + tex2D(_Bloom, i.uv);
			}
			ENDCG
		}

		// =====================================================
		// PASS 1: Downsample & Threshold
		// =====================================================
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_downsample
			#pragma fragment frag_downsample
			#pragma target 2.0

			v2f_downsample vert_downsample(appdata v)
			{
				v2f_downsample o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float2 texel = _MainTex_TexelSize.xy;
				o.uv0 = v.uv + texel;
				o.uv1 = v.uv + texel * float2(-0.5, -0.5);
				o.uv2 = v.uv + texel * float2(0.5, -0.5);
				o.uv3 = v.uv + texel * float2(-0.5, 0.5);
				return o;
			}

			fixed4 frag_downsample(v2f_downsample i) : SV_Target
			{
				fixed4 sum = tex2D(_MainTex, i.uv0) + tex2D(_MainTex, i.uv1) + tex2D(_MainTex, i.uv2) + tex2D(_MainTex, i.uv3);
				fixed4 avg = sum / 4.0;
				// Subtract threshold and clamp to 0
				fixed4 thresholded = max(avg - _Parameter.z, fixed4(0,0,0,0));
				return thresholded * _Parameter.w;
			}
			ENDCG
		}

				// =====================================================
				// PASS 2 & 3: Blur (Horizontal / Vertical)
				// =====================================================
				// Note: These passes are identical except for the blur direction.
				// We'll use the same CGPROGRAM for both by adjusting the vertex input.
				Pass
				{
					CGPROGRAM
					#pragma vertex vert_blur_v
					#pragma fragment frag_blur
					#pragma target 2.0

					v2f_blur vert_blur_v(appdata v)
					{
						v2f_blur o;
						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv = v.uv;
						o.blurDir = (_MainTex_TexelSize.xy * float2(0.0, 1.0)) * _Parameter.x;
						return o;
					}

					fixed4 frag_blur(v2f_blur i) : SV_Target
					{
						fixed4 color = 0;
						color += tex2D(_MainTex, i.uv - i.blurDir * 3.0) * 0.0205;
						color += tex2D(_MainTex, i.uv - i.blurDir * 2.0) * 0.0855;
						color += tex2D(_MainTex, i.uv - i.blurDir * 1.0) * 0.232;
						color += tex2D(_MainTex, i.uv) * 0.324;
						color += tex2D(_MainTex, i.uv + i.blurDir * 1.0) * 0.232;
						color += tex2D(_MainTex, i.uv + i.blurDir * 2.0) * 0.0855;
						color += tex2D(_MainTex, i.uv + i.blurDir * 3.0) * 0.0205;
						return color;
					}
					ENDCG
				}

				Pass
				{
					CGPROGRAM
					#pragma vertex vert_blur_h
					#pragma fragment frag_blur
					#pragma target 2.0

					v2f_blur vert_blur_h(appdata v)
					{
						v2f_blur o;
						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv = v.uv;
						o.blurDir = (_MainTex_TexelSize.xy * float2(1.0, 0.0)) * _Parameter.x;
						return o;
					}

					fixed4 frag_blur(v2f_blur i) : SV_Target
					{
						fixed4 color = 0;
						color += tex2D(_MainTex, i.uv - i.blurDir * 3.0) * 0.0205;
						color += tex2D(_MainTex, i.uv - i.blurDir * 2.0) * 0.0855;
						color += tex2D(_MainTex, i.uv - i.blurDir * 1.0) * 0.232;
						color += tex2D(_MainTex, i.uv) * 0.324;
						color += tex2D(_MainTex, i.uv + i.blurDir * 1.0) * 0.232;
						color += tex2D(_MainTex, i.uv + i.blurDir * 2.0) * 0.0855;
						color += tex2D(_MainTex, i.uv + i.blurDir * 3.0) * 0.0205;
						return color;
					}
					ENDCG
				}

						// =====================================================
						// PASS 4 & 5: Optimized Blur (using 4 UV taps)
						// =====================================================
						Pass
						{
							CGPROGRAM
							#pragma vertex vert_fast_blur_v
							#pragma fragment frag_fast_blur
							#pragma target 2.0

							struct v2f_fast_blur
							{
								float4 pos : SV_POSITION;
								float2 uv0 : TEXCOORD0;
								float4 uv1 : TEXCOORD1;
								float4 uv2 : TEXCOORD2;
								float4 uv3 : TEXCOORD3;
							};

							v2f_fast_blur vert_fast_blur_v(appdata v)
							{
								v2f_fast_blur o;
								o.pos = UnityObjectToClipPos(v.vertex);
								float2 blurDir = (_MainTex_TexelSize.xy * float2(0.0, 1.0)) * _Parameter.x;
								float4 offsets = -blurDir.xyxy * 3.0;
								o.uv0 = v.uv;
								o.uv1 = v.uv.xyxy + offsets * float4(1.0, 1.0, -1.0, -1.0);
								o.uv2 = v.uv.xyxy + (offsets + blurDir.xyxy) * float4(1.0, 1.0, -1.0, -1.0);
								o.uv3 = v.uv.xyxy + (offsets + blurDir.xyxy + blurDir.xyxy) * float4(1.0, 1.0, -1.0, -1.0);
								return o;
							}

							fixed4 frag_fast_blur(v2f_fast_blur i) : SV_Target
							{
								fixed4 color = tex2D(_MainTex, i.uv0) * 0.324;
								color += (tex2D(_MainTex, i.uv1.xy) + tex2D(_MainTex, i.uv1.zw)) * 0.0205;
								color += (tex2D(_MainTex, i.uv2.xy) + tex2D(_MainTex, i.uv2.zw)) * 0.0855;
								color += (tex2D(_MainTex, i.uv3.xy) + tex2D(_MainTex, i.uv3.zw)) * 0.232;
								return color;
							}
							ENDCG
						}

						Pass
						{
							CGPROGRAM
							#pragma vertex vert_fast_blur_h
							#pragma fragment frag_fast_blur
							#pragma target 2.0

							struct v2f_fast_blur
							{
								float4 pos : SV_POSITION;
								float2 uv0 : TEXCOORD0;
								float4 uv1 : TEXCOORD1;
								float4 uv2 : TEXCOORD2;
								float4 uv3 : TEXCOORD3;
							};

							v2f_fast_blur vert_fast_blur_h(appdata v)
							{
								v2f_fast_blur o;
								o.pos = UnityObjectToClipPos(v.vertex);
								float2 blurDir = (_MainTex_TexelSize.xy * float2(1.0, 0.0)) * _Parameter.x;
								float4 offsets = -blurDir.xyxy * 3.0;
								o.uv0 = v.uv;
								o.uv1 = v.uv.xyxy + offsets * float4(1.0, 1.0, -1.0, -1.0);
								o.uv2 = v.uv.xyxy + (offsets + blurDir.xyxy) * float4(1.0, 1.0, -1.0, -1.0);
								o.uv3 = v.uv.xyxy + (offsets + blurDir.xyxy + blurDir.xyxy) * float4(1.0, 1.0, -1.0, -1.0);
								return o;
							}

							fixed4 frag_fast_blur(v2f_fast_blur i) : SV_Target
							{
								fixed4 color = tex2D(_MainTex, i.uv0) * 0.324;
								color += (tex2D(_MainTex, i.uv1.xy) + tex2D(_MainTex, i.uv1.zw)) * 0.0205;
								color += (tex2D(_MainTex, i.uv2.xy) + tex2D(_MainTex, i.uv2.zw)) * 0.0855;
								color += (tex2D(_MainTex, i.uv3.xy) + tex2D(_MainTex, i.uv3.zw)) * 0.232;
								return color;
							}
							ENDCG
						}
	}
		Fallback Off
}