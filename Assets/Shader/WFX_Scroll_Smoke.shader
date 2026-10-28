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
					fixed mask = tex2D(_MainTex, i.uv).a * i.color.a;

				float scroll = frac(_Time.x * _ScrollSpeed);
				float2 scrolledUV = float2(i.uv.x, i.uv.y - scroll);

				fixed4 tex;
				tex.rgb = tex2D(_MainTex, scrolledUV).rgb * (i.color.rgb * _TintColor.rgb);
				tex.a = mask;

				tex = lerp(fixed4(0.5, 0.5, 0.5, 0.5), tex, mask);

				return tex;
			}
			ENDCG
		}
		}
}