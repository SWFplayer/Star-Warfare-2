Shader "MADFINGER/Transparent/GodRays" {
	Properties{
	 _MainTex("Base texture", 2D) = "white" {}
	 _FadeOutDistNear("Near fadeout dist", Float) = 10
	 _FadeOutDistFar("Far fadeout dist", Float) = 10000
	 _Multiplier("Multiplier", Float) = 1
	 _ContractionAmount("Near contraction amount", Float) = 5
	 _Color("Color", Color) = (1,1,1,1)
	}

		SubShader{
		 LOD 100
		 Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }

		 Pass {
		  Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
		  ZWrite Off
		  Cull Off
		  Fog { Mode Off }
		  Blend One One

		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  #include "UnityCG.cginc"

		  sampler2D _MainTex;
		  float4 _MainTex_ST;
		  float _FadeOutDistNear;
		  float _FadeOutDistFar;
		  float _Multiplier;
		  float _ContractionAmount;
		  float4 _Color;

		  struct appdata {
			  float4 vertex : POSITION;
			  float3 normal : NORMAL;
			  float2 uv0 : TEXCOORD0;
		  };

		  struct v2f {
			  float4 pos : SV_POSITION;
			  float2 uv : TEXCOORD0;
			  fixed4 vertexColor : TEXCOORD1;
		  };

		  v2f vert(appdata v) {
			  v2f o;

			  float3 viewPos = UnityObjectToViewPos(v.vertex);
			  float dist = sqrt(dot(viewPos, viewPos));

			  float fadeNear = clamp(dist / _FadeOutDistNear, 0.0, 1.0);
			  float fadeFar = 1.0 - clamp((max(dist - _FadeOutDistFar, 0.0) * 0.2), 0.0, 1.0);

			  float fadeNearSq = fadeNear * fadeNear;
			  float fadeFactor = (fadeNearSq * fadeNearSq) * (fadeFar * fadeFar);

			  float3 normal = normalize(v.normal);
			  float4 vpos = v.vertex;
			  vpos.xyz -= ((normal * clamp(1.0 - fadeFactor, 0.0, 1.0)) * _Color.w) * _ContractionAmount;

			  o.pos = UnityObjectToClipPos(vpos);
			  o.uv = TRANSFORM_TEX(v.uv0, _MainTex);
			  o.vertexColor = ((fadeFactor * _Color) * _Multiplier);

			  return o;
		  }

		  fixed4 frag(v2f i) : SV_Target {
			  fixed4 texColor = tex2D(_MainTex, i.uv);
			  return texColor * i.vertexColor;
		  }
		  ENDCG
		 }
	 }
}