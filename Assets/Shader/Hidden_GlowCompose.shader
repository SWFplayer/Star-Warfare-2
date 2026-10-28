Shader "Hidden/GlowCompose"
{
	Properties
	{
		_Color("Glow Amount", Color) = (1,1,1,1)
		_MainTex("", 2D) = "white" {}
	}

		SubShader
	{
		ZTest Always
		ZWrite Off
		Cull Off
		Fog { Mode Off }
		Blend One One   

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			sampler2D _MainTex;
			fixed4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return (2.0 * _Color) * tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
		Fallback Off
}