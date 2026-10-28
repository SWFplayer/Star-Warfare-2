Shader "Advanced/Scroll Single AlphaBlended_No_Lightmap"
{
	Properties
	{
		_MainTex("2nd layer (RGB)", 2D) = "white" {}
		_ScrollX("2nd layer Scroll speed X", Float) = 1
		_ScrollY("2nd layer Scroll speed Y", Float) = 0
		_SineAmplX("2nd layer sine amplitude X", Float) = 0.5
		_SineAmplY("2nd layer sine amplitude Y", Float) = 0.5
		_SineFreqX("2nd layer sine freq X", Float) = 10
		_SineFreqY("2nd layer sine freq Y", Float) = 10
		_MMultiplier("Layer Multiplier", Float) = 2
	}

		SubShader
		{
			LOD 100
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "true"
				"RenderType" = "Transparent"
			}

			Pass
			{
				ZWrite Off
				Cull Off
				Fog { Color(0,0,0,0) }
				Blend One One

				CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_fog

				sampler2D _MainTex;
				float4 _MainTex_ST;
				float _ScrollX;
				float _ScrollY;
				float _MMultiplier;
				float _SineAmplX;
				float _SineAmplY;
				float _SineFreqX;
				float _SineFreqY;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					fixed4 color : COLOR;
					UNITY_FOG_COORDS(1)
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					// --- 1. Scrolling ---
					float2 scrolledUV = TRANSFORM_TEX(v.uv, _MainTex);
					float2 scrollOffset = float2(_ScrollX, _ScrollY) * _Time.xy;
					scrolledUV += frac(scrollOffset);

					// --- 2. Sine Wave Distortion ---
					scrolledUV.x += sin(_Time.y * _SineFreqX) * _SineAmplX;
					scrolledUV.y += sin(_Time.y * _SineFreqY) * _SineAmplY;

					o.uv = scrolledUV;

					// --- 3. Constant Multiplier Color ---
					o.color = fixed4(_MMultiplier, _MMultiplier, _MMultiplier, _MMultiplier);

					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv) * i.color;
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}