﻿Shader "Unlit/TextureValueComparator"
{
	Properties
	{
		_Texture16("_Texture16", 2D) = "white" {}
	    _Texture24("_Texture24", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _Texture24;
			sampler2D _Texture16;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float4 col32 = tex2D(_Texture24, i.uv);
				float4 col16 = tex2D(_Texture16, i.uv);

				float a = col32.r;
				float b = col32.g;
				float a1 = (16 * col16.r + col16.g) * 15 / 255;
				float b1 = (16 * col16.b + col16.a) * 15 / 255;

				float4 col = float4(abs(a-a1), abs(b-b1), 0, 1);

				return col;
			}
			ENDCG
		}
	}
}
