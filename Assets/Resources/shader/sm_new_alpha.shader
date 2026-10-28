Shader "StarWarfare/SNDM_new_Alpha" {
	Properties{
		_maincolor("Main Color", Color) = (1,1,1,1)
		_RGBA("Base(RGB)Trans(A)", 2D) = "white" {}
		_specColor("Specular Color", Color) = (0,0,0,1)
		_specTex_Gloss("Specular(RGB)Gloss(A)", 2D) = "black" {}
		_lightTex("Light Texture", 2D) = "black" {}
		_lightRate("Light Rate", Range(0,1)) = 0.5
		_specMultiple("Specular Multiple", Float) = 1
		_shininess("Shininess", Range(0,1)) = 0.984127
		_normalmap("Normalmap", 2D) = "bump" {}
		_paintColor("Paint Color", Color) = (1,0,0,1)
		_useCustomColor("Use Custom Color (bool)", Range(0,1)) = 0
		_Alpha("Alpha", Range(0,1)) = 1
	}

		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Opaque" "IgnoreProjector" = "False" }
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

			CGPROGRAM
			
			#pragma surface surf CustomLight fullforwardshadows alpha:fade
			#pragma target 3.0

			sampler2D _RGBA;
			sampler2D _lightTex;
			sampler2D _normalmap;
			sampler2D _specTex_Gloss;

			fixed4 _maincolor;
			fixed4 _paintColor;
			fixed4 _specColor;
			half _lightRate;
			half _useCustomColor;
			half _specMultiple;
			half _shininess;
			half _Alpha;

			struct Input {
				float2 uv_RGBA;
				float2 uv_normalmap;
				float2 uv_specTex_Gloss;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				fixed4 baseTex = tex2D(_RGBA, IN.uv_RGBA);
				fixed4 lightTex = tex2D(_lightTex, IN.uv_RGBA);
				fixed4 normalTex = tex2D(_normalmap, IN.uv_normalmap);
				fixed4 specTex = tex2D(_specTex_Gloss, IN.uv_specTex_Gloss);

				half factor = _useCustomColor * normalTex.w;

				fixed3 albedo = lightTex.rgb * _lightRate;
				albedo += (_maincolor.rgb * baseTex.rgb) * (1.0 - factor);
				albedo += _paintColor.rgb * factor;
				o.Albedo = albedo;

				o.Normal = UnpackNormal(normalTex);

				o.Specular = (specTex.rgb + _specColor.rgb) * _specMultiple;

				o.Gloss = _shininess + specTex.w;

				o.Alpha = _Alpha;
			}

			inline fixed4 LightingCustomLight(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				half3 h = normalize(lightDir + viewDir);
				half diff = max(0, dot(s.Normal, lightDir));

				half specPow = pow(max(0, dot(s.Normal, h)), s.Gloss * 128.0);

				half3 spec = s.Specular * specPow;

				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff) + (spec * _LightColor0.rgb * atten);
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}

			Fallback "Diffuse"
}