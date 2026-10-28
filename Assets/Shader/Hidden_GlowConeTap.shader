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
				float4 uv01 : TEXCOORD0; // xy = UV + offset, zw = UV + offset
				float4 uv23 : TEXCOORD1; // xy = UV + offset, zw = UV + offset
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				// Calculate the offset based on texel size and blur offsets
				float2 offset = _MainTex_TexelSize.xy * _BlurOffsets.xy;

				// Calculate 4 diagonal UVs
				float2 uv = v.uv;

				// Top-right, Bottom-right, Top-left, Bottom-left
				o.uv01.xy = uv + float2(offset.x,  offset.y);
				o.uv01.zw = uv + float2(-offset.x,  offset.y);
				o.uv23.xy = uv + float2(offset.x, -offset.y);
				o.uv23.zw = uv + float2(-offset.x, -offset.y);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// Sample the 4 cone taps and add them together
				fixed4 col = tex2D(_MainTex, i.uv01.xy) +
							 tex2D(_MainTex, i.uv01.zw) +
							 tex2D(_MainTex, i.uv23.xy) +
							 tex2D(_MainTex, i.uv23.zw);

			// Multiply by the color RGB
			col.rgb *= _Color.rgb;

			// Multiply the final result by the color Alpha
			return col * _Color.a;
		}
		ENDCG
	}
	}
		Fallback Off
}