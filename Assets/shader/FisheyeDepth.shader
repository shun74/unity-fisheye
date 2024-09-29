Shader "Custom/FisheyeDepthTexture"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RemapTex ("Remap Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off
        ZTest Always
        ZWrite Off

        Tags { "RenderType"="Opaque" }


        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _RemapTex;
            sampler2D _CameraDepthTexture;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;
                if (length(uv) > 1.0)
                {
                    discard;
                }
                float2 remapUV = tex2D(_RemapTex, i.uv).rg;
                fixed col = tex2D(_CameraDepthTexture, remapUV).r;
                return LinearEyeDepth(col);

                // float2 uv = i.uv * 2.0 - 1.0;
                // if (length(uv) > 1.0)
                // {
                //     discard;
                // }
                // float2 remapUV = tex2D(_RemapTex, i.uv).rg;
                // float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, remapUV);
                // float linearDepth = LinearEyeDepth(depth);
                // return float4(linearDepth, linearDepth, linearDepth, 1);
            }
            ENDCG
        }
    }
}
