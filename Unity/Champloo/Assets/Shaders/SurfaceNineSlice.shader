// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Splatter/Surface Nine Slice"
{
	Properties
	{
		_MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		_AlphaCutoff("Alpha Cutoff", Range(0.01, 1.0)) = 0.01
		[HideInInspector] _MinRepeatUV("Min repeat for UV", Vector) = (0,0,0,0)
		[HideInInspector] _RepeatRangeUV("Repeat range (size) for UV", Vector) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			Stencil
			{
				Ref 5
				Comp Always
				Pass Replace
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ WORLDSPACE_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 worldSpacePosition : TEXCOORD1;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;
			fixed _AlphaCutoff;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _AlphaTex;
			float _AlphaSplitEnabled;

			float4 _MinRepeatUV;
			float4 _RepeatRangeUV;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
				#endif

				OUT.worldSpacePosition = mul(unity_ObjectToWorld, IN.vertex);

				OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);

				return OUT;
			}

			float lerp(float start, float end, float percent) {
				return start * (1 - percent) + end * (percent);
			}

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);
				if (_AlphaSplitEnabled)
					color.a = tex2D(_AlphaTex, uv).r;

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 worldXY = IN.worldSpacePosition.xy;
				float2 finalXY = float2(
					lerp(
						IN.texcoord.x,
						_MinRepeatUV.x + fmod(abs(worldXY.x), _RepeatRangeUV.x),
						IN.color.r),
					lerp(
						IN.texcoord.y,
						_MinRepeatUV.y + fmod(abs(worldXY.y), _RepeatRangeUV.y),
						IN.color.g));

				fixed4 c = SampleSpriteTexture(finalXY);

				// Discard pixels below cutoff so that stencil is only updated for visible pixels.
				clip(c.a - _AlphaCutoff);

				return c;
			}
			ENDCG
		}
	}
}