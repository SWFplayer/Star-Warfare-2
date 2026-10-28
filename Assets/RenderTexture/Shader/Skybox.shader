Shader "RenderFX/Skybox" {
	Properties{
		_Tint("Tint Color", Color) = (0.5, 0.5, 0.5, 0.5)
		_FrontTex("Front (+Z)", 2D) = "white" {}
		_BackTex("Back (-Z)", 2D) = "white" {}
		_LeftTex("Left (+X)", 2D) = "white" {}
		_RightTex("Right (-X)", 2D) = "white" {}
		_UpTex("Up (+Y)", 2D) = "white" {}
		_DownTex("Down (-Y)", 2D) = "white" {}
	}

		SubShader{
			Tags { "QUEUE" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
			ZWrite Off
			Cull Off
			Fog { Mode Off }

			CGINCLUDE
			#include "UnityCG.cginc"

			sampler2D _FrontTex;
			sampler2D _BackTex;
			sampler2D _LeftTex;
			sampler2D _RightTex;
			sampler2D _UpTex;
			sampler2D _DownTex;
			fixed4 _Tint;

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 position : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};

			v2f vert(appdata_t v) {
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag(v2f i, sampler2D tex) {
				fixed4 texcol = tex2D(tex, i.texcoord);
				fixed4 col;          

				col.xyz = ((texcol.xyz + _Tint.xyz) - unity_ColorSpaceGrey.xyz);
				col.w = texcol.w * _Tint.w;

				return col;
			}
			ENDCG

			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragFront
				fixed4 fragFront(v2f i) : SV_Target { return frag(i, _FrontTex); }
				ENDCG
			}
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragBack
				fixed4 fragBack(v2f i) : SV_Target { return frag(i, _BackTex); }
				ENDCG
			}
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragLeft
				fixed4 fragLeft(v2f i) : SV_Target { return frag(i, _LeftTex); }
				ENDCG
			}
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragRight
				fixed4 fragRight(v2f i) : SV_Target { return frag(i, _RightTex); }
				ENDCG
			}
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragUp
				fixed4 fragUp(v2f i) : SV_Target { return frag(i, _UpTex); }
				ENDCG
			}
			Pass {
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment fragDown
				fixed4 fragDown(v2f i) : SV_Target { return frag(i, _DownTex); }
				ENDCG
			}
	}
}