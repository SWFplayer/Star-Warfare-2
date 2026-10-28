Shader "StarWarfare/Enemy/Alpha_Bumped_Specular" {
	Properties{
		_SpecColor("Specular Color", Color) = (0.5,0.5,0.5,1)
		_Shininess("Shininess", Range(0.03,1)) = 0.078125
		_MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_AlphaAfterDead("Alpha After Dead", Range(0,1)) = 1
		_SpecMulti("Specular Multi", Float) = 1
	}

		SubShader{
			Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
			LOD 400
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			
			#pragma surface surf CustomBumpedSpecular fullforwardshadows alpha:fade
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _BumpMap;
			fixed4 _SpecuColor;
			half _Shininess;
			half _AlphaAfterDead;
			half _SpecMulti;

			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				
				fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);

				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

				o.Albedo = mainTex.rgb;

				o.Specular = mainTex.a * _SpecMulti;
				o.Gloss = _Shininess;

				o.Alpha = _AlphaAfterDead;
			}

			inline fixed4 LightingCustomBumpedSpecular(SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten) {
				
				half3 h = normalize(lightDir + viewDir);
				half diff = max(0, dot(s.Normal, lightDir));

				half nh = max(0, dot(s.Normal, h));
				half specPow = pow(nh, s.Gloss * 128.0);
				half spec = specPow * s.Specular; 

				fixed4 c;
				c.rgb = (s.Albedo * _LightColor0.rgb * diff) + (_LightColor0.rgb * _SpecuColor.rgb * spec);

				c.rgb *= 2.0;

				c.a = s.Alpha + ((_LightColor0.a * _SpecuColor.a) * spec);

				c.rgb *= atten;
				c.a *= atten;

				return c;
			}
			ENDCG
		}

			Fallback "Mobile/Diffuse"
}