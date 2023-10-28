Shader "Crying Onion/OMWS/Stylized Surface Shader"
{
	Properties
	{
		_MainColor("Main Color", Color) = (1,1,1,1)
		_Albedo("Albedo", 2D) = "white" {}
		[HDR]_EmissionColor("Emission Color", Color) = (1,1,1,1)
		_Emission("Emission", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
		_SmoothnessMultiplier("Smoothness Multiplier", Range( 0 , 1)) = 0.5
		_Smoothness("Smoothness", 2D) = "white" {}
		_MetallicMultiplier("Metallic Multiplier", Range( 0 , 1)) = 0.5
		_Metallic("Metallic", 2D) = "white" {}
		_SnowAttraction("Snow Attraction", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float4 OMWS_SnowColor;
		uniform sampler2D OMWS_SnowTexture;
		uniform float4 OMWS_SnowTexture_ST;
		uniform float4 _MainColor;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float OMWS_SnowScale;
		uniform float _SnowAttraction;
		uniform float OMWS_SnowAmount;
		uniform float4 _EmissionColor;
		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float _MetallicMultiplier;
		uniform sampler2D _Metallic;
		uniform float4 _Metallic_ST;
		uniform float OMWS_PuddleScale;
		uniform float OMWS_WetnessAmount;
		uniform float _SmoothnessMultiplier;
		uniform sampler2D _Smoothness;
		uniform float4 _Smoothness_ST;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float2 voronoihash5_g8( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi5_g8( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash5_g8( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		float2 voronoihash1_g7( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi1_g7( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash1_g7( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return (F2 + F1) * 0.5;
		}


		float2 voronoihash8_g7( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi8_g7( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash8_g7( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = f - g - o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			
			 		}
			 	}
			}
			return F1;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float4 Normal16 = tex2D( _Normal, uv_Normal );
			o.Normal = Normal16.rgb;
			float2 uvOMWS_SnowTexture = i.uv_texcoord * OMWS_SnowTexture_ST.xy + OMWS_SnowTexture_ST.zw;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float2 appendResult3_g8 = (float2(ase_worldPos.x , ase_worldPos.z));
			float temp_output_6_0_g8 = ( 1.0 / OMWS_SnowScale );
			float simplePerlin2D7_g8 = snoise( appendResult3_g8*temp_output_6_0_g8 );
			simplePerlin2D7_g8 = simplePerlin2D7_g8*0.5 + 0.5;
			float time5_g8 = 0.0;
			float2 voronoiSmoothId5_g8 = 0;
			float2 coords5_g8 = appendResult3_g8 * ( temp_output_6_0_g8 / 0.1 );
			float2 id5_g8 = 0;
			float2 uv5_g8 = 0;
			float voroi5_g8 = voronoi5_g8( coords5_g8, time5_g8, id5_g8, uv5_g8, 0, voronoiSmoothId5_g8 );
			float4 lerpResult19_g8 = lerp( ( OMWS_SnowColor * tex2D( OMWS_SnowTexture, uvOMWS_SnowTexture ) ) , ( _MainColor * tex2D( _Albedo, uv_Albedo ) ) , ( ( pow( ( pow( ase_worldNormal.y , 7.0 ) * ( simplePerlin2D7_g8 * ( 1.0 - voroi5_g8 ) ) ) , 0.5 ) * _SnowAttraction ) > ( 1.0 - OMWS_SnowAmount ) ? 0.0 : 1.0 ));
			float4 Albedo11 = lerpResult19_g8;
			o.Albedo = Albedo11.rgb;
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float4 Emission12 = ( _EmissionColor * tex2D( _Emission, uv_Emission ) );
			o.Emission = Emission12.rgb;
			float2 uv_Metallic = i.uv_texcoord * _Metallic_ST.xy + _Metallic_ST.zw;
			float4 Metallic24 = ( _MetallicMultiplier * tex2D( _Metallic, uv_Metallic ) );
			o.Metallic = Metallic24.r;
			float temp_output_5_0_g7 = ( 1.0 / OMWS_PuddleScale );
			float time1_g7 = 0.0;
			float2 voronoiSmoothId1_g7 = 0;
			float2 appendResult3_g7 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 coords1_g7 = appendResult3_g7 * temp_output_5_0_g7;
			float2 id1_g7 = 0;
			float2 uv1_g7 = 0;
			float voroi1_g7 = voronoi1_g7( coords1_g7, time1_g7, id1_g7, uv1_g7, 0, voronoiSmoothId1_g7 );
			float time8_g7 = 2.16;
			float2 voronoiSmoothId8_g7 = 0;
			float2 coords8_g7 = i.uv_texcoord * ( temp_output_5_0_g7 * 3.0 );
			float2 id8_g7 = 0;
			float2 uv8_g7 = 0;
			float voroi8_g7 = voronoi8_g7( coords8_g7, time8_g7, id8_g7, uv8_g7, 0, voronoiSmoothId8_g7 );
			float2 uv_Smoothness = i.uv_texcoord * _Smoothness_ST.xy + _Smoothness_ST.zw;
			float Smoothness18 = ( ( ase_worldNormal.y * 2.0 * ( (1.0 + (voroi1_g7 - 0.0) * (0.0 - 1.0) / (0.4 - 0.0)) + (0.1 + (voroi8_g7 - 0.0) * (-0.3 - 0.1) / (0.21 - 0.0)) ) * (0.3 + (OMWS_WetnessAmount - 0.0) * (1.0 - 0.3) / (1.0 - 0.0)) ) > ( 1.0 - ( OMWS_WetnessAmount * ( _SmoothnessMultiplier * tex2D( _Smoothness, uv_Smoothness ) ).r ) ) ? 1.0 : 0.0 );
			o.Smoothness = Smoothness18;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}