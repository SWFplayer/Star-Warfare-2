Shader "Hidden/BlendForBloom"
{
	Properties
	{
		_MainTex("Screen Blended", 2D) = "white" {}
		_ColorBuffer("Color", 2D) = "white" {}
	}

		CGINCLUDE
#include "UnityCG.cginc"

		sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	sampler2D _ColorBuffer;
	float _Intensity;

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float2 uv3 : TEXCOORD3;
		float2 uv4 : TEXCOORD4;
	};

	v2f vert_simple(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	v2f vert_offset(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);

		float2 texel = _MainTex_TexelSize.xy;
		o.uv = v.uv + texel * 0.5;
		o.uv1 = v.uv - texel * 0.5;
		o.uv2 = v.uv - float2(texel.x, -texel.y) * 0.5;
		o.uv3 = v.uv + float2(texel.x, -texel.y) * 0.5;
		o.uv4 = v.uv;

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
			#pragma fragment frag_screen
			#pragma target 2.0

			fixed4 frag_screen(v2f i) : SV_Target
			{
				fixed4 bloom = tex2D(_MainTex, i.uv) * _Intensity;
				fixed4 screen = tex2D(_ColorBuffer, i.uv);

				return 1.0 - ((1.0 - bloom) * (1.0 - screen));
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert_simple
			#pragma fragment frag_add
			#pragma target 2.0

			fixed4 frag_add(v2f i) : SV_Target
			{
				fixed4 bloom = tex2D(_MainTex, i.uv);
				fixed4 screen = tex2D(_ColorBuffer, i.uv);
				return (_Intensity * bloom) + screen;
			}
			ENDCG
		}

				Pass
				{
					CGPROGRAM
					#pragma vertex vert_offset
					#pragma fragment frag_max
					#pragma target 2.0

					fixed4 frag_max(v2f i) : SV_Target
					{
						fixed4 c0 = tex2D(_MainTex, i.uv);
						fixed4 c1 = tex2D(_MainTex, i.uv1);
						fixed4 c2 = tex2D(_MainTex, i.uv2);
						fixed4 c3 = tex2D(_MainTex, i.uv3);
						fixed4 c4 = tex2D(_MainTex, i.uv4);

						return max(max(max(max(c4, c0), c1), c2), c3);
					}
					ENDCG
				}

				Pass
				{
					CGPROGRAM
					#pragma vertex vert_simple
					#pragma fragment frag_mul
					#pragma target 2.0

					fixed4 frag_mul(v2f i) : SV_Target
					{
						fixed4 c1 = tex2D(_MainTex, i.uv);
						fixed4 c2 = tex2D(_ColorBuffer, i.uv);
						return c1 * c2;
					}
					ENDCG
				}

						Pass
						{
							CGPROGRAM
							#pragma vertex vert_simple
							#pragma fragment frag_alpha
							#pragma target 2.0

							fixed4 frag_alpha(v2f i) : SV_Target
							{
								fixed4 bloom = tex2D(_MainTex, i.uv) * _Intensity;
								fixed4 screen = tex2D(_ColorBuffer, i.uv);
								return 1.0 - ((1.0 - bloom) * (1.0 - screen));
							}
							ENDCG
						}

						Pass
						{
							CGPROGRAM
							#pragma vertex vert_offset
							#pragma fragment frag_blur
							#pragma target 2.0

							fixed4 frag_blur(v2f i) : SV_Target
							{
								fixed4 c = tex2D(_MainTex, i.uv) + tex2D(_MainTex, i.uv1) + tex2D(_MainTex, i.uv2) + tex2D(_MainTex, i.uv3);
								return c / 4.0;
							}
							ENDCG
						}

								Pass
								{
									Blend Zero SrcAlpha

									CGPROGRAM
									#pragma vertex vert_simple
									#pragma fragment frag_mask
									#pragma target 2.0

									fixed4 frag_mask(v2f i) : SV_Target
									{
										fixed4 c;
										c.xyz = float3(1.0, 1.0, 1.0);
										c.w = tex2D(_ColorBuffer, i.uv).x;
										return c;
									}
									ENDCG
								}

								Pass
								{
									CGPROGRAM
									#pragma vertex vert_simple
									#pragma fragment frag_clear
									#pragma target 2.0

									fixed4 frag_clear(v2f i) : SV_Target
									{
										return fixed4(0.0, 0.0, 0.0, 0.0);
									}
									ENDCG
								}


										Pass
										{
											Blend One One

											CGPROGRAM
											#pragma vertex vert_simple
											#pragma fragment frag_addcolor
											#pragma target 2.0

											fixed4 frag_addcolor(v2f i) : SV_Target
											{
												return tex2D(_MainTex, i.uv) * _Intensity;
											}
											ENDCG
										}

										Pass
										{
											Blend One One
											BlendOp Max

											CGPROGRAM
											#pragma vertex vert_simple
											#pragma fragment frag_blendmax
											#pragma target 2.0

											fixed4 frag_blendmax(v2f i) : SV_Target
											{
												return tex2D(_MainTex, i.uv);
											}
											ENDCG
										}
	}
		Fallback Off
}