

Shader "StarWarfare/Scene/Emissive_Reflection_Detail_Normal_Lightmap" {
	Properties{
	 _Shininess("Shininess for Light Map", Range(0.03,1)) = 1
	 _LightMap("Brightness Light Map ", Float) = 0.4
	 _Color("Diffuse Color", Color) = (0.5,0.5,0.5,0.5)
	 _SpecColor("Specular Color", Color) = (1,1,1,1)
	 _SpecularPower("Specular", Float) = 1
	 _GlossPower("Gloss", Float) = 1
	 _UVDetailNormalTile("UV Detail Normal Tile", Float) = 1
	 _DetailNormalMulti("Detail Normal Multi", Float) = 1
	 _MainTex("Diffuse (RGB) Specular (A)", 2D) = "white" {}
	 _NormalTex("Normal (RGB)", 2D) = "bump" {}
	 _DetailNormal("Detail Normal map", 2D) = "white" {}
	 _EyeDir("EyeDirection", Vector) = (0,0,0,0)
	 _LightDir("LightDirection", Vector) = (0,0,0,0)
	 _Attenuation("Attenuation", Range(0,1)) = 0.5
	 _SpecTex("Specular Tex", 2D) = "Black" {}
	 _Emissive("Emissive", 2D) = "black" {}
	 _ReflectionAlpha("Reflection Alpha", 2D) = "black" {}
	 _ReflectionPower("Reflection Power", Float) = 0
	 _2DReflect("2D Reflection", 2D) = "black" {}
	 _Param1("reflection Parameter 1", Float) = 0.6
	 _Param2("reflection Parameter 2", Float) = 0.07
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
		  sampler2D _NormalTex;
		  sampler2D _DetailNormal;
		  sampler2D _SpecTex;
		  sampler2D _2DReflect;
		  sampler2D _Emissive;
		  sampler2D _ReflectionAlpha;
		  // sampler2D unity_Lightmap;
		  // float4 unity_LightmapST;

		  float _Shininess;
		  float _LightMap;
		  float4 _Color;
		  float4 _SpecColor;
		  float _SpecularPower;
		  float _GlossPower;
		  float _UVDetailNormalTile;
		  float _DetailNormalMulti;
		  float3 _EyeDir;
		  float3 _LightDir;
		  float _Attenuation;
		  float _ReflectionPower;
		  float _Param1;
		  float _Param2;

		  struct appdata {
			  float4 vertex : POSITION;
			  float2 uv0 : TEXCOORD0;
			  float2 uv1 : TEXCOORD1;
		  };

		  struct v2f {
			  float4 pos : SV_POSITION;
			  float2 uv0 : TEXCOORD0;
			  float2 uv1 : TEXCOORD1;
			  float2 uvReflect : TEXCOORD3;
		  };

		  v2f vert(appdata v) {
			  v2f o;
			  o.pos = UnityObjectToClipPos(v.vertex);
			  o.uv0 = TRANSFORM_TEX(v.uv0, _MainTex);

			  float3 worldSpace = mul(unity_ObjectToWorld, v.vertex).xyz;
			  worldSpace = (-_WorldSpaceCameraPos * _Param1 + worldSpace) * _Param2;
			  o.uvReflect = worldSpace.xy;

			  #if LIGHTMAP_ON
			  o.uv1 = v.uv1 * unity_LightmapST.xy + unity_LightmapST.zw;
			  #else
			  o.uv1 = 0;
			  #endif

			  return o;
		  }

		  fixed4 frag(v2f i) : SV_Target {
			  float4 tex = tex2D(_MainTex, i.uv0);
			  float4 reflectTex = tex2D(_2DReflect, i.uvReflect);
			  float4 reflectAlpha = tex2D(_ReflectionAlpha, i.uv0);
			  float specTex = tex2D(_SpecTex, i.uv0).r;
			  float3 emissive = tex2D(_Emissive, i.uv0).xyz;

			  float3 normal = tex2D(_NormalTex, i.uv0).xyz * 2.0 - 1.0;
			  float2 detailUV = i.uv0 * _UVDetailNormalTile;
			  float3 detailNormal = tex2D(_DetailNormal, detailUV).xyz * 2.0 - 1.0;
			  normal.xy += detailNormal.xy * _DetailNormalMulti;
			  normal = normalize(normal);

			  float3 col = 0;

			  #if LIGHTMAP_ON
			  
			  float4 lightMap = float4(2.0 * UNITY_SAMPLE_TEX2D(unity_Lightmap, i.uv1).xyz, 0.0);
			  float3 lm = lightMap.xyz * _LightMap;
			  float3 shadow = min(lm, (_Attenuation * 2.0) * 0.8);

			  float NdotL = max(0.0, (dot(normal, _LightDir) * 0.5) + 1.0);
			  float3 baseColor = tex.xyz + ((reflectTex.xyz * _ReflectionPower) * reflectAlpha.x);
			  float3 diff = ((baseColor * shadow) * NdotL) * _Color.xyz;
			  float3 spec = (((specTex * shadow) * _SpecularPower)) * _SpecColor.xyz;

			  col = diff + spec + emissive;
		  #else
			  
			  float3 shadow = _Attenuation * 2.0;
			  float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz;
			  float NdotL = max(0.0, dot(normal, _LightDir));

			  float3 lightTerm = lerp((shadow * NdotL), (ambient + 1.0), ambient);
			  float3 baseColor = tex.xyz + ((reflectTex.xyz * _ReflectionPower) * reflectAlpha.x);
			  float3 diff = (baseColor * lightTerm) * _Color.xyz;
			  float3 spec = (((specTex * shadow) * _SpecularPower)) * _SpecColor.xyz;

			  col = diff + spec + emissive;
		  #endif

		  return fixed4(col, 0.0);
	  }
	  ENDCG
	 }
	 }

		 Fallback "StarWarfare/Scene/Emissive_Reflection_Detail_Normal_Lightmap_LOD100"
}
