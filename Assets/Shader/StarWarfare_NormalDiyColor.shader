Shader "StarWarfare/NormalDiyColor"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Range(0.03,1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_MaskMap(" And Mask", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_Width("Half Width", Range(0,0.5)) = 0.02
		_RollFromX("ScanFromX", Range(0,1)) = 0.5
		_Roll("show range", Range(0,1)) = 0
	}

		SubShader
		{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma surface surf BlinnPhong
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _MaskMap;
			fixed4 _Color;
			fixed4 _TintColor;
			float _Width;
			float _Roll;
			float _RollFromX;
			float _Shininess;

			struct Input
			{
				float2 uv_MainTex;
				float2 uv_MaskMap;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 maskTex = tex2D(_MaskMap, IN.uv_MaskMap);

				fixed3 finalColor = mainTex.rgb;
				if (maskTex.w >= 0.5)
				{
					float p = 0.0;
					if (_RollFromX > 0.5)
					{
						p = IN.uv_MaskMap.x - _Roll;
					}
					else
					{
						p = IN.uv_MaskMap.y - _Roll;
					}

					if (p > -_Width && p < _Width)
					{
						float intensity = abs(p / _Width);
						finalColor = (_TintColor.rgb * (1.0 - intensity)) + (mainTex.rgb * intensity);
					}
				}

				o.Albedo = finalColor;
				o.Normal = UnpackNormal(maskTex);          
				o.Gloss = mainTex.a;                    
				o.Specular = _Shininess;              
				o.Alpha = mainTex.a * _Color.a;
			}
			ENDCG
		}
			Fallback "Specular"
}