Shader "Crying Onion/OMWS/Stylized Clouds Ghibli"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector][HDR][Header(General Cloud Settings)]_CloudColor("Cloud Color", Color) = (0.7264151,0.7264151,0.7264151,0)
		[HideInInspector][HDR]_CloudHighlightColor("Cloud Highlight Color", Color) = (1,1,1,0)
		[HideInInspector]_WindSpeed("Wind Speed", Float) = 0
		[HideInInspector][Header(Cumulus Clouds)]_CumulusCoverageMultiplier("Cumulus Coverage Multiplier", Range( 0 , 2)) = 1
		[HideInInspector]_MaxCloudCover("Max Cloud Cover", Float) = 1
		[HideInInspector]_CloudCohesion("Cloud Cohesion", Range( 0 , 1)) = 0.887
		[HideInInspector]_MainCloudScale("Main Cloud Scale", Float) = 0.8
		[HideInInspector]_Spherize("Spherize", Range( 0 , 1)) = 0.36
		[HideInInspector]_ShadowingDistance("Shadowing Distance", Range( 0 , 0.1)) = 0.07
		[HideInInspector]_MainCloudWindDir("Main Cloud Wind Dir", Vector) = (0.1,0.2,0,0)
		[HideInInspector][HDR]_SecondLayer("Second Layer", Color) = (0.8396226,0.8396226,0.8396226,0)
		[ASEEnd][HDR]_AltoCloudColor("Alto Cloud Color", Color) = (0.8160377,0.9787034,1,0)
		[HideInInspector]_CloudThickness("CloudThickness", Range( 0 , 4)) = 1
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent+1" }

		Cull Front
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
			
			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 110000


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			
			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"


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
			float4 _AltoCloudColor;
			float4 _CloudHighlightColor;
			float4 _CloudColor;
			float4 _SecondLayer;
			float2 _MainCloudWindDir;
			float _Spherize;
			float _MainCloudScale;
			float _WindSpeed;
			float _CloudCohesion;
			float _CumulusCoverageMultiplier;
			float _MaxCloudCover;
			float _ShadowingDistance;
			float _CloudThickness;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			

			
					float2 voronoihash35_g52( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g52( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g52( n + g );
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
			
					float2 voronoihash13_g52( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g52( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g52( n + g );
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
			
					float2 voronoihash11_g52( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g52( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g52( n + g );
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
			
					float2 voronoihash35_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g49( n + g );
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
			
					float2 voronoihash13_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g49( n + g );
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
			
					float2 voronoihash11_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g49( n + g );
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
			
					float2 voronoihash35_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g50( n + g );
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
			
					float2 voronoihash13_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g50( n + g );
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
			
					float2 voronoihash11_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g50( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g51( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g51( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g51( n + g );
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
			
					float2 voronoihash13_g51( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g51( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g51( n + g );
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
			
					float2 voronoihash11_g51( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g51( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g51( n + g );
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
			
					float2 voronoihash35_g53( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g53( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g53( n + g );
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
			
					float2 voronoihash13_g53( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g53( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g53( n + g );
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
			
					float2 voronoihash11_g53( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g53( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g53( n + g );
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

				float4 CloudHighlightColor334 = _CloudHighlightColor;
				float4 CloudColor332 = _CloudColor;
				Gradient gradient1145 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.5411765 ), float4( 1, 1, 1, 0.6441138 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord1042 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1043_0 = ( texCoord1042 - float2( 0.5,0.5 ) );
				float dotResult1045 = dot( temp_output_1043_0 , temp_output_1043_0 );
				float Dot1071 = saturate( (0.85 + (dotResult1045 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g52 = 0.0;
				float2 voronoiSmoothId35_g52 = 0;
				float2 texCoord955 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV998 = ( texCoord955 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g52 = (CentralUV998*1.58 + 0.0);
				float2 break2_g52 = abs( temp_output_21_0_g52 );
				float saferPower4_g52 = abs( break2_g52.x );
				float saferPower3_g52 = abs( break2_g52.y );
				float saferPower6_g52 = abs( ( pow( saferPower4_g52 , 2.0 ) + pow( saferPower3_g52 , 2.0 ) ) );
				float Spherize1078 = _Spherize;
				float Flatness1076 = ( 20.0 * _Spherize );
				float Scale1080 = ( _MainCloudScale * 0.1 );
				float mulTime61 = _TimeParameters.x * ( 0.001 * _WindSpeed );
				float Time152 = mulTime61;
				float2 Wind1035 = ( Time152 * _MainCloudWindDir );
				float2 temp_output_10_0_g52 = (( ( temp_output_21_0_g52 * ( pow( saferPower6_g52 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
				float2 coords35_g52 = temp_output_10_0_g52 * 60.0;
				float2 id35_g52 = 0;
				float2 uv35_g52 = 0;
				float fade35_g52 = 0.5;
				float voroi35_g52 = 0;
				float rest35_g52 = 0;
				for( int it35_g52 = 0; it35_g52 <2; it35_g52++ ){
				voroi35_g52 += fade35_g52 * voronoi35_g52( coords35_g52, time35_g52, id35_g52, uv35_g52, 0,voronoiSmoothId35_g52 );
				rest35_g52 += fade35_g52;
				coords35_g52 *= 2;
				fade35_g52 *= 0.5;
				}//Voronoi35_g52
				voroi35_g52 /= rest35_g52;
				float time13_g52 = 0.0;
				float2 voronoiSmoothId13_g52 = 0;
				float2 coords13_g52 = temp_output_10_0_g52 * 25.0;
				float2 id13_g52 = 0;
				float2 uv13_g52 = 0;
				float fade13_g52 = 0.5;
				float voroi13_g52 = 0;
				float rest13_g52 = 0;
				for( int it13_g52 = 0; it13_g52 <2; it13_g52++ ){
				voroi13_g52 += fade13_g52 * voronoi13_g52( coords13_g52, time13_g52, id13_g52, uv13_g52, 0,voronoiSmoothId13_g52 );
				rest13_g52 += fade13_g52;
				coords13_g52 *= 2;
				fade13_g52 *= 0.5;
				}//Voronoi13_g52
				voroi13_g52 /= rest13_g52;
				float time11_g52 = 17.23;
				float2 voronoiSmoothId11_g52 = 0;
				float2 coords11_g52 = temp_output_10_0_g52 * 9.0;
				float2 id11_g52 = 0;
				float2 uv11_g52 = 0;
				float fade11_g52 = 0.5;
				float voroi11_g52 = 0;
				float rest11_g52 = 0;
				for( int it11_g52 = 0; it11_g52 <2; it11_g52++ ){
				voroi11_g52 += fade11_g52 * voronoi11_g52( coords11_g52, time11_g52, id11_g52, uv11_g52, 0,voronoiSmoothId11_g52 );
				rest11_g52 += fade11_g52;
				coords11_g52 *= 2;
				fade11_g52 *= 0.5;
				}//Voronoi11_g52
				voroi11_g52 /= rest11_g52;
				float2 texCoord1055 = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1056_0 = ( texCoord1055 - float2( 0.5,0.5 ) );
				float dotResult1057 = dot( temp_output_1056_0 , temp_output_1056_0 );
				float ModifiedCohesion1074 = ( _CloudCohesion * 1.0 * ( 1.0 - dotResult1057 ) );
				float lerpResult15_g52 = lerp( saturate( ( voroi35_g52 + voroi13_g52 ) ) , voroi11_g52 , ModifiedCohesion1074);
				float CumulusCoverage376 = ( _CumulusCoverageMultiplier * _MaxCloudCover );
				float lerpResult16_g52 = lerp( lerpResult15_g52 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float time35_g49 = 0.0;
				float2 voronoiSmoothId35_g49 = 0;
				float2 temp_output_21_0_g49 = CentralUV998;
				float2 break2_g49 = abs( temp_output_21_0_g49 );
				float saferPower4_g49 = abs( break2_g49.x );
				float saferPower3_g49 = abs( break2_g49.y );
				float saferPower6_g49 = abs( ( pow( saferPower4_g49 , 2.0 ) + pow( saferPower3_g49 , 2.0 ) ) );
				float2 temp_output_10_0_g49 = (( ( temp_output_21_0_g49 * ( pow( saferPower6_g49 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
				float2 coords35_g49 = temp_output_10_0_g49 * 60.0;
				float2 id35_g49 = 0;
				float2 uv35_g49 = 0;
				float fade35_g49 = 0.5;
				float voroi35_g49 = 0;
				float rest35_g49 = 0;
				for( int it35_g49 = 0; it35_g49 <2; it35_g49++ ){
				voroi35_g49 += fade35_g49 * voronoi35_g49( coords35_g49, time35_g49, id35_g49, uv35_g49, 0,voronoiSmoothId35_g49 );
				rest35_g49 += fade35_g49;
				coords35_g49 *= 2;
				fade35_g49 *= 0.5;
				}//Voronoi35_g49
				voroi35_g49 /= rest35_g49;
				float time13_g49 = 0.0;
				float2 voronoiSmoothId13_g49 = 0;
				float2 coords13_g49 = temp_output_10_0_g49 * 25.0;
				float2 id13_g49 = 0;
				float2 uv13_g49 = 0;
				float fade13_g49 = 0.5;
				float voroi13_g49 = 0;
				float rest13_g49 = 0;
				for( int it13_g49 = 0; it13_g49 <2; it13_g49++ ){
				voroi13_g49 += fade13_g49 * voronoi13_g49( coords13_g49, time13_g49, id13_g49, uv13_g49, 0,voronoiSmoothId13_g49 );
				rest13_g49 += fade13_g49;
				coords13_g49 *= 2;
				fade13_g49 *= 0.5;
				}//Voronoi13_g49
				voroi13_g49 /= rest13_g49;
				float time11_g49 = 17.23;
				float2 voronoiSmoothId11_g49 = 0;
				float2 coords11_g49 = temp_output_10_0_g49 * 9.0;
				float2 id11_g49 = 0;
				float2 uv11_g49 = 0;
				float fade11_g49 = 0.5;
				float voroi11_g49 = 0;
				float rest11_g49 = 0;
				for( int it11_g49 = 0; it11_g49 <2; it11_g49++ ){
				voroi11_g49 += fade11_g49 * voronoi11_g49( coords11_g49, time11_g49, id11_g49, uv11_g49, 0,voronoiSmoothId11_g49 );
				rest11_g49 += fade11_g49;
				coords11_g49 *= 2;
				fade11_g49 *= 0.5;
				}//Voronoi11_g49
				voroi11_g49 /= rest11_g49;
				float lerpResult15_g49 = lerp( saturate( ( voroi35_g49 + voroi13_g49 ) ) , voroi11_g49 , ModifiedCohesion1074);
				float lerpResult16_g49 = lerp( lerpResult15_g49 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float temp_output_1054_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g49 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1PreAlpha1159 = temp_output_1054_0;
				float time35_g50 = 0.0;
				float2 voronoiSmoothId35_g50 = 0;
				float2 temp_output_21_0_g50 = CentralUV998;
				float2 break2_g50 = abs( temp_output_21_0_g50 );
				float saferPower4_g50 = abs( break2_g50.x );
				float saferPower3_g50 = abs( break2_g50.y );
				float saferPower6_g50 = abs( ( pow( saferPower4_g50 , 2.0 ) + pow( saferPower3_g50 , 2.0 ) ) );
				float2 temp_output_10_0_g50 = (( ( temp_output_21_0_g50 * ( pow( saferPower6_g50 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale1080 * 1.5 ) ) + ( Wind1035 * float2( 0.5,0.5 ) ));
				float2 coords35_g50 = temp_output_10_0_g50 * 60.0;
				float2 id35_g50 = 0;
				float2 uv35_g50 = 0;
				float fade35_g50 = 0.5;
				float voroi35_g50 = 0;
				float rest35_g50 = 0;
				for( int it35_g50 = 0; it35_g50 <2; it35_g50++ ){
				voroi35_g50 += fade35_g50 * voronoi35_g50( coords35_g50, time35_g50, id35_g50, uv35_g50, 0,voronoiSmoothId35_g50 );
				rest35_g50 += fade35_g50;
				coords35_g50 *= 2;
				fade35_g50 *= 0.5;
				}//Voronoi35_g50
				voroi35_g50 /= rest35_g50;
				float time13_g50 = 0.0;
				float2 voronoiSmoothId13_g50 = 0;
				float2 coords13_g50 = temp_output_10_0_g50 * 25.0;
				float2 id13_g50 = 0;
				float2 uv13_g50 = 0;
				float fade13_g50 = 0.5;
				float voroi13_g50 = 0;
				float rest13_g50 = 0;
				for( int it13_g50 = 0; it13_g50 <2; it13_g50++ ){
				voroi13_g50 += fade13_g50 * voronoi13_g50( coords13_g50, time13_g50, id13_g50, uv13_g50, 0,voronoiSmoothId13_g50 );
				rest13_g50 += fade13_g50;
				coords13_g50 *= 2;
				fade13_g50 *= 0.5;
				}//Voronoi13_g50
				voroi13_g50 /= rest13_g50;
				float time11_g50 = 17.23;
				float2 voronoiSmoothId11_g50 = 0;
				float2 coords11_g50 = temp_output_10_0_g50 * 9.0;
				float2 id11_g50 = 0;
				float2 uv11_g50 = 0;
				float fade11_g50 = 0.5;
				float voroi11_g50 = 0;
				float rest11_g50 = 0;
				for( int it11_g50 = 0; it11_g50 <2; it11_g50++ ){
				voroi11_g50 += fade11_g50 * voronoi11_g50( coords11_g50, time11_g50, id11_g50, uv11_g50, 0,voronoiSmoothId11_g50 );
				rest11_g50 += fade11_g50;
				coords11_g50 *= 2;
				fade11_g50 *= 0.5;
				}//Voronoi11_g50
				voroi11_g50 /= rest11_g50;
				float lerpResult15_g50 = lerp( saturate( ( voroi35_g50 + voroi13_g50 ) ) , voroi11_g50 , ( ModifiedCohesion1074 * 1.1 ));
				float lerpResult16_g50 = lerp( lerpResult15_g50 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float temp_output_1183_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g50 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2PreAlpha1184 = temp_output_1183_0;
				float temp_output_1143_0 = (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g52 ) ) - 0.6) * (max( IT1PreAlpha1159 , IT2PreAlpha1184 ) - 0.0) / (1.5 - 0.6));
				float clampResult1158 = clamp( temp_output_1143_0 , 0.0 , 0.9 );
				float AdditionalLayer1147 = SampleGradient( gradient1145, clampResult1158 ).r;
				float4 lerpResult1150 = lerp( CloudColor332 , ( CloudColor332 * _SecondLayer ) , AdditionalLayer1147);
				float4 ModifiedCloudColor1165 = lerpResult1150;
				Gradient gradient1014 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4411841 ), float4( 1, 1, 1, 0.5794156 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g51 = 0.0;
				float2 voronoiSmoothId35_g51 = 0;
				float2 ShadowUV997 = ( CentralUV998 + ( CentralUV998 * float2( -1,-1 ) * _ShadowingDistance * Dot1071 ) );
				float2 temp_output_21_0_g51 = ShadowUV997;
				float2 break2_g51 = abs( temp_output_21_0_g51 );
				float saferPower4_g51 = abs( break2_g51.x );
				float saferPower3_g51 = abs( break2_g51.y );
				float saferPower6_g51 = abs( ( pow( saferPower4_g51 , 2.0 ) + pow( saferPower3_g51 , 2.0 ) ) );
				float2 temp_output_10_0_g51 = (( ( temp_output_21_0_g51 * ( pow( saferPower6_g51 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
				float2 coords35_g51 = temp_output_10_0_g51 * 60.0;
				float2 id35_g51 = 0;
				float2 uv35_g51 = 0;
				float fade35_g51 = 0.5;
				float voroi35_g51 = 0;
				float rest35_g51 = 0;
				for( int it35_g51 = 0; it35_g51 <2; it35_g51++ ){
				voroi35_g51 += fade35_g51 * voronoi35_g51( coords35_g51, time35_g51, id35_g51, uv35_g51, 0,voronoiSmoothId35_g51 );
				rest35_g51 += fade35_g51;
				coords35_g51 *= 2;
				fade35_g51 *= 0.5;
				}//Voronoi35_g51
				voroi35_g51 /= rest35_g51;
				float time13_g51 = 0.0;
				float2 voronoiSmoothId13_g51 = 0;
				float2 coords13_g51 = temp_output_10_0_g51 * 25.0;
				float2 id13_g51 = 0;
				float2 uv13_g51 = 0;
				float fade13_g51 = 0.5;
				float voroi13_g51 = 0;
				float rest13_g51 = 0;
				for( int it13_g51 = 0; it13_g51 <2; it13_g51++ ){
				voroi13_g51 += fade13_g51 * voronoi13_g51( coords13_g51, time13_g51, id13_g51, uv13_g51, 0,voronoiSmoothId13_g51 );
				rest13_g51 += fade13_g51;
				coords13_g51 *= 2;
				fade13_g51 *= 0.5;
				}//Voronoi13_g51
				voroi13_g51 /= rest13_g51;
				float time11_g51 = 17.23;
				float2 voronoiSmoothId11_g51 = 0;
				float2 coords11_g51 = temp_output_10_0_g51 * 9.0;
				float2 id11_g51 = 0;
				float2 uv11_g51 = 0;
				float fade11_g51 = 0.5;
				float voroi11_g51 = 0;
				float rest11_g51 = 0;
				for( int it11_g51 = 0; it11_g51 <2; it11_g51++ ){
				voroi11_g51 += fade11_g51 * voronoi11_g51( coords11_g51, time11_g51, id11_g51, uv11_g51, 0,voronoiSmoothId11_g51 );
				rest11_g51 += fade11_g51;
				coords11_g51 *= 2;
				fade11_g51 *= 0.5;
				}//Voronoi11_g51
				voroi11_g51 /= rest11_g51;
				float lerpResult15_g51 = lerp( saturate( ( voroi35_g51 + voroi13_g51 ) ) , voroi11_g51 , ModifiedCohesion1074);
				float lerpResult16_g51 = lerp( lerpResult15_g51 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float4 lerpResult989 = lerp( CloudHighlightColor334 , ModifiedCloudColor1165 , saturate( SampleGradient( gradient1014, saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g51 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) ) ).r ));
				float4 IT1Color923 = lerpResult989;
				Gradient gradient1198 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4411841 ), float4( 1, 1, 1, 0.5794156 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g53 = 0.0;
				float2 voronoiSmoothId35_g53 = 0;
				float2 temp_output_21_0_g53 = ShadowUV997;
				float2 break2_g53 = abs( temp_output_21_0_g53 );
				float saferPower4_g53 = abs( break2_g53.x );
				float saferPower3_g53 = abs( break2_g53.y );
				float saferPower6_g53 = abs( ( pow( saferPower4_g53 , 2.0 ) + pow( saferPower3_g53 , 2.0 ) ) );
				float2 temp_output_10_0_g53 = (( ( temp_output_21_0_g53 * ( pow( saferPower6_g53 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale1080 * 1.5 ) ) + ( Wind1035 * float2( 0.5,0.5 ) ));
				float2 coords35_g53 = temp_output_10_0_g53 * 60.0;
				float2 id35_g53 = 0;
				float2 uv35_g53 = 0;
				float fade35_g53 = 0.5;
				float voroi35_g53 = 0;
				float rest35_g53 = 0;
				for( int it35_g53 = 0; it35_g53 <2; it35_g53++ ){
				voroi35_g53 += fade35_g53 * voronoi35_g53( coords35_g53, time35_g53, id35_g53, uv35_g53, 0,voronoiSmoothId35_g53 );
				rest35_g53 += fade35_g53;
				coords35_g53 *= 2;
				fade35_g53 *= 0.5;
				}//Voronoi35_g53
				voroi35_g53 /= rest35_g53;
				float time13_g53 = 0.0;
				float2 voronoiSmoothId13_g53 = 0;
				float2 coords13_g53 = temp_output_10_0_g53 * 25.0;
				float2 id13_g53 = 0;
				float2 uv13_g53 = 0;
				float fade13_g53 = 0.5;
				float voroi13_g53 = 0;
				float rest13_g53 = 0;
				for( int it13_g53 = 0; it13_g53 <2; it13_g53++ ){
				voroi13_g53 += fade13_g53 * voronoi13_g53( coords13_g53, time13_g53, id13_g53, uv13_g53, 0,voronoiSmoothId13_g53 );
				rest13_g53 += fade13_g53;
				coords13_g53 *= 2;
				fade13_g53 *= 0.5;
				}//Voronoi13_g53
				voroi13_g53 /= rest13_g53;
				float time11_g53 = 17.23;
				float2 voronoiSmoothId11_g53 = 0;
				float2 coords11_g53 = temp_output_10_0_g53 * 9.0;
				float2 id11_g53 = 0;
				float2 uv11_g53 = 0;
				float fade11_g53 = 0.5;
				float voroi11_g53 = 0;
				float rest11_g53 = 0;
				for( int it11_g53 = 0; it11_g53 <2; it11_g53++ ){
				voroi11_g53 += fade11_g53 * voronoi11_g53( coords11_g53, time11_g53, id11_g53, uv11_g53, 0,voronoiSmoothId11_g53 );
				rest11_g53 += fade11_g53;
				coords11_g53 *= 2;
				fade11_g53 *= 0.5;
				}//Voronoi11_g53
				voroi11_g53 /= rest11_g53;
				float lerpResult15_g53 = lerp( saturate( ( voroi35_g53 + voroi13_g53 ) ) , voroi11_g53 , ( ModifiedCohesion1074 * 1.1 ));
				float lerpResult16_g53 = lerp( lerpResult15_g53 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float4 lerpResult1206 = lerp( CloudHighlightColor334 , ModifiedCloudColor1165 , saturate( SampleGradient( gradient1198, saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g53 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) ) ).r ));
				float4 IT2Color1207 = lerpResult1206;
				Gradient gradient1199 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float IT2Alpha1202 = SampleGradient( gradient1199, temp_output_1183_0 ).r;
				float4 lerpResult1218 = lerp( ( _AltoCloudColor * IT1Color923 ) , IT2Color1207 , IT2Alpha1202);
				
				Gradient gradient1021 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float IT1Alpha953 = SampleGradient( gradient1021, temp_output_1054_0 ).r;
				float temp_output_1216_0 = max( IT1Alpha953 , IT2Alpha1202 );
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = lerpResult1218.rgb;
				float Alpha = saturate( ( temp_output_1216_0 + ( temp_output_1216_0 * 2.0 * _CloudThickness ) ) );
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
			#define _SURFACE_TYPE_TRANSPARENT 1
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

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"


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
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _AltoCloudColor;
			float4 _CloudHighlightColor;
			float4 _CloudColor;
			float4 _SecondLayer;
			float2 _MainCloudWindDir;
			float _Spherize;
			float _MainCloudScale;
			float _WindSpeed;
			float _CloudCohesion;
			float _CumulusCoverageMultiplier;
			float _MaxCloudCover;
			float _ShadowingDistance;
			float _CloudThickness;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			

			
					float2 voronoihash35_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g49( n + g );
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
			
					float2 voronoihash13_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g49( n + g );
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
			
					float2 voronoihash11_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g49( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g50( n + g );
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
			
					float2 voronoihash13_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g50( n + g );
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
			
					float2 voronoihash11_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g50( n + g );
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

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;

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

				Gradient gradient1021 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord1042 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1043_0 = ( texCoord1042 - float2( 0.5,0.5 ) );
				float dotResult1045 = dot( temp_output_1043_0 , temp_output_1043_0 );
				float Dot1071 = saturate( (0.85 + (dotResult1045 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g49 = 0.0;
				float2 voronoiSmoothId35_g49 = 0;
				float2 texCoord955 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV998 = ( texCoord955 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g49 = CentralUV998;
				float2 break2_g49 = abs( temp_output_21_0_g49 );
				float saferPower4_g49 = abs( break2_g49.x );
				float saferPower3_g49 = abs( break2_g49.y );
				float saferPower6_g49 = abs( ( pow( saferPower4_g49 , 2.0 ) + pow( saferPower3_g49 , 2.0 ) ) );
				float Spherize1078 = _Spherize;
				float Flatness1076 = ( 20.0 * _Spherize );
				float Scale1080 = ( _MainCloudScale * 0.1 );
				float mulTime61 = _TimeParameters.x * ( 0.001 * _WindSpeed );
				float Time152 = mulTime61;
				float2 Wind1035 = ( Time152 * _MainCloudWindDir );
				float2 temp_output_10_0_g49 = (( ( temp_output_21_0_g49 * ( pow( saferPower6_g49 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
				float2 coords35_g49 = temp_output_10_0_g49 * 60.0;
				float2 id35_g49 = 0;
				float2 uv35_g49 = 0;
				float fade35_g49 = 0.5;
				float voroi35_g49 = 0;
				float rest35_g49 = 0;
				for( int it35_g49 = 0; it35_g49 <2; it35_g49++ ){
				voroi35_g49 += fade35_g49 * voronoi35_g49( coords35_g49, time35_g49, id35_g49, uv35_g49, 0,voronoiSmoothId35_g49 );
				rest35_g49 += fade35_g49;
				coords35_g49 *= 2;
				fade35_g49 *= 0.5;
				}//Voronoi35_g49
				voroi35_g49 /= rest35_g49;
				float time13_g49 = 0.0;
				float2 voronoiSmoothId13_g49 = 0;
				float2 coords13_g49 = temp_output_10_0_g49 * 25.0;
				float2 id13_g49 = 0;
				float2 uv13_g49 = 0;
				float fade13_g49 = 0.5;
				float voroi13_g49 = 0;
				float rest13_g49 = 0;
				for( int it13_g49 = 0; it13_g49 <2; it13_g49++ ){
				voroi13_g49 += fade13_g49 * voronoi13_g49( coords13_g49, time13_g49, id13_g49, uv13_g49, 0,voronoiSmoothId13_g49 );
				rest13_g49 += fade13_g49;
				coords13_g49 *= 2;
				fade13_g49 *= 0.5;
				}//Voronoi13_g49
				voroi13_g49 /= rest13_g49;
				float time11_g49 = 17.23;
				float2 voronoiSmoothId11_g49 = 0;
				float2 coords11_g49 = temp_output_10_0_g49 * 9.0;
				float2 id11_g49 = 0;
				float2 uv11_g49 = 0;
				float fade11_g49 = 0.5;
				float voroi11_g49 = 0;
				float rest11_g49 = 0;
				for( int it11_g49 = 0; it11_g49 <2; it11_g49++ ){
				voroi11_g49 += fade11_g49 * voronoi11_g49( coords11_g49, time11_g49, id11_g49, uv11_g49, 0,voronoiSmoothId11_g49 );
				rest11_g49 += fade11_g49;
				coords11_g49 *= 2;
				fade11_g49 *= 0.5;
				}//Voronoi11_g49
				voroi11_g49 /= rest11_g49;
				float2 texCoord1055 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1056_0 = ( texCoord1055 - float2( 0.5,0.5 ) );
				float dotResult1057 = dot( temp_output_1056_0 , temp_output_1056_0 );
				float ModifiedCohesion1074 = ( _CloudCohesion * 1.0 * ( 1.0 - dotResult1057 ) );
				float lerpResult15_g49 = lerp( saturate( ( voroi35_g49 + voroi13_g49 ) ) , voroi11_g49 , ModifiedCohesion1074);
				float CumulusCoverage376 = ( _CumulusCoverageMultiplier * _MaxCloudCover );
				float lerpResult16_g49 = lerp( lerpResult15_g49 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float temp_output_1054_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g49 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1Alpha953 = SampleGradient( gradient1021, temp_output_1054_0 ).r;
				Gradient gradient1199 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g50 = 0.0;
				float2 voronoiSmoothId35_g50 = 0;
				float2 temp_output_21_0_g50 = CentralUV998;
				float2 break2_g50 = abs( temp_output_21_0_g50 );
				float saferPower4_g50 = abs( break2_g50.x );
				float saferPower3_g50 = abs( break2_g50.y );
				float saferPower6_g50 = abs( ( pow( saferPower4_g50 , 2.0 ) + pow( saferPower3_g50 , 2.0 ) ) );
				float2 temp_output_10_0_g50 = (( ( temp_output_21_0_g50 * ( pow( saferPower6_g50 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale1080 * 1.5 ) ) + ( Wind1035 * float2( 0.5,0.5 ) ));
				float2 coords35_g50 = temp_output_10_0_g50 * 60.0;
				float2 id35_g50 = 0;
				float2 uv35_g50 = 0;
				float fade35_g50 = 0.5;
				float voroi35_g50 = 0;
				float rest35_g50 = 0;
				for( int it35_g50 = 0; it35_g50 <2; it35_g50++ ){
				voroi35_g50 += fade35_g50 * voronoi35_g50( coords35_g50, time35_g50, id35_g50, uv35_g50, 0,voronoiSmoothId35_g50 );
				rest35_g50 += fade35_g50;
				coords35_g50 *= 2;
				fade35_g50 *= 0.5;
				}//Voronoi35_g50
				voroi35_g50 /= rest35_g50;
				float time13_g50 = 0.0;
				float2 voronoiSmoothId13_g50 = 0;
				float2 coords13_g50 = temp_output_10_0_g50 * 25.0;
				float2 id13_g50 = 0;
				float2 uv13_g50 = 0;
				float fade13_g50 = 0.5;
				float voroi13_g50 = 0;
				float rest13_g50 = 0;
				for( int it13_g50 = 0; it13_g50 <2; it13_g50++ ){
				voroi13_g50 += fade13_g50 * voronoi13_g50( coords13_g50, time13_g50, id13_g50, uv13_g50, 0,voronoiSmoothId13_g50 );
				rest13_g50 += fade13_g50;
				coords13_g50 *= 2;
				fade13_g50 *= 0.5;
				}//Voronoi13_g50
				voroi13_g50 /= rest13_g50;
				float time11_g50 = 17.23;
				float2 voronoiSmoothId11_g50 = 0;
				float2 coords11_g50 = temp_output_10_0_g50 * 9.0;
				float2 id11_g50 = 0;
				float2 uv11_g50 = 0;
				float fade11_g50 = 0.5;
				float voroi11_g50 = 0;
				float rest11_g50 = 0;
				for( int it11_g50 = 0; it11_g50 <2; it11_g50++ ){
				voroi11_g50 += fade11_g50 * voronoi11_g50( coords11_g50, time11_g50, id11_g50, uv11_g50, 0,voronoiSmoothId11_g50 );
				rest11_g50 += fade11_g50;
				coords11_g50 *= 2;
				fade11_g50 *= 0.5;
				}//Voronoi11_g50
				voroi11_g50 /= rest11_g50;
				float lerpResult15_g50 = lerp( saturate( ( voroi35_g50 + voroi13_g50 ) ) , voroi11_g50 , ( ModifiedCohesion1074 * 1.1 ));
				float lerpResult16_g50 = lerp( lerpResult15_g50 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float temp_output_1183_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g50 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2Alpha1202 = SampleGradient( gradient1199, temp_output_1183_0 ).r;
				float temp_output_1216_0 = max( IT1Alpha953 , IT2Alpha1202 );
				

				float Alpha = saturate( ( temp_output_1216_0 + ( temp_output_1216_0 * 2.0 * _CloudThickness ) ) );
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
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 110000

			
			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"


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
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _AltoCloudColor;
			float4 _CloudHighlightColor;
			float4 _CloudColor;
			float4 _SecondLayer;
			float2 _MainCloudWindDir;
			float _Spherize;
			float _MainCloudScale;
			float _WindSpeed;
			float _CloudCohesion;
			float _CumulusCoverageMultiplier;
			float _MaxCloudCover;
			float _ShadowingDistance;
			float _CloudThickness;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			

			
					float2 voronoihash35_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g49( n + g );
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
			
					float2 voronoihash13_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g49( n + g );
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
			
					float2 voronoihash11_g49( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g49( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g49( n + g );
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
			
			float4 SampleGradient( Gradient gradient, float time )
			{
				float3 color = gradient.colors[0].rgb;
				UNITY_UNROLL
				for (int c = 1; c < 8; c++)
				{
				float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, gradient.colorsLength-1));
				color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
				}
				#ifndef UNITY_COLORSPACE_GAMMA
				color = SRGBToLinear(color);
				#endif
				float alpha = gradient.alphas[0].x;
				UNITY_UNROLL
				for (int a = 1; a < 8; a++)
				{
				float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, gradient.alphasLength-1));
				alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
				}
				return float4(color, alpha);
			}
			
					float2 voronoihash35_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi35_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash35_g50( n + g );
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
			
					float2 voronoihash13_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi13_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash13_g50( n + g );
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
			
					float2 voronoihash11_g50( float2 p )
					{
						
						p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
						return frac( sin( p ) *43758.5453);
					}
			
					float voronoi11_g50( float2 v, float time, inout float2 id, inout float2 mr, float smoothness, inout float2 smoothId )
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
						 		float2 o = voronoihash11_g50( n + g );
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
			

			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.ase_texcoord2.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.zw = 0;

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

				Gradient gradient1021 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float2 texCoord1042 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1043_0 = ( texCoord1042 - float2( 0.5,0.5 ) );
				float dotResult1045 = dot( temp_output_1043_0 , temp_output_1043_0 );
				float Dot1071 = saturate( (0.85 + (dotResult1045 - 0.0) * (3.0 - 0.85) / (1.0 - 0.0)) );
				float time35_g49 = 0.0;
				float2 voronoiSmoothId35_g49 = 0;
				float2 texCoord955 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 CentralUV998 = ( texCoord955 + float2( -0.5,-0.5 ) );
				float2 temp_output_21_0_g49 = CentralUV998;
				float2 break2_g49 = abs( temp_output_21_0_g49 );
				float saferPower4_g49 = abs( break2_g49.x );
				float saferPower3_g49 = abs( break2_g49.y );
				float saferPower6_g49 = abs( ( pow( saferPower4_g49 , 2.0 ) + pow( saferPower3_g49 , 2.0 ) ) );
				float Spherize1078 = _Spherize;
				float Flatness1076 = ( 20.0 * _Spherize );
				float Scale1080 = ( _MainCloudScale * 0.1 );
				float mulTime61 = _TimeParameters.x * ( 0.001 * _WindSpeed );
				float Time152 = mulTime61;
				float2 Wind1035 = ( Time152 * _MainCloudWindDir );
				float2 temp_output_10_0_g49 = (( ( temp_output_21_0_g49 * ( pow( saferPower6_g49 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / Scale1080 ) + Wind1035);
				float2 coords35_g49 = temp_output_10_0_g49 * 60.0;
				float2 id35_g49 = 0;
				float2 uv35_g49 = 0;
				float fade35_g49 = 0.5;
				float voroi35_g49 = 0;
				float rest35_g49 = 0;
				for( int it35_g49 = 0; it35_g49 <2; it35_g49++ ){
				voroi35_g49 += fade35_g49 * voronoi35_g49( coords35_g49, time35_g49, id35_g49, uv35_g49, 0,voronoiSmoothId35_g49 );
				rest35_g49 += fade35_g49;
				coords35_g49 *= 2;
				fade35_g49 *= 0.5;
				}//Voronoi35_g49
				voroi35_g49 /= rest35_g49;
				float time13_g49 = 0.0;
				float2 voronoiSmoothId13_g49 = 0;
				float2 coords13_g49 = temp_output_10_0_g49 * 25.0;
				float2 id13_g49 = 0;
				float2 uv13_g49 = 0;
				float fade13_g49 = 0.5;
				float voroi13_g49 = 0;
				float rest13_g49 = 0;
				for( int it13_g49 = 0; it13_g49 <2; it13_g49++ ){
				voroi13_g49 += fade13_g49 * voronoi13_g49( coords13_g49, time13_g49, id13_g49, uv13_g49, 0,voronoiSmoothId13_g49 );
				rest13_g49 += fade13_g49;
				coords13_g49 *= 2;
				fade13_g49 *= 0.5;
				}//Voronoi13_g49
				voroi13_g49 /= rest13_g49;
				float time11_g49 = 17.23;
				float2 voronoiSmoothId11_g49 = 0;
				float2 coords11_g49 = temp_output_10_0_g49 * 9.0;
				float2 id11_g49 = 0;
				float2 uv11_g49 = 0;
				float fade11_g49 = 0.5;
				float voroi11_g49 = 0;
				float rest11_g49 = 0;
				for( int it11_g49 = 0; it11_g49 <2; it11_g49++ ){
				voroi11_g49 += fade11_g49 * voronoi11_g49( coords11_g49, time11_g49, id11_g49, uv11_g49, 0,voronoiSmoothId11_g49 );
				rest11_g49 += fade11_g49;
				coords11_g49 *= 2;
				fade11_g49 *= 0.5;
				}//Voronoi11_g49
				voroi11_g49 /= rest11_g49;
				float2 texCoord1055 = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 temp_output_1056_0 = ( texCoord1055 - float2( 0.5,0.5 ) );
				float dotResult1057 = dot( temp_output_1056_0 , temp_output_1056_0 );
				float ModifiedCohesion1074 = ( _CloudCohesion * 1.0 * ( 1.0 - dotResult1057 ) );
				float lerpResult15_g49 = lerp( saturate( ( voroi35_g49 + voroi13_g49 ) ) , voroi11_g49 , ModifiedCohesion1074);
				float CumulusCoverage376 = ( _CumulusCoverageMultiplier * _MaxCloudCover );
				float lerpResult16_g49 = lerp( lerpResult15_g49 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float temp_output_1054_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g49 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT1Alpha953 = SampleGradient( gradient1021, temp_output_1054_0 ).r;
				Gradient gradient1199 = NewGradient( 0, 2, 2, float4( 0.06119964, 0.06119964, 0.06119964, 0.4617685 ), float4( 1, 1, 1, 0.5117723 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
				float time35_g50 = 0.0;
				float2 voronoiSmoothId35_g50 = 0;
				float2 temp_output_21_0_g50 = CentralUV998;
				float2 break2_g50 = abs( temp_output_21_0_g50 );
				float saferPower4_g50 = abs( break2_g50.x );
				float saferPower3_g50 = abs( break2_g50.y );
				float saferPower6_g50 = abs( ( pow( saferPower4_g50 , 2.0 ) + pow( saferPower3_g50 , 2.0 ) ) );
				float2 temp_output_10_0_g50 = (( ( temp_output_21_0_g50 * ( pow( saferPower6_g50 , Spherize1078 ) * Flatness1076 ) ) + float2( 0.5,0.5 ) )*( 2.0 / ( Scale1080 * 1.5 ) ) + ( Wind1035 * float2( 0.5,0.5 ) ));
				float2 coords35_g50 = temp_output_10_0_g50 * 60.0;
				float2 id35_g50 = 0;
				float2 uv35_g50 = 0;
				float fade35_g50 = 0.5;
				float voroi35_g50 = 0;
				float rest35_g50 = 0;
				for( int it35_g50 = 0; it35_g50 <2; it35_g50++ ){
				voroi35_g50 += fade35_g50 * voronoi35_g50( coords35_g50, time35_g50, id35_g50, uv35_g50, 0,voronoiSmoothId35_g50 );
				rest35_g50 += fade35_g50;
				coords35_g50 *= 2;
				fade35_g50 *= 0.5;
				}//Voronoi35_g50
				voroi35_g50 /= rest35_g50;
				float time13_g50 = 0.0;
				float2 voronoiSmoothId13_g50 = 0;
				float2 coords13_g50 = temp_output_10_0_g50 * 25.0;
				float2 id13_g50 = 0;
				float2 uv13_g50 = 0;
				float fade13_g50 = 0.5;
				float voroi13_g50 = 0;
				float rest13_g50 = 0;
				for( int it13_g50 = 0; it13_g50 <2; it13_g50++ ){
				voroi13_g50 += fade13_g50 * voronoi13_g50( coords13_g50, time13_g50, id13_g50, uv13_g50, 0,voronoiSmoothId13_g50 );
				rest13_g50 += fade13_g50;
				coords13_g50 *= 2;
				fade13_g50 *= 0.5;
				}//Voronoi13_g50
				voroi13_g50 /= rest13_g50;
				float time11_g50 = 17.23;
				float2 voronoiSmoothId11_g50 = 0;
				float2 coords11_g50 = temp_output_10_0_g50 * 9.0;
				float2 id11_g50 = 0;
				float2 uv11_g50 = 0;
				float fade11_g50 = 0.5;
				float voroi11_g50 = 0;
				float rest11_g50 = 0;
				for( int it11_g50 = 0; it11_g50 <2; it11_g50++ ){
				voroi11_g50 += fade11_g50 * voronoi11_g50( coords11_g50, time11_g50, id11_g50, uv11_g50, 0,voronoiSmoothId11_g50 );
				rest11_g50 += fade11_g50;
				coords11_g50 *= 2;
				fade11_g50 *= 0.5;
				}//Voronoi11_g50
				voroi11_g50 /= rest11_g50;
				float lerpResult15_g50 = lerp( saturate( ( voroi35_g50 + voroi13_g50 ) ) , voroi11_g50 , ( ModifiedCohesion1074 * 1.1 ));
				float lerpResult16_g50 = lerp( lerpResult15_g50 , 1.0 , ( ( 1.0 - CumulusCoverage376 ) + -0.7 ));
				float temp_output_1183_0 = saturate( (0.0 + (( Dot1071 * ( 1.0 - lerpResult16_g50 ) ) - 0.6) * (1.0 - 0.0) / (1.0 - 0.6)) );
				float IT2Alpha1202 = SampleGradient( gradient1199, temp_output_1183_0 ).r;
				float temp_output_1216_0 = max( IT1Alpha953 , IT2Alpha1202 );
				

				float Alpha = saturate( ( temp_output_1216_0 + ( temp_output_1216_0 * 2.0 * _CloudThickness ) ) );
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