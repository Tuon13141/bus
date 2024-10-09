Shader "Lpk/LightModel/ToonLightBase" {
	Properties {
		_BaseMap ("Texture", 2D) = "white" {}
		_BaseColor ("Color", Vector) = (0.5,0.5,0.5,1)
		[Space] _ShadowStep ("ShadowStep", Range(0, 1)) = 0.5
		_ShadowStepSmooth ("ShadowStepSmooth", Range(0, 1)) = 0.04
		[Space] _SpecularStep ("SpecularStep", Range(0, 1)) = 0.6
		_SpecularStepSmooth ("SpecularStepSmooth", Range(0, 1)) = 0.05
		[HDR] _SpecularColor ("SpecularColor", Vector) = (1,1,1,1)
		[Space] _RimStep ("RimStep", Range(0, 1)) = 0.65
		_RimStepSmooth ("RimStepSmooth", Range(0, 1)) = 0.4
		_RimColor ("RimColor", Vector) = (1,1,1,1)
		[Space] _OutlineWidth ("OutlineWidth", Range(0, 130)) = 0.15
		_OutlineColor ("OutlineColor", Vector) = (0,0,0,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}