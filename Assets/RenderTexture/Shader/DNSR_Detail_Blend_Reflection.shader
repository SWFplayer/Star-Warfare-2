// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

Shader "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap" {
	Properties{
	 _Shininess("Shininess for Light Map", Range(0.03,1)) = 1
	 _LightMap("Brightness Light Map ", Float) = 0.4
	 _Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	 _SpecColor("Specular Color", Color) = (1,1,1,1)
	 _SpecularPower("Specular", Float) = 1
	 _GlossPower("Gloss", Float) = 1
	 _UVDetailNormalTile("UV Detail Normal Tile", Float) = 1
	 _MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
	 _LightDir("LightDirection", Vector) = (0,0,0,0)
	 _Attenuation("Attenuation", Range(0,1)) = 0.5
	 _MainTexB("Diffuse (RGB) Specular (A) blend", 2D) = "white" {}
	 _LightPowerOfB("Light Power of B map", Float) = 1
	 _BlendRate("Blend Rate For Test", Range(0,1)) = 0
	 _MaskMap("Blend Mask map", 2D) = "white" {}
	 _SpecTex("Specular Tex", 2D) = "Black" {}
	 _SpecTexB("Specular Tex B", 2D) = "Black" {}
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
		  #include "UnityCG.cginc"

		  sampler2D _MainTex;
		  float4 _MainTex_ST;
		  sampler2D _MainTexB;
		  float4 _MainTexB_ST;
		  sampler2D _SpecTex;
		  sampler2D _SpecTexB;
		  sampler2D _MaskMap;
		  // sampler2D unity_Lightmap;
		  // float4 unity_LightmapST;

		  float _Shininess;
		  float _LightMap;
		  float4 _Color;
		  float4 _SpecColor;
		  float _SpecularPower;
		  float _GlossPower;
		  float _UVDetailNormalTile;
		  float3 _LightDir;
		  float _Attenuation;
		  float _LightPowerOfB;
		  float _BlendRate;

		  struct appdata {
			  float4 vertex : POSITION;
			  float4 color : COLOR;
			  float2 uv0 : TEXCOORD0;
			  float2 uv1 : TEXCOORD1;
		  };

		  struct v2f {
			  float4 pos : SV_POSITION;
			  float2 uv0 : TEXCOORD0;
			  float2 uv1 : TEXCOORD1;
			  float2 uvLightmap : TEXCOORD2;
			  float2 uvMask : TEXCOORD3;
		  };

		  v2f vert(appdata v) {
			  v2f o;
			  o.pos = UnityObjectToClipPos(v.vertex);
			  o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);
			  o.uv1 = TRANSFORM_TEX(v.uv0, _MainTexB);
			  o.uvMask = v.color.xy;

			  #if LIGHTMAP_ON
			  o.uvLightmap = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
			  #else
			  o.uvLightmap = 0;
			  #endif

			  return o;
		  }

		  fixed4 frag(v2f i) : SV_Target {
			  half3 texA = tex2D(_MainTex, i.uv0).xyz;
			  half specA = tex2D(_SpecTex, i.uv0).x;

			  half4 texB = tex2D(_MainTexB, i.uv1);
			  half specB = tex2D(_SpecTexB, i.uv0).w;

			  half3 colA = 0;
			  half3 colB = 0;
			  half3 fixedLightDir = float3(0.57735, 0.57735, 0.57735);

			  #if LIGHTMAP_ON
			  // Lightmap active logic
			  half4 lightMap = half4(2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uvLightmap).xyz, 0.0);
			  half3 lm = lightMap.xyz * _LightMap;
			  half3 shadow = min(lm, (_Attenuation * 2.0) * 0.8);

			  half NdotL = max(0.0, (dot(fixedLightDir, _LightDir) * 0.5) + 1.0);

			  half3 diffA = ((texA * shadow) * NdotL) * _Color.xyz;
			  half3 specA_term = (((specA * shadow) * _SpecularPower)) * _SpecColor.xyz;
			  colA = diffA + specA_term;

			  half3 diffB = (((texB.xyz * shadow) * _LightPowerOfB) * _Color.xyz);
			  half3 specB_term = (((specB * shadow) * _SpecularPower)) * _SpecColor.xyz;
			  colB = diffB + specB_term;
		  #else
			  // Standard / Lightmap off logic
			  half3 shadow = _Attenuation * 2.0;
			  half3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			  half NdotL = max(0.0, dot(fixedLightDir, _LightDir));

			  half3 lightTermA = lerp((shadow * NdotL), (ambient + 1.0), ambient);
			  half3 diffA = (texA * lightTermA) * _Color.xyz;
			  half3 specA_term = (((specA * shadow) * _SpecularPower)) * _SpecColor.xyz;
			  colA = diffA + specA_term;

			  half3 lightTermB = lerp((shadow * _LightPowerOfB), (ambient + 1.0), ambient);
			  half3 diffB = (texB.xyz * lightTermB) * _Color.xyz;
			  half3 specB_term = (((specB * shadow) * _SpecularPower)) * _SpecColor.xyz;
			  colB = diffB + specB_term;
		  #endif

		  float mask = tex2D(_MaskMap, i.uvMask).w;
		  half3 finalCol = lerp(colB, colA, mask);

		  return fixed4(finalCol, 0.0);
	  }
	  ENDCG
	 }
	}

		Fallback "StarWarfare/Scene/Blend_NoReflection_Detail_Normal_Lightmap_LOD100"
}