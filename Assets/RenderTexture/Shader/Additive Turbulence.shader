Shader "FX PACK 1/Particles/Additive Turbulence" {
	Properties{
		_MainTex("Main_Texture", 2D) = "white" {}
		_Color01("Color", Color) = (1,1,1,1)
		_Blend_Texture("Blend_Texture_01", 2D) = "white" {}
		_Color02("Color", Color) = (1,1,1,1)
		_Blend_Texture01("Blend_Texture_02", 2D) = "black" {}
		_Color03("Color", Color) = (1,1,1,1)
		_Speed01("Blend_Texture_01_Speed", Float) = 1
		_Speed02("Blend_Texture_02_Speed", Float) = 1
		_LightenMain("Brightness_Main", Float) = 1
		_Lighten("Brightness_Blend", Float) = 1
	}

		SubShader{
			Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
			LOD 100
			Blend One One
			ZWrite Off
			Cull Off

			CGPROGRAM

			#pragma surface surf UnlitParticle fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _Blend_Texture;
			sampler2D _Blend_Texture01;

			fixed4 _Color01;
			fixed4 _Color02;
			fixed4 _Color03;
			half _Speed01;
			half _Speed02;
			half _LightenMain;
			half _Lighten;

			struct Input {
				float2 uv_MainTex;
				float2 uv_Blend_Texture;
				float2 uv_Blend_Texture01;
				float4 color : COLOR;
			};

			void surf(Input IN, inout SurfaceOutput o) {

				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 colMain = _Color01 * mainTex;

				float2 uv1 = IN.uv_Blend_Texture + float2(_Time.x * _Speed01, _Time.x * _Speed01);
				fixed4 blendTex1 = tex2D(_Blend_Texture, uv1);
				fixed4 colBlend1 = _Color02 * blendTex1;

				float2 uv2 = IN.uv_Blend_Texture01 + float2(_Time.x * _Speed02, _Time.x * _Speed02);
				fixed4 blendTex2 = tex2D(_Blend_Texture01, uv2);
				fixed4 colBlend2 = _Color03 * blendTex2;

				fixed4 turbulence = (colBlend1 + colBlend2) * (colBlend1 * colBlend2);

				fixed4 finalColor = colMain + ((colMain * turbulence) * _Lighten);

				finalColor *= _LightenMain * IN.color;

				o.Albedo = fixed3(0,0,0);
				o.Emission = finalColor.rgb;
				o.Alpha = 1.0; 
			}

			inline fixed4 LightingUnlitParticle(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten) {
				fixed4 c;
				c.rgb = s.Emission; 
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}

			Fallback "Diffuse"
}