Shader "Hidden/Glow Downsample"
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
			fixed4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
				float2 uv3 : TEXCOORD3;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				float2 texelSize = _MainTex_TexelSize.xy;
				float2 uv = v.uv;

				o.uv0 = uv + float2(-texelSize.x, -texelSize.y);
				o.uv1 = uv + float2(texelSize.x, -texelSize.y);
				o.uv2 = uv + float2(texelSize.x,  texelSize.y);
				o.uv3 = uv + float2(-texelSize.x,  texelSize.y);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = (tex2D(_MainTex, i.uv0) +
							  tex2D(_MainTex, i.uv1) +
							  tex2D(_MainTex, i.uv2) +
							  tex2D(_MainTex, i.uv3)) / 4.0;

			col.rgb *= _Color.rgb;
			col.rgb *= (col.a + _Color.a);

			col.a = 0.0;
			return col;
		}
		ENDCG
	}
	}
		Fallback Off
}