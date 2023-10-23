Shader "Crying Onion/OMWS/Stylized Painted Sky"
{
	Properties
	{
		[HideInInspector][HDR]_HorizonColor("Horizon Color", Color) = (0.6399965,0.9474089,0.9622642,0)
		[HideInInspector][HDR]_GalaxyColor3("Galaxy Color 3", Color) = (0.6399965,0.9474089,0.9622642,0)
		[HideInInspector][HDR]_GalaxyColor2("Galaxy Color 2", Color) = (0.6399965,0.9474089,0.9622642,0)
		[HideInInspector][HDR]_GalaxyColor1("Galaxy Color 1", Color) = (0.6399965,0.9474089,0.9622642,0)
		[HideInInspector][HDR]_ZenithColor("Zenith Color", Color) = (0.4000979,0.6638572,0.764151,0)
		[HideInInspector]_Power("Power", Float) = 1
		_SunFlareFalloff("Sun Flare Falloff", Float) = 1
		[HideInInspector]_MoonFlareFalloff("Moon Flare Falloff", Float) = 1
		[HideInInspector][HDR]_SunFlareColor("Sun Flare Color", Color) = (0.355693,0.4595688,0.4802988,1)
		[HideInInspector][HDR]_MoonFlareColor("Moon Flare Color", Color) = (0.355693,0.4595688,0.4802988,1)
		[HideInInspector]_SunSize("Sun Size", Float) = 0
		[HideInInspector]_RainbowWidth("Rainbow Width", Float) = 0
		[HideInInspector]_RainbowSize("Rainbow Size", Float) = 0
		[HideInInspector][HDR]_SunColor("Sun Color", Color) = (0,0,0,0)
		_GalaxyStars("Galaxy Stars", 2D) = "white" {}
		_GalaxyPattern("Galaxy Pattern", 2D) = "white" {}
		_SkyPatchwork("Sky Patchwork", 2D) = "white" {}
		_LightColumns("Light Columns", 2D) = "white" {}
		[HideInInspector][Header(Border Clouds)]_PatchworkHeight("Patchwork Height", Range( 0 , 1)) = 1
		[HideInInspector]_PatchworkVariation("Patchwork Variation", Range( 0 , 1)) = 1
		[HideInInspector]_PatchworkBias("Patchwork Bias", Range( -1 , 1)) = 0
		[HDR]_StarColor("Star Color", Color) = (0,0,0,0)
		[HideInInspector][HDR]_LightColumnColor("Light Column Color", Color) = (0,0,0,0)
		[HideInInspector]_GalaxyMultiplier("Galaxy Multiplier", Range( 0 , 1)) = 0
		[HideInInspector]_RainbowIntensity("Rainbow Intensity", Range( 0 , 1)) = 0
		_ClippingThreshold("Clipping Threshold", Float) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Pass
		{
			ColorMask 0
			ZWrite On
		}

		Tags{ "RenderType" = "Opaque"  "Queue" = "Transparent-100" "IsEmissive" = "true"  }
		Cull Front
		Stencil
		{
			Ref 221
			Comp Always
			Pass Replace
		}
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float _Power;
		uniform sampler2D _SkyPatchwork;
		uniform float _PatchworkHeight;
		uniform float _PatchworkVariation;
		uniform float _PatchworkBias;
		uniform float _ClippingThreshold;
		uniform float4 _ZenithColor;
		uniform float4 _HorizonColor;
		uniform float3 OMWS_SunDirection;
		uniform float _SunFlareFalloff;
		uniform float4 _SunFlareColor;
		uniform float4 _SunColor;
		uniform float _SunSize;
		uniform float3 OMWS_MoonDirection;
		uniform half _MoonFlareFalloff;
		uniform float4 _MoonFlareColor;
		uniform sampler2D _GalaxyStars;
		uniform sampler2D _GalaxyPattern;
		uniform float4 _StarColor;
		uniform float4 _GalaxyColor1;
		uniform float4 _GalaxyColor2;
		uniform float4 _GalaxyColor3;
		uniform float _GalaxyMultiplier;
		uniform float _RainbowSize;
		uniform float _RainbowWidth;
		uniform float _RainbowIntensity;
		uniform sampler2D _LightColumns;
		uniform float4 _LightColumnColor;


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


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 temp_output_168_0 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult169 = dot( temp_output_168_0 , temp_output_168_0 );
			float SimpleGradient170 = dotResult169;
			float GradientPos97 = ( 1.0 - saturate( pow( saturate( (0.0 + (SimpleGradient170 - 0.0) * (2.0 - 0.0) / (1.0 - 0.0)) ) , _Power ) ) );
			float2 Pos83 = i.uv_texcoord;
			float mulTime89 = _Time.y * 0.001;
			float cos106 = cos( mulTime89 );
			float sin106 = sin( mulTime89 );
			float2 rotator106 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos106 , -sin106 , sin106 , cos106 )) + float2( 0.5,0.5 );
			float PatchworkPattern150 = min( tex2D( _SkyPatchwork, (Pos83*0.6 + mulTime89) ).r , tex2D( _SkyPatchwork, (rotator106*1.0 + 0.0) ).r );
			float2 temp_output_136_0 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float dotResult137 = dot( temp_output_136_0 , temp_output_136_0 );
			float temp_output_134_0 = ( -2.0 * ( 1.0 - _PatchworkVariation ) );
			float temp_output_144_0 = saturate( (( ( 1.0 - _PatchworkHeight ) * temp_output_134_0 ) + (dotResult137 - 0.0) * (( temp_output_134_0 * -4.0 ) - ( ( 1.0 - _PatchworkHeight ) * temp_output_134_0 )) / (0.5 - 0.0)) );
			float clampResult148 = clamp( ( ( PatchworkPattern150 * temp_output_144_0 ) * -10.0 * _PatchworkBias ) , -1.0 , 1.0 );
			float PatchworkFinal149 = clampResult148;
			float4 ZenithColor111 = _ZenithColor;
			float4 HorizonColor110 = _HorizonColor;
			float4 SimpleSkyGradient68 = ( saturate( ( GradientPos97 + PatchworkFinal149 ) ) > _ClippingThreshold ? ZenithColor111 : HorizonColor110 );
			float3 ase_worldPos = i.worldPos;
			float3 normalizeResult26 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult27 = dot( normalizeResult26 , OMWS_SunDirection );
			float SunDot51 = dotResult27;
			half4 SunFlare35 = abs( ( saturate( pow( abs( (SunDot51*0.5 + 0.5) ) , _SunFlareFalloff ) ) * _SunFlareColor ) );
			float4 SunRender70 = ( _SunColor * ( ( 1.0 - SunDot51 ) > ( pow( _SunSize , 3.0 ) * 0.0007 ) ? 0.0 : 1.0 ) );
			float3 normalizeResult65 = normalize( ( ase_worldPos - _WorldSpaceCameraPos ) );
			float dotResult66 = dot( normalizeResult65 , OMWS_MoonDirection );
			float MoonDot67 = dotResult66;
			half4 MoonFlare76 = abs( ( saturate( pow( abs( (MoonDot67*0.5 + 0.5) ) , _MoonFlareFalloff ) ) * _MoonFlareColor ) );
			float mulTime202 = _Time.y * 0.005;
			float cos203 = cos( mulTime202 );
			float sin203 = sin( mulTime202 );
			float2 rotator203 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos203 , -sin203 , sin203 , cos203 )) + float2( 0.5,0.5 );
			float mulTime205 = _Time.y * -0.02;
			float simplePerlin2D213 = snoise( (Pos83*5.0 + mulTime205) );
			simplePerlin2D213 = simplePerlin2D213*0.5 + 0.5;
			float StarPlacementPattern217 = saturate( ( min( tex2D( _SkyPatchwork, (Pos83*5.0 + mulTime202) ).r , tex2D( _SkyPatchwork, (rotator203*2.0 + 0.0) ).r ) * simplePerlin2D213 * (0.2 + (SimpleGradient170 - 0.0) * (0.0 - 0.2) / (1.0 - 0.0)) ) );
			float2 panner321 = ( 1.0 * _Time.y * float2( 0.0007,0 ) + Pos83);
			float mulTime175 = _Time.y * 0.001;
			float cos176 = cos( 0.01 * _Time.y );
			float sin176 = sin( 0.01 * _Time.y );
			float2 rotator176 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos176 , -sin176 , sin176 , cos176 )) + float2( 0.5,0.5 );
			float temp_output_181_0 = min( tex2D( _GalaxyStars, (panner321*4.0 + mulTime175) ).r , tex2D( _GalaxyPattern, (rotator176*0.1 + 0.0) ).r );
			float2 panner318 = ( 1.0 * _Time.y * float2( 0.0007,0 ) + Pos83);
			float mulTime153 = _Time.y * 0.005;
			float2 panner320 = ( 1.0 * _Time.y * float2( 0.001,0 ) + Pos83);
			float mulTime165 = _Time.y * -0.02;
			float simplePerlin2D161 = snoise( (Pos83*15.0 + mulTime165) );
			simplePerlin2D161 = simplePerlin2D161*0.5 + 0.5;
			float GalaxyPattern160 = saturate( ( min( tex2D( _GalaxyPattern, (panner318*10.0 + mulTime153) ).r , tex2D( _GalaxyPattern, (panner320*7.0 + mulTime153) ).r ) * simplePerlin2D161 * (0.2 + (SimpleGradient170 - 0.0) * (0.0 - 0.2) / (0.05 - 0.0)) ) );
			float StarPattern182 = ( ( ( StarPlacementPattern217 * temp_output_181_0 ) + ( temp_output_181_0 * GalaxyPattern160 ) ) * ( 1.0 - MoonFlare76.r ) );
			float cos221 = cos( 0.02 * _Time.y );
			float sin221 = sin( 0.02 * _Time.y );
			float2 rotator221 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos221 , -sin221 , sin221 , cos221 )) + float2( 0.5,0.5 );
			float cos229 = cos( 0.04 * _Time.y );
			float sin229 = sin( 0.04 * _Time.y );
			float2 rotator229 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos229 , -sin229 , sin229 , cos229 )) + float2( 0.5,0.5 );
			float cos235 = cos( 0.01 * _Time.y );
			float sin235 = sin( 0.01 * _Time.y );
			float2 rotator235 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos235 , -sin235 , sin235 , cos235 )) + float2( 0.5,0.5 );
			float4 appendResult227 = (float4(tex2D( _SkyPatchwork, (rotator221*6.0 + 0.0) ).r , tex2D( _SkyPatchwork, (rotator229*5.5 + 2.04) ).r , tex2D( _SkyPatchwork, (rotator235*5.0 + 2.04) ).r , 1.0));
			float4 GalaxyColoring226 = appendResult227;
			float4 break241 = GalaxyColoring226;
			float4 FinalGalaxyColoring236 = ( ( _GalaxyColor1 * break241.r ) + ( _GalaxyColor2 * break241.g ) + ( _GalaxyColor3 * break241.b ) );
			float4 GalaxyFullColor194 = ( saturate( ( StarPattern182 * _StarColor ) ) + ( GalaxyPattern160 * FinalGalaxyColoring236 * _GalaxyMultiplier ) );
			Gradient gradient289 = NewGradient( 0, 8, 4, float4( 1, 0, 0, 0.1205921 ), float4( 1, 0.3135593, 0, 0.2441138 ), float4( 1, 0.8774895, 0.2216981, 0.3529412 ), float4( 0.3030533, 1, 0.2877358, 0.4529488 ), float4( 0.3726415, 1, 0.9559749, 0.5529412 ), float4( 0.4669811, 0.7253776, 1, 0.6470588 ), float4( 0.1561944, 0.3586135, 0.735849, 0.802945 ), float4( 0.2576377, 0.08721964, 0.5283019, 0.9264668 ), float2( 0, 0 ), float2( 0, 0.08235294 ), float2( 0.6039216, 0.8264744 ), float2( 0, 1 ), 0, 0, 0, 0 );
			float temp_output_276_0 = ( 1.0 - SunDot51 );
			float temp_output_275_0 = ( _RainbowSize * 0.01 );
			float temp_output_285_0 = ( temp_output_275_0 + ( _RainbowWidth * 0.01 ) );
			float4 RainbowClipping283 = ( SampleGradient( gradient289, (0.0 + (temp_output_276_0 - temp_output_275_0) * (1.0 - 0.0) / (temp_output_285_0 - temp_output_275_0)) ) * ( ( temp_output_276_0 < temp_output_275_0 ? 0.0 : 1.0 ) * ( temp_output_276_0 > temp_output_285_0 ? 0.0 : 1.0 ) ) * SampleGradient( gradient289, (0.0 + (temp_output_276_0 - temp_output_275_0) * (1.0 - 0.0) / (temp_output_285_0 - temp_output_275_0)) ).a * _RainbowIntensity );
			float cos316 = cos( -0.005 * _Time.y );
			float sin316 = sin( -0.005 * _Time.y );
			float2 rotator316 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos316 , -sin316 , sin316 , cos316 )) + float2( 0.5,0.5 );
			float cos295 = cos( 0.01 * _Time.y );
			float sin295 = sin( 0.01 * _Time.y );
			float2 rotator295 = mul( Pos83 - float2( 0.5,0.5 ) , float2x2( cos295 , -sin295 , sin295 , cos295 )) + float2( 0.5,0.5 );
			float4 transform325 = mul(unity_WorldToObject,float4( ase_worldPos , 0.0 ));
			float saferPower329 = abs( ( ( abs( transform325.y ) * 0.03 ) + 0.0 ) );
			float LightColumnsPattern309 = saturate( ( min( tex2D( _LightColumns, rotator316 ).r , tex2D( _LightColumns, rotator295 ).r ) * saturate( (1.0 + (saturate( pow( saferPower329 , 3.17 ) ) - 0.0) * (0.0 - 1.0) / (1.0 - 0.0)) ) ) );
			float4 LightColumnsColor315 = ( LightColumnsPattern309 * _LightColumnColor );
			o.Emission = ( SimpleSkyGradient68 + SunFlare35 + SunRender70 + MoonFlare76 + GalaxyFullColor194 + RainbowClipping283 + LightColumnsColor315 ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
}