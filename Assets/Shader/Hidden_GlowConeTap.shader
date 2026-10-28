Shader "Hidden/GlowConeTap"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,0)
		_MainTex("", 2D) = "white" {}
	}

		SubShader
	{
		ZTest Always
		ZWrite Off
		Cull Off
		Fog { Mode Off }

		Pass
		{
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _BlurOffsets;
			fixed4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 uv01 : TEXCOORD0;           
				float4 uv23 : TEXCOORD1;           
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float2 offset = _MainTex_TexelSize.xy * _BlurOffsets.xy;

				float2 uv = v.uv;

				o.uv01.xy = uv + float2(offset.x,  offset.y);
				o.uv01.zw = uv + float2(-offset.x,  offset.y);
				o.uv23.xy = uv + float2(offset.x, -offset.y);
				o.uv23.zw = uv + float2(-offset.x, -offset.y);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv01.xy) +
							 tex2D(_MainTex, i.uv01.zw) +
							 tex2D(_MainTex, i.uv23.xy) +
							 tex2D(_MainTex, i.uv23.zw);

			col.rgb *= _Color.rgb;

			return col * _Color.a;
		}
		ENDCG
	}
	}
		Fallback Off
}