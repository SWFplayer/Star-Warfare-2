Shader "WFX/Scroll/Smoke" {
	Properties{
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Texture", 2D) = "white" {}
		_ScrollSpeed("Scroll Speed", Float) = 2
	}

		SubShader{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

			Pass {
				Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

				ZWrite Off
				Cull Off
				Fog { Color(0,0,0,0) }
				Blend DstColor SrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float4 color  : COLOR;
					float2 uv     : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 uv     : TEXCOORD0;
					fixed4 color : COLOR;
				};

				sampler2D _MainTex;
				fixed4 _TintColor;
				float  _ScrollSpeed;

				v2f vert(appdata_t v) {
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.color = v.color;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target {
					// 1. Unscrolled alpha mask
					fixed mask = tex2D(_MainTex, i.uv).a * i.color.a;

				// 2. Scrolling offset (fmod with sign preservation)
				//    The bytecode computes _Time * _ScrollSpeed, takes fract(abs(...)),
				//    then restores the sign. For _ScrollSpeed >= 0 this is
				//    just fract(_Time.x * _ScrollSpeed).
				float scroll = frac(_Time.x * _ScrollSpeed);
				// If negative scroll speed is possible, use a signed fmod.
				// For the default positive speed, the simple frac is exact.

				// 3. Sample the scrolled UV
				float2 scrolledUV = float2(i.uv.x, i.uv.y - scroll);

				// 4. RGB from scrolled sample, tinted by vertex color and material tint
				fixed4 tex;
				tex.rgb = tex2D(_MainTex, scrolledUV).rgb * (i.color.rgb * _TintColor.rgb);
				tex.a = mask;

				// 5. Blend with mid-gray constant using the mask
				tex = lerp(fixed4(0.5, 0.5, 0.5, 0.5), tex, mask);

				return tex;
			}
			ENDCG
		}
		}
}