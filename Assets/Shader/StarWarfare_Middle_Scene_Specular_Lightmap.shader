Shader "StarWarfare/Middle/Scene/Specular_Lightmap" {
	Properties{
		_LightMap("Brightness Light Map ", Float) = 0.4
		_Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
		_LightPower("lightPower", Float) = 1
		_Attenuation("Attenuation", Range(0,1)) = 0.5
	}

		SubShader{
			LOD 400
			Tags { "RenderType" = "Opaque" }

			CGPROGRAM
			#pragma surface surf CustomSpecularLightmap fullforwardshadows
			#pragma target 3.0

			sampler2D _MainTex;
			fixed4 _Color;
			half _LightPower;
			half _Attenuation;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
				o.Alpha = 1.0;
			}

			inline fixed4 LightingCustomSpecularLightmap(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				half NdotL = _LightPower;

				half3 shadow = _Attenuation * 2.0;
				half3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

				half3 diffLight = lerp(shadow * NdotL, ambient + 1.0, ambient);

				fixed3 color = (s.Albedo * diffLight) * _Color.rgb;

				fixed4 c;
				c.rgb = color;
				c.a = 1.0;
				return c;
			}
			ENDCG
		}

			Fallback "StarWarfare/Middle/Scene/Specular_Lightmap_LOD100"
}