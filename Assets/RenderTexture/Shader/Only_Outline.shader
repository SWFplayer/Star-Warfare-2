Shader "StarWarfare/Only_Outline"
{
	Properties
	{
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(0.002,0.1)) = 0.006
	}

		SubShader
	{
		LOD 200
		Tags { "RenderType" = "Opaque" }

		
		Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

			Stencil
			{
				Ref 4
				Pass Replace
				ZFail Replace
			}

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				
				return fixed4(0.0, 0.0, 0.0, 0.0);
			}
			ENDCG
		}

		Pass
		{
			Name "OUTLINE"

			ZTest Always
			ZWrite Off
			Cull Front

			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

			Stencil
			{
				Ref 4
				Comp NotEqual
				Pass Replace
				ZFail Zero
			}

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			float _Outline;
			fixed4 _OutlineColor;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;

				float4 extrudedPos = v.vertex;
				extrudedPos.xyz += normalize(v.normal) * _Outline;

				o.pos = UnityObjectToClipPos(extrudedPos);
				o.color = _OutlineColor;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
		Fallback "Diffuse"
}