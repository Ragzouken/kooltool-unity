Shader "Border/BorderShader"
{
    Properties
    {
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Off
        Blend One Zero

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color : COLOR;
                half2 texcoord  : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            float4 _SpriteUV;
            float4 _Sizes;
			float4 _Color;

			float2 _SrcPixel;
			float2 _DstPixel;
			float2 _Scale;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.worldPosition = IN.vertex;
                OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);

                OUT.texcoord = IN.texcoord;

                #ifdef UNITY_HALF_TEXEL_OFFSET
                OUT.vertex.xy += (_ScreenParams.zw - 1.0)*float2(-1,1);
                #endif

                OUT.color = IN.color * _Color;
                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                float edge = 3;

				float2 pixel = _SrcPixel;
				float2 offset = edge * _DstPixel;
				float2 scale = _Scale;

				float2 corrected = (IN.texcoord - offset) * scale;
                float2 final = lerp(_SpriteUV.xy, _SpriteUV.zw, corrected);

                fixed4 color = tex2D(_MainTex, final);

                if (corrected.x < 0
                 || corrected.y < 0
                 || corrected.x > 1
                 || corrected.y > 1)
                {
                    color *= 0;
                }

				float2 coord;
				half4 neighbour;
				float mult = 1;

                for (int x = -3; x <= 3; ++x)
                {
                    for (int y = -3; y <= 3; ++y)
                    {
                        coord = final + pixel * float2(x, y);
                        neighbour = tex2D(_MainTex, coord);

                        if (coord.x < _SpriteUV.x
                         || coord.y < _SpriteUV.y
                         || coord.x > _SpriteUV.z
                         || coord.y > _SpriteUV.w)
                        {
                            neighbour *= 0;
                        }

                        float d = abs(x) + abs(y);

                        if (neighbour.a > 0
						 && d <= 4)
                        {
                            color = fixed4(1, 1, 1, 1);
                        }

						if (d == 0 && neighbour.a > 0) mult = 0;
                    }
                }

                return color * mult;
            }
            ENDCG
        }
    }
}
