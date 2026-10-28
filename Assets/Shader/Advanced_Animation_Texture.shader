Shader "Advanced/Animation_Texture"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_NumTexTiles("Num tex tiles", Vector) = (4,4,0,0)
		_ReplaySpeed("Replay speed - FPS", Float) = 4
		_Randomize("Randomize", Float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_Fade("Fade", Float) = 1
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "true"
				"RenderType" = "Transparent"
			}

			Pass
			{
				Tags
				{
					"Queue" = "Transparent"
					"IgnoreProjector" = "true"
					"RenderType" = "Transparent"
				}

				ZWrite Off
				Cull   Off
				Fog    { Color(0,0,0,0) }
				Blend  One One           

				CGPROGRAM
				#pragma vertex   vert
				#pragma fragment frag
				#pragma target   3.0

				sampler2D _MainTex;
				float4    _NumTexTiles;
				float     _ReplaySpeed;
				float     _Randomize;
				float4    _Color;
				float     _Fade;

				struct appdata
				{
					float4 vertex  : POSITION;
					float4 color   : COLOR;
					float2 uv      : TEXCOORD0;
				};

				struct v2f
				{
					float4 pos     : SV_POSITION;
					float4 uv      : TEXCOORD0;           
					float4 color   : COLOR;
				};

				v2f vert(appdata v)
				{
					v2f o;

					float t = (v.color.w * 60.0 + _Time.y) * _ReplaySpeed;

					float frameA = floor(t);
					float frameB = frameA + 1.0;
					float blend = t - frameA;        

					float2 tileA = float2(frameA, floor(frameA / _NumTexTiles.x));
					float2 tileB = float2(frameB, floor(frameB / _NumTexTiles.x));

					float4 tile = float4(tileA, tileB);
					float4 uvNorm = tile / _NumTexTiles.xyxy;
					float4 wrapped = frac(abs(uvNorm)) * _NumTexTiles.xyxy;

					float4 tileUV;
					tileUV.x = uvNorm.x >= 0.0 ? wrapped.x : -wrapped.x;
					tileUV.y = uvNorm.y >= 0.0 ? wrapped.y : -wrapped.y;
					tileUV.z = uvNorm.z >= 0.0 ? wrapped.z : -wrapped.z;
					tileUV.w = uvNorm.w >= 0.0 ? wrapped.w : -wrapped.w;

					float2 invTiles = 1.0 / _NumTexTiles.xy;

					o.pos = UnityObjectToClipPos(v.vertex);

					o.uv = (v.uv.xyxy + tileUV) * invTiles.xyxy;

					o.color = float4(_Color.rgb * v.color.rgb, blend);

					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					float4 c;
					if (_Fade > 0.0)
					{
						c = lerp(
								tex2D(_MainTex, i.uv.xy),
								tex2D(_MainTex, i.uv.zw),
								i.color.w
							) * i.color;
					}
					else
					{
						c = tex2D(_MainTex, i.uv.xy) * i.color;
					}
					return c;
				}
				ENDCG
			}
		}
}