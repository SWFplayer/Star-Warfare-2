Shader "StarWarfare/Scene/Emissive_Specular_Lightmap" {
	Properties{
	 _Shininess("Shininess for Light Map", Range(0.03,1)) = 1
	 _LightMap("Brightness Light Map ", Float) = 0.4
	 _Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	 _SpecColor("Specular Color", Color) = (1,1,1,1)
	 _SpecularPower("Specular", Float) = 1
	 _MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
	 _LightPower("lightPower", Float) = 1
	 _Attenuation("Attenuation", Range(0,1)) = 0.5
	 _SpecTex("Specular Tex", 2D) = "Black" {}
	 _Emissive("Emissive", 2D) = "black" {}
	}

		SubShader{
		 LOD 400
		 Tags { "RenderType" = "Opaque" }

		 Pass {
		  Name "CONTENTBASE"
		  Tags { "LIGHTMODE" = "Always" "SHADOWSUPPORT" = "true" "RenderType" = "Opaque" }

		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #pragma multi_compile __ LIGHTMAP_ON
		  #pragma multi_compile_fog
		  #include "UnityCG.cginc"

		  sampler2D _MainTex;
		  float4 _MainTex_ST;
		  sampler2D _SpecTex;
		  sampler2D _Emissive;
		  float _Shininess;
		  float _LightMap;
		  float4 _Color;
		  float4 _SpecColor;
		  float _SpecularPower;
		  float _LightPower;
		  float _Attenuation;

		  struct appdata {
			  float4 vertex : POSITION;
			  float2 uv0 : TEXCOORD0;
			  float2 uv1 : TEXCOORD1;
		  };

		  struct v2f {
			  float4 pos : SV_POSITION;
			  float2 uv0 : TEXCOORD0;
			  float2 uv1 : TEXCOORD1;
			  UNITY_FOG_COORDS(2)
		  };

		  v2f vert(appdata v) {
			  v2f o;
			  o.pos = UnityObjectToClipPos(v.vertex);
			  o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);

			  #if LIGHTMAP_ON
			  o.uv1 = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
			  #else
			  o.uv1 = 0;
			  #endif

			  UNITY_TRANSFER_FOG(o, o.pos);

			  return o;
		  }

		  fixed4 frag(v2f i) : SV_Target {
			  half3 tex = tex2D(_MainTex, i.uv0).xyz;
			  half specTex = tex2D(_SpecTex, i.uv0).r;
			  half3 emissive = tex2D(_Emissive, i.uv0).xyz;
			  half3 col = 0;

			  #if LIGHTMAP_ON
			  half4 lightMap = half4(2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1).xyz, 0.0);
			  half3 lm = lightMap.xyz * _LightMap;
			  half3 shadow = min(lm, (_Attenuation * 2.0) * 0.8);

			  half3 diff = ((tex * shadow) * _LightPower) * _Color.xyz;
			  half3 spec = (((specTex * shadow) * _SpecularPower)) * _SpecColor.xyz;
			  col = diff + spec + emissive;
		  #else
			  half3 shadow = _Attenuation * 2.0;
			  half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;

			  half3 lightTerm = lerp((shadow * _LightPower), (ambient + 1.0), ambient);
			  half3 diff = (tex * lightTerm) * _Color.xyz;
			  half3 spec = (((specTex * shadow) * _SpecularPower)) * _SpecColor.xyz;
			  col = diff + spec + emissive;
		  #endif

			  UNITY_APPLY_FOG(i.fogCoord, col);

		  return fixed4(col, 0.0);
	  }
	  ENDCG
	 }
	}

		Fallback "StarWarfare/Scene/Emissive_Specular_Lightmap_LOD100"
}