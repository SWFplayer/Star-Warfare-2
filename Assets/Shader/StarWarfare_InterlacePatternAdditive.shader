Shader "StarWarfare/InterlacePatternAdditive"
{
	Properties
	{
		_MainTex("Base", 2D) = "white" {}
		_TintColor("TintColor", Color) = (1,1,1,1)
		_InterlacePattern("InterlacePattern", 2D) = "white" {}
		_Illum("_Illum", 2D) = "white" {}
		_EmissionLM("Emission (Lightmapper)", Float) = 1
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"RenderType" = "Transparent"
				"Reflection" = "RenderReflectionTransparentAdd"
			}

			Pass
			{
				ZWrite Off
				Cull Off
				Blend One One // Additive Blending

				CGPROGRAM
				#include "UnityCG.cginc"
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0
				#pragma multi_compile_fog

				sampler2D _MainTex;
				sampler2D _InterlacePattern;
				float4 _InterlacePattern_ST;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 uvInterlace : TEXCOORD1;
					UNITY_FOG_COORDS(2)
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);

					// Standard UV for the main texture
					o.uv = v.uv;

					// --- Interlace Pattern Scrolling ---
					// Base UV transform for the interlace pattern
					float2 patternUV = TRANSFORM_TEX(v.uv, _InterlacePattern);

					// The original shader offsets the UV using: _Time.x * _InterlacePattern_ST.zw
					// Note: _InterlacePattern_ST.zw contains the Offset (X, Y) of the material.
					// This is a strange but specific behavior in this shader.
					float2 scrollOffset = _Time.xx * _InterlacePattern_ST.zw;

					o.uvInterlace = patternUV + scrollOffset;

					UNITY_TRANSFER_FOG(o, o.pos);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// Sample base texture and interlace pattern, then multiply them
					fixed4 col = tex2D(_MainTex, i.uv) * tex2D(_InterlacePattern, i.uvInterlace);

					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
			Fallback Off
}