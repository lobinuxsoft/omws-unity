Shader "Crying Onion/OMWS/Stylized Terrain Add Pass"
{
	Properties
	{
		[HideInInspector]_TerrainHolesTexture("_TerrainHolesTexture", 2D) = "white" {}
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Normal0("Normal0", 2D) = "white" {}
		[HideInInspector]_Normal1("Normal1", 2D) = "white" {}
		[HideInInspector]_Normal2("Normal2", 2D) = "white" {}
		[HideInInspector]_Normal3("Normal3", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest-99" "IgnoreProjector"="True" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#define TERRAIN_SPLAT_ADDPASS
		#define TERRAIN_STANDARD_SHADER
		#pragma multi_compile_local __ _ALPHATEST_ON
		#pragma shader_feature_local _MASKMAP
		#pragma surface surf Standard keepalpha  decal:add
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Mask2;
		uniform sampler2D _Mask0;
		uniform sampler2D _Mask1;
		uniform sampler2D _Mask3;
		uniform float4 _MaskMapRemapScale0;
		uniform float4 _MaskMapRemapOffset2;
		uniform float4 _MaskMapRemapScale2;
		uniform float4 _MaskMapRemapScale1;
		uniform float4 _MaskMapRemapOffset1;
		uniform float4 _MaskMapRemapScale3;
		uniform float4 _MaskMapRemapOffset3;
		uniform float4 _MaskMapRemapOffset0;
		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _Normal0;
		uniform sampler2D _Splat0;
		uniform float4 _Splat0_ST;
		uniform sampler2D _Normal1;
		uniform sampler2D _Splat1;
		uniform float4 _Splat1_ST;
		uniform sampler2D _Normal2;
		uniform sampler2D _Splat2;
		uniform float4 _Splat2_ST;
		uniform sampler2D _Normal3;
		uniform sampler2D _Splat3;
		uniform float4 _Splat3_ST;
		uniform float _Smoothness0;
		uniform float _Smoothness1;
		uniform float _Smoothness2;
		uniform float _Smoothness3;
		uniform sampler2D _TerrainHolesTexture;
		uniform float4 _TerrainHolesTexture_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode5_g69 = tex2D( _Control, uv_Control );
			float dotResult20_g69 = dot( tex2DNode5_g69 , float4(1,1,1,1) );
			float SplatWeight22_g69 = dotResult20_g69;
			float localSplatClip74_g69 = ( SplatWeight22_g69 );
			float SplatWeight74_g69 = SplatWeight22_g69;
			{
			#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight74_g69 == 0.0f ? -1 : 1);
			#endif
			}
			float4 SplatControl26_g69 = ( tex2DNode5_g69 / ( localSplatClip74_g69 + 0.001 ) );
			float4 temp_output_59_0_g69 = SplatControl26_g69;
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			float4 weightedBlendVar8_g69 = temp_output_59_0_g69;
			float4 weightedBlend8_g69 = ( weightedBlendVar8_g69.x*tex2D( _Normal0, uv_Splat0 ) + weightedBlendVar8_g69.y*tex2D( _Normal1, uv_Splat1 ) + weightedBlendVar8_g69.z*tex2D( _Normal2, uv_Splat2 ) + weightedBlendVar8_g69.w*tex2D( _Normal3, uv_Splat3 ) );
			float3 temp_output_61_0_g69 = UnpackNormal( weightedBlend8_g69 );
			o.Normal = temp_output_61_0_g69;
			float4 appendResult33_g69 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
			float4 tex2DNode4_g69 = tex2D( _Splat0, uv_Splat0 );
			float3 _Vector1 = float3(1,1,1);
			float4 appendResult258_g69 = (float4(_Vector1 , 1.0));
			float4 tintLayer0253_g69 = appendResult258_g69;
			float4 appendResult36_g69 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
			float4 tex2DNode3_g69 = tex2D( _Splat1, uv_Splat1 );
			float3 _Vector2 = float3(1,1,1);
			float4 appendResult261_g69 = (float4(_Vector2 , 1.0));
			float4 tintLayer1254_g69 = appendResult261_g69;
			float4 appendResult39_g69 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
			float4 tex2DNode6_g69 = tex2D( _Splat2, uv_Splat2 );
			float3 _Vector3 = float3(1,1,1);
			float4 appendResult263_g69 = (float4(_Vector3 , 1.0));
			float4 tintLayer2255_g69 = appendResult263_g69;
			float4 appendResult42_g69 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
			float4 tex2DNode7_g69 = tex2D( _Splat3, uv_Splat3 );
			float3 _Vector4 = float3(1,1,1);
			float4 appendResult265_g69 = (float4(_Vector4 , 1.0));
			float4 tintLayer3256_g69 = appendResult265_g69;
			float4 weightedBlendVar9_g69 = temp_output_59_0_g69;
			float4 weightedBlend9_g69 = ( weightedBlendVar9_g69.x*( appendResult33_g69 * tex2DNode4_g69 * tintLayer0253_g69 ) + weightedBlendVar9_g69.y*( appendResult36_g69 * tex2DNode3_g69 * tintLayer1254_g69 ) + weightedBlendVar9_g69.z*( appendResult39_g69 * tex2DNode6_g69 * tintLayer2255_g69 ) + weightedBlendVar9_g69.w*( appendResult42_g69 * tex2DNode7_g69 * tintLayer3256_g69 ) );
			float4 MixDiffuse28_g69 = weightedBlend9_g69;
			float4 temp_output_60_0_g69 = MixDiffuse28_g69;
			float4 localClipHoles100_g69 = ( temp_output_60_0_g69 );
			float2 uv_TerrainHolesTexture = i.uv_texcoord * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
			float holeClipValue99_g69 = tex2D( _TerrainHolesTexture, uv_TerrainHolesTexture ).r;
			float Hole100_g69 = holeClipValue99_g69;
			{
			#ifdef _ALPHATEST_ON
				clip(Hole100_g69 == 0.0f ? -1 : 1);
			#endif
			}
			o.Albedo = localClipHoles100_g69.xyz;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}