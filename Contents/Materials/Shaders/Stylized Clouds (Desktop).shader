Shader "Crying Onion/OMWS/Stylized Clouds Desktop"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector][HDR][Header(General Cloud Settings)]_CloudColor("Cloud Color", Color) = (0.7264151,0.7264151,0.7264151,0)
		[HideInInspector][HDR]_MoonColor("Moon Color", Color) = (1,1,1,0)
		[HideInInspector][HDR]_CloudHighlightColor("Cloud Highlight Color", Color) = (1,1,1,0)
		[HideInInspector][HDR]_AltoCloudColor("Alto Cloud Color", Color) = (1,1,1,0)
		[HideInInspector]_ClippingThreshold("Clipping Threshold", Range( 0 , 1)) = 0.5
		[HideInInspector]_MainCloudScale("Main Cloud Scale", Float) = 10
		[HideInInspector]_MoonFlareFalloff("Moon Flare Falloff", Float) = 1
		[HideInInspector]_SunFlareFalloff("Sun Flare Falloff", Float) = 1
		[HideInInspector]_CloudFlareFalloff("Cloud Flare Falloff", Float) = 1
		[HideInInspector]_MaxCloudCover("Max Cloud Cover", Float) = 1
		[HideInInspector]_MinCloudCover("Min Cloud Cover", Float) = 0
		[HideInInspector]_WindSpeed("Wind Speed", Float) = 0
		[HideInInspector][Header(Cumulus Clouds)]_CumulusCoverageMultiplier("Cumulus Coverage Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_DetailScale("Detail Scale", Float) = 0.5
		[HideInInspector]_DetailAmount("Detail Amount", Float) = 1
		[HideInInspector][Header(Border Clouds)]_BorderHeight("Border Height", Range( 0 , 1)) = 1
		[HideInInspector]_BorderVariation("Border Variation", Range( 0 , 1)) = 1
		[HideInInspector]_BorderEffect("Border Effect", Range( -1 , 1)) = 0
		[HideInInspector][Header(Nimbus Clouds)]_NimbusMultiplier("Nimbus Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_NimbusVariation("Nimbus Variation", Range( 0 , 1)) = 1
		[HideInInspector]_NimbusHeight("Nimbus Height", Range( 0 , 1)) = 1
		[HideInInspector]_StormDirection("Storm Direction", Vector) = (0,0,0,0)
		[HideInInspector][Header(Altocumulus Clouds)]_AltocumulusMultiplier("Altocumulus Multiplier", Range( 0 , 2)) = 0
		[HideInInspector]_AltocumulusScale("Altocumulus Scale", Float) = 3
		[HideInInspector]_AltocumulusWindSpeed("Altocumulus Wind Speed", Vector) = (1,-2,0,0)
		[HideInInspector][Header(Cirrostratus Clouds)]_CirrostratusMultiplier("Cirrostratus Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_CirrostratusMoveSpeed("Cirrostratus Move Speed", Float) = 0
		_CirrostratusTexture("Cirrostratus Texture", 2D) = "white" {}
		[HideInInspector][Header(Cirrus Clouds)]_CirrusMultiplier("Cirrus Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_CirrusMoveSpeed("Cirrus Move Speed", Float) = 0
		_CirrusTexture("Cirrus Texture", 2D) = "white" {}
		[HideInInspector]_ChemtrailsMultiplier("Chemtrails Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_ChemtrailsMoveSpeed("Chemtrails Move Speed", Float) = 0
		[ASEEnd]_ChemtrailsTexture("Chemtrails Texture", 2D) = "white" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" "Queue"="Transparent" }

		Cull Off
		AlphaToMask Off

		Stencil
		{
			Ref 221
			Pass Zero
		}

		HLSLINCLUDE

		#pragma target 3.0

		#pragma prefer_hlslcc gles
		#pragma exclude_renderers d3d11_9x 

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS

		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend One Zero, One Zero
			ZWrite On
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 110000


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudColor;
			float4 _CloudHighlightColor;
			float4 _AltoCloudColor;
			float4 _MoonColor;
			float3 _StormDirection;
			float2 _AltocumulusWindSpeed;
			float _MaxCloudCover;
			float _AltocumulusMultiplier;
			float _AltocumulusScale;
			float _WindSpeed;
			half _CloudFlareFalloff;
			float _ClippingThreshold;
			float _CirrusMultiplier;
			float _CirrusMoveSpeed;
			float _ChemtrailsMultiplier;
			float _ChemtrailsMoveSpeed;
			float _NimbusVariation;
			float _NimbusMultiplier;
			float _NimbusHeight;
			float _CirrostratusMoveSpeed;
			float _BorderEffect;
			float _BorderVariation;
			float _BorderHeight;
			float _DetailAmount;
			float _DetailScale;
			float _MainCloudScale;
			half _MoonFlareFalloff;
			half _SunFlareFalloff;
			float _CumulusCoverageMultiplier;
			float _MinCloudCover;
			float _CirrostratusMultiplier;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			float3 OMWS_SunDirection;
			float3 OMWS_MoonDirection;
			sampler2D _ChemtrailsTexture;
			sampler2D _CirrusTexture;
			sampler2D _CirrostratusTexture;


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
			
					float2 voronoihash148( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi148( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash148( n + g );
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
			
					float2 voronoihash234( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi234( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash234( n + g );
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
			
					float2 voronoihash77( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi77( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash77( n + g );
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
			
					float2 voronoihash601( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi601( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash601( n + g );
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
			
					float2 voronoihash492( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi492( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash492( n + g );
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
			
					float2 voronoihash463( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi463( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash463( n + g );
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
			

			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord3.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 CloudColor332 = _CloudColor;
				float4 CloudHighlightColor334 = _CloudHighlightColor;
				float2 texCoord94 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 Pos159 = texCoord94;
				float mulTime61 = _TimeParameters.x * ( 0.001 * _WindSpeed );
				float TIme152 = mulTime61;
				float simplePerlin2D37 = snoise( ( Pos159 + ( TIme152 * float2( 0.2,-0.4 ) ) )*( 100.0 / _MainCloudScale ) );
				simplePerlin2D37 = simplePerlin2D37*0.5 + 0.5;
				float SimpleCloudDensity314 = simplePerlin2D37;
				float time148 = 0.0;
				float2 voronoiSmoothId148 = 0;
				float2 temp_output_66_0 = ( Pos159 + ( TIme152 * float2( 0.3,0.2 ) ) );
				float2 coords148 = temp_output_66_0 * ( 140.0 / _MainCloudScale );
				float2 id148 = 0;
				float2 uv148 = 0;
				float voroi148 = voronoi148( coords148, time148, id148, uv148, 0, voronoiSmoothId148 );
				float time234 = 0.0;
				float2 voronoiSmoothId234 = 0;
				float2 coords234 = temp_output_66_0 * ( 500.0 / _MainCloudScale );
				float2 id234 = 0;
				float2 uv234 = 0;
				float voroi234 = voronoi234( coords234, time234, id234, uv234, 0, voronoiSmoothId234 );
				float2 appendResult312 = (float2(voroi148 , voroi234));
				float2 VoroDetails313 = appendResult312;
				float simplePerlin2D80 = snoise( ( ( float2( 5,20 ) * TIme152 ) + Pos159 )*( 100.0 / 400.0 ) );
				simplePerlin2D80 = simplePerlin2D80*0.5 + 0.5;
				float CurrentCloudCover240 = (_MinCloudCover + (simplePerlin2D80 - 0.0) * (_MaxCloudCover - _MinCloudCover) / (1.0 - 0.0));
				float CumulusCoverage376 = _CumulusCoverageMultiplier;
				float ComplexCloudDensity344 = (0.0 + (min( SimpleCloudDensity314 , ( 1.0 - VoroDetails313.x ) ) - ( 1.0 - ( CurrentCloudCover240 * CumulusCoverage376 ) )) * (1.0 - 0.0) / (1.0 - ( 1.0 - ( CurrentCloudCover240 * CumulusCoverage376 ) )));
				float4 lerpResult53 = lerp( CloudHighlightColor334 , CloudColor332 , saturate( (2.0 + (ComplexCloudDensity344 - 0.0) * (0.7 - 2.0) / (1.0 - 0.0)) ));
				float3 normalizeResult259 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult261 = dot( normalizeResult259 , OMWS_SunDirection );
				float temp_output_264_0 = abs( (dotResult261*0.5 + 0.5) );
				half LightMask267 = saturate( pow( temp_output_264_0 , _SunFlareFalloff ) );
				float2 appendResult318 = (float2(_MinCloudCover , _MaxCloudCover));
				float2 RequestedCloudCover317 = appendResult318;
				float CloudThicknessDetails329 = ( VoroDetails313.y * saturate( ( ( RequestedCloudCover317 * CumulusCoverage376 ).y - 0.8 ) ) );
				float3 normalizeResult779 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float dotResult780 = dot( normalizeResult779 , OMWS_MoonDirection );
				half MoonlightMask790 = saturate( pow( abs( (dotResult780*0.5 + 0.5) ) , _MoonFlareFalloff ) );
				float4 MoonlightColor797 = _MoonColor;
				float4 lerpResult227 = lerp( ( lerpResult53 + ( LightMask267 * CloudHighlightColor334 * ( 1.0 - CloudThicknessDetails329 ) ) + ( MoonlightMask790 * MoonlightColor797 * ( 1.0 - CloudThicknessDetails329 ) ) ) , ( CloudColor332 * float4( 0.5660378,0.5660378,0.5660378,0 ) ) , CloudThicknessDetails329);
				float time77 = 0.0;
				float2 voronoiSmoothId77 = 0;
				float2 coords77 = ( Pos159 + ( TIme152 * float2( 0.3,0.2 ) ) ) * ( 100.0 / _DetailScale );
				float2 id77 = 0;
				float2 uv77 = 0;
				float fade77 = 0.5;
				float voroi77 = 0;
				float rest77 = 0;
				for( int it77 = 0; it77 <3; it77++ ){
				voroi77 += fade77 * voronoi77( coords77, time77, id77, uv77, 0,voronoiSmoothId77 );
				rest77 += fade77;
				coords77 *= 2;
				fade77 *= 0.5;
				}//Voronoi77
				voroi77 /= rest77;
				float temp_output_47_0 = ( (0.0 + (( 1.0 - voroi77 ) - 0.3) * (0.5 - 0.0) / (1.0 - 0.3)) * 0.1 * _DetailAmount );
				float DetailedClouds347 = saturate( ( ComplexCloudDensity344 + temp_output_47_0 ) );
				float CloudDetail294 = temp_output_47_0;
				float2 texCoord113 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_114_0 = ( texCoord113 - float2( 0.5,0.5 ) );
				float dotResult140 = dot( temp_output_114_0 , temp_output_114_0 );
				float BorderHeight386 = ( 1.0 - _BorderHeight );
				float temp_output_392_0 = ( -2.0 * ( 1.0 - _BorderVariation ) );
				float clampResult420 = clamp( ( ( ( CloudDetail294 + SimpleCloudDensity314 ) * saturate( (( BorderHeight386 * temp_output_392_0 ) + (dotResult140 - 0.0) * (( temp_output_392_0 * -4.0 ) - ( BorderHeight386 * temp_output_392_0 )) / (0.5 - 0.0)) ) ) * 10.0 * _BorderEffect ) , -1.0 , 1.0 );
				float BorderLightTransport418 = clampResult420;
				float3 normalizeResult745 = normalize( ( WorldPosition - _WorldSpaceCameraPos ) );
				float3 normalizeResult773 = normalize( _StormDirection );
				float dotResult743 = dot( normalizeResult745 , normalizeResult773 );
				float2 texCoord721 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_726_0 = ( texCoord721 - float2( 0.5,0.5 ) );
				float dotResult728 = dot( temp_output_726_0 , temp_output_726_0 );
				float temp_output_763_0 = ( -2.0 * ( 1.0 - ( _NimbusVariation * 0.9 ) ) );
				float NimbusLightTransport739 = saturate( ( ( ( CloudDetail294 + SimpleCloudDensity314 ) * saturate( (( ( 1.0 - _NimbusMultiplier ) * temp_output_763_0 ) + (( dotResult743 + ( _NimbusHeight * 4.0 * dotResult728 ) ) - 0.5) * (( temp_output_763_0 * -4.0 ) - ( ( 1.0 - _NimbusMultiplier ) * temp_output_763_0 )) / (7.0 - 0.5)) ) ) * 10.0 ) );
				float mulTime566 = _TimeParameters.x * 0.01;
				float simplePerlin2D563 = snoise( (Pos159*1.0 + mulTime566)*2.0 );
				float mulTime560 = _TimeParameters.x * _ChemtrailsMoveSpeed;
				float cos553 = cos( ( mulTime560 * 0.01 ) );
				float sin553 = sin( ( mulTime560 * 0.01 ) );
				float2 rotator553 = mul( Pos159 - float2( 0.5,0.5 ) , float2x2( cos553 , -sin553 , sin553 , cos553 )) + float2( 0.5,0.5 );
				float cos561 = cos( ( mulTime560 * -0.02 ) );
				float sin561 = sin( ( mulTime560 * -0.02 ) );
				float2 rotator561 = mul( Pos159 - float2( 0.5,0.5 ) , float2x2( cos561 , -sin561 , sin561 , cos561 )) + float2( 0.5,0.5 );
				float mulTime568 = _TimeParameters.x * 0.01;
				float simplePerlin2D570 = snoise( (Pos159*1.0 + mulTime568)*4.0 );
				float4 ChemtrailsPattern576 = ( ( saturate( simplePerlin2D563 ) * tex2D( _ChemtrailsTexture, (rotator553*0.5 + 0.0) ) ) + ( tex2D( _ChemtrailsTexture, rotator561 ) * saturate( simplePerlin2D570 ) ) );
				float2 texCoord583 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_581_0 = ( texCoord583 - float2( 0.5,0.5 ) );
				float dotResult584 = dot( temp_output_581_0 , temp_output_581_0 );
				float ChemtrailsFinal590 = ( ( ChemtrailsPattern576 * saturate( (0.4 + (dotResult584 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - ( _ChemtrailsMultiplier * 0.5 ) ) ? 1.0 : 0.0 );
				float mulTime673 = _TimeParameters.x * 0.01;
				float simplePerlin2D681 = snoise( (Pos159*1.0 + mulTime673)*2.0 );
				float mulTime666 = _TimeParameters.x * _CirrusMoveSpeed;
				float cos677 = cos( ( mulTime666 * 0.01 ) );
				float sin677 = sin( ( mulTime666 * 0.01 ) );
				float2 rotator677 = mul( Pos159 - float2( 0.5,0.5 ) , float2x2( cos677 , -sin677 , sin677 , cos677 )) + float2( 0.5,0.5 );
				float cos676 = cos( ( mulTime666 * -0.02 ) );
				float sin676 = sin( ( mulTime666 * -0.02 ) );
				float2 rotator676 = mul( Pos159 - float2( 0.5,0.5 ) , float2x2( cos676 , -sin676 , sin676 , cos676 )) + float2( 0.5,0.5 );
				float mulTime670 = _TimeParameters.x * 0.01;
				float simplePerlin2D682 = snoise( (Pos159*1.0 + mulTime670) );
				simplePerlin2D682 = simplePerlin2D682*0.5 + 0.5;
				float4 CirrusPattern696 = ( ( saturate( simplePerlin2D681 ) * tex2D( _CirrusTexture, (rotator677*1.5 + 0.75) ) ) + ( tex2D( _CirrusTexture, (rotator676*1.0 + 0.0) ) * saturate( simplePerlin2D682 ) ) );
				float2 texCoord685 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_690_0 = ( texCoord685 - float2( 0.5,0.5 ) );
				float dotResult694 = dot( temp_output_690_0 , temp_output_690_0 );
				float4 temp_output_701_0 = ( CirrusPattern696 * saturate( (0.0 + (dotResult694 - 0.0) * (2.0 - 0.0) / (0.2 - 0.0)) ) );
				float Clipping515 = _ClippingThreshold;
				float CirrusAlpha715 = ( ( temp_output_701_0 * ( _CirrusMultiplier * 10.0 ) ).r > Clipping515 ? 1.0 : 0.0 );
				float SimpleRadiance506 = saturate( ( DetailedClouds347 + BorderLightTransport418 + NimbusLightTransport739 + ChemtrailsFinal590 + CirrusAlpha715 ) );
				float4 lerpResult171 = lerp( CloudColor332 , lerpResult227 , ( 1.0 - SimpleRadiance506 ));
				float CloudLight271 = saturate( pow( temp_output_264_0 , _CloudFlareFalloff ) );
				float4 lerpResult272 = lerp( float4( 0,0,0,0 ) , CloudHighlightColor334 , ( saturate( ( ( CurrentCloudCover240 * CumulusCoverage376 ) - 1.0 ) ) * CloudDetail294 * CloudLight271 ));
				float4 SunThroughClouds273 = ( lerpResult272 * 1.3 );
				float4 CirrusCustomLightColor512 = ( CloudColor332 * _AltoCloudColor );
				float time601 = 0.0;
				float2 voronoiSmoothId601 = 0;
				float mulTime602 = _TimeParameters.x * 0.003;
				float2 coords601 = (Pos159*1.0 + ( float2( 1,-2 ) * mulTime602 )) * 10.0;
				float2 id601 = 0;
				float2 uv601 = 0;
				float voroi601 = voronoi601( coords601, time601, id601, uv601, 0, voronoiSmoothId601 );
				float time492 = ( 10.0 * mulTime602 );
				float2 voronoiSmoothId492 = 0;
				float2 coords492 = IN.ase_texcoord3.xy * 10.0;
				float2 id492 = 0;
				float2 uv492 = 0;
				float voroi492 = voronoi492( coords492, time492, id492, uv492, 0, voronoiSmoothId492 );
				float AltoCumulusPlacement461 = saturate( ( ( ( 1.0 - 0.0 ) - (1.0 + (voroi601 - 0.0) * (-0.5 - 1.0) / (1.0 - 0.0)) ) - voroi492 ) );
				float time463 = 51.2;
				float2 voronoiSmoothId463 = 0;
				float2 coords463 = (Pos159*1.0 + ( _AltocumulusWindSpeed * TIme152 )) * ( 100.0 / _AltocumulusScale );
				float2 id463 = 0;
				float2 uv463 = 0;
				float fade463 = 0.5;
				float voroi463 = 0;
				float rest463 = 0;
				for( int it463 = 0; it463 <2; it463++ ){
				voroi463 += fade463 * voronoi463( coords463, time463, id463, uv463, 0,voronoiSmoothId463 );
				rest463 += fade463;
				coords463 *= 2;
				fade463 *= 0.5;
				}//Voronoi463
				voroi463 /= rest463;
				float AltoCumulusLightTransport447 = ( ( AltoCumulusPlacement461 * ( 0.1 > voroi463 ? (0.5 + (voroi463 - 0.0) * (0.0 - 0.5) / (0.15 - 0.0)) : 0.0 ) * _AltocumulusMultiplier ) > 0.2 ? 1.0 : 0.0 );
				float ACCustomLightsClipping521 = ( AltoCumulusLightTransport447 * ( SimpleRadiance506 > Clipping515 ? 0.0 : 1.0 ) );
				float mulTime611 = _TimeParameters.x * 0.01;
				float simplePerlin2D620 = snoise( (Pos159*1.0 + mulTime611)*2.0 );
				float mulTime607 = _TimeParameters.x * _CirrostratusMoveSpeed;
				float cos615 = cos( ( mulTime607 * 0.01 ) );
				float sin615 = sin( ( mulTime607 * 0.01 ) );
				float2 rotator615 = mul( Pos159 - float2( 0.5,0.5 ) , float2x2( cos615 , -sin615 , sin615 , cos615 )) + float2( 0.5,0.5 );
				float cos621 = cos( ( mulTime607 * -0.02 ) );
				float sin621 = sin( ( mulTime607 * -0.02 ) );
				float2 rotator621 = mul( Pos159 - float2( 0.5,0.5 ) , float2x2( cos621 , -sin621 , sin621 , cos621 )) + float2( 0.5,0.5 );
				float mulTime612 = _TimeParameters.x * 0.01;
				float simplePerlin2D618 = snoise( (Pos159*10.0 + mulTime612)*4.0 );
				float4 CirrostratPattern634 = ( ( saturate( simplePerlin2D620 ) * tex2D( _CirrostratusTexture, (rotator615*1.5 + 0.75) ) ) + ( tex2D( _CirrostratusTexture, (rotator621*1.5 + 0.75) ) * saturate( simplePerlin2D618 ) ) );
				float2 texCoord625 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_629_0 = ( texCoord625 - float2( 0.5,0.5 ) );
				float dotResult630 = dot( temp_output_629_0 , temp_output_629_0 );
				float clampResult798 = clamp( ( _CirrostratusMultiplier * 0.5 ) , 0.0 , 0.98 );
				float CirrostratLightTransport641 = ( ( CirrostratPattern634 * saturate( (0.4 + (dotResult630 - 0.0) * (2.0 - 0.4) / (0.1 - 0.0)) ) ).r > ( 1.0 - clampResult798 ) ? 1.0 : 0.0 );
				float CSCustomLightsClipping648 = ( CirrostratLightTransport641 * ( SimpleRadiance506 > Clipping515 ? 0.0 : 1.0 ) );
				float CustomRadiance657 = saturate( ( ACCustomLightsClipping521 + CSCustomLightsClipping648 ) );
				float4 lerpResult522 = lerp( ( lerpResult171 + SunThroughClouds273 ) , CirrusCustomLightColor512 , CustomRadiance657);
				float FinalAlpha408 = saturate( ( DetailedClouds347 + BorderLightTransport418 + AltoCumulusLightTransport447 + ChemtrailsFinal590 + CirrostratLightTransport641 + CirrusAlpha715 + NimbusLightTransport739 ) );
				clip( FinalAlpha408 - Clipping515);
				float4 FinalCloudColor351 = lerpResult522;
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = FinalCloudColor351.rgb;
				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			ZWrite On
			ZTest LEqual
			AlphaToMask Off
			ColorMask 0

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 110000

			
			#pragma vertex vert
			#pragma fragment frag

			#if ASE_SRP_VERSION >= 110000
				#pragma multi_compile _ _CASTING_PUNCTUAL_LIGHT_SHADOW
			#endif

			#define SHADERPASS SHADERPASS_SHADOWCASTER

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudColor;
			float4 _CloudHighlightColor;
			float4 _AltoCloudColor;
			float4 _MoonColor;
			float3 _StormDirection;
			float2 _AltocumulusWindSpeed;
			float _MaxCloudCover;
			float _AltocumulusMultiplier;
			float _AltocumulusScale;
			float _WindSpeed;
			half _CloudFlareFalloff;
			float _ClippingThreshold;
			float _CirrusMultiplier;
			float _CirrusMoveSpeed;
			float _ChemtrailsMultiplier;
			float _ChemtrailsMoveSpeed;
			float _NimbusVariation;
			float _NimbusMultiplier;
			float _NimbusHeight;
			float _CirrostratusMoveSpeed;
			float _BorderEffect;
			float _BorderVariation;
			float _BorderHeight;
			float _DetailAmount;
			float _DetailScale;
			float _MainCloudScale;
			half _MoonFlareFalloff;
			half _SunFlareFalloff;
			float _CumulusCoverageMultiplier;
			float _MinCloudCover;
			float _CirrostratusMultiplier;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			

			
			float3 _LightDirection;
			#if ASE_SRP_VERSION >= 110000 
				float3 _LightPosition;
			#endif

			VertexOutput VertexFunction( VertexInput v )
			{
				VertexOutput o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				float3 normalWS = TransformObjectToWorldDir( v.ase_normal );

				#if ASE_SRP_VERSION >= 110000 
				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif

				float4 clipPos = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

					#if UNITY_REVERSED_Z
						clipPos.z = min(clipPos.z, UNITY_NEAR_CLIP_VALUE);
					#else
						clipPos.z = max(clipPos.z, UNITY_NEAR_CLIP_VALUE);
					#endif
				#else
						float4 clipPos = TransformWorldToHClip( ApplyShadowBias( positionWS, normalWS, _LightDirection ) );

						#if UNITY_REVERSED_Z
							clipPos.z = min(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
						#else
							clipPos.z = max(clipPos.z, clipPos.w * UNITY_NEAR_CLIP_VALUE);
						#endif
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				o.clipPos = clipPos;

				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					#ifdef _ALPHATEST_SHADOW_ON
						clip(Alpha - AlphaClipThresholdShadow);
					#else
						clip(Alpha - AlphaClipThreshold);
					#endif
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			#define ASE_SRP_VERSION 110000

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _CloudColor;
			float4 _CloudHighlightColor;
			float4 _AltoCloudColor;
			float4 _MoonColor;
			float3 _StormDirection;
			float2 _AltocumulusWindSpeed;
			float _MaxCloudCover;
			float _AltocumulusMultiplier;
			float _AltocumulusScale;
			float _WindSpeed;
			half _CloudFlareFalloff;
			float _ClippingThreshold;
			float _CirrusMultiplier;
			float _CirrusMoveSpeed;
			float _ChemtrailsMultiplier;
			float _ChemtrailsMoveSpeed;
			float _NimbusVariation;
			float _NimbusMultiplier;
			float _NimbusHeight;
			float _CirrostratusMoveSpeed;
			float _BorderEffect;
			float _BorderVariation;
			float _BorderHeight;
			float _DetailAmount;
			float _DetailScale;
			float _MainCloudScale;
			half _MoonFlareFalloff;
			half _SunFlareFalloff;
			float _CumulusCoverageMultiplier;
			float _MinCloudCover;
			float _CirrostratusMultiplier;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			

			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				

				float Alpha = 1;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

	
	}
	
	
	Fallback "Hidden/InternalErrorShader"
	
}