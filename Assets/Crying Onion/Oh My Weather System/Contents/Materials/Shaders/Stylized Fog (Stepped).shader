Shader "Distant Lands/Cozy/Stepped Fog"
{
	Properties
	{
		[HDR]_FogColor1("Fog Color 1", Color) = (1,0,0.8999224,1)
		[HDR]_FogColor2("Fog Color 2", Color) = (1,0,0,1)
		[HDR]_FogColor3("Fog Color 3", Color) = (1,0,0.7469492,1)
		[HDR]_FogColor4("Fog Color 4", Color) = (0,0.8501792,1,1)
		[HDR]_FogColor5("Fog Color 5", Color) = (0.164721,0,1,1)
		_FogColorStart1("FogColorStart1", Float) = 1
		_FogColorStart2("FogColorStart2", Float) = 2
		_FogColorStart3("FogColorStart3", Float) = 3
		_FogColorStart4("FogColorStart4", Float) = 4
		[HideInInspector]_SunDirection("Sun Direction", Vector) = (0,0,0,0)
		_FlareSquish("Flare Squish", Float) = 1
		_FogDepthMultiplier("Fog Depth Multiplier", Float) = 1
		_LightFalloff("Light Falloff", Float) = 1
		_LightIntensity("Light Intensity", Float) = 0
		[HDR]_LightColor("Light Color", Color) = (0,0,0,0)
		_FogSmoothness("Fog Smoothness", Float) = 0.1
		_FogIntensity("Fog Intensity", Float) = 1
		_FogOffset("Fog Offset", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "HeightFog"  "Queue" = "Transparent+1" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Front
		ZWrite Off
		ZTest Always
		Stencil
		{
			Ref 222
			Comp NotEqual
			Pass Replace
		}
		Blend SrcAlpha OneMinusSrcAlpha
		
		GrabPass{ }
		CGPROGRAM
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float4 screenPos;
			float3 worldPos;
		};

		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform float _FogDepthMultiplier;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _FogColorStart4;
		uniform float4 _FogColor5;
		uniform float _FogColorStart3;
		uniform float4 _FogColor4;
		uniform float _FogColorStart2;
		uniform float4 _FogColor3;
		uniform float _FogColorStart1;
		uniform float4 _FogColor2;
		uniform float4 _FogColor1;
		uniform float4 _LightColor;
		uniform float _FlareSquish;
		uniform float3 _SunDirection;
		uniform half _LightIntensity;
		uniform half _LightFalloff;
		uniform float _FogSmoothness;
		uniform float _FogOffset;
		uniform float _FogIntensity;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 screenColor246 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,ase_grabScreenPos.xy/ase_grabScreenPos.w);
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float eyeDepth229 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float Distance314 = ( _FogDepthMultiplier * sqrt( eyeDepth229 ) );
			float4 FogColors317 = ( Distance314 > _FogColorStart4 ? _FogColor5 : ( Distance314 > _FogColorStart3 ? _FogColor4 : ( Distance314 > _FogColorStart2 ? _FogColor3 : ( Distance314 > _FogColorStart1 ? _FogColor2 : _FogColor1 ) ) ) );
			float3 hsvTorgb224 = RGBToHSV( _LightColor.rgb );
			float3 hsvTorgb265 = RGBToHSV( FogColors317.rgb );
			float3 hsvTorgb249 = HSVToRGB( float3(hsvTorgb224.x,hsvTorgb224.y,( hsvTorgb224.z * hsvTorgb265.z )) );
			float3 ase_worldPos = i.worldPos;
			float3 appendResult245 = (float3(1.0 , _FlareSquish , 1.0));
			float3 normalizeResult266 = normalize( ( ( ase_worldPos * appendResult245 ) - _WorldSpaceCameraPos ) );
			float dotResult263 = dot( normalizeResult266 , _SunDirection );
			half LightMask250 = saturate( pow( abs( ( (dotResult263*0.5 + 0.5) * _LightIntensity ) ) , _LightFalloff ) );
			float temp_output_275_0 = ( FogColors317.a * saturate( Distance314 ) );
			float4 lerpResult260 = lerp( FogColors317 , float4( hsvTorgb249 , 0.0 ) , saturate( ( LightMask250 * ( 1.5 * temp_output_275_0 ) ) ));
			float4 lerpResult240 = lerp( screenColor246 , lerpResult260 , temp_output_275_0);
			o.Emission = lerpResult240.rgb;
			o.Alpha = saturate( ( ( 1.0 - saturate( ( ( ( ( ase_worldPos - _WorldSpaceCameraPos ).y * 0.1 ) * ( 1.0 / _FogSmoothness ) ) + ( 1.0 - _FogOffset ) ) ) ) * _FogIntensity ) );
		}

		ENDCG
	}
}